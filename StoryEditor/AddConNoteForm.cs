using System;
using System.Windows.Forms;
using NetLoc;

namespace OneStoryProjectEditor
{
    public partial class AddConNoteForm : TopForm
    {
        private AddConNoteForm()
        {
            InitializeComponent();
            Localizer.Ctrl(this);
        }

        // buildNoteHtml builds the one-note preview for the dialog pane's own engine (its host may differ from the
        //  parent pane's, or fall back to IE while the dialog is open: then the preview is built again, not replaced by
        //  the whole notes page)
        public AddConNoteForm(Type typeConNotePane, StoryEditor theSe, StoryData storyData,
            Func<HtmlEngine, string> buildNoteHtml)
        {
            InitializeComponent();
            Localizer.Ctrl(this);

            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            var pane = Activator.CreateInstance(typeConNotePane) as HtmlConNoteControl;
            System.Diagnostics.Debug.Assert(pane != null);
            pane.TheSE = theSe;
            pane.StoryData = storyData;
            pane.Dock = DockStyle.Fill;
            pane.Tag = "Don't LoadDocument if font is changed";
            tableLayoutPanel.Controls.Add(pane, 0, 1);
            tableLayoutPanel.SetColumnSpan(pane, 2);
            textBoxConNotes.Text = pane.PaneLabel();
            pane.LoadPage(buildNoteHtml);
            this.tableLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private void ButtonOkClick(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
