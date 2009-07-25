using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace StoryEditor
{
    public partial class StoryEditor : Form
    {
        internal const string cstrCaption = "OneStory Project Editor";
        protected string m_strProjectFilename = null;

        public enum UserTypes
        {
            eUndefined = 0,
            eCrafter,
            eUNS,
            eConsultant,
            eCoach
        }

        public StoryEditor(UserTypes e)
        {
            InitializeComponent();
#if DEBUG
            OpenProjectFile(@"C:\Code\StoryEditor\StoryEditor\StoryProject.osp");
#endif
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                OpenProjectFile(openFileDialog.FileName);
            }
        }

        protected void OpenProjectFile(string strProjectFilename)
        {
            if (Program.Modified)
                CheckForSaveDirtyFile();

            StoryProject aProj = new StoryProject();
            try
            {
                aProj.ReadXml(strProjectFilename);
                SetupTitleBar(strProjectFilename, aProj.story[0].name);

                flowLayoutPanelVerses.VerticalScroll.Enabled = true;
                int i = 1;
                foreach (StoryProject.verseRow aRow in aProj.stories[0].GetstoryRows()[0].GetversesRows()[0].GetverseRows())
                {
                    VerseBtControl aVerseCtrl = new VerseBtControl(aRow, i++);
                    aVerseCtrl.SetWidth(splitContainerUpper.Panel1.Width - splitContainerUpper.Margin.Right - splitContainerUpper.Margin.Left);
                    flowLayoutPanelVerses.Controls.Add(aVerseCtrl);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(String.Format("Unable to open project file '{1}'{0}{0}{2}{0}{0}Send the project file to bob_eaton@sall.com for help",
                    Environment.NewLine, strProjectFilename, ex.Message), cstrCaption);
            }
        }

        private void splitContainerUpper_SplitterMoved(object sender, SplitterEventArgs e)
        {
            foreach (Control ctrl in flowLayoutPanelVerses.Controls)
            {
                VerseBtControl aVerseCtrl = (VerseBtControl)ctrl;
                aVerseCtrl.SetWidth(splitContainerUpper.Panel1.Width - splitContainerUpper.Margin.Right - splitContainerUpper.Margin.Left);
            }
        }

        private DialogResult CheckForSaveDirtyFile()
        {
            DialogResult res = DialogResult.None;
            if (Program.Modified)
            {
                res = MessageBox.Show("Do you want to save the opened project first?", cstrCaption, MessageBoxButtons.YesNoCancel);
                if (res == DialogResult.Yes)
                    SaveClicked();
            }
            return res;
        }

        protected void SaveClicked()
        {
            if (String.IsNullOrEmpty(m_strProjectFilename))
                SaveAsClicked();
            else
                SaveFile(m_strProjectFilename);
        }

        protected void SaveAsClicked()
        {
            if (this.saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                m_strProjectFilename = saveFileDialog.FileName;
                SaveFile(m_strProjectFilename);
            }
        }

        protected void SaveFile(string strFilename)
        {
            /*
            try
            {
                File.WriteAllLines(strFilename, this.richTextBoxMapEditor.Lines, m_enc);
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show(String.Format("The map file '{0}' is locked. Is it read-only? Or opened in some other program? Unlock it and try again.", strFilename), cstrCaption);
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Unable to save the map file '{1}'{0}{0}{2}", Environment.NewLine, strFilename, ex.Message), cstrCaption);
                return;
            }

            Program.Modified = (m_strMapNameReal != strFilename);
            Program.AddFilenameToTitle(strFilename);

            // if it's been 5 minutes since our last backup...
            if ((DateTime.Now - m_dtLastSave) > m_tsBetweenBackups)
            {
                // ... hide a copy in the user's Application Data file
                File.Copy(m_strMapNameReal, GetBackupFilename(strFilename), true);
            }
            */
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveClicked();
        }

        protected void SetupTitleBar(string strProjectName, string strStoryName)
        {
            String str = String.Format("{0} -- {1} -- {2}", cstrCaption, strProjectName, strStoryName);
        }
    }
}
