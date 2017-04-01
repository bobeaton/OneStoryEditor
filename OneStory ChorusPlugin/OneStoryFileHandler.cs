using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Chorus.FileTypeHandlers;
using Chorus.FileTypeHandlers.xml;
using Chorus.Utilities;
using Chorus.merge;
using Chorus.merge.xml.generic;
using Chorus.VcsDrivers.Mercurial;
using SIL.Progress;
using SIL.IO;

namespace OneStory_ChorusPlugin
{
    public class OneStoryFileHandler : IChorusFileTypeHandler
    {
		internal OneStoryFileHandler()
		{}

        public const string CstrAppName = "StoryEditor.exe";

        private bool OneStoryAssemblyIsAvailable
        {
            get
            {
                return File.Exists(Path.Combine(
                                       ExecutionEnvironment.DirectoryOfExecutingAssembly, CstrAppName));
            }
        }

        protected bool HasOneStoryExtension(string strPathToFile)
        {
            return (Path.GetExtension(strPathToFile).ToLower() == ".onestory");
        }

        public bool CanDiffFile(string pathToFile)
        {
            return OneStoryAssemblyIsAvailable && HasOneStoryExtension(pathToFile);
        }

        public bool CanMergeFile(string pathToFile)
        {
            return HasOneStoryExtension(pathToFile);
        }

        public bool CanPresentFile(string pathToFile)
        {
            return OneStoryAssemblyIsAvailable && HasOneStoryExtension(pathToFile);
        }

        public bool CanValidateFile(string pathToFile)
        {
            return false;
        }
        public string ValidateFile(string pathToFile, IProgress progress)
        {
            throw new NotImplementedException();
        }

        public void Do3WayMerge(MergeOrder mergeOrder)
        {
            var merger = new XmlMerger(mergeOrder.MergeSituation);
            SetupElementStrategies(merger);

            merger.EventListener = mergeOrder.EventListener;
            var result = merger.MergeFiles(mergeOrder.pathToOurs, mergeOrder.pathToTheirs, mergeOrder.pathToCommonAncestor);

            // use linq to write the merged XML out, so it is as much as possible like the format that the OneStory editor 
            //  (which uses linq also) writes out. Do this so we maximally keep indentation the same, so that if you do 
            //  "view changesets" in TortoiseHG (a line-by-line differencer) it will highlight bona fide differences as much 
            //  as possible.
            XDocument doc = XDocument.Parse(result.MergedNode.OuterXml);
            doc.Save(mergeOrder.pathToOurs);
        }

        private void SetupElementStrategies(XmlMerger merger)
        {
            // add our own context description generator (because the default behavior is to include the 
            //  entire element in the conflict report (which for some of these could be huge)
            var elementStrategy = ElementStrategy.CreateSingletonElement();
            elementStrategy.ContextDescriptorGenerator = new OneStoryContextGenerator("StoryProject", "ProjectName");
            merger.MergeStrategies.SetStrategy("StoryProject", elementStrategy);

            elementStrategy = ElementStrategy.CreateSingletonElement();
            elementStrategy.ContextDescriptorGenerator = new OneStoryContextGenerator("Members");
            merger.MergeStrategies.SetStrategy("Members", elementStrategy);

            // no context generator for this, because the default behavior of comparing the entire element is fine
            elementStrategy = ElementStrategy.CreateForKeyedElement("memberKey", false);
            elementStrategy.ContextDescriptorGenerator = new OneStoryContextGenerator("Member", "memberKey");
            merger.MergeStrategies.SetStrategy("Member", elementStrategy);

            elementStrategy = ElementStrategy.CreateSingletonElement();
            elementStrategy.ContextDescriptorGenerator = new OneStoryContextGenerator("Languages");
            merger.MergeStrategies.SetStrategy("Languages", elementStrategy);

            // no context generator for this, because the default behavior of comparing the entire element is fine
            elementStrategy = ElementStrategy.CreateForKeyedElement("lang", false);
            elementStrategy.ContextDescriptorGenerator = new OneStoryContextGenerator("LanguageInfo", "lang");
            merger.MergeStrategies.SetStrategy("LanguageInfo", elementStrategy);

            // AdaptIt configuration settings (we use AI for BT)
            elementStrategy = ElementStrategy.CreateSingletonElement();
            elementStrategy.ContextDescriptorGenerator = new OneStoryContextGenerator("AdaptItConfigurations");
            merger.MergeStrategies.SetStrategy("AdaptItConfigurations", elementStrategy);

            // no context generator for this, because the default behavior of comparing the entire element is fine
            elementStrategy = ElementStrategy.CreateForKeyedElement("BtDirection", false);
            elementStrategy.ContextDescriptorGenerator = new OneStoryContextGenerator("AdaptItConfiguration", "BtDirection");
            merger.MergeStrategies.SetStrategy("AdaptItConfiguration", elementStrategy);

            // Language and culture notes
            elementStrategy = ElementStrategy.CreateSingletonElement();
            elementStrategy.ContextDescriptorGenerator = new OneStoryContextGenerator("LnCNotes");
            merger.MergeStrategies.SetStrategy("LnCNotes", elementStrategy);

            // no context generator for this, because the default behavior of comparing the entire element is fine
            elementStrategy = ElementStrategy.CreateForKeyedElement("guid", false);
            elementStrategy.ContextDescriptorGenerator = new OneStoryContextGenerator("LnCNote", "guid");
            merger.MergeStrategies.SetStrategy("LnCNote", elementStrategy);

            // story sets and stories
            elementStrategy = ElementStrategy.CreateForKeyedElement("SetName", false);
            elementStrategy.ContextDescriptorGenerator = new OneStoryContextGenerator("stories", "SetName");
            merger.MergeStrategies.SetStrategy("stories", elementStrategy);

            elementStrategy = ElementStrategy.CreateForKeyedElement("guid", false);
            elementStrategy.AttributesToIgnoreForMerging.Add("stageDateTimeStamp");
            elementStrategy.ContextDescriptorGenerator = new OneStoryContextGenerator("story", "guid");
            merger.MergeStrategies.SetStrategy("story", elementStrategy);

            // the rest is used only if the same story was edited by two or more people at the same time
            //  not supposed to happen, but let's be safer
            elementStrategy = ElementStrategy.CreateSingletonElement();
            elementStrategy.ContextDescriptorGenerator = new OneStoryContextGenerator("CraftingInfo");
            merger.MergeStrategies.SetStrategy("CraftingInfo", elementStrategy);

            // no context generator for this, because the default behavior of comparing the entire element is fine
            elementStrategy = ElementStrategy.CreateSingletonElement();
            merger.MergeStrategies.SetStrategy("StoryCrafter", elementStrategy);

            // no context generator for this, because the default behavior of comparing the entire element is fine
            elementStrategy = ElementStrategy.CreateSingletonElement();
            merger.MergeStrategies.SetStrategy("ProjectFacilitator", elementStrategy);

            // no context generator for this, because the default behavior of comparing the entire element is fine
            elementStrategy = ElementStrategy.CreateSingletonElement();
            merger.MergeStrategies.SetStrategy("Consultant", elementStrategy);

            // no context generator for this, because the default behavior of comparing the entire element is fine
            elementStrategy = ElementStrategy.CreateSingletonElement();
            merger.MergeStrategies.SetStrategy("Coach", elementStrategy);

            // no context generator for this, because the default behavior of comparing the entire element is fine
            elementStrategy = ElementStrategy.CreateSingletonElement();
            merger.MergeStrategies.SetStrategy("StoryPurpose", elementStrategy);

            // no context generator for this, because the default behavior of comparing the entire element is fine
            elementStrategy = ElementStrategy.CreateSingletonElement();
            merger.MergeStrategies.SetStrategy("ResourcesUsed", elementStrategy);

            // no context generator for this, because the default behavior of comparing the entire element is fine
            elementStrategy = ElementStrategy.CreateSingletonElement();
            merger.MergeStrategies.SetStrategy("MiscellaneousStoryInfo", elementStrategy);

            // no context generator for this, because the default behavior of comparing the entire element is fine
            elementStrategy = ElementStrategy.CreateSingletonElement();
            merger.MergeStrategies.SetStrategy("BackTranslator", elementStrategy);

            elementStrategy = ElementStrategy.CreateSingletonElement();
            elementStrategy.ContextDescriptorGenerator = new OneStoryContextGenerator("TestsRetellings");
            merger.MergeStrategies.SetStrategy("TestsRetellings", elementStrategy);

            // no context generator for this, because the default behavior of comparing the entire element is fine
            elementStrategy = ElementStrategy.CreateForKeyedElement("memberID", true);
            merger.MergeStrategies.SetStrategy("TestRetelling", elementStrategy);

            elementStrategy = ElementStrategy.CreateSingletonElement();
            elementStrategy.ContextDescriptorGenerator = new OneStoryContextGenerator("TestsTqAnswers");
            merger.MergeStrategies.SetStrategy("TestsTqAnswers", elementStrategy);

            // no context generator for this, because the default behavior of comparing the entire element is fine
            elementStrategy = ElementStrategy.CreateForKeyedElement("memberID", true);
            merger.MergeStrategies.SetStrategy("TestTqAnswer", elementStrategy);

            elementStrategy = ElementStrategy.CreateSingletonElement();
            elementStrategy.ContextDescriptorGenerator = new OneStoryContextGenerator("TransitionHistory");
            merger.MergeStrategies.SetStrategy("TransitionHistory", elementStrategy);

            // this doesn't need a merge strategy, because it's always add-only (and somehow on one user's machine
            //  OSE was spitting out a whole bunch of these with the same TransitionDateTime...
            // merger.MergeStrategies.SetStrategy("StateTransition", ElementStrategy.CreateForKeyedElement("TransitionDateTime", true));

            elementStrategy = ElementStrategy.CreateSingletonElement();
            elementStrategy.ContextDescriptorGenerator = new OneStoryContextGenerator("Verses");
            merger.MergeStrategies.SetStrategy("Verses", elementStrategy);

            // make the context generator use the 'label' "Line" even though it's a 'Verse'
            elementStrategy = ElementStrategy.CreateForKeyedElement("guid", true);
            elementStrategy.ContextDescriptorGenerator = new OneStoryContextGenerator("Line", "guid");
            merger.MergeStrategies.SetStrategy("Verse", elementStrategy);


            // no context generator for this, because the default behavior of comparing the entire element is fine
            elementStrategy = ElementStrategy.CreateForKeyedElement("lang", false);
            merger.MergeStrategies.SetStrategy("StoryLine", elementStrategy);


            // no context generator for this, because the default behavior of comparing the entire element is fine
            elementStrategy = ElementStrategy.CreateSingletonElement();
            merger.MergeStrategies.SetStrategy("Anchors", elementStrategy);

            // no context generator for this, because the default behavior of comparing the entire element is fine
            elementStrategy = ElementStrategy.CreateForKeyedElement("jumpTarget", false);
            merger.MergeStrategies.SetStrategy("Anchor", elementStrategy);


            elementStrategy = ElementStrategy.CreateSingletonElement();
            elementStrategy.ContextDescriptorGenerator = new OneStoryContextGenerator("ExegeticalHelps");
            merger.MergeStrategies.SetStrategy("ExegeticalHelps", elementStrategy);

            // there can be multiple exegeticalHelp elements, but a) their order doesn't matter and b) they don't need a key
            //  I think if I left this uncommented, then it would only allow one and if another user added one, it
            //  would just replace the one that's there... (i.e. I think that's what ElementStrategy.CreateSingletonElement
            //  means, so... commenting out):
            // merger.MergeStrategies.SetStrategy("exegeticalHelp", ElementStrategy.CreateSingletonElement());

            elementStrategy = ElementStrategy.CreateSingletonElement();
            elementStrategy.ContextDescriptorGenerator = new OneStoryContextGenerator("TestQuestions");
            merger.MergeStrategies.SetStrategy("TestQuestions", elementStrategy);

            elementStrategy = ElementStrategy.CreateForKeyedElement("guid", false);
            elementStrategy.ContextDescriptorGenerator = new OneStoryContextGenerator("TestQuestion", "guid");
            merger.MergeStrategies.SetStrategy("TestQuestion", elementStrategy);

            // no context generator for this, because the default behavior of comparing the entire element is fine
            elementStrategy = ElementStrategy.CreateForKeyedElement("lang", false);
            merger.MergeStrategies.SetStrategy("TestQuestionLine", elementStrategy);


            elementStrategy = ElementStrategy.CreateSingletonElement();
            elementStrategy.ContextDescriptorGenerator = new OneStoryContextGenerator("Answers");
            merger.MergeStrategies.SetStrategy("Answers", elementStrategy);

            // now the answer and retelling have a 2nd attribute which uniquely defines a singleton
            elementStrategy = new ElementStrategy(true)
                                  {
                                      MergePartnerFinder =
                                          new FindByMultipleKeyAttributes(new List<string> {"memberID", "lang"})
                                  };
            merger.MergeStrategies.ElementStrategies.Add("Answer", elementStrategy);

            elementStrategy = ElementStrategy.CreateSingletonElement();
            elementStrategy.ContextDescriptorGenerator = new OneStoryContextGenerator("Retellings");
            merger.MergeStrategies.SetStrategy("Retellings", elementStrategy);

            // now the answer and retelling have a 2nd attribute which uniquely defines a singleton
            elementStrategy = new ElementStrategy(true)
                                  {
                                      MergePartnerFinder =
                                          new FindByMultipleKeyAttributes(new List<string> {"memberID", "lang"})
                                  };
            merger.MergeStrategies.ElementStrategies.Add("Retelling", elementStrategy);

            elementStrategy = ElementStrategy.CreateSingletonElement();
            elementStrategy.ContextDescriptorGenerator = new OneStoryContextGenerator("ConsultantNotes");
            merger.MergeStrategies.SetStrategy("ConsultantNotes", elementStrategy);

            elementStrategy = ElementStrategy.CreateForKeyedElement("guid", true);
            elementStrategy.ContextDescriptorGenerator = new OneStoryContextGenerator("ConsultantConversation", "guid");
            merger.MergeStrategies.SetStrategy("ConsultantConversation", elementStrategy);


            // no context generator for this, because the default behavior of comparing the entire element is fine
            elementStrategy = ElementStrategy.CreateForKeyedElement("guid", true);
            elementStrategy.AttributesToIgnoreForMerging.Add("timeStamp");
            elementStrategy.ContextDescriptorGenerator = new OneStoryContextGenerator("ConsultantNote", "guid");
            merger.MergeStrategies.SetStrategy("ConsultantNote", elementStrategy);

            elementStrategy = ElementStrategy.CreateSingletonElement();
            elementStrategy.ContextDescriptorGenerator = new OneStoryContextGenerator("CoachNotes");
            merger.MergeStrategies.SetStrategy("CoachNotes", elementStrategy);

            elementStrategy = ElementStrategy.CreateForKeyedElement("guid", true);
            elementStrategy.ContextDescriptorGenerator = new OneStoryContextGenerator("CoachConversation", "guid");
            merger.MergeStrategies.SetStrategy("CoachConversation", elementStrategy);


            // no context generator for this, because the default behavior of comparing the entire element is fine
            elementStrategy = ElementStrategy.CreateForKeyedElement("guid", true);
            elementStrategy.AttributesToIgnoreForMerging.Add("timeStamp");
            elementStrategy.ContextDescriptorGenerator = new OneStoryContextGenerator("CoachNote", "guid");
            merger.MergeStrategies.SetStrategy("CoachNote", elementStrategy);
        }

        private XmlNode _projFile;
        public IEnumerable<IChangeReport> Find2WayDifferences(FileInRevision parent, FileInRevision child, HgRepository repository)
        {
            var listener = new ChangeAndConflictAccumulator();
            //pull the files out of the repository so we can read them
            using (var childFile = child.CreateTempFile(repository))
            using (var parentFile = parent.CreateTempFile(repository))
            {
                var differ = OneStoryDiffer.CreateFromFiles(parent, child, repository.PathToRepo, parentFile.Path, childFile.Path, listener);
                differ.ReportDifferencesToListener(out _projFile);
                return listener.Changes;
            }
        }

        public IChangePresenter GetChangePresenter(IChangeReport report, HgRepository repository)
        {
            if ((report as IXmlChangeReport) != null)
                return new OneStoryChangePresenter(report as IXmlChangeReport, _projFile, repository.PathToRepo);
            
            return new DefaultChangePresenter(report, repository);
        }

        public IEnumerable<IChangeReport> DescribeInitialContents(FileInRevision fileInRevision, TempFile file)
        {
            //this is never called because we said we don't present diffs; review is handled some other way
            throw new NotImplementedException();
        }

		/// <summary>
		/// Get a list or one, or more, extensions this file type handler can process
		/// </summary>
		/// <returns>A collection of extensions (without leading period (.)) that can be processed.</returns>
        public IEnumerable<string> GetExtensionsOfKnownTextFileTypes()
        {
            yield return "onestory";
        }

    	/// <summary>
    	/// Return the maximum file size that can be added to the repository.
    	/// </summary>
    	/// <remarks>
    	/// Return UInt32.MaxValue for no limit.
    	/// </remarks>
    	public uint MaximumFileSize
    	{
			get { return UInt32.MaxValue; }
    	}
    }
}

