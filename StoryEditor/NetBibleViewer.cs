using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    [System.Runtime.InteropServices.ComVisible(true)]
    public partial class NetBibleViewer : UserControl
    {
        protected string m_strScriptureReference = "gen 1:1";

        #region "format strings for HTML items"
        protected const string CstrHtmlTableBegin = "<table border=\"1\">";
        protected const string CstrHtmlLineFormat = "<tr><td><button type=\"button\">{0}</button></td><td>{1}</td>";
        protected const string CstrAddFontFormat = "<font face=\"{1}\">{0}</font>";
        protected const string CstrAddDirFormat = "<p dir=\"RTL\">{0}</p>";
        protected const string CstrHtmlTableEnd = "</table>";
        protected const string verseLineBreak = "<br />";
        protected const string preDocumentDOMScript = "<script>" +
            "function OpenHoverWindow(link)" +
            "{" +
            "  window.external.ShowHoverOver(link.getAttribute(\"href\").substr(6,link.length));" +
            "  return false;" +
            "}" +
            "" +
            "function DoOnMouseOut(button)" +
            "{" +
            "  window.external.OnMouseOut(button.getAttribute(\"value\"));" +
            "  return false;" +
            "}" +
            "function DoOnMouseDown()" +
            "{" +
            "  window.external.OnMouseDown();" +
            "  return false;" +
            "}" +
            "function DoOnMouseUp(button)" +
            "{" +
            "  window.external.OnDoOnMouseUp(button.getAttribute(\"value\"));" +
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
        MarkupFilterMgr filterManager;
        SWMgr manager;
        SWModule moduleVersion;
        NetBibleFootnoteTooltip tooltipNBFNs;
        int m_nBook = 0, m_nChapter = 0, m_nVerse = 0;
        protected const string CstrNetFreeModuleName = "NETfree";
        protected const string CstrNetModuleName = "NET";
        protected const string CstrOtherSwordModules = "Other";
        protected const string CstrRadioButtonPrefix = "radioButton";
        protected const string CstrFontForHindi = "Arial Unicode MS";
        protected const string CstrFontForFarsi = "Nafees Nastaleeq";
        protected const string CstrHindiModule = "HINDI";
        protected const string CstrKangriModule = "XNR";
        protected const string CstrFarsiModule = "FarsiOPV";


        public class SwordResource
        {
            internal string Name;
            internal string Description;
            internal bool Loaded;

            internal SwordResource(string strName, string strDescription, bool bLoaded)
            {
                Name = strName;
                Description = strDescription;
                Loaded = bLoaded;
            }
        }

        protected List<SwordResource> lstBibleResources = new List<SwordResource>();
        protected static char[] achAnchorDelimiters = new char[] { ' ', ':' };
        protected List<string> lstModulesToSuppress = new List<string>
        {
            "NETtext"   // probably more to come...
        };

        #endregion

        public NetBibleViewer()
        {
            InitializeComponent();

            InitDropDown("Law", 0, 5);
            InitDropDown("History", 5, 17);
            InitDropDown("Poetry", 17, 22);
            InitDropDown("Prophets", 22, 39);
            InitDropDown("Gospels", 39, 43);
            InitDropDown("Epistles+", 43, 66);

            domainUpDownBookNames.ContextMenuStrip = contextMenuStripBibleBooks;
        }

        void InitDropDown(string strDropDownName, int nStart, int nEnd)
        {
            ToolStripMenuItem tsmi = new ToolStripMenuItem(strDropDownName);
            contextMenuStripBibleBooks.Items.Add(tsmi);
            for (int i = nStart; i < nEnd; i++)
                tsmi.DropDown.Items.Add((string)domainUpDownBookNames.Items[i], null, BibleBookCtx_Click);

        }

        void BibleBookCtx_Click(object sender, EventArgs e)
        {
            ToolStripItem tsi = (ToolStripItem)sender;
            domainUpDownBookNames.SelectedItem = tsi.Text;
        }

        // do this outside of the ctor so in case it throws an error (e.g. Sword not installed),
        //  we can catch it and let the parent form create anyway.
        public void InitNetBibleViewer()
        {
            if (manager == null)
                InitializeSword();
        }

        public string ScriptureReference
        {
            get { return m_strScriptureReference; }
            set { m_strScriptureReference = value; }
        }

        #region "Code for Sword support"
        protected void InitializeSword()
        {
            // Initialize Module Variables
            filterManager = new MarkupFilterMgr((char)Sword.FMT_HTMLHREF, (char)Sword.ENC_HTML);

            /* NOTE: GC.SuppressFinalize(filterManager);
			 *  This must be placed here so the garbage collector (GC) doesn't try to clean up
			 * something that was already cleaned up.  If this is not left in an error will
			 * occur when the application closes.  This happens because when the SWMgr is
			 * cleaned by the GC it cleans its own filter and removes it from memory.  When
			 * the GC then tries to clean up the filterManager object it doesn't really
			 * exist in memory anymore and therefore it throws an exception saying some
			 * memory is probably corrupt because this object points to trash in memory.
			 * -Richard Parsons 11-21-2006
			 */
            GC.SuppressFinalize(filterManager);

            manager = new SWMgr(filterManager);
            if (manager == null)
                throw new ApplicationException("Unable to create the Sword utility manager");

            foreach (string strPath in GetModuleLocations())
                manager.augmentModules(strPath);

            // first determine all the possible resources available
            int numOfModules = (int)manager.getModules().size();
            for (int i = 0; i < numOfModules; i++)
                if (manager.getModuleAt(i).Type().Equals("Biblical Texts")) //Limit to just bibles, comment out to see all modules
                {
                    string strModuleName = manager.getModuleAt(i).Name();
                    if (lstModulesToSuppress.Contains(strModuleName))
                        continue;
                    
                    string strModuleDesc = manager.getModuleAt(i).Description();
                    if (Properties.Settings.Default.SwordModulesUsed.Contains(strModuleName))
                    {
                        lstBibleResources.Add(new SwordResource(strModuleName, strModuleDesc, true));
                        InitSwordResourceRadioButton(strModuleName, strModuleDesc);
                    }
                    else
                        lstBibleResources.Add(new SwordResource(strModuleName, strModuleDesc, false));
                }

            string moduleToStartWith = CstrNetFreeModuleName;
            if (!string.IsNullOrEmpty(Properties.Settings.Default.LastSwordModuleUsed))
                moduleToStartWith = Properties.Settings.Default.LastSwordModuleUsed;

            moduleVersion = manager.getModule(moduleToStartWith);
            if (moduleVersion == null)
                throw new ApplicationException(String.Format("Can't find the Sword module '{0}'. Is Sword installed?", Properties.Settings.Default.SwordModulesUsed[0]));

            // Setup the active module
            // Word of Christ in red
            manager.setGlobalOption("Words of Christ in Red", "On");

            //Footnotes
            manager.setGlobalOption("Footnotes", "On");

            /* NOTE: This is needed so the DOM Script I'm using for strongs numbers,
             * morph, and footnote tags will work.  This basicly allows the webbrowser
             * control to talk to my form control using DOM Script using the command
             * window.external.<the public method from this form>;
             * -Richard Parsons 01-31-2007
             */
            webBrowserNetBible.ObjectForScripting = this;

            if (tableLayoutPanelSpinControls.Controls[CstrRadioButtonPrefix + moduleToStartWith] is RadioButton)
            {
                RadioButton rb = (RadioButton)tableLayoutPanelSpinControls.Controls[CstrRadioButtonPrefix + moduleToStartWith];
                rb.Checked = true;
            }
        }

        protected RadioButton InitSwordResourceRadioButton(string strModuleName, string strModuleDescription)
        {
            RadioButton rb = new RadioButton
                                 {
                                     AutoSize = true,
                                     Name = CstrRadioButtonPrefix + strModuleName,
                                     Text = strModuleName,
                                     UseVisualStyleBackColor = true
                                 };
            toolTip.SetToolTip(rb, strModuleDescription);
            rb.CheckedChanged += rb_CheckedChanged;

            int nIndex = tableLayoutPanelSpinControls.Controls.Count - 1;   // insert at the penultimate position
            tableLayoutPanelSpinControls.InsertColumn(nIndex, new ColumnStyle(SizeType.AutoSize));
            tableLayoutPanelSpinControls.Controls.Add(rb, nIndex, 0);
            return rb;
        }

        private void radioButtonShowOtherSwordResources_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonShowOtherSwordResources.Checked)
            {
                ViewSwordOptionsForm dlg = new ViewSwordOptionsForm(ref lstBibleResources);
                RadioButton rbOn = null;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    foreach (SwordResource aSR in lstBibleResources)
                    {
                        if (aSR.Loaded)
                        {
                            if (tableLayoutPanelSpinControls.Controls[CstrRadioButtonPrefix + aSR.Name] == null)
                                // means the user selected it, but it's not there. So add it
                                rbOn = InitSwordResourceRadioButton(aSR.Name, aSR.Description);
                            else
                                rbOn = (RadioButton)tableLayoutPanelSpinControls.Controls[CstrRadioButtonPrefix + aSR.Name];

                            // add this one to the user's list of used modules
                            if (!Properties.Settings.Default.SwordModulesUsed.Contains(aSR.Name))
                                Properties.Settings.Default.SwordModulesUsed.Add(aSR.Name);
                        }
                        else
                        {
                            if (tableLayoutPanelSpinControls.Controls[CstrRadioButtonPrefix + aSR.Name] != null)
                            {
                                tableLayoutPanelSpinControls.DumpTable();
                                // means the user deselected it and it's there. So remove it.
                                tableLayoutPanelSpinControls.Controls.RemoveByKey(CstrRadioButtonPrefix + aSR.Name);
                                tableLayoutPanelSpinControls.DumpTable();
                            }

                            // remove this one to the user's list of used modules
                            if (Properties.Settings.Default.SwordModulesUsed.Contains(aSR.Name))
                                Properties.Settings.Default.SwordModulesUsed.Remove(aSR.Name);
                        }
                    }
                }

                if (rbOn != null)
                    rbOn.Checked = true;
            }
        }

        private void rb_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
            if (rb.Checked)
            {
                TurnOnResource(rb.Text);
                Properties.Settings.Default.LastSwordModuleUsed = rb.Text;
                Properties.Settings.Default.Save();
            }
        }

        protected void TurnOnResource(string strModuleName)
        {
            moduleVersion = manager.getModule(strModuleName);
            System.Diagnostics.Debug.Assert(moduleVersion != null);
            m_nBook = 0;    // forces a refresh
            m_nChapter = 0;
            m_nVerse = 0;
            DisplayVerses();
        }


        // the anchor comes in as, for example, "gen 1:1"
        // this form is usually called from outside
        public void DisplayVerses(string strScriptureReference)
        {
            ScriptureReference = strScriptureReference;
            DisplayVerses();
        }

        protected void DisplayVerses()
        {
            VerseKey keyVerse = new VerseKey(ScriptureReference);
            int nBook = keyVerse.Book();
            int nChapter = keyVerse.Chapter();
            int nVerse = keyVerse.Verse();

            if ((nBook != m_nBook) || (nChapter != m_nChapter) || (nVerse != m_nVerse))
            {
                // something changed
                // Build up the string which we're going to put in the HTML viewer
                StringBuilder sb = new StringBuilder(CstrHtmlTableBegin);

                // Let's just read in what left in the chapter
                VerseKey keyRestOfChapter = new VerseKey(keyVerse);
                while ((keyRestOfChapter.Chapter() == nChapter) && (keyRestOfChapter.Book() == nBook) && (keyRestOfChapter.Error() == '\0'))
                {
                    // get the verse and remove any line break signals
                    string strVerseHtml = moduleVersion.RenderText(keyRestOfChapter).Replace(verseLineBreak, null);
                    if (String.IsNullOrEmpty(strVerseHtml))
                        strVerseHtml = "Passage not available in this version";

                    // insert a button (for drag-drop) and the HTML into a table format
                    // kindof a cheat, but I don't mind (this should be done better...)
                    string strModuleVersion = moduleVersion.Name();
                    if ((strModuleVersion == CstrHindiModule)
                        ||
                        (strModuleVersion == CstrKangriModule))
                    {
                        strVerseHtml = String.Format(CstrAddFontFormat, strVerseHtml, CstrFontForHindi);
                    }
                    else if (strModuleVersion == CstrFarsiModule)
                    {
                        strVerseHtml = String.Format(CstrAddDirFormat, strVerseHtml);
                        strVerseHtml = String.Format(CstrAddFontFormat, strVerseHtml, CstrFontForFarsi);
                    }

                    string strLineHtml = String.Format(CstrHtmlLineFormat, keyRestOfChapter.getShortText(), strVerseHtml);
                    sb.Append(strLineHtml);

                    // next verse until end of chapter
                    keyRestOfChapter.Verse(keyRestOfChapter.Verse() + 1);
                }

                // delimit the table
                sb.Append(CstrHtmlTableEnd);

                // set this along with scripts for clicks and such into the web browser.
                webBrowserNetBible.DocumentText = preDocumentDOMScript + sb.ToString() + postDocumentDOMScript;

                Properties.Settings.Default.LastNetBibleReference = ScriptureReference;
            }

            // update the updown controls
            UpdateUpDowns(keyVerse);
        }

        protected void UpdateUpDowns(VerseKey keyVerse)
        {
            m_bDisableInterrupts = true;

            // initialize the combo boxes for this new situation
            if (keyVerse.Verse() != m_nBook)
            {
                domainUpDownBookNames.SelectedItem = keyVerse.getBookAbbrev();
                m_nBook = keyVerse.Book();

                int nNumChapters = keyVerse.chapterCount(keyVerse.Testament(), keyVerse.Book());
                numericUpDownChapterNumber.Maximum = nNumChapters;

                // if the book changes, then the chapter number changes implicitly
                m_nChapter = 0;
            }

            if (keyVerse.Chapter() != m_nChapter)
            {
                m_nChapter = keyVerse.Chapter();
                numericUpDownChapterNumber.Value = m_nChapter;

                int nNumVerses = keyVerse.verseCount(keyVerse.Testament(), keyVerse.Book(), keyVerse.Chapter());
                numericUpDownVerseNumber.Maximum = nNumVerses;
            }

            if (keyVerse.Verse() != m_nVerse)
            {
                m_nVerse = keyVerse.Verse();
                numericUpDownVerseNumber.Value = (decimal)m_nVerse;
            }

            m_bDisableInterrupts = false;
        }

        protected static List<string> GetModuleLocations()
        {
            List<string> lst = new List<string>();
            string strSwordProjectPath = Environment.GetEnvironmentVariable("SWORD_PATH");
            if (!String.IsNullOrEmpty(strSwordProjectPath))
                lst.Add(strSwordProjectPath);

            strSwordProjectPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) +
                                  @"\CrossWire\The SWORD Project";
            if (Directory.Exists(strSwordProjectPath))
                lst.Add(strSwordProjectPath);

#if DEBUG
            string strWorkingFolder = @"C:\src\StoryEditor\StoryEditor";
#else
            string strWorkingFolder = StoryProjectData.GetRunningFolder;
#endif

            // finally, we put at least the NetBible below our working dir.
            strSwordProjectPath = Path.Combine(strWorkingFolder, "SWORD");
            System.Diagnostics.Debug.Assert(Directory.Exists(strSwordProjectPath));
            lst.Add(strSwordProjectPath);

            return lst;
        }

        #endregion  // "Defines for Sword capability"

        #region "Callbacks from HTML script"
        protected bool m_bMouseDown = false;

        public void OnMouseDown()
        {
            Console.WriteLine("OnMouseDown:");
            m_bMouseDown = true;
        }

        public void OnMouseOut(string strScriptureReference)
        {
            Console.WriteLine("OnMouseOut: " + strScriptureReference + ((m_bMouseDown) ? " with mouse down" : " with mouse up"));
            if (m_bMouseDown)
            {
                ScriptureReference = strScriptureReference;
                webBrowserNetBible.DoDragDrop(this, DragDropEffects.Link | DragDropEffects.Copy);
            }
        }

        public void OnDoOnMouseUp(string strScriptureReference)
        {
            Console.WriteLine("OnDoOnMouseUp: " + strScriptureReference);
            m_bMouseDown = false;
            ScriptureReference = strScriptureReference;
            DisplayVerses();
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
            if (!m_bDisableInterrupts)
            {
                ScriptureReference = String.Format("{0} {1}:{2}",
                    domainUpDownBookNames.SelectedItem,
                    numericUpDownChapterNumber.Value,
                    numericUpDownVerseNumber.Value);

                DisplayVerses();
            }
        }

        private void domainUpDownBookNames_SelectedItemChanged(object sender, EventArgs e)
        {
            CallUpdateUpDowns();
        }

        private void numericUpDown_ValueChanged(object sender, EventArgs e)
        {
            CallUpdateUpDowns();
        }
    }
}
