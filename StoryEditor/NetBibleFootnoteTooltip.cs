using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Xml;

namespace OneStoryProjectEditor
{
    public partial class MinimalHtmlForm : Form
    {
        public MinimalHtmlForm()
        {
            InitializeComponent();
        }

        private void MinimalHtmlForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
            e.Cancel = true;
        }

        private void webBrowser_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                Close();
        }

        private void MinimalHtmlForm_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                Close();
        }
    }

    public class MoveConNoteTooltip : MinimalHtmlForm
    {
        public void SetDocumentText(ConsultNoteDataConverter aConNote, StoryEditor theSE,
            Point ptLocation)
        {
            Location = ptLocation;
            if (!theSE.splitContainerLeftRight.Panel2Collapsed)
                Width = theSE.splitContainerLeftRight.Panel2.Size.Width;

            webBrowser.DocumentText = aConNote.Html(null, theSE.theCurrentStory.ProjStage,
                theSE.LoggedOnMember, 0, 0);
        }
    }

	public class NetBibleFootnoteTooltip : MinimalHtmlForm
	{
        SWMgr _manager;
        Dictionary<string, SWModule> _lstModules = new Dictionary<string, SWModule>();
        string action, type, value;

        public NetBibleFootnoteTooltip(SWMgr manager)
		{
            _manager = manager;
		}

        public void ShowFootnote(string key, Point location)
        {
            //Record the key so we don't popup this hover over again
            Tag = key;

            //Set the location
            Location = location;

            //Parse the link
            string strModule = null;
            key = key.Substring(key.IndexOf('?') + 1); //key.Replace("passagestudy.jsp?", "");
            string[] splitKey = key.Split('&');
            action = splitKey[0].Replace("action=", "");
            type = splitKey[1].Replace("type=", "");
            value = splitKey[2].Replace("value=", "");
            if (splitKey.GetUpperBound(0) >= 3)
                strModule = splitKey[3].Replace("module=", "");

            string strReference = null;
            if (splitKey.GetUpperBound(0) >= 4)
            {
                strReference = splitKey[4].Replace("passage=", "");
                strReference = strReference.Replace("%3A", ":");
                strReference = strReference.Replace("+", " ");
            }

            /*
            if (action.Equals("showStrongs") && type.Equals("Greek"))
                ShowStrongsGreek(value);
            else if (action.Equals("showStrongs") && type.Equals("Hebrew"))
                ShowStrongsHebrew(value);
            else if (action.Equals("showMorph") && type.Contains("strongMorph"))
                ShowMorphRobinson(value);
            else if (action.Equals("showMorph") && type.Contains("robinson"))
                ShowMorphRobinson(value);
            else 
            */
            SWModule moduleForNote = null;
            if (action.Equals("showNote") && type.Contains("x") && !String.IsNullOrEmpty(strModule))
            {
                // I'm imagining that the module of the note could be different from the module of the text
                if (!_lstModules.TryGetValue(strModule, out moduleForNote))
                {
                    moduleForNote = _manager.getModule(strModule);
                    _lstModules.Add(strModule, moduleForNote);
                }
                ShowNote(moduleForNote, new SWKey(strReference), value);
            }
            else if (action.Equals("showNote") && type.Contains("n") && !String.IsNullOrEmpty(strModule))
            {
                // I'm imagining that the module of the note could be different from the module of the text
                if (!_lstModules.TryGetValue(strModule, out moduleForNote))
                {
                    moduleForNote = _manager.getModule(strModule);
                    _lstModules.Add(strModule, moduleForNote);
                }
                ShowNote(moduleForNote, new SWKey(strReference), value);
            }
            else
            {
                System.Diagnostics.Debug.Assert(false);
                SetDisplayText(moduleForNote, "");
            }

            Show();
            webBrowser.Focus();
        }

        private void ShowNote(SWModule module, SWKey tmpKey, string NoteType)
        {
            AttributeListMap list;
            AttributeTypeListMap listType;
            AttributeValueMap listValue;

            // this line looks like it's not needed, but it is. It has some non-obvious side effect
            //  such that if it's missing, then the call to "list.get(new SWBuf(NoteType));" fails
            string s = module.RenderText(tmpKey);

            listType = module.getEntryAttributesMap();
            list = listType.get(new SWBuf("Footnote"));
            listValue = list.get(new SWBuf(NoteType));
            string strFootnote = listValue.get(new SWBuf("body")).c_str();

            SetDisplayText(module, strFootnote);    // module.StripText(strFootnote));
        }

        internal void SetDisplayText(SWModule module, string text)
		{
			//Used until I 
			if (string.IsNullOrEmpty(text))
				text =
				  "UNKNOWN KEY\r\n\r\n"
				+ "Key: " + Tag + "\r\n"
				+ "Action: " + action + "\r\n"
				+ "Type: " + type + "\r\n"
				+ "Value: " + value + "\r\n";

            // c_str is incorrectly returning a utf-8 encode string as widened utf-16, so work-around:
            byte[] aby = Encoding.Default.GetBytes(text);
            text = Encoding.UTF8.GetString(aby);

            if (module != null)
            {
                string strFontName, strModuleVersion = module.Name();
                if (Program._mapSwordModuleToFont.TryGetValue(strModuleVersion, out strFontName))
                    text = String.Format(NetBibleViewer.CstrAddFontFormat, text, strFontName);
            }

            //Display text in a web browser so we get Greek/Hebrew, etc.
            webBrowser.DocumentText = text; // .Replace("<br>", "\r\n").Replace("<br />", "\r\n");
		}

        /*
		private void ShowStrongsGreek(string value)
		{
			//Get the current verse info
			SWKey swKey = new SWKey(value);
			
			//Set the active module for this NetBibleFootnoteTooltip to StrongsGreek
			activeModule = manager.getModule("StrongsGreek");

			//Display strongs information
			SetDisplayText(activeModule.RenderText(swKey));
		}

		private void ShowStrongsHebrew(string value)
		{
			//Get the current verse info
			SWKey swKey = new SWKey(value);

			//Set the active module for this NetBibleFootnoteTooltip to StrongsGreek
			activeModule = manager.getModule("StrongsHebrew");

			//Display strongs information
			SetDisplayText(activeModule.RenderText(swKey));
		}

		private void ShowMorphRobinson(string value)
		{
			//Get the current verse info
			SWKey swKey = new SWKey(value);

			//Set the active module for this NetBibleFootnoteTooltip to StrongsGreek
			activeModule = manager.getModule("Robinson");

			//Display strongs information
			SetDisplayText(activeModule.RenderText(swKey));
		}
        */
	}
}