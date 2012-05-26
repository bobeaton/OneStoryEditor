using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using NetLoc;

namespace OneStoryProjectEditor
{
    public partial class SayMoreImportForm : Form
    {
        public string StoryName;
        public List<string> VernacularLines;
        public List<string> BackTranslationLines;

        private const int CnColumnClickToInstall = 0;
        private const int CnColumnTitle = 2;

        public SayMoreImportForm()
        {
            InitializeComponent();
            Localizer.Ctrl(this);
            InitGrid();
        }

        private void InitGrid()
        {
            // this monsterous Linq statement says: give me any sub-folders of "<My Document>\SayMore" (which
            //  are the project names), which have a 'Events' sub-folder which itself has at least one sub-folder
            //  that contains a file with a '.eaf' extension (which is the file we get the transcriptions out of)
            var projectFolders = Directory.GetDirectories(ProjectSettings.SayMoreFolderRoot)
                .Where(fp => Directory.GetDirectories(fp)
                                 .Any(fps => (Path.GetFileName(fps) == "Events") &&
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
            var eventsFolder = Path.Combine(Path.Combine(ProjectSettings.SayMoreFolderRoot,
                                                         listBoxProjects.SelectedItem.ToString()),
                                            "Events");
            var eventFolders = Directory.GetDirectories(eventsFolder)
                .Where(fpe => Directory.GetFiles(fpe)
                                  .Any(fpef => Path.GetExtension(fpef) == ".eaf"));

            foreach (var eventFolder in eventFolders)
            {
                var eventFile = Directory.GetFiles(eventFolder, "*.event").FirstOrDefault();
                if (!File.Exists(eventFile))
                    continue;

                var transcriptionFile = Directory.GetFiles(eventFolder, "*.eaf").FirstOrDefault();
                if (!File.Exists(transcriptionFile))
                    continue;

                var eventName = Path.GetFileName(eventFolder);
                var doc = XDocument.Load(eventFile);
                if (doc.Root == null)
                    continue;

                var title = GetSafeValue(doc, "title");
                var date = GetSafeValue(doc, "date");
                var speaker = GetSafeValue(doc, "participants");
                var aobjs = new object[] {Localizer.Str("Click to import"), eventName, title, date, speaker};
                var nIndex = dataGridViewEvents.Rows.Add(aobjs);
                dataGridViewEvents.Rows[nIndex].Tag = transcriptionFile;
            }
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
                tabControl.SelectTab(tabPageEvents);
        }

        private void TabControlSelecting(object sender, TabControlCancelEventArgs e)
        {
            if (e.TabPage == tabPageEvents)
            {
                InitializeGrid();
            }
        }

        private void DataGridViewEventsCellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= dataGridViewEvents.Rows.Count)
                return;

            var theRow = dataGridViewEvents.Rows[e.RowIndex];
            if (e.ColumnIndex != CnColumnClickToInstall)
                return;

            var eafFile = theRow.Tag as string;
            if (!File.Exists(eafFile))
            {
                LocalizableMessageBox.Show(
                    Localizer.Str(
                        "Unable to find the transcription file (the file with the .eaf extension)! Was it just deleted?"),
                    StoryEditor.OseCaption);
                return;
            }

            StoryName = theRow.Cells[CnColumnTitle].Value as string;
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
            DialogResult = DialogResult.OK;
            Close();
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
    }
}
