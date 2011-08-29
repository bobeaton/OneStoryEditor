#define DebuggingDeserialize
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Xsl;
#if DEBUG
using Chorus.merge.xml.generic;
using NetLoc;

#endif

namespace EditResxLocalization
{
    public partial class FormEditResx : Form
    {
        private const string CstrDefLangId = "en";
        private EditableLocalization _elf;
        public bool Modified;

        private string strCurrentLangId = CstrDefLangId;

        public FormEditResx()
        {
            InitializeComponent();
            Localizer.Ctrl(this);

#if DebuggingDeserialize
            _elf = new EditableLocalization(true);
            _elf.Languages[0].SendEmailOnUpdate.Add("pete_dembrowski@hotmail.com");
            var str = _elf.ToString();
#endif
        }

        private void UpdateGrid(string strLangId)
        {
            if (String.IsNullOrEmpty(strLangId))
            {
                // first see if the previously selected language is in the current _elf
                if (_elf.Languages[strCurrentLangId] != null)
                    strLangId = strCurrentLangId;
                else
                    strLangId = strCurrentLangId = CstrDefLangId;
            }

            dataGridView.Rows.Clear();
            foreach (var entry in _elf.Entries)
            {
                System.Diagnostics.Debug.Assert(entry.Values.Count > 0);
                var valueEn = entry.Values[CstrDefLangId];
                var valueTr = entry.Values[strLangId];
                System.Diagnostics.Debug.Assert(valueEn != null);
                var aoValues = new object[]
                                   {
                                       entry.Id,
                                       valueEn.Text,
                                       (valueTr != null) ? valueTr.Text : null
                                   };
                var nIndex = dataGridView.Rows.Add(aoValues);
            }
        }

        private void MenuFileDropDownOpening(object sender, EventArgs e)
        {
            menuFileSave.Enabled =
                menuFileSaveAs.Enabled =
                (_elf != null);
        }

        private void MenuImportMergeDropDownOpening(object sender, EventArgs e)
        {
            menuImportMergeFromResx.Enabled = (_elf != null);
        }

        private void MenuFileNewClick(object sender, EventArgs e)
        {
            if (!CheckForSaveFirst())
                return;
            _elf = EditableLocalization.GetEditableLocalizations(null);
            Modified = true;
            UpdateGrid(CstrDefLangId);
        }

        private void MenuFileOpenClick(object sender, EventArgs e)
        {
            if (!CheckForSaveFirst())
                return;

            openFileDialog.DefaultExt = "elf";
            openFileDialog.FileName = null;
            openFileDialog.Filter = "Localization Files|*.elf";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                _elf = EditableLocalization.GetEditableLocalizations(openFileDialog.FileName);
                UpdateGrid(null);
            }
        }

        private bool CheckForSaveFirst()
        {
            if ((_elf != null) && Modified)
            {
                DialogResult res = MessageBox.Show(Properties.Resources.IDS_SaveFirstQuery,
                                                   Properties.Resources.IDS_Caption,
                                                   MessageBoxButtons.YesNoCancel);
                if (res == DialogResult.Cancel)
                    return false;

                if (res == DialogResult.Yes)
                {
                    if (String.IsNullOrEmpty(_elf.Path2Elf))
                        return MenuFileSaveAsClickEx();

                    MenuFileSaveClick(null, null);
                }
            }
            
            return true;
        }

        private void MenuFileSaveClick(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert(_elf != null);
            if (String.IsNullOrEmpty(_elf.Path2Elf))
                MenuFileSaveAsClickEx();
            else
            {
                _elf.SaveEditableLocalizations();
                Modified = false;
            }
        }

        private void MenuFileSaveAsClick(object sender, EventArgs e)
        {
            MenuFileSaveAsClickEx();
        }

        private bool MenuFileSaveAsClickEx()
        {
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                _elf.Path2Elf = saveFileDialog.FileName;
                MenuFileSaveClick(null, null);
                return true;
            }
            return false;
        }

        private void MenuImportMergeFromResxClick(object sender, EventArgs e)
        {
            if (!CheckForSaveFirst())
                return;

            openFileDialog.DefaultExt = "resx";
            openFileDialog.FileName = "Resources.resx";
            openFileDialog.Filter = "Resource Files|*.resx";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var strResxAsElf = OpenResxFile(openFileDialog.FileName);
                    var strMergedElf = _elf.MergeXmlFragment(strResxAsElf);
                    var strPath2Elf = _elf.Path2Elf;
                    _elf = EditableLocalization.CreateEditableLocalizationsFromString(strMergedElf);
                    _elf.Path2Elf = strPath2Elf;
                    Modified = true;
                    UpdateGrid(null);
#if DEBUG
                    var str = _elf.ToString();
                    var doc1 = new XmlDocument();
                    doc1.LoadXml(strMergedElf);
                    var doc2 = new XmlDocument();
                    doc2.LoadXml(_elf.ToString());
                    System.Diagnostics.Debug.Assert(XmlUtilities.AreXmlElementsEqual(doc1.OuterXml, doc2.OuterXml));
#endif
                }
                catch (Exception ex)
                {
                    Program.ShowException(ex);
                }
            }
        }

        private static string OpenResxFile(string strResxFile)
        {
            if (String.IsNullOrEmpty(strResxFile) || !File.Exists(strResxFile))
                return null;

            // get the xml (.onestory) file into a memory string so it can be the 
            //  input to the transformer
            var streamResxData = new MemoryStream(Encoding.UTF8.GetBytes(File.ReadAllText(strResxFile)));

            // write the formatted XSLT to another memory stream.
            var streamXslt = new MemoryStream(Encoding.UTF8.GetBytes(Properties.Resources.resx2elf));
            return TransformedXml(streamXslt, streamResxData);
        }

        private static string TransformedXml(Stream streamXslt, Stream streamData)
        {
            var myProcessor = new XslCompiledTransform();
            var xslReader = XmlReader.Create(streamXslt);
            myProcessor.Load(xslReader);

            // rewind
            streamData.Seek(0, SeekOrigin.Begin);

            var reader = XmlReader.Create(streamData);
            var strBuilder = new StringBuilder();
            var settings = new XmlWriterSettings { ConformanceLevel = ConformanceLevel.Fragment };
            var writer = XmlWriter.Create(strBuilder, settings);
            myProcessor.Transform(reader, writer);

            return strBuilder.ToString();
        }

        private void MenuLanguageDropDownOpening(object sender, EventArgs e)
        {
            // add sub-menu items for each language in the project
            if (_elf == null)
                return;

            foreach (var language in _elf.Languages)
            {
                var strMenuText = String.Format("{0} ({1})",
                                                language.Name, language.Id);

                if (IsSomeUntranslatedEntries(language.Id))
                    strMenuText += " [translator needed]";
                menuLanguage.DropDownItems.Add(strMenuText,
                                               null,
                                               onChangeLanguage).Tag = language.Id;
            }
        }

        private void onChangeLanguage(object sender, EventArgs e)
        {
            var tsi = sender as ToolStripItem;
            if (tsi != null) 
                UpdateGrid(tsi.Tag as string);
        }

        private bool IsSomeUntranslatedEntries(string strId)
        {
            return _elf.Entries.Any(entry => !IsValuePresentAndFinished(entry, strId));
        }

        private static bool IsValuePresentAndFinished(Entry entry, string strId)
        {
            return (from value in entry.Values 
                    where value.Lang == strId 
                    select !value.NeedsUpdating).FirstOrDefault();
        }

        private void localizationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var localizationEditor = new LocDataEditorForm(Localizer.Default,
                                                           new [] {"EditResxLocalization"});
            if (localizationEditor.ShowDialog() == DialogResult.OK)
            {
                
            }
        }
    }
}
