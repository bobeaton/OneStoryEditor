using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Xsl;
#if DEBUG
using Chorus.merge.xml.generic;
#endif

namespace EditResxLocalization
{
    public partial class FormEditResx : Form
    {
        private EditableLocalization _elf;

        public FormEditResx()
        {
            InitializeComponent();
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
            }
        }

        private bool CheckForSaveFirst()
        {
            if (_elf != null)
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
            _elf.SaveEditableLocalizations();
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
                    _elf = EditableLocalization.CreateEditableLocalizationsFromString(strMergedElf);
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
            var strLanguageName = "English";
            var strLanguageId = "en";
            string strFallbackId = null;
            return TransformedXml(streamXslt, streamResxData,
                strLanguageName, strLanguageId, strFallbackId);
        }

        private static string TransformedXml(Stream streamXslt, Stream streamData,
            string strParam1, string strParam2, string strParam3)
        {
            var myProcessor = new XslCompiledTransform();
            var xslReader = XmlReader.Create(streamXslt);
            myProcessor.Load(xslReader);

            // rewind
            streamData.Seek(0, SeekOrigin.Begin);

            /*
              <xsl:param name="languageName"/>
              <xsl:param name="languageId"/>
              <xsl:param name="fallbackId" />
            */
            var xslArg = new XsltArgumentList();
            xslArg.AddParam("param1", "", strParam1);
            if (!String.IsNullOrEmpty(strParam2))
                xslArg.AddParam("param2", "", strParam2);
            if (!String.IsNullOrEmpty(strParam3))
                xslArg.AddParam("param3", "", strParam3);

            var reader = XmlReader.Create(streamData);
            var strBuilder = new StringBuilder();
            var settings = new XmlWriterSettings { ConformanceLevel = ConformanceLevel.Fragment };
            var writer = XmlWriter.Create(strBuilder, settings);
            myProcessor.Transform(reader, xslArg, writer);

            return strBuilder.ToString();
        }
    }
}
