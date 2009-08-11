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
                    new XElement(StoryEditor.ns + strLangType,
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
            ProjectFolder = String.Format(@"{0}\{1}\{2}",
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                StoryEditor.CstrCaption,
                ProjectName);
        }

        public ProjectSettings(StoryProject projFile, string strProjectName)
        {
            ProjectName = strProjectName;
            System.Diagnostics.Debug.Assert(projFile != null);
            ProjectFolder = String.Format(@"{0}\{1}\{2}",
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                StoryEditor.CstrCaption,
                ProjectName);

            StoryProject.LanguagesRow theLangRow = InsureLanguagesRow(projFile);

            if (projFile.VernacularLang.Count == 0)
                projFile.VernacularLang.AddVernacularLangRow(Vernacular.LangName,
                    Vernacular.LangCode, Vernacular.Font.Name, Vernacular.Font.Size,
                    Vernacular.FontColor.Name, Vernacular.FullStop, theLangRow);
            else
            {
                System.Diagnostics.Debug.Assert(projFile.VernacularLang.Count == 1);
                StoryProject.VernacularLangRow theVernRow = projFile.VernacularLang[0];
                Vernacular.LangName = theVernRow.name;
                Vernacular.LangCode = theVernRow.code;
                Vernacular.Font = new Font(theVernRow.FontName, theVernRow.FontSize);
                Vernacular.FontColor = Color.FromName(theVernRow.FontColor);
                Vernacular.FullStop = theVernRow.SentenceFinalPunct;

            }

            if (projFile.NationalBTLang.Count == 0)
                projFile.NationalBTLang.AddNationalBTLangRow(NationalBT.LangName,
                    NationalBT.LangCode, NationalBT.Font.Name, NationalBT.Font.Size,
                    NationalBT.FontColor.Name, NationalBT.FullStop, theLangRow);
            else
            {
                System.Diagnostics.Debug.Assert(projFile.NationalBTLang.Count == 1);
                StoryProject.NationalBTLangRow rowNatlRow = projFile.NationalBTLang[0];
                NationalBT.LangName = rowNatlRow.name;
                NationalBT.LangCode = rowNatlRow.code;
                NationalBT.Font = new Font(rowNatlRow.FontName, rowNatlRow.FontSize);
                NationalBT.FontColor = Color.FromName(rowNatlRow.FontColor);
                NationalBT.FullStop = rowNatlRow.SentenceFinalPunct;
            }

            if (projFile.InternationalBTLang.Count == 0)
                projFile.InternationalBTLang.AddInternationalBTLangRow(
                    InternationalBT.LangName, InternationalBT.LangCode,
                    InternationalBT.Font.Name, InternationalBT.Font.Size,
                    InternationalBT.FontColor.Name, InternationalBT.FullStop, 
                    theLangRow);
            else
            {
                System.Diagnostics.Debug.Assert(projFile.InternationalBTLang.Count == 1);
                StoryProject.InternationalBTLangRow rowEngRow = projFile.InternationalBTLang[0];
                InternationalBT.LangName = rowEngRow.name;
                InternationalBT.LangCode = rowEngRow.code;
                InternationalBT.Font = new Font(rowEngRow.FontName, rowEngRow.FontSize);
                InternationalBT.FontColor = Color.FromName(rowEngRow.FontColor);
                InternationalBT.FullStop = rowEngRow.SentenceFinalPunct;
            }
        }

        public XElement GetXml
        {
            get
            {
                return new XElement(StoryEditor.ns + "Languages",
                    Vernacular.GetXml("VernacularLang"),
                    NationalBT.GetXml("NationalBTLang"),
                    InternationalBT.GetXml("InternationalBTLang"));
            }
        }

        protected StoryProject.LanguagesRow InsureLanguagesRow(StoryProject projFile)
        {
            System.Diagnostics.Debug.Assert(projFile.stories.Count == 1);
            if (projFile.Languages.Count == 0)
                return projFile.Languages.AddLanguagesRow(projFile.stories[0]);
            else
            {
                System.Diagnostics.Debug.Assert(projFile.Languages.Count == 1);
                return projFile.Languages[0];
            }
        }
    }
}
