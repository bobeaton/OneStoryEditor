using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    [System.Runtime.InteropServices.ComVisible(true)]
    public partial class NetBibleViewer : UserControl
    {
        protected string m_strScriptureReference = "gen 1:1";

        #region "format strings for HTML items"
        // protected const string cstrHtmlLineFormat = "<button type=\"button\">{0}</button>{1}<br />";
        protected const string cstrHtmlLineFormat = "<tr><td><button type=\"button\">{0}</button></td><td>{1}</td>";
        protected const string cstrHtmlTableBegin = "<table border=\"1\">";
        protected const string cstrHtmlTableEnd = "</table>";

        protected const string verseLineBreak = "<br />";
        protected const string preDocumentDOMScript = "<script>" +
            "function OpenHoverWindow(link)" +
            "{" +
            "  window.external.ShowHoverOver(link.getAttribute(\"href\").substr(6,link.length));" +
            "  return false;" +
            "}" +
            "" +
            "function CloseHoverWindow()" +
            "{" +
            "  window.external.HideHoverOver();" +
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
            "  links[i].onmouseover = function(){return OpenHoverWindow(this);};" +
            "  links[i].onmouseout = function(){CloseHoverWindow();};" +
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
        MarkupFilterMgr filterManager = null;
        SWMgr manager = null;
        SWModule moduleVersion = null;
        NetBibleFootnoteTooltip tooltipNBFNs = null;
        int m_nBook = 0, m_nChapter = 0, m_nVerse = 0;

        protected static char[] achAnchorDelimiters = new char[] { ' ', ':' };
        #endregion

        public NetBibleViewer()
        {
            InitializeComponent();
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
            foreach (string strPath in GetModuleLocations())
                manager.augmentModules(strPath);

            moduleVersion = manager.getModule("NETfree");
            if (moduleVersion == null)
            {
#if DEBUG
                string strModules = null;
                int numOfBibles = (int)manager.getModules().size();
                for (int i = 0; i < numOfBibles; i++)
                    if (manager.getModuleAt(i).Type().Equals("Biblical Texts")) //Limit to just bibles, comment out to see all modules
                        strModules += manager.getModuleAt(i).Name() + '\n';
                MessageBox.Show(String.Format("Found modules:{0}{0}{1}", Environment.NewLine, strModules), StoryEditor.cstrCaption);
#else
                throw new ApplicationException(String.Format("Can't find Sword module '{0}'. Is Sword installed?", "NETfree"));
#endif
            }

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
                StringBuilder sb = new StringBuilder(cstrHtmlTableBegin);

                // Let's just read in what left in the chapter
                VerseKey keyRestOfChapter = new VerseKey(keyVerse);
                while ((keyRestOfChapter.Chapter() == nChapter) && (keyRestOfChapter.Book() == nBook) && (keyRestOfChapter.Error() == '\0'))
                {
                    // get the verse and remove any line break signals
                    string strVerseHtml = moduleVersion.RenderText(keyRestOfChapter).Replace(verseLineBreak, null);

                    // insert a button (for drag-drop) and the HTML into a table format
                    string strLineHtml = String.Format(cstrHtmlLineFormat, keyRestOfChapter.getShortText(), strVerseHtml);
                    sb.Append(strLineHtml);

                    // next verse until end of chapter
                    keyRestOfChapter.Verse(keyRestOfChapter.Verse() + 1);
                }

                // delimit the table
                sb.Append(cstrHtmlTableEnd);

                // set this along with scripts for clicks and such into the web browser.
                webBrowserNetBible.DocumentText = preDocumentDOMScript + sb.ToString() + postDocumentDOMScript;
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
                this.domainUpDownBookNames.SelectedItem = keyVerse.getBookAbbrev();
                m_nBook = keyVerse.Book();

                int nNumChapters = keyVerse.chapterCount(keyVerse.Testament(), keyVerse.Book());
                this.numericUpDownChapterNumber.Maximum = nNumChapters;

                // if the book changes, then the chapter number changes implicitly
                m_nChapter = 0;
            }

            if (keyVerse.Chapter() != m_nChapter)
            {
                m_nChapter = keyVerse.Chapter();
                this.numericUpDownChapterNumber.Value = (decimal)m_nChapter;

                int nNumVerses = keyVerse.verseCount(keyVerse.Testament(), keyVerse.Book(), keyVerse.Chapter());
                this.numericUpDownVerseNumber.Maximum = nNumVerses;
            }

            if (keyVerse.Verse() != m_nVerse)
            {
                m_nVerse = keyVerse.Verse();
                this.numericUpDownVerseNumber.Value = (decimal)m_nVerse;
            }

            m_bDisableInterrupts = false;
        }

        protected static string[] GetModuleLocations()
        {
            string strSwordProjectPath = Environment.GetEnvironmentVariable("SWORD_PATH");
            if (String.IsNullOrEmpty(strSwordProjectPath))
                strSwordProjectPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + @"\CrossWire\The SWORD Project";
            return new string[] { strSwordProjectPath };
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
                webBrowserNetBible.DoDragDrop(this, DragDropEffects.Link);
            }
        }

        public void OnDoOnMouseUp(string strScriptureReference)
        {
            Console.WriteLine("OnDoOnMouseUp: " + strScriptureReference);
            m_bMouseDown = false;
            ScriptureReference = strScriptureReference;
            DisplayVerses();
        }

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
            // Point ptTooltip = new Point(Cursor.Position.X - ClientRectangle.Left, Cursor.Position.Y - ClientRectangle.Top);
            Point ptTooltip = Cursor.Position;
            ptTooltip.Offset(-ClientRectangle.Location.X + 20, -ClientRectangle.Location.Y - 20);
            tooltipNBFNs = new NetBibleFootnoteTooltip(s, ptTooltip);
            tooltipNBFNs.Show();
        }

        public void HideHoverOver()
        {
            if (tooltipNBFNs != null)
            {
                //if there is a different tooltip showing destroy it
                tooltipNBFNs.Dispose();
                tooltipNBFNs = null;
            }
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
