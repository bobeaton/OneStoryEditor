using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using NetLoc;
using Utilities;

namespace OneStoryProjectEditor
{
	/// <summary>
	/// Allow user to edit renderings associated with a Biblical term.
	/// </summary>
	public partial class EditRenderingsForm : TopForm
	{
		private TermRendering termRendering;

		public EditRenderingsForm(
			Font font, 
			string renderings, 
			TermRendering termRendering, 
			string scrTextName,
			Localization termLocalization)
            : base(true)
        {
			InitializeComponent();
			Localizer.Ctrl(this);
			DialogRestorer.Register(this);

			this.termRendering = termRendering;

            textRenderings.Font = font;
			if (renderings != "")
				textRenderings.Text = renderings;
			else
				textRenderings.Text = termRendering.Renderings.Trim();

			textNotes.Text = termRendering.Notes.Trim();
			textTag.Text = termRendering.Tag.Trim();

			txtTermDefinition.Text = termLocalization.Gloss + " --- " + termLocalization.Notes;

			//! set alignment to match text direction
			this.Text = Localizer.Str("Edit Renderings") + ": " + scrTextName;
		}

		private EditRenderingsForm()
		{
			InitializeComponent();
			Localizer.Ctrl(this);
		}

		private void cmdOK_Click(object sender, EventArgs e)
		{
            try
            {
                termRendering.Renderings = textRenderings.Text.Trim();
            }
            catch (ArgumentException exc)
            {
                MessageBox.Show(exc.Message);
                return;
            }

			termRendering.Notes = textNotes.Text.Trim();
			termRendering.Tag = textTag.Text.Trim();
			termRendering.Guess = false;

			this.DialogResult = DialogResult.OK;
		}

		private void cmdCancel_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
		}

		private void EditRenderingsForm_Shown(object sender, EventArgs e)
		{
			// Place cursor at end of string, unselect text string so that
			// it will not be accidentally deleted.
			textRenderings.SelectionStart = textRenderings.Text.Length;
			textRenderings.SelectionLength = 0;
			textRenderings.Focus();
		}
	}
}