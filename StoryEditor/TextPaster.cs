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
using NetLoc;

namespace OneStoryProjectEditor
{
    public partial class TextPaster : Form
    {
        private List<string> _listLines;
        private int _nIndex = 0;

        private TextPaster()
        {
            InitializeComponent();
            Localizer.Ctrl(this);
        }

        public TextPaster(string dummy)
        {
            InitializeComponent();
            Localizer.Ctrl(this);

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                _listLines = File.ReadAllLines(openFileDialog.FileName).ToList();
                InitTextBoxes(_nIndex);
                StoryEditor.TextPaster = this;
                TopMost = true;
            }

            int screenHeight = Screen.PrimaryScreen.WorkingArea.Height;
            var location = Location;
            location.Y = screenHeight - Height;
            Location = location;
        }
        private void InitTextBoxes(int nIndex)
        {
            var nMaxIndex = _listLines.Count - 1;
            if (_nIndex < nMaxIndex)
                CurrentLine = _listLines[nIndex];

            if (_nIndex < (nMaxIndex - 1))
                NextLine = _listLines[nIndex + 1];

            if (_nIndex < (nMaxIndex - 2))
                NextNextLine = _listLines[nIndex + 2];
        }

        private static bool _bAskAboutStrippingSfm = true;
        private static bool _bStripSfms = false;
        
        public string CurrentLine
        {
            get { return textBoxCurrentLine.Text; }
            set { textBoxCurrentLine.Text = value; }
        }

        public string NextLine
        {
            get { return textBoxNextLine.Text; }
            set { textBoxNextLine.Text = value; }
        }

        public string NextNextLine
        {
            get { return textBoxNextNextLine.Text; }
            set { textBoxNextNextLine.Text = value; }
        }

        public int LineNumber
        {
            get { return _nIndex; }
            set { _nIndex = value; }
        }

        private void TextPasterFormClosing(object sender, FormClosingEventArgs e)
        {
            StoryEditor.TextPaster = null;
        }

        private void ButtonDeleteLineClick(object sender, EventArgs e)
        {
            if (LineNumber < _listLines.Count)
            {
                _undoStack.Add(null, null);
                InitTextBoxes(++LineNumber);
            }
        }

        internal void ViewNextLine()
        {
            if (LineNumber < _listLines.Count) 
                InitTextBoxes(++LineNumber);
        }

        private void ButtonUndoLastClick(object sender, EventArgs e)
        {
            _undoStack.UndoLast();
            if (LineNumber > 0)
                InitTextBoxes(--LineNumber);
        }

        private delegate void SetText(object tb, string str);
        private void SetTextBoxText(object tb, string str)
        {
            var textBox = tb as CtrlTextBox;
            Debug.Assert(textBox != null);
            textBox.Text = str;
            textBox._ctrlVerseParent.TheSE.Modified = true;
        }
        private void SetTextareaText(object tb, string str)
        {
            var elemTextArea = tb as HtmlElement;
            Debug.Assert(elemTextArea != null);
            SetElementText(elemTextArea, str);
        }

        internal static void SetElementText(HtmlElement elemTextArea, string str)
        {
            elemTextArea.InnerText = str;
            elemTextArea.InvokeMember("onchange"); // triggers the update to the data buffer
        }

        private delegate string GetText(object tb);
        private string GetTextBoxText(object tb)
        {
            var textBox = tb as CtrlTextBox;
            return textBox.Text;
        }

        private string GetTextareaText(object tb)
        {
            var elemTextArea = tb as HtmlElement;
            return elemTextArea.InnerText;
        }

        private void TriggerPaste(bool bLeftClicked, object tb, 
            SetText setter, GetText getter)
        {
            if (bLeftClicked)
            {
                var txtCurrent = getter(tb);
                _undoStack.Add(tb, txtCurrent);
                if (!String.IsNullOrEmpty(txtCurrent) && (txtCurrent[txtCurrent.Length - 1] != ' '))
                    txtCurrent += ' ';
                txtCurrent += GetNextLine(true);
                setter(tb, txtCurrent);
                ViewNextLine();
                Focus();    // so it gets the next keyboard input
            }
            else
            {
                ButtonUndoLastClick(null, null);
            }
        }
        internal void TriggerPaste(bool bLeftClicked, CtrlTextBox textBox)
        {
            TriggerPaste(bLeftClicked, textBox, SetTextBoxText, GetTextBoxText);
        }

        internal void TriggerPaste(bool bLeftClicked, HtmlElement elemTextArea)
        {
            TriggerPaste(bLeftClicked, elemTextArea, SetTextareaText, GetTextareaText);
        }

        public string GetNextLine(bool bCheckForSfm)
        {
            var strText = CurrentLine;

            if (bCheckForSfm)
            {
                // if the text is SFM... ask if they want it stripped off`
                int nIndex;
                if (!String.IsNullOrEmpty(strText) &&
                    (strText.Length > 1) &&
                    (strText[0] == '\\') &&
                    ((nIndex = strText.IndexOf(' ')) != -1))
                {
                    if (_bAskAboutStrippingSfm)
                    {
                        _bAskAboutStrippingSfm = false; // just ask once per run
                        var res =
                            LocalizableMessageBox.Show(
                                Localizer.Str("This looks like SFM data. Would you like me to strip off the markers?"),
                                StoryEditor.OseCaption,
                                MessageBoxButtons.YesNoCancel);
                        if (res == DialogResult.Yes)
                            _bStripSfms = true;
                    }

                    if (_bStripSfms)
                        strText = strText.Substring(nIndex + 1);
                }
            }

            return strText;
        }

        UndoStack _undoStack = new UndoStack();

        private void CheckBoxPauseCheckedChanged(object sender, EventArgs e)
        {
            Debug.Assert(sender == checkBoxPause);
            StoryEditor.TextPaster = (checkBoxPause.Checked) ? null : this;
        }

        private class UndoStack : List<Tuple<object, string>>
        {
            public void Add(object textBox, string strText)
            {
                Add(new Tuple<object, string>(textBox, strText));
            }

            internal void UndoLast()
            {
                if (Count == 0)
                    return;

                var nLastIndex = Count - 1;
                var val = this[nLastIndex];
                RemoveAt(nLastIndex);
                if (val.Item1 == null)
                    ; // no op -- means we deleted a line of data
                else if (val.Item1 is CtrlTextBox)
                    (val.Item1 as CtrlTextBox).Text = val.Item2;
                else if (val.Item1 is HtmlElement)
                    SetElementText(val.Item1 as HtmlElement, val.Item2);
                else
                    Debug.Assert(false, "oops, did we add support for something new?");
            }
        }
    }
}
