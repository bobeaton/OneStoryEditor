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
    // Imports a transcribed text from another program into a new story, a retelling, or
    //  the answers to the test questions. With SayMore, the user first picks the project
    //  and session (which has the .eaf file); for ELAN (.eaf) and FLEx (.flextext) files,
    //  the caller reads the file and passes in the ImportedText, so only the last tab is
    //  shown: the one where each tier found in the file is mapped to an OSE field.
    public partial class SayMoreImportForm : TopForm
    {
        public string StoryName { get; set; }
        public string Crafter { get; set; }
        public string FullRecordingFileSpec { get; set; }

        // the tiers to import and which field of the line each goes into
        public List<ImportMapping> Mappings = new List<ImportMapping>();

        // for a retelling or answers: the story lines (or test questions) that line i of
        //  each (aligned) tier in Mappings goes with
        public List<AlignTarget> AlignTargets;

        // everything that was read in (e.g. including tiers that weren't mapped to a field)
        public ImportedText ImportedText
        {
            get { return _importedText; }
        }

        private const int CnColumnClickToInstall = 0;
        private const int CnColumnTitle = 2;
        private const int CnColumnCrafter = 4;

        private const string CstrOrigFullRecordingSuffix = "_Original.wav";
        private const string CstrOrigFullRecordingSuffix2 = "_OralTranslation.wav";

        private readonly ProjectSettings _projSettings;
        private readonly StoryData _storyData;
        private ImportedText _importedText;

        // version used by Localization
        private SayMoreImportForm()
        {
            InitializeComponent();
            Localizer.Ctrl(this);
        }

        // import from a SayMore session (browses the SayMore projects)
        public SayMoreImportForm(StoryData storyData, ProjectSettings projSettings)
        {
            _projSettings = projSettings;
            _storyData = storyData;
            InitializeComponent();
            Localizer.Ctrl(this);
            InitImportTypes(storyData);
            InitGrid();
        }

        // import from a file that's already been read (e.g. ELAN .eaf or FLEx .flextext)
        public SayMoreImportForm(StoryData storyData, ProjectSettings projSettings, ImportedText importedText)
        {
            _projSettings = projSettings;
            _storyData = storyData;
            InitializeComponent();
            Localizer.Ctrl(this);
            InitImportTypes(storyData);

            Text = String.Format(Localizer.Str("Import from {0}"), Path.GetFileName(importedText.SourceFile));
            tabControlImport.TabPages.Remove(tabPageProjects);
            tabControlImport.TabPages.Remove(tabPageEvents);

            _importedText = importedText;
            StoryName = importedText.ToString();
            Crafter = importedText.Speaker;
            FullRecordingFileSpec = importedText.MediaFile;
            InitTierGrid();
        }

        private void InitImportTypes(StoryData storyData)
        {
            // if we have a current story, then we can import into a retelling also
            if (storyData == null)
                radioButtonAsRetelling.Enabled = radioButtonAsAnswers.Enabled = false;
            else
            {
                radioButtonAsRetelling.Text = String.Format(Localizer.Str("Retelling {0} of story {1}"),
                                                            storyData.CraftingInfo.TestersToCommentsRetellings.Count + 1,
                                                            storyData.Name);

                radioButtonAsAnswers.Text = String.Format(Localizer.Str("Answer test {0} of story {1}"),
                                                          storyData.CraftingInfo.TestersToCommentsTqAnswers.Count + 1,
                                                          storyData.Name);
            }
            LayoutButtons();
        }

        private void InitGrid()
        {
            // this monsterous Linq statement says: give me any sub-folders of "<My Document>\SayMore" (which
            //  are the project names), which have a 'Sessions' sub-folder which itself has at least one sub-folder
            //  that contains a file with a '.eaf' extension (which is the file we get the transcriptions out of)
            var projectFolders = !Directory.Exists(ProjectSettings.SayMoreFolderRoot)
                ? new object[0]
                : Directory.GetDirectories(ProjectSettings.SayMoreFolderRoot)
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
                // newer versions of SayMore have a .session file; older ones, an .event file
                var files = Directory.GetFiles(eventFolder);
                var eventFile = files.FirstOrDefault(fp => Path.GetExtension(fp) == ".session") ??
                                files.FirstOrDefault(fp => Path.GetExtension(fp) == ".event");
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
            // when importing from a file, the field matching tab is the only one
            if (!tabControlImport.TabPages.Contains(tabPageProjects))
                return;

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
                if (_importedText == null)
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
                InitTierGrid();
            }
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

            var importedText = EafReader.Read(eafFile);
            if ((importedText == null) || !importedText.HasData)
            {
                LocalizableMessageBox.Show(
                    Localizer.Str(
                        "Unable to find any transcription or back-translation data in the .eaf file! Has the event been transcribed and/or back-translated yet?"),
                    StoryEditor.OseCaption);
                return;
            }

            FullRecordingFileSpec = files.Item2 ?? importedText.MediaFile;
            StoryName = theRow.Cells[CnColumnTitle].Value as string;
            Crafter = theRow.Cells[CnColumnCrafter].Value as string;
            _importedText = importedText;
            tabControlImport.SelectTab(tabPageFieldMatching);
        }

        public enum SaymoreImportTypes
        {
            NewStory,
            Retelling,
            Answers
        }
        public SaymoreImportTypes SaymoreImportType { get; set; }

        private SaymoreImportTypes SelectedImportType
        {
            get
            {
                return (radioButtonNewStory.Checked)
                           ? SaymoreImportTypes.NewStory
                           : (radioButtonAsRetelling.Checked)
                                 ? SaymoreImportTypes.Retelling
                                 : SaymoreImportTypes.Answers;
            }
        }

        private class FieldChoice
        {
            public string Name { get; set; }
            public StoryEditor.TextFields Field { get; set; }
        }

        // the fields that are configured in the project for the type of import
        private List<StoryEditor.TextFields> AvailableFields
        {
            get
            {
                var fields = new List<StoryEditor.TextFields>();
                switch (SelectedImportType)
                {
                    case SaymoreImportTypes.NewStory:
                        AddIf(fields, _projSettings.Vernacular.HasData, StoryEditor.TextFields.Vernacular);
                        AddIf(fields, _projSettings.NationalBT.HasData, StoryEditor.TextFields.NationalBt);
                        AddIf(fields, _projSettings.InternationalBT.HasData, StoryEditor.TextFields.InternationalBt);
                        AddIf(fields, _projSettings.FreeTranslation.HasData, StoryEditor.TextFields.FreeTranslation);
                        break;
                    case SaymoreImportTypes.Retelling:
                        AddShowFields(fields, _projSettings.ShowRetellings);
                        break;
                    case SaymoreImportTypes.Answers:
                        AddShowFields(fields, _projSettings.ShowAnswers);
                        break;
                }
                return fields;
            }
        }

        private static void AddShowFields(List<StoryEditor.TextFields> fields, ShowLanguageFields show)
        {
            AddIf(fields, show.Vernacular, StoryEditor.TextFields.Vernacular);
            AddIf(fields, show.NationalBt, StoryEditor.TextFields.NationalBt);
            AddIf(fields, show.InternationalBt, StoryEditor.TextFields.InternationalBt);
        }

        private static void AddIf(List<StoryEditor.TextFields> fields, bool bAdd, StoryEditor.TextFields field)
        {
            if (bAdd)
                fields.Add(field);
        }

        private ProjectSettings.LanguageInfo LanguageInfo(StoryEditor.TextFields field)
        {
            switch (field)
            {
                case StoryEditor.TextFields.Vernacular:
                    return _projSettings.Vernacular;
                case StoryEditor.TextFields.NationalBt:
                    return _projSettings.NationalBT;
                case StoryEditor.TextFields.InternationalBt:
                    return _projSettings.InternationalBT;
                case StoryEditor.TextFields.FreeTranslation:
                    return _projSettings.FreeTranslation;
            }
            return null;
        }

        private string FieldDisplayName(StoryEditor.TextFields field)
        {
            string strField;
            switch (field)
            {
                case StoryEditor.TextFields.Vernacular:
                    strField = Localizer.Str("Story language");
                    break;
                case StoryEditor.TextFields.NationalBt:
                    strField = Localizer.Str("National/Regional language BT");
                    break;
                case StoryEditor.TextFields.InternationalBt:
                    strField = Localizer.Str("English language BT");
                    break;
                case StoryEditor.TextFields.FreeTranslation:
                    strField = Localizer.Str("Free Translation");
                    break;
                default:
                    return Localizer.Str("(don't import)");
            }

            var li = LanguageInfo(field);
            return ((li != null) && !String.IsNullOrEmpty(li.LangName))
                       ? String.Format("{0} ({1})", strField, li.LangName)
                       : strField;
        }

        // fills the grid with one row per tier found in the file, each with a drop down
        //  of the fields (available for this type of import) it could be imported into
        private void InitTierGrid()
        {
            if (_importedText == null)
                return;

            var availableFields = AvailableFields;
            var choices = new[] {StoryEditor.TextFields.Undefined}
                .Concat(availableFields)
                .Select(f => new FieldChoice {Name = FieldDisplayName(f), Field = f})
                .ToList();

            dataGridViewTiers.Rows.Clear();
            ColumnTierField.DataSource = choices;
            ColumnTierField.DisplayMember = "Name";
            ColumnTierField.ValueMember = "Field";
            ColumnTierField.ValueType = typeof(StoryEditor.TextFields);

            var defaults = DefaultMapping(_importedText, availableFields);
            foreach (var tier in _importedText.Tiers)
            {
                StoryEditor.TextFields field;
                if (!defaults.TryGetValue(tier, out field))
                    field = StoryEditor.TextFields.Undefined;

                var nIndex = dataGridViewTiers.Rows.Add(tier.Name, tier.LangCode, tier.FirstNonEmptyLine, field);
                dataGridViewTiers.Rows[nIndex].Tag = tier;
            }
        }

        // The initial choice for which tier goes into which field (the user can change them):
        //  the transcription goes into the story language field, and the translations into
        //  the BT fields. When there's also a word-by-word gloss (e.g. from Seth's MTT tool
        //  or FLEx), the gloss goes into the national language BT and the (free) translation
        //  into the English BT (cf. PR #18). Otherwise, they go into the next free BT field
        //  (which is what the SayMore import has always done).
        private Dictionary<ImportedTier, StoryEditor.TextFields> DefaultMapping(ImportedText importedText,
                                                                                List<StoryEditor.TextFields> availableFields)
        {
            var mapping = new Dictionary<ImportedTier, StoryEditor.TextFields>();
            var free = new List<StoryEditor.TextFields>(availableFields);

            Func<ImportedTier, StoryEditor.TextFields, bool> assign = (tier, field) =>
            {
                if ((tier == null) || mapping.ContainsKey(tier) || !free.Contains(field))
                    return false;
                mapping[tier] = field;
                free.Remove(field);
                return true;
            };

            var baseline = importedText.Baseline;
            if ((baseline != null) && !assign(baseline, StoryEditor.TextFields.Vernacular) && free.Any())
                assign(baseline, free.First());

            var translations = importedText.Tiers.Where(t => t.Kind == ImportedTier.TierKind.PhraseTranslation).ToList();
            var glosses = importedText.Tiers.Where(t => t.Kind == ImportedTier.TierKind.WordGloss).ToList();

            if (glosses.Any() && translations.Any() &&
                free.Contains(StoryEditor.TextFields.NationalBt) && free.Contains(StoryEditor.TextFields.InternationalBt))
            {
                assign(glosses.First(), StoryEditor.TextFields.NationalBt);
                assign(translations.First(), StoryEditor.TextFields.InternationalBt);
            }

            var btFields = new[]
            {
                StoryEditor.TextFields.NationalBt,
                StoryEditor.TextFields.InternationalBt,
                StoryEditor.TextFields.FreeTranslation
            };

            foreach (var tier in translations.Concat(glosses))
            {
                var field = btFields.FirstOrDefault(free.Contains);
                if (field == StoryEditor.TextFields.Undefined)
                    break;
                assign(tier, field);
            }

            return mapping;
        }

        private void DataGridViewTiersDataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            // e.g. a value no longer in the drop down after changing the type of import
            e.ThrowException = false;
        }

        // the tiers the user chose to import and the fields they go in (or null, after
        //  telling the user why, if that isn't possible)
        private List<ImportMapping> GetMappings()
        {
            dataGridViewTiers.EndEdit();
            SaymoreImportType = SelectedImportType;

            var mappings = new List<ImportMapping>();
            foreach (DataGridViewRow row in dataGridViewTiers.Rows)
            {
                var tier = row.Tag as ImportedTier;
                var value = row.Cells[ColumnTierField.Index].Value;
                if ((tier == null) || !(value is StoryEditor.TextFields))
                    continue;

                var field = (StoryEditor.TextFields)value;
                if (field == StoryEditor.TextFields.Undefined)
                    continue;

                var dup = mappings.FirstOrDefault(m => m.Field == field);
                if (dup != null)
                {
                    LocalizableMessageBox.Show(
                        String.Format(
                            Localizer.Str("You can't import both the '{0}' and '{1}' tiers into the {2} field"),
                            dup.Tier.Name, tier.Name, FieldDisplayName(field)),
                        StoryEditor.OseCaption);
                    return null;
                }

                mappings.Add(new ImportMapping {Field = field, Tier = tier});
            }

            if (!mappings.Any())
            {
                string strMessage;
                if (AvailableFields.Any())
                    strMessage = Localizer.Str("Choose the field to import at least one of the tiers into");
                else if (SaymoreImportType == SaymoreImportTypes.Retelling)
                    strMessage = Localizer.Str("This project doesn't have any languages configured for retellings. To add them, click 'Project', 'Settings' and check the boxes in the 'Retellings' column of the 'Languages' tab.");
                else if (SaymoreImportType == SaymoreImportTypes.Answers)
                    strMessage = Localizer.Str("This project doesn't have any languages configured for the answers to the testing questions. To add them, click 'Project', 'Settings' and check the boxes in the 'Answers' column of the 'Languages' tab.");
                else
                    strMessage = Localizer.Str("This project doesn't have any languages configured for the story. To add them, click 'Project', 'Settings' and check the boxes in the 'Story' column of the 'Languages' tab.");

                LocalizableMessageBox.Show(strMessage, StoryEditor.OseCaption);
                return null;
            }

            return mappings;
        }

        private void ButtonImportClick(object sender, EventArgs e)
        {
            var mappings = GetMappings();
            if (mappings == null)
                return;

            // e.g. an audio segment that was never transcribed isn't a line of the story (or
            //  the retelling). (For the answers, the user can choose which lines to leave out
            //  when lining them up, where the empty ones start out left out.)
            if (SaymoreImportType != SaymoreImportTypes.Answers)
                mappings = ImportMapping.WithoutEmptyLines(mappings);

            if (SaymoreImportType != SaymoreImportTypes.NewStory)
            {
                // a retelling or the answers won't normally match the story line for line, so
                //  have the user line them up first
                var bAnswers = (SaymoreImportType == SaymoreImportTypes.Answers);
                var targets = AlignImportLinesForm.GetTargets(_storyData, bAnswers);
                if (!targets.Any())
                {
                    LocalizableMessageBox.Show(
                        bAnswers
                            ? Localizer.Str("This story doesn't have any testing questions to import the answers for")
                            : Localizer.Str("This story doesn't have any lines to import the retelling for"),
                        StoryEditor.OseCaption);
                    return;
                }

                using (var dlg = new AlignImportLinesForm(targets, mappings, _projSettings, bAnswers))
                {
                    if (dlg.ShowDialog(this) != DialogResult.OK)
                        return;
                    mappings = dlg.AlignedMappings;
                }
                AlignTargets = targets;
            }

            Finish(mappings);
        }

        // for a new story, lets the user see the lines first and choose which to import
        private void ButtonViewLinesClick(object sender, EventArgs e)
        {
            var mappings = GetMappings();
            if (mappings == null)
                return;

            using (var dlg = new AlignImportLinesForm(null, mappings, _projSettings, false))
            {
                if (dlg.ShowDialog(this) != DialogResult.OK)
                    return;
                Finish(dlg.AlignedMappings);
            }
        }

        private void Finish(List<ImportMapping> mappings)
        {
            Mappings = mappings;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void RadioButtonNewStoryCheckedChanged(object sender, EventArgs e)
        {
            // the fields available depend on the type of import, so redo the choices
            var radioButton = sender as RadioButton;
            if ((radioButton == null) || radioButton.Checked)
            {
                InitTierGrid();
                buttonImport.Text = radioButtonNewStory.Checked
                                        ? Localizer.Str("&Import")
                                        : Localizer.Str("&Next >");
                LayoutButtons();
            }
        }

        // the 'View Lines' button is only for a new story (a retelling or answers always
        //  goes to the lines next), so keep whichever buttons are visible centered.
        // For now, a new story is just imported as is (without its empty lines), so the
        //  button is never shown. To show it again (and let the user leave out lines of a
        //  new story or retelling), define OSE_IMPORT_LEAVE_OUT_LINES (cf. AlignImportLinesForm).
        private void LayoutButtons()
        {
#if OSE_IMPORT_LEAVE_OUT_LINES
            buttonViewLines.Visible = radioButtonNewStory.Checked;
#else
            buttonViewLines.Visible = false;
#endif
            var nWidth = buttonImport.Width + (buttonViewLines.Visible ? buttonViewLines.Width + 6 : 0);
            buttonImport.Left = (tabPageFieldMatching.ClientSize.Width - nWidth) / 2;
            buttonViewLines.Left = buttonImport.Right + 6;
        }

        private void TabPageFieldMatchingResize(object sender, EventArgs e)
        {
            LayoutButtons();
        }
    }
}
