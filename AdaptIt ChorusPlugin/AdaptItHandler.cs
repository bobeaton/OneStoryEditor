using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;
using Chorus.merge;
using Chorus.merge.xml.generic;
using Chorus.VcsDrivers.Mercurial;
using System.ComponentModel.Composition;
using Chorus.FileTypeHandlers;
using SIL.Progress;
using SIL.IO;
using SIL.Reporting;
using OseCommon;

namespace AdaptIt_ChorusPlugin
{
    [Export(typeof(IChorusFileTypeHandler))]
    public class AdaptItFileHandler : IChorusFileTypeHandler
    {
        internal AdaptItFileHandler()
 		{}

        public bool CanDiffFile(string pathToFile)
        {
            return false;
        }

        /// <summary>
        /// This flag indicates whether the KB file is from the pre-6.0.0 version of Adapt It (false) 
        /// or post-6.0.0 (true)
        /// </summary>
        public bool IsKbV2;

        public bool CanMergeFile(string pathToFile)
        {
            if (Path.GetExtension(pathToFile).ToLower() != ".xml")
                return false;

            //inexpensively detect if this is an AdaptItKnowledgeBase
            using (var reader = File.OpenText(pathToFile))
            {
                for (int i = 0; i < 10; i++)
                {
                    var line = reader.ReadLine();
                    if (String.IsNullOrEmpty(line)) 
                        continue;
                    if (line.Contains("<KB docVersion"))
                        return true;
                    if (!line.Contains("<KB kbVersion"))
                        continue;
                    IsKbV2 = true;
                        return true;
                }
            }
            return false;
        }

        public bool CanPresentFile(string pathToFile)
        {
            return false;
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
            System.Diagnostics.Debug.Fail("stop here");

            var merger = new XmlMerger(mergeOrder.MergeSituation);
            SetupElementStrategies(merger);

            merger.EventListener = mergeOrder.EventListener;
            var result = merger.MergeFiles(mergeOrder.pathToOurs, mergeOrder.pathToTheirs, mergeOrder.pathToCommonAncestor);

            // write it out using XDocument so it's easier to view with wincmp
            //  (plus we need to sort the KB and the easiest way seems to be with xslt)
#if !DontUseXDocument
            XElement elem = XElement.Parse(result.MergedNode.OuterXml);

            // create the root portions of the XML document and tack on the fragment we've been building
            var doc = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XComment(Properties.Resources.AdaptItKbDescription));

            // sort it
            var xslt = new XslCompiledTransform();
            xslt.Load(XmlReader.Create(new StringReader(Properties.Resources.SortAIKB)));

            using (XmlWriter writer = doc.CreateWriter())
            {
                // Execute the transform and output the results to a writer.
                xslt.Transform(elem.CreateReader(), writer);
#if MONO
				// Note that mono closes the writer in Dispose. If the writer is already closed
				// by the call to writer.Close mono throws an InvalidOperationException.
#else
                writer.Close();
#endif
            }


            // it seems that there may be some situations in which the file is not properly written to the disk (see 
            //  https://stackoverflow.com/questions/49260358/what-could-cause-an-xml-file-to-be-filled-with-null-characters
            //  for example). To avoid this situation, let's use the SerializeToFileWithWriteThrough method in SIL.Core 
            //  to see if that ends this problem
            // doc.Save(mergeOrder.pathToOurs);
            OseXmlSerializer.SaveDoc(mergeOrder.pathToOurs, doc, (Exception error) => Logger.WriteError(error));
#else
            File.WriteAllText(mergeOrder.pathToOurs, result.MergedNode.OuterXml);
#endif
        }

        /* rde: 
         * here is an example of the KB structure:
          <KB ...>
            <MAP mn="1">                    // @mn = number of words/phrase (in TU/@k)
              <TU f="0" k="Δ">              // @f = forcing (something in the AI UI; not sure)
                                            // @k = source word 'key'
                <RS n="1" a="ασδγδ" />      // @n = number of occurrences of this adaptation
                                            // @a = target word 'adaptation'
              </TU>
            </MAP>
          </KB>
         * 
         * V6.0.0 of AI adds these new attributes to the RS element:
         *  @df = DeleteFlag (indicates whether the adaptation was deleted)
         *  @cDT = time/date stamp of the creation of this adaptation
         *  @mDT = time/date stamp of the last modification of this adaptation
         *  @dDT = time/date stamp of the deletion of this adaptation
         *  @wC = stands for 'who created' and gives a username/computer mapping for 
         *          the creator/last modifier
         * However, we don't care about any of these (except TU/@k = source word
         * and RS/@a = target word (and I guess MAP/mn, which AI seems to need to
         * be sorted in order of increasing value of 'mn'.
         * Doing the sorting in the MergeStrategies, however, is very costly, so
         * we'll pretend that the order doesn't matter and then just before saving
         * the result, we do an xslt sort (which is very efficient).
         * Also, we will explicitly ask the MergeStrategies to ignore all of these
         * other superflueous attributes or we get lots of conflicts for very (well
         * zero) return on investment.
        */
        private void SetupElementStrategies(XmlMerger merger)
        {
            // new versions of AI no longer use this element
            //  merger.MergeStrategies.SetStrategy("AdaptItKnowledgeBase", ElementStrategy.CreateSingletonElement());
            var elementStrategy = ElementStrategy.CreateSingletonElement();
            elementStrategy.ContextDescriptorGenerator = new AdaptItContextGenerator("KB", null);
            merger.MergeStrategies.SetStrategy("KB", elementStrategy);

            // Are the listed attributes really unique? rde: yes and their order is probably crucial
            //  rde: 4/4/11: using order = true makes the merging incredibly slow. We sort it afterwards
            //  anyway, so pretend the order doesn't matter.
            elementStrategy = ElementStrategy.CreateForKeyedElement("mn", false);
            elementStrategy.ContextDescriptorGenerator = new AdaptItContextGenerator("MAP", "mn");
            merger.MergeStrategies.SetStrategy("MAP", elementStrategy);
            
            // ignore whether one user wanted to 'force' the source word or not (too little
            //  importance to have conflicts over)
            //  rde: 4/4/11: using order = true makes the merging incredibly slow. We sort it afterwards
            //  anyway, so pretend the order doesn't matter.
            elementStrategy = ElementStrategy.CreateForKeyedElement("k", false);
            elementStrategy.AttributesToIgnoreForMerging.Add("f");
            elementStrategy.ContextDescriptorGenerator = new AdaptItContextGenerator("TU", "k");
            merger.MergeStrategies.SetStrategy("TU", elementStrategy);

            // ... whereas for RS@a, if there's a conflict, just pick one or the other is fine (if there 
            //  were the ability, what we'd want to do is add the differentials from the ancestor--e.g. 
            //  if ancestor has 1 and 'mine' is 3 (I've added 2 occurrences of this interpretation) and 
            //  theirs is 2 (they've added 1 new occurrence of this interpretation), then make it 4
            //  =1 + 2 + 1. This is what we're really want to do, but otherwise, it isn't a big deal
            //  as far as AI or other users are concerned).
            //  rde: 4/4/11: using order = true makes the merging incredibly slow. We sort it afterwards
            //  anyway, so pretend the order doesn't matter.
            elementStrategy = ElementStrategy.CreateForKeyedElement("a", false);
            elementStrategy.ContextDescriptorGenerator = new AdaptItContextGenerator("RS", "a");
            // rde: 10/12/11: adding ignore for merging for all the new attributes
            //  added to AI in 6.0.0 (someone's going to win and none of these matter)
            elementStrategy.AttributesToIgnoreForMerging.AddRange(new[]
                                                                      {
                                                                          "n",
                                                                          "df",
                                                                          "cDT",
                                                                          "mDT",
                                                                          "dDT",
                                                                          "wC"
                                                                      });

            merger.MergeStrategies.SetStrategy("RS", elementStrategy);
          
            //Banana leaves are great for covering food in a mumu.  But the gorgor leaf is better than that of a banana for cooking manget.
        }

        public IEnumerable<IChangeReport> Find2WayDifferences(FileInRevision parent, FileInRevision child, HgRepository repository)
        {
            //this is never called because we said we don't do diffs yet; review is handled some other way
            throw new NotImplementedException();
        }

        public IChangePresenter GetChangePresenter(IChangeReport report, HgRepository repository)
        {
            //this is never called because we said we don't present diffs; review is handled some other way
            throw new NotImplementedException();
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
            yield return "xml";
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