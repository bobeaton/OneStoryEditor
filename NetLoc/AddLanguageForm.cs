using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NetLoc
{
	partial class AddLanguageForm : Form
	{
		public AddLanguageForm()
		{
			InitializeComponent();
            Localizer.Ctrl(this);
		}

		public string LanguageId
		{
			get { return uiLangId.Text; }
		}

		public string LanguageName
		{
			get { return uiLangName.Text; }
		}
	}
}