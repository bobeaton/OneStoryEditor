using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.InteropServices;

namespace NetLoc
{
	[XmlRoot("Language")]
	public class LocLanguage
	{
		public string Id;		// e.g. "en"
		public string Name;		// e.g. "English"
	    public string FontName; // e.g. "Arial Unicode MS"
        public float FontSize;  // e.g. 12

		// Map of path to value
		Dictionary<string, string> entryMap = new Dictionary<string, string>();

        static XmlSerializer ser;

        static LocLanguage()
        {
            // Safely creates an XmlSerializer. 
            // See https://connect.microsoft.com/VisualStudio/feedback/ViewFeedback.aspx?FeedbackID=422414&wa=wsignin1.0
            try
            {
                ser = new XmlSerializer(typeof(LocLanguage));
            }
            catch (ExternalException)
            {
                Environment.CurrentDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                ser = new XmlSerializer(typeof(LocLanguage));
            }
        }

		public void Save(string path)
		{
			TextWriter w = new StreamWriter(path, false, Encoding.UTF8);
			ser.Serialize(w, this);
			w.Close();
		}

		public static LocLanguage Load(string path)
		{
            using (TextReader r = new StreamReader(path, Encoding.UTF8))
            {
                LocLanguage data = (LocLanguage)ser.Deserialize(r);
                return data;
            }
		}

		[XmlElement("Entry", typeof(LocLanguageEntry))]
		public LocLanguageEntry[] Entries
		{
			get
			{
				List<LocLanguageEntry> entries = new List<LocLanguageEntry>();
				foreach (KeyValuePair<string, string> entry in entryMap)
					entries.Add(new LocLanguageEntry(entry.Key, entry.Value));

				return entries.ToArray();
			}
			set
			{
				entryMap.Clear();
				if (value != null)
				{
					foreach (LocLanguageEntry entry in value)
						entryMap[entry.Path] = entry.Value;
				}
			}
		}

		[XmlIgnore]
		public string this[string path]
		{
			get
			{
				string val;
				if (entryMap.TryGetValue(path, out val))
					return val;
				return null;
			}
			set
			{
				if (value != null)
					entryMap[path] = value;
				else
					entryMap.Remove(path);
			}
		}

	    private Font _font;

        [XmlIgnore]
        public Font Font
        {
            get
            {
                return _font ?? (_font = (!String.IsNullOrEmpty(FontName) &&
                                          (FontSize != 0))
                                             ? new Font(FontName, FontSize)
                                             : null);
            }
            set
            {
                if (value != null)
                {
                    FontName = value.Name;
                    FontSize = value.Size;
                }
            }
        }

        public void SetFont(Control control)
        {
            var font = Font;
            if (font != null)
                control.Font = font;
        }
    }

	public class LocLanguageEntry
	{
		[XmlAttribute]
		public string Path;

		[XmlText]
		public string Value;

		public LocLanguageEntry()
		{
		}

		public LocLanguageEntry(string path, string value)
		{
			this.Path = path;
			this.Value = value;
		}
	}
}
