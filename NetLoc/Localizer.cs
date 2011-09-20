using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Reflection;
using System.Xml.Serialization;
using System.IO;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Specialized;
using System.Data;
using System.Drawing;

namespace NetLoc
{
	/// <summary>
	/// Localizes strings and controls.
	/// This class is a singleton (stored in Default property)
	/// 
	/// To localize a string, one of the following must be done:
	/// 
	/// 1) Be a standard string property in a control within a Form or UserControl. (e.g. Text property, Tooltip)
	/// In order to be localized, Localizer.Ctrl(this) must be called immediately after InitializeComponents() in
	/// all constructors. As well there must be at least one no-args constructor though it may be private.
	/// 
	/// 2) Be a string created using Localizer.Str("some string"). The method call must be a string literal
    /// but can be constructed from multiple string literals added together. e.g. Localizer.Str("some string" + "some other string")
    /// 
    /// 3) Be declared as a static field or method of any class that is 
    /// a) tagged with the [Localize] attribute and 
    /// b) returns either a LocKey or List&lt;LocKey&gt;
    /// 
    /// To localize the string, call Localizer.Default[lockey] where lockey is the localization key of type LocKey.
    /// 
    /// 4) Be an enum that is tagged with the [Localize] attribute
    /// 
    /// To localize the enum, call Localizer.Str(enumvalue) where enumvalue is the enumeration member to localize.
    /// 
    /// To setup localization:
    /// 
    /// Localizer.Default = new NetLoc.Localizer(writableLocFolder, languageId);
    /// 
    /// writableLocFolder is the folder containing the localization files. Writable if you want users to be able to edit it.
    /// 
    /// languageId is "en", "es", etc.
    ///
	/// </summary>
	public class Localizer
	{
		string languageId;
		string locDataPath;
		LocLanguage locLanguage;

		bool suspendLocalization = false;		// Internal flag that when true always returns default value

		public bool EnableLocalizeHotkey = true;	// True to have forms capture Shift+Ctrl+Alt+L

		static List<LocKey> capturedKeys = new List<LocKey>();	// Keys that are captured when Ctrl is called
		static Type captureType;								// Type of control to capture for. Null for none

        public static readonly PropertyInfo ControlTextProp = typeof(Control).GetProperty("Text");
        public static readonly PropertyInfo ToolStripItemTextProp = typeof(ToolStripItem).GetProperty("Text");
        public static readonly PropertyInfo ToolStripItemToolTipTextProp = typeof(ToolStripItem).GetProperty("ToolTipText");
		public static readonly PropertyInfo ToolStripMenuItemShortcutKeyDisplayStringProp = typeof(ToolStripMenuItem).GetProperty("ShortcutKeyDisplayString");
		public static readonly PropertyInfo ListBoxItemProp = typeof(ListBox.ObjectCollection).GetProperty("Item");
        public static readonly PropertyInfo ComboBoxItemProp = typeof(ComboBox.ObjectCollection).GetProperty("Item");
        public static readonly PropertyInfo DataGridViewColumnHeaderTextProp = typeof(DataGridViewColumn).GetProperty("HeaderText");
		public Icon Icon;				// Icon to use for forms

		/// <summary>
		/// True to put "#" before and after string
		/// </summary>
		public bool TestMode = false;	

		/// <summary>
		/// Static instance of localizer for static methods
		/// </summary>
		public static Localizer Default = new Localizer(null);

        /// <summary>
        /// Fired when the currently selected language has changed
        /// </summary>
		public event EventHandler LanguageChanged;

        /// <summary>
        /// Fired when Update() has been explicitly called, or 
        /// when the currently selected language has changed
        /// </summary>
        public event EventHandler LocalizationsUpdated;

        /// <summary>
        /// Fired when a control is localized using Localizer.Ctrl
        /// </summary>
        public event EventHandler<ControlLocalizedEventArgs> ControlLocalized;

        List<LocUpdater> locUpdaters = new List<LocUpdater>();

        /// <summary>
        /// ctl/alt F2 multi lingual screen capture will save to a figures sub folder of this path
        /// </summary>
	    public string ScreenCapturePath = null;

		/// <summary>
		/// Localizes the specified string
		/// </summary>
		/// <param name="str">string to localize</param>
		/// <returns>localized value</returns>
        public static string Str(string str)
        {
            if (Default == null)
                return str;

            // Create a stack trace to find calling method	
            StackTrace trace = new StackTrace();
            MethodBase callingMethod = trace.GetFrame(1).GetMethod();

            LocKey key;

            // If anonymous (mangled) names, just use namespace
            if (callingMethod.DeclaringType.Name.Contains("<") ||
                callingMethod.Name.Contains("<"))
                key = new LocKey(string.Format("{0}.\"{1}\"",
                    callingMethod.DeclaringType.Namespace,
                    str), str);
            else
                key = new LocKey(string.Format("{0}.{1}.{2}.\"{3}\"",
                    callingMethod.DeclaringType.Namespace,
                    callingMethod.DeclaringType.Name,
                    callingMethod.Name,
                    str), str);

            return Default[key];
        }

		/// <summary>
		/// Localizes the specified key using the default localizer
		/// </summary>
		/// <param name="key">key to localize</param>
		/// <returns>localized value</returns>
		public static string Str(LocKey key)
		{
			if (Default == null)
				return key.DefaultValue;

			return Default[key];
		}

		public static string Str(Enum enumValue) {
			return Localizer.Str(EnumToLok(enumValue));
		}

		/// <summary>
		/// Localizes the specified control and all of its children
		/// </summary>
		/// <param name="ctrl">control (usually Form or UserControl) to localize</param>
		public static void Ctrl(Control ctrl)
		{
			if (Default == null)
				return;

            // Get locs (loc updaters which both give the loc key and set the localization)
            List<LocUpdater> locs = FindControlLoc(ctrl);

			// If capturing, just get keys and throw exception to finish
			if ((captureType != null) && (ctrl.GetType().Equals(captureType)))
			{
                foreach (LocUpdater locUpdater in locs) 
				    capturedKeys.Add(locUpdater.LocKey);

                // Dispose control since we will throw an exception in constructor
                ctrl.Dispose();
                throw new CaptureFinishedException();
			}

			// Listen for special keystroke to launch localizing control from forms
			if ((ctrl is Form) && Default.EnableLocalizeHotkey)
			{
				// This prevents multiple registrations
				if (!(ctrl as Form).Visible)
				{
					(ctrl as Form).KeyPreview = true;
					(ctrl as Form).KeyDown += new KeyEventHandler(Localizer_KeyDown);
				}
			}

            // Update all localizations
            foreach (LocUpdater locUpdater in locs)
            {
                // Save loc updaters
                Default.RegisterLocUpdater(locUpdater);
                locUpdater.Update(Default);
            }

            // Fire event
            if (Default.ControlLocalized != null)
                Default.ControlLocalized(Default, new ControlLocalizedEventArgs() { Control = ctrl });
		}

		static void Localizer_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Alt && e.Control && e.Shift && e.KeyCode == Keys.L)
			{
				Form form = sender as Form;
				if (form == null)
					return;

				// If no language files loaded, cannot edit.
				if (Default.locDataPath == null)
					return;

				// Get lockeys
				List<LocKey> keys = new List<LocKey>();
				FindAllLocKeysInType(keys, form.GetType());

                // Get lockeys of MDI parent
                if (form.MdiParent != null)
                    FindAllLocKeysInType(keys, form.MdiParent.GetType());
				
				// Launch editor form
				LocDataEditorForm editor = new LocDataEditorForm(Default, keys);
				editor.ShowDialog();
				Localizer.Ctrl(form);
				e.Handled = true;
			    return;
			}

            if (e.Alt && e.Control && e.KeyCode == Keys.F2)
            {
                Form form = sender as Form;
                if (form == null)
                    return;

                ScreenCaptureForm dialog = new ScreenCaptureForm(form);
                dialog.Show();

                e.Handled = true;
                return;
            }
        }


	    /// <summary>
		/// Creates a localizer which uses the current country code as language id
		/// </summary>
		/// <param name="locDataPath">path to directory of xml files containing localization data. 
		/// Null to always use default.</param>
		public Localizer(string locDataPath)
			: this(locDataPath, null)
		{
		}

		/// <summary>
		/// Creates a localizer
		/// </summary>
		/// <param name="locDataPath">path to directory of xml files containing localization data. 
		/// Null to always use default.</param>
		/// <param name="languageId">language id to localize to (e.g. en, fr, etc.)</param>
		public Localizer(string locDataPath, string languageId)
		{
			this.locDataPath = locDataPath;

			if ((locDataPath != null) && (!Directory.Exists(locDataPath)))
			{
				// Create directory
				Directory.CreateDirectory(locDataPath);

				// Upgrade from XML file
				if (File.Exists(locDataPath + ".xml"))
				{
					LocData locData = new LocData();
					locData.ReadXml(locDataPath + ".xml");
					foreach (LocData.LanguagesRow langRow in locData.Languages)
					{
						LocLanguage lang = new LocLanguage();
						lang.Id = langRow.Id;
						lang.Name = langRow.Name;
						foreach (LocData.StringsRow strRow in
							langRow.GetChildRows("FK_Languages_Strings"))
						{
							lang[strRow.Path] = strRow.Value;
						}
						lang.Save(GetLocLanguagePath(lang.Id));
					}
				}
			}
	
			// Set language id if not set
			if (languageId != null)
				LanguageId = languageId;
			else
				LanguageId = CultureInfo.CurrentCulture.Name.Substring(0, 2);

            //Register manager for the MessageBox.Show localization
            MessageBoxManager.Unregister();
            MessageBoxManager.Register();
        }

		private string GetLocLanguagePath(string langId)
		{
			return Path.Combine(locDataPath, langId + ".xml");
		}

        /// <summary>
        /// Gets the loc data path specified in the constructor
        /// </summary>
        public string LocDataPath
        {
            get { return locDataPath; }
        }

		/// <summary>
		/// Gets or sets the language id of the localizer. When set, 
        /// updates localizations. Warning: save changes before 
        /// switching languages, or changes will be lost!
		/// </summary>
		public string LanguageId
		{
			get { return languageId; }
			set
			{
				languageId = value;
				if (locDataPath == null)
				{
					locLanguage = null;
					return;
				}

				// If language found, use it, otherwise default to English
				if (File.Exists(GetLocLanguagePath(languageId)))
					locLanguage = LocLanguage.Load(GetLocLanguagePath(languageId));
				else
				{
					if (!File.Exists(GetLocLanguagePath("en")))
						CreateLanguage("en", "English");
					locLanguage = LocLanguage.Load(GetLocLanguagePath("en"));
                    languageId = "en";
				}

				if (LanguageChanged != null)
					LanguageChanged(this, EventArgs.Empty);

                Update();
            }
		}

		/// <summary>
		/// Deletes a language, but does not change the current language, so it 
		/// must be first changed if the current language is to be deleted
		/// </summary>
		/// <param name="langId">id of language</param>
		public void DeleteLanguage(string langId)
		{
			File.Delete(GetLocLanguagePath(langId));
		}

		/// <summary>
		/// Creates a new language, but does not change the current language
		/// </summary>
		/// <param name="langId">id of language</param>
		/// <param name="langName">name of language</param>
		public void CreateLanguage(string langId, string langName)
		{
			LocLanguage lang = new LocLanguage();
			lang.Id = langId;
			lang.Name = langName;
			lang.Save(GetLocLanguagePath(langId));
		}

		/// <summary>
		/// Gets the localization data currently used by the localizer
		/// </summary>
		public LocLanguage LocLanguage
		{
			get { return locLanguage; }
		}

		/// <summary>
		/// Saves localization data back to the file from which it was loaded
		/// </summary>
		public void Save()
		{
			if (locLanguage!= null)
				locLanguage.Save(GetLocLanguagePath(languageId));
		}

		/// <summary>
		/// Import the specified locData 
		/// </summary>
		/// <param name="path">path of xml file to merge</param>
		public void MergeLocData(string path)
		{
			LocLanguage importLang = LocLanguage.Load(path);

			// Determine if language already exists
			if (!File.Exists(GetLocLanguagePath(importLang.Id))) 
				CreateLanguage(importLang.Id, importLang.Name);

			// Merge language
			LocLanguage destLang = LocLanguage.Load(GetLocLanguagePath(importLang.Id));
			foreach (LocLanguageEntry entry in importLang.Entries)
				destLang[entry.Path] = entry.Value;

			// Save language
			destLang.Save(GetLocLanguagePath(destLang.Id));
		}

		/// <summary>
		/// Makes a copy of the current language
		/// </summary>
		/// <param name="langId"></param>
		/// <param name="langName"></param>
		public void CopyLanguage(string langId, string langName) 
		{
			// Determine if language already exists
			if (!File.Exists(GetLocLanguagePath(langId)))
				CreateLanguage(langId, langName);

			// Merge language
			LocLanguage destLang = LocLanguage.Load(GetLocLanguagePath(langId));
			foreach (LocLanguageEntry entry in locLanguage.Entries)
				destLang[entry.Path] = entry.Value;

			// Save language
			destLang.Save(GetLocLanguagePath(destLang.Id));
		}

		/// <summary>
		/// Localizes a single string from a key
		/// </summary>
		/// <param name="key">key to localize</param>
		/// <returns>localized value</returns>
		public string this[LocKey key]
		{
			get
			{
				if (suspendLocalization)
					return key.DefaultValue;

				if (TestMode)
					return "#" + key.DefaultValue + "#";

				if ((languageId == null) || (locLanguage == null))
					return key.DefaultValue;

				string value = locLanguage[key.Path];
				if (value != null)
					return value;
				else
					return key.DefaultValue;
			}
		}

        /// <summary>
        /// Registers a loc updateer to be called when localizations have changed
        /// </summary>
        /// <param name="locUpdater"></param>
        public void RegisterLocUpdater(LocUpdater locUpdater)
        {
            locUpdaters.Add(locUpdater);
        }

        /// <summary>
        /// Registers a loc updateer to be called when localizations have changed, then
        /// updates it immediately
        /// </summary>
        public void RegisterAndUpdateLocUpdater(LocUpdater locUpdater)
        {
            locUpdaters.Add(locUpdater);
            locUpdater.Update(this);
        }

        /// <summary>
        /// Updates all localizations and fires a localization updated event
        /// </summary>
        public void Update()
        {
            var newLocUpdaters = new List<LocUpdater>(locUpdaters.Count);
            newLocUpdaters.AddRange(locUpdaters.Where(t => t.Update(this)));

            locUpdaters = newLocUpdaters;

            if (LocalizationsUpdated != null)
                LocalizationsUpdated(this, EventArgs.Empty);
        }

		public IEnumerable<LocLanguage> Languages
		{
			get
			{
				if (locDataPath == null)
					yield break;

				foreach (string locLangFile in Directory.GetFiles(locDataPath, "*.xml"))
				{
					LocLanguage lang = LocLanguage.Load(locLangFile);
					yield return lang;
				}
			}
		}

		/// <summary>
		/// Creates a menu which contains languages available. When a language is
		/// selected, the localizer is switched to that language, and then 
		/// the action is executed.
		/// </summary>
        /// <param name="before">action to execute before switching languages. Can be null</param>
        /// <param name="after">action to execute after switching languages. Can be null</param>
		/// <returns>array of menu items, one for each language available</returns>
		public ToolStripItem[] CreateLanguagesSwitchMenu(MethodInvoker before, MethodInvoker after)
		{
			if (locDataPath == null)
				return new ToolStripItem[0];

			List<ToolStripItem> items = new List<ToolStripItem>();
			foreach (string locLangFile in Directory.GetFiles(locDataPath, "*.xml")) 
			{
				LocLanguage lang = LocLanguage.Load(locLangFile);

				ToolStripMenuItem item = new ToolStripMenuItem(lang.Name);
				
				item.Checked = (lang.Id.Equals(languageId));
				string langId = lang.Id;
				item.Click += new EventHandler(delegate(object sender, EventArgs e)
				{
                    if (before != null)
                        before();

                    LanguageId = langId;

					if (after != null)
						after();
				});
				items.Add(item);
			}
			return items.ToArray();
		}

		public delegate void LanguageMenuSelectedDelegate(string languageId);

		/// <summary>
		/// Creates a menu which contains languages available. When a language is
		/// selected, the action is executed with the selected language as parameter
		/// </summary>
		/// <param name="checkedLanguageId">language that should be checked</param>
		/// <param name="action">action to execute after switching languages</param>
		/// <returns>array of menu items, one for each language available</returns>
		public ToolStripItem[] CreateLanguagesMenu(string checkedLanguageId, LanguageMenuSelectedDelegate action)
		{
			if (locDataPath == null)
				return new ToolStripItem[0];

			List<ToolStripItem> items = new List<ToolStripItem>();
			foreach (string locLangFile in Directory.GetFiles(locDataPath, "*.xml"))
			{
				LocLanguage lang = LocLanguage.Load(locLangFile);

				ToolStripMenuItem item = new ToolStripMenuItem(lang.Name);

				item.Checked = (lang.Id.Equals(checkedLanguageId));
				string langId = lang.Id;
				item.Click += new EventHandler(delegate(object sender, EventArgs e)
				{
					action(langId);
				});
				items.Add(item);
			}
			return items.ToArray();
		}

		#region Component Localization Functions

        /// <summary>
        /// Find localization updaters for the specified control
        /// </summary>
        /// <param name="ctrl">control to be localized</param>
        /// <returns>localization updaters for the control</returns>
        static List<LocUpdater> FindControlLoc(Control ctrl)
        {
            return FindComponentLoc(ctrl,
                string.Format("{0}.{1}.", ctrl.GetType().Namespace, ctrl.Name));
        }

        /// <summary>
        /// Find localization updaters for the specified component (and sub-components)
        /// </summary>
        /// <param name="comp">component to be localized</param>
        /// <param name="keyPrefix">string prefix to all localization key paths</param>
        /// <returns>localization updaters for the component</returns>
        static List<LocUpdater> FindComponentLoc(IComponent comp, string keyPrefix)
        {
            // Find all subcomponents
            List<IComponent> subcomps = FindSubComponents(comp);

            // Find loc updaters of each component
            List<LocUpdater> locs = new List<LocUpdater>();
            foreach (IComponent subcomp in subcomps)
                FindSingleComponentLoc(locs, subcomps, subcomp, keyPrefix);

            return locs;
        }

        /// <summary>
        /// Find localization updaters for the specified component (and sub-components)
        /// </summary>
        /// <param name="locs">localization updaters for the component</param>
        /// <param name="comps">all components being localized</param>
        /// <param name="comp">specific component to be localized</param>
        /// <param name="keyPrefix">string prefix to all localization key paths</param>
        static void FindSingleComponentLoc(List<LocUpdater> locs, List<IComponent> comps, IComponent comp, string keyPrefix)
        {
            if (comp is Control)
            {
				Control control = comp as Control;
                if (!(comp is MenuStrip) && !(comp is NumericUpDown)
                    && (control.Name.Length > 0) && (control.Text.Length > 0))
                    locs.Add(new PropertyLocUpdater(keyPrefix + control.Name + ".Text",
                                                    control, ControlTextProp));

				// Find properties with Localize attribute
				PropertyInfo[] properties = comp.GetType().GetProperties(
						BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
				foreach (PropertyInfo property in properties)
				{
					// Only string properties supported
					if (property.PropertyType != typeof(String))
						continue;
					if (property.GetCustomAttributes(typeof(LocalizeAttribute), false).Length > 0)
						locs.Add(new PropertyLocUpdater(keyPrefix + control.Name + "." + property.Name, control, property));
				}

                // Find variables of type FileDialog
                FieldInfo[] fields =
                    control.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                foreach (FieldInfo field in fields)
                {
                    // If type is derived from FileDialog
                    object value = field.GetValue(control);
                    if (value is FileDialog)
                    {
                        PropertyInfo property = value.GetType().GetProperty("Title");
                        string curValue = property.GetValue(value, null) as string;
                        if (!string.IsNullOrEmpty(curValue))
                            locs.Add(new PropertyLocUpdater(keyPrefix + field.Name + "." + property.Name, value, property));
                    }
                }
            }
            if (comp is ToolStripItem)
            {
                ToolStripItem item = comp as ToolStripItem;
				if ((item.Name.Length > 0) && (item.Text.Length > 0))
                    locs.Add(new PropertyLocUpdater(keyPrefix + item.Name + ".Text",
                        item, ToolStripItemTextProp));
                if ((item.Name.Length > 0) && !string.IsNullOrEmpty(item.ToolTipText))
                    locs.Add(new PropertyLocUpdater(keyPrefix + item.Name + ".ToolTipText",
                        item, ToolStripItemToolTipTextProp));
				if (comp is ToolStripMenuItem) 
				{
					ToolStripMenuItem mitem = item as ToolStripMenuItem;
					if ((item.Name.Length > 0) && !string.IsNullOrEmpty(mitem.ShortcutKeyDisplayString))
	                    locs.Add(new PropertyLocUpdater(keyPrefix + item.Name + ".ShortcutKeyDisplayString",
							item, ToolStripMenuItemShortcutKeyDisplayStringProp));
				}
            }
			if (comp is ListBox)
			{
				ListBox listBox = comp as ListBox;
				for (int i = 0; i < listBox.Items.Count; i++)
				{
					if (listBox.Items[i] is string)
						locs.Add(new PropertyLocUpdater(
							string.Format("{0}{1}.Items[{2}]", keyPrefix, listBox.Name, i), 
                            listBox.Items, ListBoxItemProp, i));
				}
			}
			if (comp is ComboBox)
			{
				ComboBox comboBox = comp as ComboBox;
				for (int i = 0; i < comboBox.Items.Count; i++)
				{
					if (comboBox.Items[i] is string)
                        locs.Add(new PropertyLocUpdater(
                            string.Format("{0}{1}.Items[{2}]", keyPrefix, comboBox.Name, i),
                            comboBox.Items, ComboBoxItemProp, i));
				}
			}
            if (comp is DataGridViewColumn)
            {
                DataGridViewColumn item = comp as DataGridViewColumn;
                if (!string.IsNullOrEmpty(item.Name) && !string.IsNullOrEmpty(item.HeaderText))
                    locs.Add(new PropertyLocUpdater(keyPrefix + item.Name + ".HeaderText",
                        item, DataGridViewColumnHeaderTextProp));
            }
            if (comp is ToolTip)
            {
                // Get tooltips for all other components of parent
                ToolTip toolTip = (ToolTip) comp;
                foreach (IComponent otherComp in comps)
                {
                    Control otherCtrl = otherComp as Control;
                    if (otherCtrl == null)
                        continue;

                    // Get tooltip
                    string toolTipText = toolTip.GetToolTip(otherCtrl);
                    if (toolTipText != "")
                    {
                        LocKey toolTipKey = new LocKey(keyPrefix + otherCtrl.Name + ".ToolTip", toolTipText);
                        locs.Add(new ActionLocUpdater(toolTip, (obj, localizer) =>
                                                               toolTip.SetToolTip(otherCtrl, localizer[toolTipKey]),
                                                      toolTipKey));
                    }
                }
            }
        }

        /// <summary>
        /// Finds the subcomponents of the specified component
        /// </summary>
        /// <param name="comp"></param>
        /// <returns>subcomponents including the component itself</returns>
        public static List<IComponent> FindSubComponents(IComponent comp)
        {
            List<IComponent> subcomps = new List<IComponent>();

            // Add component
            subcomps.Add(comp);

            // Add subcomponents
            if (comp is Control)
            {
				foreach (Control ctrl in (comp as Control).Controls)
				{
					// Do not go inside user controls, but still localize public properties
					if (!(ctrl is UserControl))
						subcomps.AddRange(FindSubComponents(ctrl));
					else
						subcomps.Add(ctrl);
				}
            }
            if (comp is ToolStrip)
            {
                foreach (ToolStripItem item in (comp as ToolStrip).Items) 
                    subcomps.AddRange(FindSubComponents(item));
            }
            if (comp is ToolStripDropDownItem)
            {
                foreach (ToolStripItem item in (comp as ToolStripDropDownItem).DropDownItems)
                    subcomps.AddRange(FindSubComponents(item));
            }
            if (comp is DataGridView)
            {
                foreach (DataGridViewColumn col in (comp as DataGridView).Columns)
                    subcomps.AddRange(FindSubComponents(col));
            }

			// Add members of components private field
			FieldInfo componentsField = comp.GetType().GetField("components",
				BindingFlags.Instance | BindingFlags.NonPublic);
			if ((componentsField != null) && componentsField.FieldType.Equals(typeof(IContainer)))
			{
				IContainer components = (IContainer)componentsField.GetValue(comp);
				if (components != null)
				{
					foreach (IComponent subcomponent in components.Components)
					{
						if (subcomponent is IComponent)
							subcomps.AddRange(FindSubComponents(subcomponent as IComponent));
					}
				}
			}

            return subcomps;
		}
		#endregion

		#region LocKey Finding Functions

		/// <summary>
		/// Finds all the LocKeys defined in any references
		/// </summary>
		/// <returns></returns>
		public static List<LocKey> FindAllLocKeys()
		{
			List<LocKey> allKeys = new List<LocKey>();

			// For each assembly
			foreach (Assembly assembly in GetAllAssemblies())
			{
                try
                {
                    foreach (Type type in assembly.GetTypes())
                    {
                        // If not within System namespace
                        if ((type.Namespace != null) && !type.Namespace.StartsWith("System"))
                        {
                            FindAllLocKeysInType(allKeys, type);
                        }
                    }
                }
                catch (FileNotFoundException ex)
                {
                    Debug.Print("Unable to load assembly {0}:{1}", assembly.FullName, ex.Message);
                }
                catch (ReflectionTypeLoadException ex)
                {
                    Debug.Print("Unable to load assembly {0}:{1}", assembly.FullName, ex.Message);
                }
			}
			return allKeys;
		}
		/// <summary>
		/// Finds all the LocKeys defined in any references within certain namespaces
		/// </summary>
		/// <param name="namespaces">list of namespaces to search (* can be at the end of a 
		/// namespace to include wildcard</param>
		/// <returns></returns>
        public static List<LocKey> FindAllLocKeysInNamespaces(params string[] namespaces)
        {
            List<LocKey> allKeys = new List<LocKey>();

            // For each assembly
            foreach (Assembly assembly in GetAllAssemblies())
            {
                try
                {
                    foreach (Type type in assembly.GetTypes())
                    {
                        foreach (string ns in namespaces)
                        {
                            if (!IsMatchingNamespace(type.Namespace, ns))
                                continue;

                            FindAllLocKeysInType(allKeys, type);
                        }
                    }
                }
                catch (ReflectionTypeLoadException ex)
                {
                    Debug.Print("Unable to load assembly {0}:{1}", assembly.FullName, ex.Message);
                }
                catch (TypeLoadException ex)
                {
                    Debug.Print("Unable to load type {0}:{1}", assembly.FullName, ex.Message);
                }
            }
            return allKeys;
        }

		public static void FindAllLocKeysInType(List<LocKey> allKeys, Type type)
		{
			FindStringLocKeys(allKeys, type);
			FindControlTypeLocKeys(allKeys, type);
			FindLocalizedEnums(allKeys, type);
			FindLocalizedStaticMethods(allKeys, type);
			FindLocalizedStaticFields(allKeys, type);
		}

        public static LocKey EnumToLok(Enum enumerationValue)
        {
            string enumIdentifier = enumerationValue.ToString();
            string enumTypeName = enumerationValue.GetType().ToString();
            return new LocKey(enumTypeName + "." + enumIdentifier, enumIdentifier);
        }

        private static void FindLocalizedEnums(List<LocKey> allKeys, Type type)
        {
            if (!type.IsEnum) return;
            if (type.GetCustomAttributes(typeof(LocalizeAttribute), false).Length < 1) return;
            foreach (Enum a in Enum.GetValues(type))
                allKeys.Add(EnumToLok(a));
        }

		/// <summary>
		/// One way to localize is to provide a static method that returns a list of LokKeys
        /// that also has the attribute Localize
		/// </summary>
		/// <param name="allKeys"></param>
		/// <param name="type"></param>
        static void FindLocalizedStaticMethods(List<LocKey> allKeys, Type type)
        {
            MethodInfo[] methods = type.GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (MethodInfo method in methods)
            {
                if (method.GetCustomAttributes(typeof(LocalizeAttribute), false).Length > 0)
                {
                    object obj = method.Invoke(null, null);
                    if (obj is LocKey)
                        allKeys.Add(obj as LocKey);
                    else if (obj is List<LocKey>)
                        allKeys.AddRange(obj as List<LocKey>);
                }
            }
        }

		/// <summary>
		/// Another way to localize is to tag static fields
		/// </summary>
		/// <param name="allKeys"></param>
		/// <param name="type"></param>
        static void FindLocalizedStaticFields(List<LocKey> allKeys, Type type)
        {
            FieldInfo[] fields = type.GetFields(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (FieldInfo field in fields)
            {
                if (field.GetCustomAttributes(typeof(LocalizeAttribute), false).Length > 0)
                {
                    object obj = field.GetValue(null);
                    if (obj is LocKey)
                        allKeys.Add(obj as LocKey);
                    else if (obj is List<LocKey>)
                        allKeys.AddRange(obj as List<LocKey>);
                }
            }
        }

        private static void FindStringLocKeys(List<LocKey> allKeys, Type type)
        {
            foreach (MethodInfo method in type.GetMethods(
                BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                try
                {
                    foreach (string str in ILReader.FindStringCalls(
                        method, typeof(Localizer).GetMethod("Str", new Type[] { typeof(string) })))
                    {
                        // If anonymous (mangled) names, just use namespace
                        if (method.DeclaringType.Name.Contains("<") ||
                            method.Name.Contains("<"))
                            allKeys.Add(new LocKey(string.Format("{0}.\"{1}\"",
                                    method.DeclaringType.Namespace,
                                    str), str));
                        else
                            allKeys.Add(new LocKey(string.Format("{0}.{1}.{2}.\"{3}\"",
                                method.DeclaringType.Namespace,
                                method.DeclaringType.Name,
                                method.Name,
                                str), str));
                    }
                }
                catch (FileNotFoundException)
                {
                    // Caused by assemblies that cannot be loaded at runtime (e.g. nunit). Ignore.
                }
                catch (TypeLoadException)
                {
                    // Caused by assemblies that have odd runtime loading problems (e.g. Chorus). Ignore.
                }
            }

            foreach (ConstructorInfo constr in type.GetConstructors(
                BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                try
                {
                    foreach (string str in ILReader.FindStringCalls(
                        constr, typeof(Localizer).GetMethod("Str", new Type[] { typeof(string) })))
                    {
                        allKeys.Add(new LocKey(string.Format("{0}.{1}.{2}.\"{3}\"",
                            constr.DeclaringType.Namespace,
                            constr.DeclaringType.Name,
                            constr.Name,
                            str), str));
                    }
                }
                catch (FileNotFoundException)
                {
                    // Caused by assemblies that cannot be loaded at runtime (e.g. nunit). Ignore.
                }
                catch (TypeLoadException)
                {
                    // Caused by assemblies that have odd runtime loading problems (e.g. Chorus). Ignore.
                }
            }
        }

		static void FindControlTypeLocKeys(List<LocKey> allKeys, Type type) 
		{
			// If type is control
			if (!type.IsSubclassOf(typeof(Control)))
				return;

			// If type has default constructor
			ConstructorInfo constructor = type.GetConstructor(
				BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance,
				null, Type.EmptyTypes, null);
			if (constructor == null)
				return;

			// Create instance (suspending localization so as not to change default values)
			try
			{
				// Capture keys
				capturedKeys.Clear();
				captureType = type;
				Default.suspendLocalization = true;

				// Fails for generic constructors
                //  rde: shouldn't be checked for abstract types either
				if (!constructor.DeclaringType.ContainsGenericParameters 
					&& !constructor.ContainsGenericParameters 
                    && ((constructor.DeclaringType.Attributes & TypeAttributes.Abstract) != TypeAttributes.Abstract))
				{
                    Control ctrl = null;
                    try
                    {
                        ctrl = (Control)constructor.Invoke(null);
                        Trace.TraceWarning("Missing localization for control {0}", type.FullName);
                    }
                    catch (TargetInvocationException tie)
                    {
                        // Normal occurrence if capture exception
                        if (!(tie.InnerException is CaptureFinishedException))
                            throw;
                    }

					// Add keys
					allKeys.AddRange(capturedKeys);
				}
			}
			finally
			{
				Default.suspendLocalization = false;
				captureType = null;
			}
		}

        /// <summary>
        /// Checks if a namespace matches the specified pattern
        /// </summary>
        /// <param name="ns"></param>
        /// <param name="pattern">pattern with optional wildcards at the end</param>
        /// <returns>truth matches, false otherwise</returns>
		private static bool IsMatchingNamespace(string ns, string pattern)
		{
			if (ns == null)
				return false;
			if (pattern.EndsWith("*"))
				return ns.StartsWith(pattern.Substring(0, pattern.Length - 1));
			else
				return ns == pattern;
		}

        /// <summary>
        /// Gets a list of all assemblies referenced by the entry assembly
        /// </summary>
        /// <returns></returns>
		static List<Assembly> GetAllAssemblies()
		{
			List<Assembly> assemblies = new List<Assembly>();

            // If no entry assembly, just get assemblies loaded in AppDomain
            if (Assembly.GetEntryAssembly() != null)
            {
                foreach (AssemblyName assemblyName in Assembly.GetEntryAssembly().GetReferencedAssemblies())
                {
                    try
                    {
                        Assembly.Load(assemblyName);
                    }
                    catch (FileNotFoundException)
                    {
                        // Ignore assemblies that aren't distributed with the released version
                    }
                }
            }

			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
				assemblies.Add(assembly);

			return assemblies;
		}

		/// <summary>
		/// Finds all defined lockeys within the specified object
		/// To be found, an object must have a non-static property, 
		/// field or method with the Localize attribute
		/// which returns a LocKey or a List of LocKeys
		/// </summary>
		public static List<LocKey> FindLocKeysInObject(object target)
		{
			List<LocKey> allKeys = new List<LocKey>();

			// Find non-static methods with Localize attribute
			MethodInfo[] methods = target.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			foreach (MethodInfo method in methods)
			{
				if (method.GetCustomAttributes(typeof(LocalizeAttribute), false).Length > 0)
				{
					object obj = method.Invoke(target, null);
					if (obj is LocKey)
						allKeys.Add(obj as LocKey);
					else if (obj is List<LocKey>)
						allKeys.AddRange(obj as List<LocKey>);
				}
			}

			// Find non-static fields with Localize attribute
			FieldInfo[] fields = target.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			foreach (FieldInfo field in fields)
			{
				if (field.GetCustomAttributes(typeof(LocalizeAttribute), false).Length > 0)
				{
					object obj = field.GetValue(target);
					if (obj is LocKey)
						allKeys.Add(obj as LocKey);
					else if (obj is List<LocKey>)
						allKeys.AddRange(obj as List<LocKey>);
				}
			}

			// Find properties with Localize attribute
			PropertyInfo[] properties = target.GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			foreach (PropertyInfo property in properties)
			{
				if (property.GetCustomAttributes(typeof(LocalizeAttribute), false).Length > 0)
				{
					object obj = property.GetValue(target, null);
					if (obj is LocKey)
						allKeys.Add(obj as LocKey);
					else if (obj is List<LocKey>)
						allKeys.AddRange(obj as List<LocKey>);
				}
			}

			return allKeys;
		}
		#endregion
	}

    /// <summary>
    /// Thrown when localization keys have been captured in a form's constructor
    /// to prevent further commands from running in the constructor.
    /// Done automatically, do not catch in form constructor. It is normal 
    /// to be thrown.
    /// </summary>
    public class CaptureFinishedException : ApplicationException
    {
    }

    /// <summary>
    /// Arguments for ControlLocalized event
    /// </summary>
    public class ControlLocalizedEventArgs : EventArgs
    {
        public Control Control;
    }

    /// <summary>
    /// Object which is registered with the localizer
    /// which is called to update localizations when they have
    /// changed.
    /// There is one of these for each active ui control.
    /// </summary>
    public interface LocUpdater
    {
        /// <summary>
        /// Updates localization (such as control labels)
        /// </summary>
        /// <param name="localizer">localizer in effect</param>
        /// <returns>true if the localization is updated, false if it is 
        /// no longer applicable and should be removed from the list</returns>
        bool Update(Localizer localizer);

        /// <summary>
        /// Localization key if applicable, or null if not
        /// </summary>
        LocKey LocKey { get; }
    }

    /// <summary>
    /// Localization updater which uses reflection to update
    /// a property of an object. The object is held by a weak reference
    /// which allows the object to be garbage collected normally.
    /// </summary>
    public class PropertyLocUpdater : LocUpdater
    {
        LocKey locKey;
        WeakReference objRef;
        PropertyInfo prop;
        object[] index;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="path">localization key path. Default value will be obtained from the property</param>
        /// <param name="obj">object to localize property of</param>
        /// <param name="prop">property of object to localize</param>
        /// <param name="index">optional index for indexed properties</param>
        public PropertyLocUpdater(string path, object obj, PropertyInfo prop, params object[] index)
        {
            this.index = (index.Length > 0) ? index : null;
            this.locKey = new LocKey(path, (prop.GetValue(obj, this.index) as string) ?? ""); ;
            this.objRef = new WeakReference(obj);
            this.prop = prop;
        }

        #region LocUpdater Members

        public LocKey LocKey
        {
            get { return locKey; }
        }

        public bool Update(Localizer localizer)
        {
            // Dereference object
            object obj = objRef.Target;
            if (obj == null)
                return false;

            // Check for disposed controls
            if ((obj is Control)&& ((obj as Control).IsDisposed))
                return false;

            try
            {
                prop.SetValue(obj, localizer[locKey], index);

                // for ToolStrip Items and controls which aren't Form, set the 
                //  font that was saved in the localization file (if there was
                //  one). This is needed because the default system fonts 
                //  are terrible for languages like Hindi, etc.
                // p.s. not doing 'Form', because that affects embedded controls
                //  as well, and that's not helpful (at least to my app -- rde)
                if ((localizer.LocLanguage != null) && 
                    (localizer.LocLanguage.Font != null))
                {
                    if (obj is ToolStripItem)
                        (obj as ToolStripItem).Font = localizer.LocLanguage.Font;
                    else if ((obj is Control) && !(obj is Form))
                        (obj as Control).Font = localizer.LocLanguage.Font;
                }
            }
            catch (TargetInvocationException tie)
            {
                // Print message, but otherwise ignore as object may
                // simply be in wrong state to update the localization
                // e.g. item may have disappeared
                Debug.Print(tie.InnerException.ToString());
                return true;
            }
            return true;
        }

        #endregion
    }

    public delegate void LocUpdateActionDelegate(object obj, Localizer localizer);

    /// <summary>
    /// Localization updater which performs an action
    /// on the specified object when localization has been
    /// updated. Object is held with a weak reference.
    /// </summary>
    public class ActionLocUpdater : LocUpdater
    {
        LocUpdateActionDelegate action;
        WeakReference objRef;
        LocKey locKey;          // Optional localization key

        public ActionLocUpdater(object obj, LocUpdateActionDelegate action, LocKey locKey)
        {
            this.action = action;
            this.objRef = new WeakReference(obj);
            this.locKey = locKey;
        }

        public ActionLocUpdater(object obj, LocUpdateActionDelegate action) : this(obj, action, null)
        {
        }

        #region LocUpdater Members

        public bool Update(Localizer localizer)
        {
            // Dereference object
            object obj = objRef.Target;
            if (obj == null)
                return false;

            // Check for disposed controls
            if ((obj is Control) && ((obj as Control).IsDisposed))
                return false;

            action(obj, localizer);
            return true;
        }

        public LocKey LocKey
        {
            get { return locKey; }
        }

        #endregion
    }
}
