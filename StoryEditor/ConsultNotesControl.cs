using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    public partial class ConsultNotesControl : OneStoryProjectEditor.ResizableControl
    {
        protected const string cstrFieldNameConsultantNote = "ConsultantNoteControl";

        internal int VerseNumber = -1;

        public ConsultNotesControl(ConsultNotesDataConverter aCNsDC, int nVerseNumber)
        {
            VerseNumber = nVerseNumber;
            InitializeComponent();

            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();

            this.labelReference.Text = VerseBtControl.cstrVerseName + nVerseNumber.ToString();
            this.tableLayoutPanel.Controls.Add(this.labelReference, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.buttonDragDropHandle, 1, 0);

            if (aCNsDC.IsConsultNotes)
            {
                int nRowIndex = 1;
                foreach (ConsultNoteDataConverter aCNDC in aCNsDC.ConsultNotes)
                {
                    ConsultNoteControl aCNCtrl = new ConsultNoteControl(aCNDC);
                    aCNCtrl.Name = cstrFieldNameConsultantNote + nRowIndex.ToString();
                    aCNCtrl.ParentControl = this;

                    InsertRow(nRowIndex);
                    tableLayoutPanel.SetColumnSpan(aCNCtrl, 2);
                    tableLayoutPanel.Controls.Add(aCNCtrl, 0, nRowIndex);
                    nRowIndex++;
                }
            }

            this.tableLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}
