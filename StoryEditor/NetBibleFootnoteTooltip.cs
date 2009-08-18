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
	public partial class NetBibleFootnoteTooltip : Form
	{
        SWMgr _manager;
        Dictionary<string, SWModule> _lstModules = new Dictionary<string, SWModule>();
        string action, type, value;

        public NetBibleFootnoteTooltip(SWMgr manager)
		{
            _manager = manager;
			InitializeComponent();
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
            if (action.Equals("showNote") && type.Contains("x") && !String.IsNullOrEmpty(strModule))
            {
                // I'm imagining that the module of the note could be different from the module of the text
                SWModule moduleForNote;
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
                SWModule moduleForNote;
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
                SetDisplayText("");
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

            SetDisplayText(strFootnote);    // module.StripText(strFootnote));
        }

        internal void SetDisplayText(string text)
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

			//Display text in a web browser so we get Greek/Hebrew, etc.
            webBrowser.DocumentText = text; // .Replace("<br>", "\r\n").Replace("<br />", "\r\n");
		}

        private void NetBibleFootnoteTooltip_FormClosing(object sender, FormClosingEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("NetBibleFootnoteTooltip_FormClosing");
            Hide();
            e.Cancel = true;
        }

        private void webBrowser_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(String.Format("webBrowser_PreviewKeyDown: KeyCode: {0}", e.KeyCode));
            if (e.KeyCode == Keys.Escape)
                Close();
        }

        private void NetBibleFootnoteTooltip_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(String.Format("NetBibleFootnoteTooltip_PreviewKeyDown: KeyCode: {0}", e.KeyCode));
            if (e.KeyCode == Keys.Escape)
                Close();
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