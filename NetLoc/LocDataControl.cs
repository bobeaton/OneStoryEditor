using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace NetLoc
{
	public partial class LocDataControl : UserControl
	{
		LocLanguage locLanguage;
		List<LocKey> keys;
		string languageId;

		DataTable editTable;

		public LocDataControl()
		{
			InitializeComponent();
            Localizer.Ctrl(this);
        }

		public void Bind(LocLanguage locLanguage, List<LocKey> keys, string languageId) 
		{
			// Remove any current data source and commit changes
			this.Validate();
			uiGrid.EndEdit();

			this.locLanguage = locLanguage;
			this.keys = keys;
			this.languageId = languageId;

			if (locLanguage == null)
			{
				uiGrid.DataSource = null;
				uiGrid.Rows.Clear();
				uiGrid.Refresh();
				this.Enabled = false;
			}
			else
				this.Enabled = true;

			editTable = new DataTable();
			editTable.Columns.Add(new DataColumn("Path"));
			editTable.Columns.Add(new DataColumn("Default"));
			editTable.Columns.Add(new DataColumn("Value"));
			editTable.RowChanged += new DataRowChangeEventHandler(editTable_RowChanged);
			editTable.PrimaryKey = new DataColumn[] { editTable.Columns["Path"] };

			foreach (LocKey key in keys)
			{
				if (editTable.Rows.Find(key.Path) != null)
					continue;

				DataRow row = editTable.NewRow();
				row["Path"] = key.Path;
				row["Default"] = EscapeString(key.DefaultValue);
				string value = locLanguage[key.Path];
				if (value != null)
					row["Value"] = EscapeString(value);
				editTable.Rows.Add(row);
			}
			uiGrid.DataSource = editTable;
		}

		public void Unbind()
		{
			// Remove any current data source and commit changes
			this.Validate();
			uiGrid.EndEdit();
            if (editTable != null)      // rde: happened once
                editTable.Rows.Clear();
			uiGrid.Refresh();
			this.Enabled = false;
		}

		void editTable_RowChanged(object sender, DataRowChangeEventArgs e)
		{
			if (e.Action == DataRowAction.Change)
			{
				if (e.Row["Value"].ToString().Length > 0)
					locLanguage[(string)e.Row["Path"]] = UnescapeString(e.Row["Value"].ToString());
				else
					locLanguage[(string)e.Row["Path"]] = null;
			}
		}

		static private string UnescapeString(string str)
		{
			// Escaping is now disabled
			//str = str.Replace(@"\\", @"(some string that will never be found)");
			//str = str.Replace(@"\n", "\n");
			//str = str.Replace(@"\r", "\r");
			//str = str.Replace(@"(some string that will never be found)", @"\");
			return str;
		}

		static private string EscapeString(string str)
		{
			// Escaping is now disabled
			//str = str.Replace(@"\", @"\\");
			//str = str.Replace("\n", @"\n");
			//str = str.Replace("\r", @"\r");
			return str;
		}

	    public void SetTranslationColumnFont(Font font)
	    {
            if ((uiGrid != null) && (uiGrid.Columns["ValueColumn"] != null))
            {
                uiGrid.Columns["ValueColumn"].DefaultCellStyle.Font = font;
                uiGrid.RowTemplate.Height = font.Height + 6;    // magic number
            }
	    }
	}
}
