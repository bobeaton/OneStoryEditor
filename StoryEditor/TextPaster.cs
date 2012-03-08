using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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

        internal void TriggerPaste(bool bLeftClicked, CtrlTextBox textBox)
        {
            if (bLeftClicked)
            {
                var txtCurrent = textBox.Text;
                _undoStack.Add(textBox, txtCurrent);
                if (txtCurrent.LastOrDefault() != ' ')
                if ((Text.Length > 0) && (Text[Text.Length - 1] != ' '))
                    txtCurrent += ' ';
                txtCurrent += CurrentLine;
                textBox.Text = txtCurrent;
                ViewNextLine();
                Focus();    // so it gets the next keyboard input
            }
            else
            {
                ButtonUndoLastClick(null, null);
            }
        }

        internal void TriggerPaste(bool bLeftClicked, HtmlElement elemTextArea)
        {
            if (bLeftClicked)
            {
                var txtCurrent = elemTextArea.InnerText;
                _undoStack.Add(elemTextArea, txtCurrent);
                if ((txtCurrent != null) && (txtCurrent.LastOrDefault() != ' '))
                    txtCurrent += ' ';
                txtCurrent += CurrentLine;
                elemTextArea.InnerText = txtCurrent;
                ViewNextLine();
                Focus();    // so it gets the next keyboard input
            }
            else
            {
                ButtonUndoLastClick(null, null);
            }
        }

        UndoStack _undoStack = new UndoStack();

        private void CheckBoxPauseCheckedChanged(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert(sender == checkBoxPause);
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
                else if (val.Item1 is TextBox)
                    (val.Item1 as TextBox).Text = val.Item2;
                else if (val.Item1 is HtmlElement)
                    (val.Item1 as HtmlElement).InnerText = val.Item2;
                else
                    System.Diagnostics.Debug.Assert(false, "oops, did we add support for something new?");
            }
        }
    }
}
