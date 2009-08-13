using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml.Linq;
using System.Text;

namespace OneStoryProjectEditor
{
    public class ProjectSettings
    {
        public string ProjectName = null;
        public string ProjectFolder = null;

        public class LanguageInfo
        {
            public string LangName = null;
            public string LangCode = null;
            public Font Font;
            public Color FontColor;
            public string FullStop;

            public LanguageInfo(Font font, Color fontColor)
            {
                FullStop = ".";
                Font = font;
                FontColor = fontColor;
            }

            public XElement GetXml(string strLangType)
            {
                XElement elemLang =
                    new XElement(strLangType,
                        new XAttribute("name", LangName),
                        new XAttribute("code", LangCode),
                        new XAttribute("FontName", Font.Name),
                        new XAttribute("FontSize", Font.Size),
                        new XAttribute("FontColor", FontColor.Name));

                if (!String.IsNullOrEmpty(FullStop))
                    elemLang.Add(new XAttribute("SentenceFinalPunct", FullStop));

                return elemLang;
            }
        }

        public LanguageInfo Vernacular = new LanguageInfo(new Font("Arial Unicode MS", 12), Color.Maroon);
        public LanguageInfo NationalBT = new LanguageInfo(new Font("Arial Unicode MS", 12), Color.Green);
        public LanguageInfo InternationalBT = new LanguageInfo(new Font("Times New Roman", 10), Color.Blue);

        public ProjectSettings(string strProjectName)
        {
            ProjectName = strProjectName;
            ProjectFolder = String.Format(@"{0}\OneStory\{1}",
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                ProjectName);
        }

        public XElement GetXml
        {
            get
            {
                return new XElement("Languages",
                    Vernacular.GetXml("VernacularLang"),
                    NationalBT.GetXml("NationalBTLang"),
                    InternationalBT.GetXml("InternationalBTLang"));
            }
        }
    }
}
