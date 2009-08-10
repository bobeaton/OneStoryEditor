using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml.Linq;
using System.Text;

namespace OneStoryProjectEditor
{
    public class ProjectSettings
    {
        internal const string CstrDefaultNationalBTText = "<Enter national language back translation>";

        public string ProjectName = null;
        public string ProjectFolder = null;

        public string VernacularLangName = "<Enter the story in its own language>";
        public string VernacularLangCode = "vern";
        public Font VernacularFont = new Font("Arial Unicode MS", 12);
        public Color VernacularFontColor = Color.Maroon;

        public string NationalBTLangName = CstrDefaultNationalBTText;
        public string NationalBTLangCode = "nat";
        public Font NationalBTFont = new Font("Arial Unicode MS", 12);
        public Color NationalBTFontColor = Color.Green;

        public string InternationalBTLangName = "<Enter the English back translation>";
        public string InternationalBTLangCode = "en";
        public Font InternationalBTFont = new Font("Times New Roman", 10);
        public Color InternationalBTFontColor = Color.Blue;

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
                projFile.VernacularLang.AddVernacularLangRow(VernacularLangName,
                    VernacularLangCode, VernacularFont.Name, VernacularFont.Size,
                    VernacularFontColor.Name, theLangRow);
            else
            {
                System.Diagnostics.Debug.Assert(projFile.VernacularLang.Count == 1);
                StoryProject.VernacularLangRow theVernRow = projFile.VernacularLang[0];
                VernacularLangName = theVernRow.name;
                VernacularLangCode = theVernRow.code;
                VernacularFont = new Font(theVernRow.FontName, theVernRow.FontSize);
                VernacularFontColor = Color.FromName(theVernRow.FontColor);
            }

            if (projFile.NationalBTLang.Count == 0)
                projFile.NationalBTLang.AddNationalBTLangRow(NationalBTLangName,
                    NationalBTLangCode, NationalBTFont.Name, NationalBTFont.Size,
                    NationalBTFontColor.Name, theLangRow);
            else
            {
                System.Diagnostics.Debug.Assert(projFile.NationalBTLang.Count == 1);
                StoryProject.NationalBTLangRow rowNatlRow = projFile.NationalBTLang[0];
                NationalBTLangName = rowNatlRow.name;
                NationalBTLangCode = rowNatlRow.code;
                NationalBTFont = new Font(rowNatlRow.FontName, rowNatlRow.FontSize);
                NationalBTFontColor = Color.FromName(rowNatlRow.FontColor);
            }

            if (projFile.InternationalBTLang.Count == 0)
                projFile.InternationalBTLang.AddInternationalBTLangRow(InternationalBTLangName,
                    InternationalBTLangCode, InternationalBTFont.Name, InternationalBTFont.Size,
                    InternationalBTFontColor.Name, theLangRow);
            else
            {
                System.Diagnostics.Debug.Assert(projFile.InternationalBTLang.Count == 1);
                StoryProject.InternationalBTLangRow rowEngRow = projFile.InternationalBTLang[0];
                InternationalBTLangName = rowEngRow.name;
                InternationalBTLangCode = rowEngRow.code;
                InternationalBTFont = new Font(rowEngRow.FontName, rowEngRow.FontSize);
                InternationalBTFontColor = Color.FromName(rowEngRow.FontColor);
            }
        }

        public XElement GetXml
        {
            get
            {
                return new XElement(StoryEditor.ns + "Languages",
                    new XElement(StoryEditor.ns + "VernacularLang",
                        new XAttribute("name", VernacularLangName),
                        new XAttribute("code", VernacularLangCode),
                        new XAttribute("FontName", VernacularFont.Name),
                        new XAttribute("FontSize", VernacularFont.Size),
                        new XAttribute("FontColor", VernacularFontColor.Name)),
                    new XElement(StoryEditor.ns + "NationalBTLang",
                        new XAttribute("name", NationalBTLangName),
                        new XAttribute("code", NationalBTLangCode),
                        new XAttribute("FontName", NationalBTFont.Name),
                        new XAttribute("FontSize", NationalBTFont.Size),
                        new XAttribute("FontColor", NationalBTFontColor.Name)),
                    new XElement(StoryEditor.ns + "InternationalBTLang",
                        new XAttribute("name", InternationalBTLangName),
                        new XAttribute("code", InternationalBTLangCode),
                        new XAttribute("FontName", InternationalBTFont.Name),
                        new XAttribute("FontSize", InternationalBTFont.Size),
                        new XAttribute("FontColor", InternationalBTFontColor.Name)));
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
