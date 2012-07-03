using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using NetLoc;

namespace OneStoryProjectEditor
{
    public partial class SayMoreImportForm : TopForm
    {
        public string StoryName { get; set; }
        public string Crafter { get; set; }
        public string FullRecordingFileSpec { get; set; }
        public List<string> VernacularLines;
        public List<string> BackTranslationLines;

        private const int CnColumnClickToInstall = 0;
        private const int CnColumnTitle = 2;
        private const int CnColumnCrafter = 4;

        private const string CstrOrigFullRecordingSuffix = "_Original.wav";
        private const string CstrOrigFullRecordingSuffix2 = "_OralTranslation.wav";

        private readonly ProjectSettings _projSettings;

        public SayMoreImportForm(StoryData storyData, ProjectSettings projSettings)
        {
            _projSettings = projSettings;
            InitializeComponent();
            Localizer.Ctrl(this);
            InitGrid();

            // if we have a current story, then we can import into a retelling also
            if (storyData == null)
                radioButtonAsRetelling.Enabled = false;
            else
                radioButtonAsRetelling.Text = String.Format(Localizer.Str("Retelling {0} of story {1}"),
                                                            storyData.CraftingInfo.TestersToCommentsRetellings.Count + 1,
                                                            storyData.Name);
        }

        private void InitGrid()
        {
            // this monsterous Linq statement says: give me any sub-folders of "<My Document>\SayMore" (which
            //  are the project names), which have a 'Sessions' sub-folder which itself has at least one sub-folder
            //  that contains a file with a '.eaf' extension (which is the file we get the transcriptions out of)
            var projectFolders = Directory.GetDirectories(ProjectSettings.SayMoreFolderRoot)
                .Where(fp => Directory.GetDirectories(fp)
                                 .Any(fps => (Path.GetFileName(fps) == "Sessions") &&
                                             (Directory.GetDirectories(fps)
                                                 .Any(fpe => Directory.GetFiles(fpe)
                                                                 .Any(fpef => Path.GetExtension(fpef) == ".eaf")))))
                .Select(Path.GetFileName).ToArray<object>();

            if (!projectFolders.Any())
            {
                LocalizableMessageBox.Show(
                    Localizer.Str("Unable to find any SayMore projects with transcribed events!?"),
                    StoryEditor.OseCaption);
                return;
            }

            listBoxProjects.Items.AddRange(projectFolders);
        }

        private void InitializeGrid()
        {
            dataGridViewEvents.Rows.Clear();
            var eventsFolder = Path.Combine(Path.Combine(ProjectSettings.SayMoreFolderRoot,
                                                         listBoxProjects.SelectedItem.ToString()),
                                            "Sessions");
            var eventFolders = Directory.GetDirectories(eventsFolder)
                .Where(fpe => Directory.GetFiles(fpe)
                                  .Any(fpef => Path.GetExtension(fpef) == ".eaf"));

            foreach (var eventFolder in eventFolders)
            {
                var files = Directory.GetFiles(eventFolder);
                var eventFile = files.FirstOrDefault(fp => Path.GetExtension(fp) == ".session");
                if (String.IsNullOrEmpty(eventFile) || !File.Exists(eventFile))
                    continue;

                var transcriptionFile = files.FirstOrDefault(fp => Path.GetExtension(fp) == ".eaf");
                if (String.IsNullOrEmpty(transcriptionFile) || !File.Exists(transcriptionFile))
                    continue;

                var origFullRecording = GetOriginalRecordingFilePath(CstrOrigFullRecordingSuffix, files);
                if (String.IsNullOrEmpty(origFullRecording))
                    origFullRecording = GetOriginalRecordingFilePath(CstrOrigFullRecordingSuffix2, files);

                var eventName = Path.GetFileName(eventFolder);
                var doc = XDocument.Load(eventFile);
                if (doc.Root == null)
                    continue;

                var title = GetSafeValue(doc, "title");
                var date = GetSafeValue(doc, "date");
                var speaker = GetSafeValue(doc, "participants");
                var aobjs = new object[] {Localizer.Str("Click to import"), eventName, title, date, speaker};
                var nIndex = dataGridViewEvents.Rows.Add(aobjs);
                dataGridViewEvents.Rows[nIndex].Tag = Tuple.Create(transcriptionFile, origFullRecording);
            }
        }

        private static string GetOriginalRecordingFilePath(string strFileSuffix, string[] files)
        {
            var origFullRecording =
                files.FirstOrDefault(
                    fp =>
                    fp.IndexOf(strFileSuffix) == (fp.Length - strFileSuffix.Length));
            return origFullRecording;
        }

        private static string GetSafeValue(XDocument doc, string strName)
        {
            Debug.Assert(doc.Root != null);
            var xElement = doc.Root.Element(strName);
            if (xElement != null)
            {
                return xElement.Value;
            }
            return null;
        }

        private void ListBoxProjectsSelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxProjects.SelectedIndex != -1)
                tabControlImport.SelectTab(tabPageEvents);
        }

        private void TabControlSelecting(object sender, TabControlCancelEventArgs e)
        {
            if (e.TabPage != tabPageProjects)
            {
                if ((listBoxProjects.Items.Count == 0) || (listBoxProjects.SelectedIndex == -1))
                {
                    LocalizableMessageBox.Show(
                        Localizer.Str(
                            "First select the project to import from on the Projects tab (if there are no projects with importable data, then none will be listed)"),
                        StoryEditor.OseCaption);
                    return;
                }
            }

            if ((e.TabPage != tabPageProjects) && (e.TabPage != tabPageEvents))
            {
                if (VernacularLines == null)
                {
                    LocalizableMessageBox.Show(
                        Localizer.Str("First click on one of the Session buttons in the Sessions tab (if there are none listed, then no importable data was found)"),
                        StoryEditor.OseCaption);
                    return;
                }
            }

            if (e.TabPage == tabPageEvents)
            {
                InitializeGrid();
            }
            else if (e.TabPage == tabPageFieldMatching)
            {
                if (radioButtonNewStory.Checked)
                {
                    SetFieldVisibility(_projSettings.Vernacular.HasData,
                                       _projSettings.NationalBT.HasData,
                                       _projSettings.InternationalBT.HasData,
                                       _projSettings.FreeTranslation.HasData);
                }
                else
                {
                    SetFieldVisibility(_projSettings.ShowRetellings.Vernacular,
                                       _projSettings.ShowRetellings.NationalBt,
                                       _projSettings.ShowRetellings.InternationalBt,
                                       false);
                }
            }
        }

        private void SetFieldVisibility(bool bVernacular, bool bNationalBt, bool bInternationalBt, bool bFreeTr)
        {
            bool bTranscriptionChosen = false, bTranslationChosen = false;
            if (bVernacular)
            {
                // radioButtonVernacularTranslation.Visible =  doesn't make sense for this field to be the translation
                radioButtonVernacularTranscription.Visible = true;

                bTranscriptionChosen = radioButtonVernacularTranscription.Checked = true;
            }
            else
                radioButtonVernacularTranscription.Visible =
                    radioButtonVernacularTranslation.Visible = false;

            if (bNationalBt)
            {
                radioButtonNationalBtTranscription.Visible =
                    radioButtonNationalBtTranslation.Visible = true;

                // if we haven't chosen the transcription yet, then this would be it
                if (!bTranscriptionChosen)
                    bTranscriptionChosen = radioButtonNationalBtTranscription.Checked = true;
                else
                    // otherwise, this is the default for translation 
                    bTranslationChosen = radioButtonNationalBtTranslation.Checked = true;
            }
            else
                radioButtonNationalBtTranscription.Visible =
                    radioButtonNationalBtTranslation.Visible = false;

            if (bInternationalBt)
            {
                radioButtonInternationalBtTranscription.Visible =
                    radioButtonInternationalBtTranslation.Visible = true;

                // if we haven't chosen the transcription yet, then this would be it
                if (!bTranscriptionChosen)
                    radioButtonInternationalBtTranscription.Checked = true;
                    // otherwise, if the translation hasn't yet been chosen, then this would be it
                else if (!bTranslationChosen)
                    bTranslationChosen = radioButtonInternationalBtTranslation.Checked = true;
            }
            else
                radioButtonInternationalBtTranscription.Visible =
                    radioButtonInternationalBtTranslation.Visible = false;

            if (bFreeTr)
            {
                radioButtonFreeTrTranscription.Visible =
                    radioButtonFreeTrTranslation.Visible = true;

                if (!bTranslationChosen)
                    radioButtonFreeTrTranslation.Checked = true;
            }
            else
                radioButtonFreeTrTranscription.Visible =
                    radioButtonFreeTrTranslation.Visible = false;
        }

        private void DataGridViewEventsCellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if ((e.RowIndex < 0) || (e.RowIndex >= dataGridViewEvents.Rows.Count) || 
                (e.ColumnIndex != CnColumnClickToInstall))
                return;

            var theRow = dataGridViewEvents.Rows[e.RowIndex];
            var files = theRow.Tag as Tuple<string, string>;
            Debug.Assert(files != null);
            var eafFile = files.Item1;
            if (!File.Exists(eafFile))
            {
                LocalizableMessageBox.Show(
                    Localizer.Str(
                        "Unable to find the transcription file (the file with the .eaf extension)! Was it just deleted?"),
                    StoryEditor.OseCaption);
                return;
            }

            FullRecordingFileSpec = files.Item2;
            StoryName = theRow.Cells[CnColumnTitle].Value as string;
            Crafter = theRow.Cells[CnColumnCrafter].Value as string;
            var doc = XDocument.Load(eafFile);
            List<string> lstVernacular, lstBackTranslation;
            GetTier(doc, "Transcription", out lstVernacular);
            GetTier(doc, "Translation", out lstBackTranslation);
            var nLen = Math.Max(lstVernacular.Count, lstBackTranslation.Count);
            if (nLen <= 0)
            {
                LocalizableMessageBox.Show(
                    Localizer.Str(
                        "Unable to find any transcription or back-translation data in the .eaf file! Has the event been transcribed and/or back-translated yet?"),
                    StoryEditor.OseCaption);
                return;
            }

            VernacularLines = lstVernacular;
            BackTranslationLines = lstBackTranslation;
            tabControlImport.SelectTab(tabPageChooseStory);
        }

        private static void GetTier(XContainer doc, string strType, out List<string> lst)
        {
            var tier = doc.Descendants("TIER")
                .Where(t =>
                           {
                               var xAttribute = t.Attribute("LINGUISTIC_TYPE_REF");
                               return xAttribute != null && xAttribute.Value == strType;
                           }).FirstOrDefault();

            if (tier == null)
            {
                lst = new List<string>();
                return;
            }

            lst = (from annotation in tier.Elements("ANNOTATION") 
                   select annotation.Descendants("ANNOTATION_VALUE").FirstOrDefault() into xValue 
                   where xValue != null 
                   select xValue.Value).ToList();
        }

        public bool CreateNewStory { get; set; }
        public string AsRetellingInStory { get; set; }
        public StoryEditor.TextFields TranscriptionField { get; set; }
        public StoryEditor.TextFields TranslationField { get; set; }

        private void ButtonImportClick(object sender, EventArgs e)
        {
            CreateNewStory = radioButtonNewStory.Checked;
            
            TranscriptionField = WhichField(radioButtonVernacularTranscription,
                                            radioButtonNationalBtTranscription,
                                            radioButtonInternationalBtTranscription,
                                            radioButtonFreeTrTranscription);
            TranslationField = WhichField(radioButtonVernacularTranslation,
                                            radioButtonNationalBtTranslation,
                                            radioButtonInternationalBtTranslation,
                                            radioButtonFreeTrTranslation);

            if (TranscriptionField == TranslationField)
            {
                LocalizableMessageBox.Show(
                    String.Format(
                        Localizer.Str(
                            "You can't import both the transcription and translation from SayMore into the {0} field"),
                        TranslationField),
                    StoryEditor.OseCaption);
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private static StoryEditor.TextFields WhichField(RadioButton radioButtonVernacular, RadioButton radioButtonNationalBt, 
            RadioButton radioButtonInternationalBt, RadioButton radioButtonFreeTr)
        {
            StoryEditor.TextFields value;
            if ((((value = StoryEditor.TextFields.Vernacular) == StoryEditor.TextFields.Vernacular) && radioButtonVernacular.Checked) ||
                (((value = StoryEditor.TextFields.NationalBt) == StoryEditor.TextFields.NationalBt) && radioButtonNationalBt.Checked) ||
                (((value = StoryEditor.TextFields.InternationalBt) == StoryEditor.TextFields.InternationalBt) && radioButtonInternationalBt.Checked) ||
                (((value = StoryEditor.TextFields.FreeTranslation) == StoryEditor.TextFields.FreeTranslation) && radioButtonFreeTr.Checked))
            {
                return value;
            }
            return value;
        }

        private void ButtonMoveToChooseFieldTabClick(object sender, EventArgs e)
        {
            tabControlImport.SelectTab(tabPageFieldMatching);
        }
    }
}
