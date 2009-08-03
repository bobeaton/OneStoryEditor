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
    // have to make this com visible, because 'this' needs to be visible to COM for the 
    // call to: webBrowserNetBible.ObjectForScripting = this;
    public partial class StoryEditor : Form
    {
        internal const string cstrCaption = "OneStory Project Editor";
        internal const string cstrButtonDropTargetName = "buttonDropTarget";

        protected string m_strProjectFilename = null;

        protected StoryProject m_projFile = null;

        public Font VernacularFont = new Font("Arial Unicode MS", 12);
        public Color VernacularFontColor = Color.Maroon;
        public Font NationalBTFont = new Font("Arial Unicode MS", 12);
        public Color NationalBTFontColor = Color.Green;
        public Font InternationalBTFont = new Font("Times New Roman", 10);
        public Color InternationalBTFontColor = Color.Blue;

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
            try
            {
                InitializeNetBibleViewer();
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Problem initializing Sword (the Net Bible viewer):{0}{0}{1}", Environment.NewLine, ex.Message), StoryEditor.cstrCaption);
            }
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

            m_projFile = new StoryProject();
            try
            {
                m_projFile.ReadXml(strProjectFilename);
                SetupTitleBar(strProjectFilename, m_projFile.story[0].name);
                /*
                StoryProject.VernacularFontRow aVFRow = aProj.VernacularFont[0];
                VernacularFont = new Font(aVFRow.FontName, aVFRow.FontSize);
                VernacularFontColor = Color.FromName(aVFRow.FontColor);

                StoryProject.NationalBTFontRow aNFRow = aProj.NationalBTFont[0];
                NationalBTFont = new Font(aNFRow.FontName, aNFRow.FontSize);
                NationalBTFontColor = Color.FromName(aNFRow.FontColor);

                StoryProject.InternationalBTFontRow aIFRow = aProj.InternationalBTFont[0];
                InternationalBTFont = new Font(aIFRow.FontName, aIFRow.FontSize);
                InternationalBTFontColor = Color.FromName(aIFRow.FontColor);
                */
                flowLayoutPanelVerses.VerticalScroll.Enabled = true;
                int nVerseIndex = 0;
                AddDropTargetToFlowLayout(nVerseIndex++);
                foreach (StoryProject.verseRow aRow in m_projFile.stories[0].GetstoryRows()[0].GetversesRows()[0].GetverseRows())
                {
                    VerseBtControl aVerseCtrl = new VerseBtControl(this, aRow, nVerseIndex);
                    aVerseCtrl.UpdateHeight(Panel1_Width);
                    flowLayoutPanelVerses.Controls.Add(aVerseCtrl);
                    AddDropTargetToFlowLayout(nVerseIndex);
                    nVerseIndex++;
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(String.Format("Unable to open project file '{1}'{0}{0}{2}{0}{0}Send the project file to bob_eaton@sall.com for help",
                    Environment.NewLine, strProjectFilename, ex.Message), cstrCaption);
            }
        }

        protected void AddDropTargetToFlowLayout(int nVerseIndex)
        {
            Button buttonDropTarget = new Button();
            buttonDropTarget.AllowDrop = true;
            buttonDropTarget.Location = new System.Drawing.Point(3, 3);
            buttonDropTarget.Name = cstrButtonDropTargetName + nVerseIndex.ToString();
            buttonDropTarget.Size = new System.Drawing.Size(75, 5);
            buttonDropTarget.TabIndex = nVerseIndex;
            buttonDropTarget.UseVisualStyleBackColor = true;
            buttonDropTarget.Visible = false;
            buttonDropTarget.DragEnter += new DragEventHandler(buttonDropTarget_DragEnter);
            buttonDropTarget.DragDrop += new DragEventHandler(buttonDropTarget_DragDrop);
            flowLayoutPanelVerses.Controls.Add(buttonDropTarget);
        }

        void buttonDropTarget_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(VerseBtControl)))
            {
                VerseBtControl aVerseCtrl = (VerseBtControl)e.Data.GetData(typeof(VerseBtControl));
                Button btnTarget = (Button)sender;
                string strTargetName = btnTarget.Name;
                string strTargetVerse = strTargetName.Substring(cstrButtonDropTargetName.Length);
                int nInsertionIndex = (int)Convert.ToInt32(strTargetVerse);
                if ((nInsertionIndex == (aVerseCtrl.VerseNumber - 1)) || (nInsertionIndex == (aVerseCtrl.VerseNumber)))
                    DoCopy(nInsertionIndex, aVerseCtrl);
                else
                    DoMove(nInsertionIndex, aVerseCtrl);
            }
        }

        void DoCopy(int nInsertionIndex, VerseBtControl aVerseCtrl)
        {
            // TODO:
        }

        void DoMove(int nInsertionIndex, VerseBtControl aVerseCtrl)
        {
            // TODO:
        }

        void buttonDropTarget_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(VerseBtControl)))
            {
                VerseBtControl aVerseCtrl = (VerseBtControl)e.Data.GetData(typeof(VerseBtControl));
                Button btnTarget = (Button)sender;
                string strTargetName = btnTarget.Name;
                string strTargetVerse = strTargetName.Substring(cstrButtonDropTargetName.Length);
                int nInsertionIndex = (int)Convert.ToInt32(strTargetVerse);
                if ((nInsertionIndex == (aVerseCtrl.VerseNumber - 1)) || (nInsertionIndex == (aVerseCtrl.VerseNumber)))
                    e.Effect = DragDropEffects.Copy;
                else
                    e.Effect = DragDropEffects.Move;
            }
        }
        
        internal void LightUpDropTargetButtons()
        {
            foreach (Control ctrl in flowLayoutPanelVerses.Controls)
            {
                if (ctrl is Button)
                {
                    ctrl.Visible = true;
                }
            }
        }

        internal void DimDropTargetButtons()
        {
            foreach (Control ctrl in flowLayoutPanelVerses.Controls)
            {
                if (ctrl is Button)
                    ctrl.Visible = false;
            }
        }

        protected void InitializeNetBibleViewer()
        {
            netBibleViewer.InitNetBibleViewer();
            string strLastRef = "gen 1:5";
            if (!String.IsNullOrEmpty(Properties.Settings.Default.LastNetBibleReference))
                strLastRef = Properties.Settings.Default.LastNetBibleReference;
            SetNetBibleVerse(strLastRef);
        }

        internal void SetNetBibleVerse(string strScriptureReference)
        {
            if (splitContainerUpDown.Panel2Collapsed == true)
                viewNetBibleMenuItem.Checked = true;

            netBibleViewer.DisplayVerses(strScriptureReference);
        }

        protected int Panel1_Width
        {
            get
            {
                return splitContainerLeftRight.Panel1.Width - splitContainerLeftRight.Margin.Horizontal -
                    SystemInformation.VerticalScrollBarWidth - 2;
            }
        }

        private void splitContainerUpper_SplitterMoved(object sender, SplitterEventArgs e)
        {
            UpdateVersePanel();
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

        protected void UpdateVersePanel()
        {
            foreach (Control ctrl in flowLayoutPanelVerses.Controls)
            {
                if (ctrl is VerseBtControl)
                {
                    VerseBtControl aVerseCtrl = (VerseBtControl)ctrl;
                    aVerseCtrl.UpdateView(this);
                    aVerseCtrl.UpdateHeight(Panel1_Width);
                }
            }
        }

        private void viewFieldMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            UpdateVersePanel();
        }

        private void viewNetBibleMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert(sender is ToolStripMenuItem);
            ToolStripMenuItem tsm = (ToolStripMenuItem)sender;
            splitContainerUpDown.Panel2Collapsed = !tsm.Checked;
        }
    }
}
