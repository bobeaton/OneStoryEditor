using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using NetLoc;
using Sword;

namespace OneStoryProjectEditor
{
    [System.Runtime.InteropServices.ComVisible(true)]
    public partial class NetBibleViewer : UserControl
    {
        protected string m_strScriptureReference = "Gen 1:1";
        protected List<string> m_astrReferences = new List<string>();
        protected int m_nReferenceArrayIndex = -1;

        #region "format strings for HTML items"
        protected const string CstrHtmlTableBegin = "<table align={0} border=\"1\">";
        protected const string CstrHtmlButtonCell = "<td dir='{2}'><button id='{0}' type=\"button\">{1}</button></td>";
        protected const string CstrHtmlTextCell = "<td dir='{1}' class='TextStyle'>{0}</td>";
        protected const string CstrHtmlRowFormat = "<tr id=\"{0}\">{1}{2}</tr>";
        protected const string CstrHtmlLineFormatCommentaryHeader = "<tr id='{0}' BGCOLOR=\"#CCFFAA\"><td>{1}</td></tr>";
        protected const string CstrHtmlLineFormatCommentary = "<tr><td>{0}</td></tr>";
        internal const string CstrAddFontFormat = "<font face=\"{1}\">{0}</font>";
        // protected const string CstrAddDirFormat = "<p dir=\"RTL\">{0}</p>";
        protected const string CstrHtmlTableEnd = "</table>";
        protected const string verseLineBreak = "<br />";
        protected const string CstrReplaceWithStyle = "<GiveMeStyle>";
        protected const string CstrTextStyleFormat = ".TextStyle {{ font-family: \"{0}\"; font-size: {1} pt; }}";

        protected string GetTextStyle(string strFontFaceName, string strFontSize)
        {
            if (String.IsNullOrEmpty(strFontSize))
                strFontSize = "12";
            return String.Format(CstrTextStyleFormat, strFontFaceName, strFontSize);
        }

        protected const string preDocumentDOMScript = "<style> body { margin:0 } " + CstrReplaceWithStyle + " </style>" + 
            "<script>" +
            "function OpenHoverWindow(link)" +
            "{" +
            "  window.external.ShowHoverOver(link.getAttribute(\"href\").substr(6,link.length));" +
            "  return false;" +
            "}" +
            "" +
            "function DoOnMouseOut(button)" +
            "{" +
            "  window.external.OnMouseOut(button.id, button.getAttribute(\"value\"));" +
            "  return false;" +
            "}" +
            "function DoOnMouseDown()" +
            "{" +
            "  window.external.OnMouseDown();" +
            "  return false;" +
            "}" +
            "function DoOnMouseUp(button)" +
            "{" +
            "  window.external.OnDoOnMouseUp(button.id, button.getAttribute(\"value\"));" +
            "  return false;" +
            "}" +
            "</script>";

        protected const string postDocumentDOMScript = "<script>" +
            "var links = document.getElementsByTagName(\"a\");" +
            "for (var i=0; i < links.length; i++)" +
            "{" +
            "  links[i].onclick = function(){return OpenHoverWindow(this);};" +
            "}" +
            "var buttons = document.getElementsByTagName(\"button\");" +
            "for (var i=0; i < buttons.length; i++)" +
            "{" +
            "  buttons[i].onmousedown = function(){return DoOnMouseDown();};" +
            "  buttons[i].onmouseup = function(){return DoOnMouseUp(this);};" +
            "  buttons[i].onmouseout = function(){return DoOnMouseOut(this);};" +
            "}" +
            "</script>";
        #endregion

        #region "Defines for Sword capability"
        Manager manager;
        Module moduleVersion;
        NetBibleFootnoteTooltip tooltipNBFNs;
        int m_nBook = 0, m_nChapter = 0, m_nVerse = 0;
        // protected const string CstrNetFreeModuleName = "NETfree";
        public const string CstrNetModuleName = "NET";
        protected const string CstrOtherSwordModules = "Other";
        protected const string CstrRadioButtonPrefix = "radioButton";

        public const string ModInfoDeltaLoaded = "Loaded";  // since we don't use the Delta field for anything, let's use it to indicate whether it's loaded or not
        protected List<ModInfo> lstBibleResources = new List<ModInfo>();
        protected List<Module> lstBibleCommentaries = new List<Module>();

        protected static char[] achAnchorDelimiters = new char[] { ' ', ':' };
        protected List<string> lstModulesToSuppress = new List<string>
        {
            "NETtext"   // probably more to come...
        };

        internal static Dictionary<string, string> MapBookNames;

#endregion

        public NetBibleViewer()
        {
            InitializeComponent();
            Localizer.Ctrl(this);

            OnLocalizationChange(false);
            domainUpDownBookNames.ContextMenuStrip = contextMenuStripBibleBooks;
            checkBoxAutoHide.Checked = Properties.Settings.Default.AutoHideBiblePane;
        }

        /// <summary>
        /// version for post-launch, which updates other bits as well
        /// </summary>
        public void OnLocalizationChange(bool bRequery)
        {
            if (bRequery || (MapBookNames == null))
                OnLocalizationChangeStatic();

            domainUpDownBookNames.Items.Clear();
            foreach (var mapBookName in MapBookNames)
                domainUpDownBookNames.Items.Add(mapBookName.Value);

            contextMenuStripBibleBooks.Items.Clear();
            InitDropDown(Localizer.Str("Law"), 0, 5);
            InitDropDown(Localizer.Str("History"), 5, 17);
            InitDropDown(Localizer.Str("Poetry"), 17, 22);
            InitDropDown(Localizer.Str("Prophets"), 22, 39);
            InitDropDown(Localizer.Str("Gospels"), 39, 43);
            InitDropDown(Localizer.Str("Epistles+"), 43, 66);

            if (bRequery)
            {
                m_nChapter = 0; // to trigger a repaint
                DisplayVerses(domainUpDownBookNames.Items[0] + " 1:1");
            }
        }

        public static void OnLocalizationChangeStatic()
        {
            // must be done non-statically, so we'll have already loaded the new Default
            MapBookNames = new Dictionary<string, string>
                               {
                                   {"Gen", Localizer.Str("Gen")},
                                   {"Exod", Localizer.Str("Exod")},
                                   {"Lev", Localizer.Str("Lev")},
                                   {"Num", Localizer.Str("Num")},
                                   {"Deut", Localizer.Str("Deut")},
                                   {"Josh", Localizer.Str("Josh")},
                                   {"Judg", Localizer.Str("Judg")},
                                   {"Ruth", Localizer.Str("Ruth")},
                                   {"1Sam", Localizer.Str("1Sam")},
                                   {"2Sam", Localizer.Str("2Sam")},
                                   {"1Kgs", Localizer.Str("1Kgs")},
                                   {"2Kgs", Localizer.Str("2Kgs")},
                                   {"1Chr", Localizer.Str("1Chr")},
                                   {"2Chr", Localizer.Str("2Chr")},
                                   {"Ezra", Localizer.Str("Ezra")},
                                   {"Neh", Localizer.Str("Neh")},
                                   {"Esth", Localizer.Str("Esth")},
                                   {"Job", Localizer.Str("Job")},
                                   {"Ps", Localizer.Str("Ps")},
                                   {"Prov", Localizer.Str("Prov")},
                                   {"Eccl", Localizer.Str("Eccl")},
                                   {"Song", Localizer.Str("Song")},
                                   {"Isa", Localizer.Str("Isa")},
                                   {"Jer", Localizer.Str("Jer")},
                                   {"Lam", Localizer.Str("Lam")},
                                   {"Ezek", Localizer.Str("Ezek")},
                                   {"Dan", Localizer.Str("Dan")},
                                   {"Hos", Localizer.Str("Hos")},
                                   {"Joel", Localizer.Str("Joel")},
                                   {"Amos", Localizer.Str("Amos")},
                                   {"Obad", Localizer.Str("Obad")},
                                   {"Jonah", Localizer.Str("Jonah")},
                                   {"Mic", Localizer.Str("Mic")},
                                   {"Nah", Localizer.Str("Nah")},
                                   {"Hab", Localizer.Str("Hab")},
                                   {"Zeph", Localizer.Str("Zeph")},
                                   {"Hag", Localizer.Str("Hag")},
                                   {"Zech", Localizer.Str("Zech")},
                                   {"Mal", Localizer.Str("Mal")},
                                   {"Matt", Localizer.Str("Matt")},
                                   {"Mark", Localizer.Str("Mark")},
                                   {"Luke", Localizer.Str("Luke")},
                                   {"John", Localizer.Str("John")},
                                   {"Acts", Localizer.Str("Acts")},
                                   {"Rom", Localizer.Str("Rom")},
                                   {"1Cor", Localizer.Str("1Cor")},
                                   {"2Cor", Localizer.Str("2Cor")},
                                   {"Gal", Localizer.Str("Gal")},
                                   {"Eph", Localizer.Str("Eph")},
                                   {"Phil", Localizer.Str("Phil")},
                                   {"Col", Localizer.Str("Col")},
                                   {"1Thess", Localizer.Str("1Thess")},
                                   {"2Thess", Localizer.Str("2Thess")},
                                   {"1Tim", Localizer.Str("1Tim")},
                                   {"2Tim", Localizer.Str("2Tim")},
                                   {"Titus", Localizer.Str("Titus")},
                                   {"Phlm", Localizer.Str("Phlm")},
                                   {"Heb", Localizer.Str("Heb")},
                                   {"Jas", Localizer.Str("Jas")},
                                   {"1Pet", Localizer.Str("1Pet")},
                                   {"2Pet", Localizer.Str("2Pet")},
                                   {"1John", Localizer.Str("1John")},
                                   {"2John", Localizer.Str("2John")},
                                   {"3John", Localizer.Str("3John")},
                                   {"Jude", Localizer.Str("Jude")},
                                   {"Rev", Localizer.Str("Rev")}
                               };
        }

        internal static string CheckForLocalization(string strJumpTarget)
        {
            var nIndex = strJumpTarget.IndexOf(' ');

            // don't know how this can happen, but someone did it...
            if (nIndex < 0)
                return strJumpTarget;

            var strBookName = strJumpTarget.Substring(0, nIndex);
            if ((MapBookNames != null) && MapBookNames.ContainsKey(strBookName))
                return MapBookNames[strBookName] + strJumpTarget.Substring(nIndex);
            return strJumpTarget;
        }

        void InitDropDown(string strDropDownName, int nStart, int nEnd)
        {
            var tsmi = new ToolStripMenuItem(strDropDownName);
            contextMenuStripBibleBooks.Items.Add(tsmi);
            for (int i = nStart; i < nEnd; i++)
                tsmi.DropDown.Items.Add((string)domainUpDownBookNames.Items[i], null, BibleBookCtx_Click);
        }

        void BibleBookCtx_Click(object sender, EventArgs e)
        {
            var tsi = (ToolStripItem)sender;
            domainUpDownBookNames.SelectedItem = tsi.Text;
        }

        // do this outside of the ctor so in case it throws an error (e.g. Sword not installed),
        //  we can catch it and let the parent form create anyway.
        public void InitNetBibleViewer()
        {
            if (manager == null)
                InitializeSword();
            domainUpDownBookNames.SelectedIndex = 0;
        }

        public string ScriptureReference
        {
            get { return m_strScriptureReference; }
            set { m_strScriptureReference = value; }
        }

        public string JumpTarget { get; set; }

        /// <summary>
        /// returns "Gen" if ScriptureReference were "Gen 1:1"
        /// </summary>
        public string ScriptureReferenceBookName
        {
            get
            {
                var str = ScriptureReference;
                int nIndex = str.LastIndexOf(' ');  // use 'last' in case of multi-word names
                return (nIndex == -1) ? str : str.Substring(0, nIndex);
            }
        }

        /// <summary>
        /// returns " 1:1" if ScriptureReference were "Gen 1:1" (yes with a space)
        /// </summary>
        public string ScriptureReferenceChapVerse
        {
            get
            {
                var str = ScriptureReference;
                int nIndex = str.LastIndexOf(' ');  // use 'last' in case of multi-word names
                return (nIndex == -1) ? str : str.Substring(nIndex);
            }
        }

        public const string SwordResourceCategoryBiblicalText = "Biblical Texts";
        public const string SwordResourcePathSubfolderName = "SWORD";

#region "Code for Sword support"
        protected void InitializeSword()
        {
            // Initialize Module Variables
            manager = new Manager(SwordResourcePathSubfolderName, MarkupFilter.FMT_HTMLHREF, TextEncoding.ENC_HTML);
            if (manager == null)
                throw new ApplicationException("Unable to create the Sword utility manager");

            // first determine all the possible resources available
            var moduleMap = manager.GetModInfoList();
            foreach (var modInfo in moduleMap)
            {
                var strModuleName = modInfo.Name;
                if (lstModulesToSuppress.Contains(strModuleName))
                    continue;

                if (modInfo.Category == SwordResourceCategoryBiblicalText)
                {
                    string strModuleDesc = modInfo.Description;
                    if (Properties.Settings.Default.SwordModulesUsed.Contains(strModuleName))
                    {
                        modInfo.Delta = ModInfoDeltaLoaded;
                        lstBibleResources.Add(modInfo);
                        InitSwordResourceRadioButton(strModuleName, strModuleDesc);
                    }
                    else
                    {
                        modInfo.Delta = null;
                        lstBibleResources.Add(modInfo);
                    }

                    // if the module has encryption, then get the decrypt key
                    string strUnlockKey;
                    if (Program.MapSwordModuleToEncryption.TryGetValue(strModuleName, out strUnlockKey))
                    {
                        strUnlockKey = EncryptionClass.Decrypt(strUnlockKey);
                        manager.SetCipherKey(strModuleName, Encoding.UTF8.GetBytes(strUnlockKey));
                    }
                }
                else
                {
                    // otherwise commentaries/lexicons, etc?
                    //  add them here so they can be used if the user clicks on a verse button
                    var swm = manager.GetModuleByName(strModuleName);
                    lstBibleCommentaries.Add(swm);
                }
            }

            string moduleToStartWith = CstrNetModuleName;
            if (!string.IsNullOrEmpty(Properties.Settings.Default.LastSwordModuleUsed) &&
                lstBibleResources.Any(m => m.Name == Properties.Settings.Default.LastSwordModuleUsed))
            {
                moduleToStartWith = Properties.Settings.Default.LastSwordModuleUsed;
            }

            moduleVersion = manager.GetModuleByName(moduleToStartWith);
            // may fail :-(
            /*
            if (moduleVersion == null)
            {
                throw new ApplicationException(String.Format("Can't find the Sword module '{0}'. Is Sword installed?", Properties.Settings.Default.SwordModulesUsed[0]));
            }
            */

            // Setup display options for the active module: Word of Christ in red, etc...
            manager.SetGlobalOption("Footnotes", "On");
            manager.SetGlobalOption("Words of Christ in Red", "On");
            manager.SetGlobalOption("Glosses", "On");
            manager.SetGlobalOption("Cross-references", "On");
            manager.SetGlobalOption("Textual Variants", "On");

            /* NOTE: This is needed so the DOM Script I'm using for strongs numbers,
             * morph, and footnote tags will work.  This basicly allows the webbrowser
             * control to talk to my form control using DOM Script using the command
             * window.external.<the public method from this form>;
             * -Richard Parsons 01-31-2007
             */
            webBrowserNetBible.ObjectForScripting = this;

            if (tableLayoutPanelSpinControls.Controls[CstrRadioButtonPrefix + moduleToStartWith] is RadioButton)
            {
                var rb = (RadioButton)tableLayoutPanelSpinControls.Controls[CstrRadioButtonPrefix + moduleToStartWith];
                _bInitializing = true;
                rb.Checked = true;
                _bInitializing = false;
            }
        }

        protected RadioButton InitSwordResourceRadioButton(string strModuleName, string strModuleDescription)
        {
            var rb = new RadioButton
                         {
                             AutoSize = true,
                             Name = CstrRadioButtonPrefix + strModuleName,
                             Text = strModuleName,
                             UseVisualStyleBackColor = true,
                             Margin = new Padding(0)
                         };
            toolTip.SetToolTip(rb, strModuleDescription);
            rb.CheckedChanged += rb_CheckedChanged;
            rb.MouseMove += CheckBiblePaneCursorPosition_MouseMove;

            int nIndex = tableLayoutPanelSpinControls.Controls.Count - 1;   // insert at the penultimate position
            tableLayoutPanelSpinControls.InsertColumn(nIndex, new ColumnStyle(SizeType.AutoSize));
            tableLayoutPanelSpinControls.Controls.Add(rb, nIndex, 0);
            return rb;
        }

        private void RadioButtonShowOtherSwordResourcesClick(object sender, EventArgs e)
        {
            var dlg = new ViewSwordOptionsForm(ref lstBibleResources, lstBibleCommentaries);
            RadioButton rbOn = null;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                foreach (var modInfo in lstBibleResources)
                {
                    if (modInfo.Delta == ModInfoDeltaLoaded)
                    {
                        if (tableLayoutPanelSpinControls.Controls[CstrRadioButtonPrefix + modInfo.Name] == null)
                            // means the user selected it, but it's not there. So add it
                            rbOn = InitSwordResourceRadioButton(modInfo.Name, modInfo.Description);
                        else
                            rbOn = (RadioButton)tableLayoutPanelSpinControls.Controls[CstrRadioButtonPrefix + modInfo.Name];

                        // add this one to the user's list of used modules
                        if (!Properties.Settings.Default.SwordModulesUsed.Contains(modInfo.Name))
                            Properties.Settings.Default.SwordModulesUsed.Add(modInfo.Name);
                    }
                    else
                    {
                        if (tableLayoutPanelSpinControls.Controls[CstrRadioButtonPrefix + modInfo.Name] != null)
                        {
                            // means the user deselected it and it's there. So remove it.
                            tableLayoutPanelSpinControls.Controls.RemoveByKey(CstrRadioButtonPrefix + modInfo.Name);
                        }

                        // remove this one to the user's list of used modules
                        if (Properties.Settings.Default.SwordModulesUsed.Contains(modInfo.Name))
                            Properties.Settings.Default.SwordModulesUsed.Remove(modInfo.Name);
                    }
                }
            }

            if (rbOn != null)
                rbOn.Checked = true;
        }

        private bool _bInitializing = false;

        private void rb_CheckedChanged(object sender, EventArgs e)
        {
            var rb = (RadioButton)sender;
            if (_bInitializing || !rb.Checked)
                return;
            TurnOnResource(rb.Text);
            Properties.Settings.Default.LastSwordModuleUsed = rb.Text;
            Properties.Settings.Default.Save();
        }

        public void TurnOnResource(string strModuleName)
        {
            moduleVersion = manager.GetModuleByName(strModuleName);
            System.Diagnostics.Debug.Assert(moduleVersion != null);
            m_nBook = 0;    // forces a refresh
            m_nChapter = 0;
            m_nVerse = 0;
            DisplayVerses();
        }


        // the anchor comes in as, for example, "Gen 1:1"
        // this form is usually called from outside
        public void DisplayVerses(string strScriptureReference)
        {
            if ((m_astrReferences.Count > 0) 
                && (m_astrReferences.Count >= (m_nReferenceArrayIndex + 2))
                && (m_astrReferences[m_nReferenceArrayIndex] != strScriptureReference))
            {
                m_astrReferences.RemoveRange(m_nReferenceArrayIndex + 1,
                                             m_astrReferences.Count - m_nReferenceArrayIndex - 1);
            }
            
            // don't add this if it's already at the head
            if ((m_nReferenceArrayIndex == -1)
                ||  (m_astrReferences.Count > m_nReferenceArrayIndex)
                    && (m_astrReferences[m_nReferenceArrayIndex] != strScriptureReference))
            {
                m_astrReferences.Add(strScriptureReference);
                m_nReferenceArrayIndex = m_astrReferences.Count - 1;
            }

            UpdateNextPreviousButtons();
            ScriptureReference = strScriptureReference;
            DisplayVerses();
        }

        private void UpdateNextPreviousButtons()
        {
            buttonPreviousReference.Enabled = (m_nReferenceArrayIndex > 0);
            buttonNextReference.Enabled = (m_astrReferences.Count >= (m_nReferenceArrayIndex + 2));
        }

        public class SwordKeyChildren
        {
            public SwordKeyChildren(IEnumerable<string> keys)
            {
                if ((keys != null) && (keys.Count() >= 9))
                {
                    Testament = Convert.ToInt32(keys.ElementAt(0));
                    BookIndex = Convert.ToInt32(keys.ElementAt(1));
                    Chapter = Convert.ToInt32(keys.ElementAt(2));
                    Verse = Convert.ToInt32(keys.ElementAt(3));
                    ChapterMax = Convert.ToInt32(keys.ElementAt(4));
                    VerseMax = Convert.ToInt32(keys.ElementAt(5));
                    BookName = keys.ElementAt(6);
                    OsisRef = keys.ElementAt(7);
                    NormalRef = keys.ElementAt(8);
                    BookAbbreviation = keys.ElementAt(9);
                }
            }
            public int Testament { get; set; }
            public int BookIndex { get; set; }
            public int Chapter { get; set; }
            public int Verse { get; set; }
            public int ChapterMax { get; set; }
            public int VerseMax { get; set; }
            public string BookName { get; set; }
            public string OsisRef { get; set; }
            public string NormalRef { get; set; }
            public string BookAbbreviation { get; set; }
            public string KeyText
            {
                get { return $"{BookAbbreviation} {Chapter}:{Verse}"; }
            }
        }

        protected void DisplayVerses()
        {
            if (moduleVersion == null)
                return;

            // first see if we're being given the localized version and convert it
            //  back to 'en' (Sword needs it this way)
            var scriptureReferenceBookName = ScriptureReferenceBookName;
            scriptureReferenceBookName = DeLocalizeScriptureReferenceBookName(scriptureReferenceBookName);

            var strScriptureReference = scriptureReferenceBookName + ScriptureReferenceChapVerse;
            var strBookNameLocalized = CheckForLocalization(strScriptureReference);
            int nIndex;
            if ((nIndex = strBookNameLocalized.IndexOf(' ')) != -1)
                strBookNameLocalized = strBookNameLocalized.Substring(0, nIndex);

            moduleVersion.KeyText = strScriptureReference;
            var swordModuleInfo = new SwordKeyChildren(moduleVersion.KeyChildren);

            int nBook = swordModuleInfo.BookIndex;
            int nChapter = swordModuleInfo.Chapter;
            int nVerse = swordModuleInfo.Verse;

            string strFontName, strFontSize, strModuleVersion = moduleVersion.Name;
            var bSpecifyFont = ReadFontNameAndSizeFromUserConfig(strModuleVersion, out strFontName, out strFontSize);
            bool bDirectionRtl = Properties.Settings.Default.ListSwordModuleToRtl.Contains(strModuleVersion);

            bool bJustUpdated = false;
            if ((nBook != m_nBook) || (nChapter != m_nChapter))
            {
                // something changed
                // Build up the string which we're going to put in the HTML viewer
                var sb = new StringBuilder(String.Format(CstrHtmlTableBegin, (bDirectionRtl) ? "right" : "left"));

                // Load the whole chapter (even if we're going to later jump to some other verse in the chapter)
                swordModuleInfo.Verse = 1;

                while (swordModuleInfo.Verse <= swordModuleInfo.VerseMax)
                {
                    // get the verse and remove any line break signals
                    moduleVersion.KeyText = swordModuleInfo.KeyText;
                    var text = moduleVersion.RenderText();
                    var strVerseHtml = text.Replace(verseLineBreak, null);
                    if (String.IsNullOrEmpty(strVerseHtml))
                        strVerseHtml = Localizer.Str("Passage not available in this version");

                    string strButtonId = String.Format("{0} {1}:{2}",
                                                       scriptureReferenceBookName,
                                                       nChapter, swordModuleInfo.Verse);

                    string strButtonLabel = String.Format("{0} {1}:{2}",
                                                          strBookNameLocalized,
                                                          nChapter, swordModuleInfo.Verse);

                    string strButtonCell = String.Format(CstrHtmlButtonCell,
                                                         strButtonId,
                                                         strButtonLabel,
                                                         (bDirectionRtl) ? "rtl" : "ltr");
                    string strTextCell = String.Format(CstrHtmlTextCell,
                                                       strVerseHtml,
                                                       (bDirectionRtl) ? "rtl" : "ltr");
                    string strLineHtml = String.Format(CstrHtmlRowFormat, 
                        swordModuleInfo.Verse,
                        (bDirectionRtl) ? strTextCell : strButtonCell,
                        (bDirectionRtl) ? strButtonCell : strTextCell);
                    sb.Append(strLineHtml);

                    // next verse until end of chapter
                    swordModuleInfo.Verse++;
                }

                // delimit the table
                sb.Append(CstrHtmlTableEnd);

                // set this along with scripts for clicks and such into the web browser.
                var strHtml = preDocumentDOMScript.Replace(CstrReplaceWithStyle, (bSpecifyFont) 
                                                                                    ? GetTextStyle(strFontName,strFontSize)
                                                                                    : String.Empty);
                strHtml += sb + postDocumentDOMScript;

                webBrowserNetBible.DocumentText = strHtml;
                bJustUpdated = true;
            }

            // if (nVerse != m_nVerse)
            {
                strIdToScrollTo = nVerse.ToString();
                if (!bJustUpdated)
                    ScrollToElement();
            }

            Properties.Settings.Default.LastNetBibleReference = ScriptureReference;

            // sometimes this throws randomly
            try
            {
                // reset to the originally requested verse and update the updown controls
                moduleVersion.KeyText = strScriptureReference;
                swordModuleInfo = new SwordKeyChildren(moduleVersion.KeyChildren);

                UpdateUpDowns(swordModuleInfo);
            }
            catch { }
        }

        public static bool ReadFontNameAndSizeFromUserConfig(string strModuleVersion, out string strFontName,
            out string strFontSize)
        {
            strFontSize = null;
            if (!Program.MapSwordModuleToFont.TryGetValue(strModuleVersion, out strFontName))
                return false;

            var astrs = strFontName.Split(';');
            if (astrs.Length <= 1)
                return true;    // no size specified

            // otherwise, size and name specified as -- e.g. "Arial Unicode MS;20"
            strFontName = astrs[0];
            strFontSize = astrs[1];
            return true;
        }

        private static string DeLocalizeScriptureReferenceBookName(string scriptureReferenceBookName)
        {
            if (!MapBookNames.ContainsKey(scriptureReferenceBookName) &&
                MapBookNames.ContainsValue(scriptureReferenceBookName))
            {
                string reference = scriptureReferenceBookName;
                foreach (var mapBookName in
                    MapBookNames.Where(mapBookName =>
                                       reference == mapBookName.Value))
                {
                    scriptureReferenceBookName = mapBookName.Key;
                    break;
                }
            }
            return scriptureReferenceBookName;
        }

        private void DisplayCommentary(string strJumpTarget)
        {
            // Build up the string which we're going to put in the HTML viewer
            var sb = new StringBuilder(CstrHtmlTableBegin);

            int i;
            for (i = 0; i < lstBibleCommentaries.Count; i++)
            {
                Module swm = lstBibleCommentaries[i];

                // get the verse and remove any line break signals
                swm.KeyText = strJumpTarget;
                var strVerseHtml = swm.RenderText();

                // add the name of the commentary (so you can see it even if there's no comment on this verse)
                sb.Append(String.Format(CstrHtmlLineFormatCommentaryHeader,
                    CommentaryHeader + i, swm.Description));

                if (String.IsNullOrEmpty(strVerseHtml))
                {
                    sb.Append(String.Format(CstrHtmlLineFormatCommentary, Localizer.Str("No commentary for this verse")));
                }
                else
                {
                    sb.Append(String.Format(CstrHtmlLineFormatCommentary, strVerseHtml));
                }
            }

            if (sb.Length == CstrHtmlTableBegin.Length)
                sb.Append(Localizer.Str("No commentary on this passage (you might want to install another \"Sword Project commentary\")"));

            // delimit the table
            sb.Append(CstrHtmlTableEnd);

            var theSE = FindForm() as StoryEditor;
            var dlg = new HtmlForm
                          {
                              Text = CommentaryHeader,
                              ClientText = sb.ToString(),
                              TheSE = theSE,
                              NumberOfResources = i
                          };

            dlg.Show();
        }

        public static string CommentaryHeader
        {
            get { return Localizer.Str("Commentary"); }
        }

        private string strIdToScrollTo = null;

        private void ScrollToElement()
        {
            if (!String.IsNullOrEmpty(strIdToScrollTo) && (webBrowserNetBible.Document != null))
            {
                HtmlDocument doc = webBrowserNetBible.Document;
                HtmlElement elem = doc.GetElementById(strIdToScrollTo);
                if (elem != null)
                    elem.ScrollIntoView(true);
            }
        }

        protected void UpdateUpDowns(SwordKeyChildren swordModuleInfo)
        {
            System.Diagnostics.Debug.Assert(!m_bDisableInterrupts);
            m_bDisableInterrupts = true;

            // initialize the combo boxes for this new situation
            if (swordModuleInfo.BookIndex != m_nBook)
            {
                domainUpDownBookNames.SelectedItem = swordModuleInfo.BookAbbreviation;
                m_nBook = swordModuleInfo.BookIndex;

                int nNumChapters = swordModuleInfo.ChapterMax;    // chapterCount(keyVerse.Testament(), keyVerse.Book());
                numericUpDownChapterNumber.Maximum = nNumChapters;

                // if the book changes, then the chapter number changes implicitly
                m_nChapter = 0;
            }

            if (swordModuleInfo.Chapter != m_nChapter)
            {
                m_nChapter = swordModuleInfo.Chapter;
                numericUpDownChapterNumber.Value = m_nChapter;

                int nNumVerses = swordModuleInfo.VerseMax;    // verseCount(keyVerse.Testament(), keyVerse.Book(), keyVerse.Chapter());
                numericUpDownVerseNumber.Maximum = nNumVerses;
            }

            if (swordModuleInfo.Verse != m_nVerse)
            {
                m_nVerse = swordModuleInfo.Verse;
                numericUpDownVerseNumber.Value = (decimal)m_nVerse;
            }

            m_bDisableInterrupts = false;
        }

        protected static List<string> GetModuleLocations()
        {
            List<string> lst = new List<string>();
            string strSwordProjectPath = Environment.GetEnvironmentVariable("SWORD_PATH");
            if (!String.IsNullOrEmpty(strSwordProjectPath))
            {
                string[] astrPaths = strSwordProjectPath.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries);
                lst.AddRange(astrPaths);
            }

            strSwordProjectPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) +
                                  @"\CrossWire\The SWORD Project";
            if (Directory.Exists(strSwordProjectPath) && !lst.Contains(strSwordProjectPath))
                lst.Add(strSwordProjectPath);


            // finally, we put at least the NetBible below our working dir.
            strSwordProjectPath = GetSwordProjectPath;
            System.Diagnostics.Debug.Assert(Directory.Exists(strSwordProjectPath));
            if (!lst.Contains(strSwordProjectPath))
                lst.Add(strSwordProjectPath);

            return lst;
        }

        public static string GetSwordProjectPath
        {
            get
            {
                return Path.Combine(StoryProjectData.GetRunningFolder, SwordResourcePathSubfolderName);
            }
        }

#endregion  // "Defines for Sword capability"

#region "Callbacks from HTML script"
        protected bool m_bMouseDown = false;

        public void OnMouseDown()
        {
            m_bMouseDown = true;
        }

        public void OnMouseOut(string strJumpTarget, string strScriptureReference)
        {
            if (m_bMouseDown)
            {
                JumpTarget = strJumpTarget;
                ScriptureReference = strScriptureReference;
                StoryEditor.SuspendSaveDialog++;
                webBrowserNetBible.DoDragDrop(this, DragDropEffects.Link | DragDropEffects.Copy);
                StoryEditor.SuspendSaveDialog--;
            }
        }

        public void OnDoOnMouseUp(string strJumpTarget, string strScriptureReference)
        {
            m_bMouseDown = false;
#if !DoDisplayVerse
            DisplayCommentary(strJumpTarget);
#else
            DisplayVerses(strScriptureReference);
            var theSE = FindForm() as StoryEditor;
            if (theSE != null)
                theSE._bAutoHide = false;
            // get it to stick if the user does this
#endif
        }

        protected NetBibleFootnoteTooltip _theFootnoteForm = null;

        public void ShowHoverOver(string s)
        {
            if (tooltipNBFNs != null)
            {
                if (tooltipNBFNs.Tag.Equals(s))
                    return; //leave if we are already displaying this tool tip

                //if there is a different tooltip showing destroy it
                tooltipNBFNs.Dispose();
                tooltipNBFNs = null;
            }

            if (_theFootnoteForm == null)
                _theFootnoteForm = new NetBibleFootnoteTooltip(manager);

            // locate the window near the cursor...
            Point ptTooltip = Cursor.Position;

            // but make sure it doesn't go off the edge
            Rectangle rectScreen = Screen.GetBounds(ptTooltip);
            int dx = (ptTooltip.X + _theFootnoteForm.Size.Width) - rectScreen.Width;
            int dy = (ptTooltip.Y + _theFootnoteForm.Size.Height) - rectScreen.Height;

            ptTooltip.Offset(-Math.Max(ClientRectangle.Location.X, dx), 
                -Math.Max(ClientRectangle.Location.Y, dy));

            _theFootnoteForm.ShowFootnote(s, ptTooltip);
            System.Diagnostics.Debug.WriteLine("ShowHoverOver");
        }
#endregion // "Callbacks from HTML script"

        protected bool m_bDisableInterrupts = false;
        protected void CallUpdateUpDowns()
        {
            if (m_bDisableInterrupts) 
                return;

            // sometimes this happens with nothing selected... ignore those
            var strSelectedBookName = domainUpDownBookNames.SelectedItem as string;
            if (!String.IsNullOrEmpty(strSelectedBookName))
            {
                var strScriptureReference = String.Format("{0} {1}:{2}",
                                                      domainUpDownBookNames.SelectedItem,
                                                      numericUpDownChapterNumber.Value,
                                                      numericUpDownVerseNumber.Value);

                DisplayVerses(strScriptureReference);
            }
        }

        private void domainUpDownBookNames_SelectedItemChanged(object sender, EventArgs e)
        {
            bool bIntValue = m_bDisableInterrupts;
            m_bDisableInterrupts = true;
            numericUpDownChapterNumber.Value = numericUpDownVerseNumber.Value = 1;
            m_bDisableInterrupts = bIntValue;
            CallUpdateUpDowns();
        }

        private void numericUpDownChapter_ValueChanged(object sender, EventArgs e)
        {
            bool bIntValue = m_bDisableInterrupts;
            m_bDisableInterrupts = true;
            numericUpDownVerseNumber.Value = 1;
            m_bDisableInterrupts = bIntValue;
            CallUpdateUpDowns();
        }

        private void numericUpDownVerse_ValueChanged(object sender, EventArgs e)
        {
            CallUpdateUpDowns();
        }

        private void webBrowserNetBible_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            ScrollToElement();
        }

        private void checkBoxAutoHide_CheckStateChanged(object sender, EventArgs e)
        {
            if (checkBoxAutoHide.Checked)
            {
                var theSE = FindForm() as StoryEditor;
                if ((theSE != null) && (theSE.splitContainerUpDown.IsMinimized))
                    theSE.splitContainerUpDown.Restore();
            }

            Properties.Settings.Default.AutoHideBiblePane = checkBoxAutoHide.Checked;
            Properties.Settings.Default.Save();
        }

        private void checkBoxAutoHide_MouseUp(object sender, MouseEventArgs e)
        {
            // if clicked with the right mouse button, then hide it now
            //   (triggered by setting theSE.LastKeyPressedTimeStamp)
            if (!checkBoxAutoHide.Checked)
            {
                var theSE = FindForm() as StoryEditor;
                if (theSE != null)
                {
                    theSE.LastKeyPressedTimeStamp = DateTime.Now;
                    /*
                    if (theSE.splitContainerUpDown.IsMinimized)
                        theSE.splitContainerUpDown.Restore();
                    else
                        theSE.splitContainerUpDown.Minimize();
                    */
                }
            }
        }

        private void numericUpDownChapterNumber_Enter(object sender, EventArgs e)
        {
            numericUpDownChapterNumber.Select(0, numericUpDownChapterNumber.Value.ToString().Length);
            CheckBiblePaneCursorPosition_MouseMove(sender, null);
        }

        private void numericUpDownVerseNumber_Enter(object sender, EventArgs e)
        {
            numericUpDownVerseNumber.Select(0, numericUpDownVerseNumber.Value.ToString().Length);
            CheckBiblePaneCursorPosition_MouseMove(sender, null);
        }

        private void CheckBiblePaneCursorPosition_MouseMove(object sender, MouseEventArgs e)
        {
            var theSE = FindForm() as StoryEditor;
            if (theSE != null)
                theSE.CheckBiblePaneCursorPosition();
        }

        private void buttonPreviousReference_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert((m_nReferenceArrayIndex > 0)
                && (m_nReferenceArrayIndex < m_astrReferences.Count));
            ScriptureReference = m_astrReferences[--m_nReferenceArrayIndex];
            DisplayVerses();
            UpdateNextPreviousButtons();
        }

        private void buttonNextReference_Click(object sender, EventArgs e)
        {
            if (m_nReferenceArrayIndex >= (m_astrReferences.Count - 1))
                m_nReferenceArrayIndex = Math.Max(0, m_astrReferences.Count - 2);
            ScriptureReference = m_astrReferences[++m_nReferenceArrayIndex];
            DisplayVerses();
            UpdateNextPreviousButtons();
        }

        private const string CstrBibRefRegexFormat = @"\b([a-zA-Z]{3,4}|[1-3][a-zA-Z]{2,5}).(\d{2,3}):(\d{2,3})\b";

        private static Regex _regexBibRef;

        private static char[] _achTrimChars = new char[] {'.', ':', ' ', ',', ';'};
        private void textBoxNetFlixViewer_MouseDown(object sender, MouseEventArgs e)
        {
            if (StoryEditor.TextPaster == null)
                return;

            try
            {
                var strText = StoryEditor.TextPaster.GetNextLine(false);
                if (strText.IndexOf(@"\anc ", StringComparison.Ordinal) == 0)
                {
                    strText = strText.Substring(5); // length of @"\anc "
                    var lstVerses = new List<string>();
#if !UseSplit
                    if (_regexBibRef == null)
                        _regexBibRef = new Regex(CstrBibRefRegexFormat,
                                                RegexOptions.Compiled |
                                                RegexOptions.CultureInvariant |
                                                RegexOptions.IgnoreCase);

                    var refs = _regexBibRef.Matches(strText);
                    if (refs.Count > 0)
                    {
                        foreach (Match bibleReference in refs)
                        {
                            var strBibleReference = String.Format("{0} {1}:{2}",
                                                                  bibleReference.Groups[1].Value,
                                                                  Convert.ToUInt32(bibleReference.Groups[2].Value),
                                                                  Convert.ToUInt32(bibleReference.Groups[3].Value));
                            
                            // capitalize the 1st letter
                            strBibleReference = strBibleReference[0].ToString(CultureInfo.InvariantCulture).ToUpper() +
                                                strBibleReference.Substring(1);
                            lstVerses.Insert(0, strBibleReference);
                        }
#else
                    var astr = strText.Split(_achTrimChars, StringSplitOptions.RemoveEmptyEntries);
                    if (astr.Length > 2)
                    {
                        var nIndex = 0;
                        while ((nIndex + 2) < astr.Length)
                        {
                            var newRef = String.Format("{0} {1}:{2}",
                                                       astr[nIndex++],
                                                       Convert.ToUInt32(astr[nIndex++]),
                                                       Convert.ToUInt32(astr[nIndex++]));
                            lstVerses.Insert(0, newRef);
                        }
#endif

                        foreach (var newRef in lstVerses)
                            DisplayVerses(newRef);
                    }
                }
            }
            catch{} // don't make a fuss out of it if the user sends us garbage
        }

        private void toolStripMenuItemChangeFont_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Properties.Settings.Default.LastSwordModuleUsed: " + Properties.Settings.Default.LastSwordModuleUsed);

            // if we have this in the user config, then pre-select it for the Font dialog
            var fontDialog = new FontDialog();
            string strFontName, strFontSize;
            if (ReadFontNameAndSizeFromUserConfig(Properties.Settings.Default.LastSwordModuleUsed,
                out strFontName, out strFontSize))
            {
                float fFontSize;
                if (!float.TryParse(strFontSize, out fFontSize))
                    fFontSize = 12F;
                fontDialog = new FontDialog {Font = new Font(strFontName, fFontSize)};
            }

            // query what the user wants
            if (fontDialog.ShowDialog() != DialogResult.OK)
                    return;

            strFontName = String.Format("{0};{1}", fontDialog.Font.Name, fontDialog.Font.Size);
            if (!Program.MapSwordModuleToFont.ContainsKey(Properties.Settings.Default.LastSwordModuleUsed))
            {
                Program.MapSwordModuleToFont.Add(Properties.Settings.Default.LastSwordModuleUsed, strFontName);
            }
            else
            {
                Program.MapSwordModuleToFont[Properties.Settings.Default.LastSwordModuleUsed] = strFontName;
            }

            // save the changes/additions
            Properties.Settings.Default.SwordModuleToFont = Program.DictionaryToArray(Program.MapSwordModuleToFont);
            Properties.Settings.Default.Save();
            TurnOnResource(Properties.Settings.Default.LastSwordModuleUsed);
        }
    }
}
