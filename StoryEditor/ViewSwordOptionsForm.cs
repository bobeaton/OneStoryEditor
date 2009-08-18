using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OneStoryProjectEditor
{
    public partial class ViewSwordOptionsForm : Form
    {
        protected const string CstrSwordLink = "www.crosswire.org/sword/modules/ModDisp.jsp?modType=Bibles";

        protected List<NetBibleViewer.SwordResource> _lstBibleResources;

        public ViewSwordOptionsForm(ref List<NetBibleViewer.SwordResource> lstBibleResources)
        {
            _lstBibleResources = lstBibleResources;

            InitializeComponent();

            foreach (NetBibleViewer.SwordResource aSR in lstBibleResources)
                checkedListBoxSwordBibles.Items.Add(String.Format("{0}: {1}", aSR.Name, aSR.Description), aSR.Loaded);

            linkLabelLinkToSword.Links.Add(6, 4, CstrSwordLink);
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < checkedListBoxSwordBibles.Items.Count; i++)
                _lstBibleResources[i].Loaded = checkedListBoxSwordBibles.GetItemChecked(i);

            DialogResult = DialogResult.OK;
            Close();
        }

        private void linkLabelLinkToSword_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string strSwordLink = (string)e.Link.LinkData;
            if (!String.IsNullOrEmpty(strSwordLink))
                System.Diagnostics.Process.Start(strSwordLink);
        }
    }
}
