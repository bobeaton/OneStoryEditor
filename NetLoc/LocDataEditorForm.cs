using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Xml.Serialization;

namespace NetLoc
{
    public partial class LocDataEditorForm : Form
    {
        Localizer localizer;
        List<LocKey> locKeys;

        public LocDataEditorForm(Localizer localizer, List<LocKey> locKeys)
        {
            InitializeComponent();

            // if we're launched again, then if we had a delegate to call on close
            //  the call it now
            if (DelegateCallOnClose != null)
            {
                DelegateCallOnClose(true);
                DelegateCallOnClose = null;
            }

            this.localizer = localizer;
            this.locKeys = locKeys;
            if (localizer.Icon != null)
                this.Icon = localizer.Icon;

            Bind();
        }

        /// <summary>
        /// Creates a form to edit localization data.
        /// </summary>
        /// <param name="localizer">Localizer to use. Use Localizer.Default for the default localizer</param>
        /// <param name="namespaces">list of namespaces of all classes to localize. Wild-cards ok (e.g. SomeSpace*). None for
        ///   all namespaces but System<param>
        public LocDataEditorForm(Localizer localizer, params string[] namespaces)
        {
            InitializeComponent();

            this.localizer = localizer;

            if (namespaces.Length > 0)
                this.locKeys = Localizer.FindAllLocKeysInNamespaces(namespaces);
            else
                this.locKeys = Localizer.FindAllLocKeys();

            if (localizer.Icon != null)
                this.Icon = localizer.Icon;

            Bind();
        }

        private void uiLanguage_DropDownOpening(object sender, EventArgs e)
        {
            uiLanguage.DropDownItems.Clear();
            uiLanguage.DropDownItems.AddRange(Localizer.Default.CreateLanguagesSwitchMenu(
            delegate()
            {
                uiLocDataControl.Unbind();
                localizer.Save();
            },
            delegate()
            {
                Bind();
            }));
        }

        void Bind()
        {
            SetDisplayFont(localizer.LocLanguage.Font);

            // Filter loc keys
            string lowerSearch = uiSearch.Text.ToLowerInvariant();
            List<LocKey> filtered = new List<LocKey>();
            if (lowerSearch == "" || uiSearch.ForeColor == Color.DarkGray)
                filtered = locKeys;
            else
            {
                foreach (LocKey key in locKeys)
                {
                    if (key.DefaultValue.ToLowerInvariant().Contains(lowerSearch)
                        || key.Path.ToLowerInvariant().Contains(lowerSearch)
                        || localizer[key].ToLowerInvariant().Contains(lowerSearch))
                        filtered.Add(key);
                }
            }

            uiLocDataControl.Bind(localizer.LocLanguage, filtered, localizer.LanguageId);
            this.Text = string.Format("Localizer ({0})", localizer.LanguageId);
        }

        private void uiFileSave_Click(object sender, EventArgs e)
        {
            localizer.Save();
        }

        private void uiFileClose_Click(object sender, EventArgs e)
        {
            _bWasClosedViaFileClose = true;
            this.Close();
        }

        private void uiFileAddLanguage_Click(object sender, EventArgs e)
        {
            AddLanguageForm form = new AddLanguageForm();
            form.Icon = this.Icon;
            if (form.ShowDialog() == DialogResult.OK)
                localizer.CreateLanguage(form.LanguageId, form.LanguageName);
            form.Dispose();
        }

        /*
        // if the caller wants to allow this dialog to remain active, he can provide a 
        //  function to call when this dialog is closing and do something like this:
        private void OnCloseLocalizationDialog(bool bWasViaFileClose)
        {
            // if the user does "File", "Close", then totally Dispose if it
            //  otherwise, just hide it (so it doesn't take so long to come up
            //  next time
            if (bWasViaFileClose)
                _localizationEditor = null;
            else
                _localizationEditor.Hide();
        }
        // then if the upper-right 'x' box is clicked, it will just hide the window
        // and if the user does "File", "Close", then it will close completely.
        */
        public delegate void CallOnFileClose(bool bWasViaFileClose);
        public static CallOnFileClose DelegateCallOnClose;
        private bool _bWasClosedViaFileClose;

        private void LocDataEditorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // if the user clicks the X in the upper right, just hide this form
            //  whereas if they use 'File', 'Close', then close it for real.
            if (_bWasClosedViaFileClose || (DelegateCallOnClose == null))
                uiLocDataControl.Unbind();
            else
                e.Cancel = true;

            // either way we want to save and update
            localizer.Save();
            localizer.Update();

            if (DelegateCallOnClose != null)
                DelegateCallOnClose(_bWasClosedViaFileClose);
        }

        private void uiFileDeleteLanguage_DropDownOpening(object sender, EventArgs e)
        {
            uiFileDeleteLanguage.DropDownItems.Clear();
            uiFileDeleteLanguage.DropDownItems.AddRange(Localizer.Default.CreateLanguagesSwitchMenu(
            delegate()
            {
                uiLocDataControl.Unbind();
                localizer.Save();
            },
            delegate()
            {
                string langId = localizer.LanguageId;
                localizer.LanguageId = "";
                localizer.DeleteLanguage(langId);
                Bind();
            }));
        }

        private void uiFileImport_Click(object sender, EventArgs e)
        {
            uiLocDataControl.Unbind();
            localizer.Save();
            string oldLangId = localizer.LanguageId;
            localizer.LanguageId = "";

            if (uiOpenXmlFileDialog.ShowDialog(this) == DialogResult.OK)
                localizer.MergeLocData(uiOpenXmlFileDialog.FileName);
            uiOpenXmlFileDialog.Dispose();

            localizer.LanguageId = oldLangId;
            Bind();
        }

        private void uiFileExport_Click(object sender, EventArgs e)
        {
            uiLocDataControl.Unbind();
            if (uiSaveXmlFileDialog.ShowDialog(this) == DialogResult.OK)
                localizer.LocLanguage.Save(uiSaveXmlFileDialog.FileName);
            Bind();
        }

        private void uiFileCopyLanguage_Click(object sender, EventArgs e)
        {
            AddLanguageForm form = new AddLanguageForm();
            form.Icon = this.Icon;
            form.Text = "Copy Language";
            if (form.ShowDialog() == DialogResult.OK)
                localizer.CopyLanguage(form.LanguageId, form.LanguageName);

            form.Dispose();
        }

        private void lMXFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (uiOpenLmxFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                // Open file as text lines
                string[] lines = File.ReadAllLines(uiOpenLmxFileDialog.FileName);

                // Skip except [Data] section
                int first = 0;
                while (first < lines.Length)
                {
                    if (lines[first].Trim() == "[Data]")
                        break;
                    first++;
                }
                first++;
                if (first >= lines.Length)
                {
                    MessageBox.Show("Missing [Data] line");
                    return;
                }

                int last = first + 1;
                while (last < lines.Length)
                {
                    if (lines[last].Trim().StartsWith("["))
                        break;
                    last++;
                }
                last--;

                List<string> from = new List<string>(), to = new List<string>();
                string fromLang = null, toLang = null;
                bool readFrom = true;
                for (int i = first; i <= last; i++)
                {
                    // Skip blank and context lines
                    if (lines[i].Trim() == "" || lines[i].Trim().StartsWith("Context"))
                        continue;

                    string lang = lines[i].Substring(0, lines[i].IndexOf('='));
                    string data = lines[i].Substring(lines[i].IndexOf('=') + 1).Trim();
                    if (data.StartsWith("\"") && data.EndsWith("\""))
                        data = data.Substring(1, data.Length - 2);

                    if (readFrom)
                    {
                        if (fromLang != null && fromLang != lang)
                        {
                            MessageBox.Show("Wrong from language " + lang);
                            return;
                        }
                        fromLang = lang;
                        from.Add(data);
                    }
                    else
                    {
                        if (toLang != null && toLang != lang)
                        {
                            MessageBox.Show("Wrong to language " + lang);
                            return;
                        }
                        toLang = lang;
                        to.Add(data);
                    }
                    readFrom = !readFrom;
                }

                if (from.Count != to.Count)
                {
                    MessageBox.Show("Invalid format");
                    return;
                }

                // For each key
                int cnt = 0;
                foreach (LocKey key in locKeys)
                {
                    // Get value (don't override existing ones)
                    string val = localizer.LocLanguage[key.Path];
                    if (val != null)
                        continue;

                    // For each translation
                    string bestVal = null;  // For closer matches
                    for (int i = 0; i < from.Count; i++)
                    {
                        // Skip empty translations
                        if (to[i].Trim().Length == 0 || from[i].Trim().Length == 0)
                            continue;

                        // If matches closely
                        if (from[i].Trim(' ', '\n', '\r').Replace("&", "").ToLowerInvariant() ==
                            key.DefaultValue.Trim(' ').Replace("&", "").ToLowerInvariant())
                        {
                            bestVal = to[i];
                        }
                        // If matches approx
                        else if (from[i].Trim('.', ' ', '\n', '\r').Replace("&", "").ToLowerInvariant() ==
                             key.DefaultValue.Trim('.', ' ').Replace("&", "").ToLowerInvariant())
                        {
                            val = to[i];
                        }
                    }

                    if (bestVal == null)
                        bestVal = val;

                    if (bestVal != null)
                    {
                        localizer.LocLanguage[key.Path] = bestVal;
                        cnt++;
                    }
                }
                MessageBox.Show(string.Format("Imported {0} strings", cnt));
            }

            uiOpenLmxFileDialog.Dispose();
            Bind();
        }

        private void uiSearch_Enter(object sender, EventArgs e)
        {
            // Remove search text
            if (uiSearch.ForeColor == Color.DarkGray)
            {
                uiSearch.ForeColor = Color.Black;
                uiSearch.Text = "";
            }
        }

        private void uiSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                e.Handled = true;
                uiLocDataControl.Focus();
            }
        }

        private void uiSearch_Leave(object sender, EventArgs e)
        {
            // Perform search
            Bind();

            if (uiSearch.Text == "")
            {
                uiSearch.ForeColor = Color.DarkGray;
                uiSearch.Text = "Search <Enter>";
            }
        }

        private void uiAutoTranslate_DropDownOpening(object sender, EventArgs e)
        {
            uiAutoTranslate.DropDownItems.Clear();
            uiAutoTranslate.DropDownItems.AddRange(Localizer.Default.CreateLanguagesMenu("",
                AutoTranslate));
        }

        void AutoTranslate(string langId)
        {
            try
            {
                // Make progress visible
                uiProgress.Visible = true;
                this.Cursor = Cursors.WaitCursor;
                this.Refresh();

                uiLocDataControl.Unbind();

                // Create localizer for other language
                Localizer fromLoc = new Localizer(localizer.LocDataPath, langId);

                // For each non-translated entry
                uiProgress.Value = 0;
                for (int i = 0; i < locKeys.Count; i++)
                {
                    // If not translated 
                    if (localizer.LocLanguage[locKeys[i].Path] == null)
                    {
                        try
                        {
                            string fromStr = fromLoc[locKeys[i]];
                            fromStr = fromStr.Replace("&", "");         // Remove accelerators
                            string trans = AutoTranslator.TranslateText(langId, localizer.LanguageId, fromStr);
                            localizer.LocLanguage[locKeys[i].Path] = trans;
                        }
                        catch (WebException wex)
                        {
                            if (wex.Status == WebExceptionStatus.ProtocolError)
                            {
                                if ((wex.Response as HttpWebResponse).StatusCode == HttpStatusCode.ServiceUnavailable)
                                {
                                    MessageBox.Show("Limit of translations at one time has been reached. Try again shortly to continue.", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    break;
                                }
                            }
                            MessageBox.Show("Translation failed to complete. Try again later.\r\n\r\n" + wex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            break;
                        }
                        catch (System.Web.Services.Protocols.SoapHeaderException)
                        {
                            // Just continue with next one
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Translation failed to complete. Try again later.\r\n\r\n" + ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                        }
                    }
                    uiProgress.Value = (1000 * i) / locKeys.Count;
                }
            }
            finally
            {
                uiProgress.Visible = false;
                this.Cursor = Cursors.Default;

                Bind();
            }
        }

        /// <summary>
        /// Export the list of all current visible localization keys
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportKeyListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            XmlSerializer ser = new XmlSerializer(typeof(List<LocKey>));

            if (uiSaveXmlFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                using (TextWriter w = new StreamWriter(uiSaveXmlFileDialog.FileName, false, Encoding.UTF8))
                {
                    ser.Serialize(w, locKeys);
                }
            }
        }

        /// <summary>
        /// Loads a list of keys and then filters the currently displayed list
        /// to only include those keys who existed in the previous list AND
        /// have changed the default value. Use this to look for places where
        /// translations need to be updated.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void compareToKeyListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            XmlSerializer ser = new XmlSerializer(typeof(List<LocKey>));

            if (uiOpenXmlFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                using (TextReader r = new StreamReader(uiOpenXmlFileDialog.FileName, Encoding.UTF8))
                {
                    List<LocKey> compareKeys = (List<LocKey>)ser.Deserialize(r);

                    uiLocDataControl.Unbind();

                    // Only keep keys that have changed default values
                    Dictionary<string, string> compareKeysMap = new Dictionary<string, string>();
                    foreach (LocKey key in compareKeys)
                        compareKeysMap[key.Path] = key.DefaultValue;

                    List<LocKey> newKeys = new List<LocKey>();
                    foreach (LocKey key in locKeys)
                    {
                        if (compareKeysMap.ContainsKey(key.Path) && compareKeysMap[key.Path] != key.DefaultValue)
                            newKeys.Add(key);
                    }
                    locKeys = newKeys;
                    Bind();
                }
            }
        }

        private void removeUnusedTranslationsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, "This will remove ALL translations for the current language that are not in the current list. If you don't have all localizations loaded, you will lose data. Continue?",
                "Remove Unused Translations", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                uiLocDataControl.Unbind();

                // Purge
                Dictionary<string, LocKey> keyMap = new Dictionary<string, LocKey>();
                foreach (LocKey key in locKeys)
                    keyMap[key.Path] = key;
                List<string> pathsToRemove = new List<string>();
                foreach (LocLanguageEntry entry in localizer.LocLanguage.Entries)
                {
                    if (!keyMap.ContainsKey(entry.Path))
                        pathsToRemove.Add(entry.Path);
                }
                foreach (string path in pathsToRemove)
                {
                    localizer.LocLanguage[path] = null;
                }

                MessageBox.Show(string.Format("Removed {0} translations", pathsToRemove.Count));
                Bind();
            }
        }

        private void uiAdvanced_DropDownOpening(object sender, EventArgs e)
        {
            useGoogleAutoTranslatorToolStripMenuItem.Checked = AutoTranslator.Engine == AutoTranslator.Engines.Google;
            useMicrosoftAutoTranslatorToolStripMenuItem.Checked = AutoTranslator.Engine == AutoTranslator.Engines.Microsoft;
        }

        private void useGoogleAutoTranslatorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AutoTranslator.Engine = AutoTranslator.Engines.Google;
        }

        private void useMicrosoftAutoTranslatorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AutoTranslator.Engine = AutoTranslator.Engines.Microsoft;
        }

        private void fontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var fontCurrent = localizer.LocLanguage.Font;
            if (fontCurrent != null)
                fontDialog.Font = fontCurrent;
            if (fontDialog.ShowDialog() != DialogResult.OK)
                return;

            localizer.LocLanguage.Font = fontDialog.Font;

            // must be unbind'd before calling setDisplayFont
            uiLocDataControl.Unbind();
            Bind();
        }

        private void SetDisplayFont(Font font)
        {
            if (font != null)
                uiLocDataControl.SetTranslationColumnFont(font);
        }
    }
}