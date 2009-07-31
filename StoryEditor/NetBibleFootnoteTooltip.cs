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
		SWMgr manager = new SWMgr();
        SWModule activeModule = null;
        string action, type, value;

        public NetBibleFootnoteTooltip(string key, Point location)
		{
			InitializeComponent();

			//Record the key so we don't popup this hover over again
			this.Tag = key;

			//Set the location
			this.Location = location;

			//Change cursor
			this.Cursor = Cursors.WaitCursor;

			//Parse the link
            string strModule = null;
			key = key.Substring(key.IndexOf('?')+1); //key.Replace("passagestudy.jsp?", "");
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

            if (action.Equals("showStrongs") && type.Equals("Greek"))
                ShowStrongsGreek(value);
            else if (action.Equals("showStrongs") && type.Equals("Hebrew"))
                ShowStrongsHebrew(value);
            else if (action.Equals("showMorph") && type.Contains("strongMorph"))
                ShowMorphRobinson(value);
            else if (action.Equals("showMorph") && type.Contains("robinson"))
                ShowMorphRobinson(value);
            else if (action.Equals("showNote") && type.Contains("x"))
            {
                SWModule module = manager.getModule(strModule);
                ShowNote(module, new SWKey(strReference), value);
            }
            else if (action.Equals("showNote") && type.Contains("n"))
            {
                SWModule module = manager.getModule(strModule);
                ShowNote(module, new SWKey(strReference), value);
            }
            else
                SetDisplayText("");

			//Change cursor
			this.Cursor = Cursors.Default;
		}

        private void ShowNote(SWModule module, SWKey tmpKey, string NoteType)
        {
            string s = tmpKey.getText();
            AttributeListMap list;
            AttributeTypeListMap listType;
            AttributeValueMap listValue;

            manager.setGlobalOption("Footnotes", "On");
            s = module.RenderText(tmpKey);
            listType = module.getEntryAttributesMap();
            list = listType.get(new SWBuf("Footnote"));
            listValue = list.get(new SWBuf(NoteType));
            s = listValue.get(new SWBuf("body")).c_str();

            SetDisplayText(module.StripText(s));
        }

        internal void SetDisplayText(string text)
		{
			//Used until I 
			if (string.IsNullOrEmpty(text))
				text =
				  "UNKNOWN KEY\r\n\r\n"
				+ "Key: " + this.Tag + "\r\n"
				+ "Action: " + action + "\r\n"
				+ "Type: " + type + "\r\n"
				+ "Value: " + value + "\r\n";

			//Display strongs information
			lblText.Text = text.Replace("<br>", "\r\n").Replace("<br />", "\r\n");
		}

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
	}
}