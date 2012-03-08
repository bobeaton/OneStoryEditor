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
            InitTextBoxes(++LineNumber);
        }

        internal void Undo()
        {
            InitTextBoxes(--LineNumber);
        }

        internal void ViewNextLine()
        {
            InitTextBoxes(++LineNumber);
        }

        private void ButtonUndoLastClick(object sender, EventArgs e)
        {
            var nLastIndex = _listUndoStack.Count - 1;
            var val = _listUndoStack[nLastIndex];
            _listUndoStack.RemoveAt(nLastIndex);
            val.Item1.Text = val.Item2;
            InitTextBoxes(--LineNumber);
        }

        internal void TriggerPaste(bool isLeftMouse, CtrlTextBox textBox)
        {
            if (isLeftMouse)
            {
                var txtCurrent = textBox.Text;
                _listUndoStack.Add(new Tuple<TextBox, string>(textBox, txtCurrent));
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

        List<Tuple<TextBox, string>> _listUndoStack = new List<Tuple<TextBox, string>>();

        private void CheckBoxPauseCheckedChanged(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert(sender == checkBoxPause);
            StoryEditor.TextPaster = (checkBoxPause.Checked) ? null : this;
        }
    }
}
