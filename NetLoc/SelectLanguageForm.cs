using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NetLoc
{
	public partial class SelectLanguageForm : Form
	{
		Localizer localizer;
		List<LocLanguage> languages;

		public SelectLanguageForm(Localizer localizer, string defaultLangId)
		{
			InitializeComponent();

			this.localizer = localizer;
			this.languages = new List<LocLanguage>(localizer.Languages);

			foreach (LocLanguage lang in languages)
			{
				uiLanguages.Items.Add(lang.Name);
				if (lang.Id == defaultLangId)
					uiLanguages.SelectedIndex = uiLanguages.Items.Count - 1;
			}

            if (uiLanguages.SelectedIndex == -1)
                uiLanguages.SelectedIndex = languages.FindIndex(lang => lang.Id == "en");
		}

		public string LanguageId
		{
			get { return languages[uiLanguages.SelectedIndex].Id; }
		}

		private void uiLanguages_SelectedIndexChanged(object sender, EventArgs e)
		{
			uiOk.Enabled = uiLanguages.SelectedIndex != -1;
		}
	}
}
