using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Xml.Linq;
using System.Text;

namespace OneStoryProjectEditor
{
    public class ProjectSettings
    {
        public string ProjectName = null;
        protected string _strProjectFolder = null;

        public LanguageInfo Vernacular = new LanguageInfo(new Font("Arial Unicode MS", 12), Color.Maroon);
        public LanguageInfo NationalBT = new LanguageInfo(new Font("Arial Unicode MS", 12), Color.Green);
        public LanguageInfo InternationalBT = new LanguageInfo("English", "en", new Font("Times New Roman", 10), Color.Blue);

        public ProjectSettings(string strProjectFolderDefaultIfNull, string strProjectName)
        {
            ProjectName = strProjectName;
            if (String.IsNullOrEmpty(strProjectFolderDefaultIfNull))
                _strProjectFolder = String.Format(@"{0}\{1}", OneStoryProjectFolderRoot, ProjectName);
            else
                _strProjectFolder = strProjectFolderDefaultIfNull;
        }

        public void SerializeProjectSettings(StoryProject projFile)
        {
            System.Diagnostics.Debug.Assert((projFile != null) && (projFile.stories[0].ProjectName == ProjectName));

            StoryProject.LanguagesRow theLangRow = InsureLanguagesRow(projFile);

            if (projFile.VernacularLang.Count == 0)
                projFile.VernacularLang.AddVernacularLangRow(Vernacular.LangName,
                    Vernacular.LangCode, Vernacular.LangFont.Name, Vernacular.LangFont.Size,
                    Vernacular.FontColor.Name, Vernacular.FullStop, Vernacular.Keyboard, Vernacular.IsRTL, theLangRow);
            else
            {
                System.Diagnostics.Debug.Assert(projFile.VernacularLang.Count == 1);
                StoryProject.VernacularLangRow theVernRow = projFile.VernacularLang[0];
                Vernacular.LangName = theVernRow.name;
                Vernacular.LangCode = theVernRow.code;
                Vernacular.LangFont = new Font(theVernRow.FontName, theVernRow.FontSize);
                Vernacular.FontColor = Color.FromName(theVernRow.FontColor);
                Vernacular.FullStop = theVernRow.SentenceFinalPunct;
                Vernacular.IsRTL = (!theVernRow.IsRTLNull() && theVernRow.RTL);
                Vernacular.Keyboard = (!theVernRow.IsKeyboardNull() && !String.IsNullOrEmpty(theVernRow.Keyboard))
                    ? theVernRow.Keyboard : null;
            }

            if (projFile.NationalBTLang.Count == 0)
                projFile.NationalBTLang.AddNationalBTLangRow(NationalBT.LangName,
                    NationalBT.LangCode, NationalBT.LangFont.Name, NationalBT.LangFont.Size,
                    NationalBT.FontColor.Name, NationalBT.FullStop, NationalBT.Keyboard, NationalBT.IsRTL, theLangRow);
            else
            {
                System.Diagnostics.Debug.Assert(projFile.NationalBTLang.Count == 1);
                StoryProject.NationalBTLangRow rowNatlRow = projFile.NationalBTLang[0];
                NationalBT.LangName = rowNatlRow.name;
                NationalBT.LangCode = rowNatlRow.code;
                NationalBT.LangFont = new Font(rowNatlRow.FontName, rowNatlRow.FontSize);
                NationalBT.FontColor = Color.FromName(rowNatlRow.FontColor);
                NationalBT.FullStop = rowNatlRow.SentenceFinalPunct;
                NationalBT.IsRTL = (!rowNatlRow.IsRTLNull() && rowNatlRow.RTL);
                NationalBT.Keyboard = (!rowNatlRow.IsKeyboardNull() && !String.IsNullOrEmpty(rowNatlRow.Keyboard))
                    ? rowNatlRow.Keyboard : null;
            }

            if (projFile.InternationalBTLang.Count == 0)
                projFile.InternationalBTLang.AddInternationalBTLangRow(
                    InternationalBT.LangName, InternationalBT.LangCode,
                    InternationalBT.LangFont.Name, InternationalBT.LangFont.Size,
                    InternationalBT.FontColor.Name, InternationalBT.FullStop, InternationalBT.Keyboard, InternationalBT.IsRTL,
                    theLangRow);
            else
            {
                System.Diagnostics.Debug.Assert(projFile.InternationalBTLang.Count == 1);
                StoryProject.InternationalBTLangRow rowEngRow = projFile.InternationalBTLang[0];
                InternationalBT.LangName = rowEngRow.name;
                InternationalBT.LangCode = rowEngRow.code;
                InternationalBT.LangFont = new Font(rowEngRow.FontName, rowEngRow.FontSize);
                InternationalBT.FontColor = Color.FromName(rowEngRow.FontColor);
                InternationalBT.FullStop = rowEngRow.SentenceFinalPunct;
                InternationalBT.IsRTL = (!rowEngRow.IsRTLNull() && rowEngRow.RTL);
                InternationalBT.Keyboard = (!rowEngRow.IsKeyboardNull() && !String.IsNullOrEmpty(rowEngRow.Keyboard))
                    ? rowEngRow.Keyboard : null;
            }
        }

        public class LanguageInfo
        {
            public string LangName = null;
            public string LangCode = null;
            public Font LangFont;
            public Color FontColor;
            public string FullStop;
            public string Keyboard = null;
            public bool IsRTL = false;

            public LanguageInfo(Font font, Color fontColor)
            {
                FullStop = ".";
                LangFont = font;
                FontColor = fontColor;
            }

            public LanguageInfo(string strLangName, string strLangCode, Font font, Color fontColor)
            {
                LangName = strLangName;
                LangCode = strLangCode;
                FullStop = ".";
                LangFont = font;
                FontColor = fontColor;
            }

            public XElement GetXml(string strLangType)
            {
                XElement elemLang =
                    new XElement(strLangType,
                        new XAttribute("name", LangName),
                        new XAttribute("code", LangCode),
                        new XAttribute("FontName", LangFont.Name),
                        new XAttribute("FontSize", LangFont.Size),
                        new XAttribute("FontColor", FontColor.Name));

                if (!String.IsNullOrEmpty(FullStop))
                    elemLang.Add(new XAttribute("SentenceFinalPunct", FullStop));

                if (IsRTL)
                    elemLang.Add(new XAttribute("RTL", IsRTL));

                if (!String.IsNullOrEmpty(Keyboard))
                    elemLang.Add(new XAttribute("Keyboard", Keyboard));

                return elemLang;
            }
        }

        internal class ProjectFileNotFoundException : ApplicationException
        {
            internal ProjectFileNotFoundException(string strMessage)
                : base(strMessage)
            {
            }
        }

        public void ThrowIfProjectFileDoesntExists()
        {
            if (!File.Exists(ProjectFileName))
                throw new ProjectFileNotFoundException(String.Format("Unable to find the file: '{0}'", ProjectFileName));
        }

        public string ProjectFileName
        {
            get { return String.Format(@"{0}\{1}.onestory", ProjectFolder, ProjectName); }
        }

        public static string GetDefaultProjectFileName(string strProjectName)
        {
            return String.Format(@"{0}\{1}\{1}.onestory", OneStoryProjectFolderRoot, strProjectName);
        }

        public string ProjectFolder
        {
            get { return _strProjectFolder; }
        }

        public static string OneStoryProjectFolderRoot
        {
            get
            {
                return String.Format(@"{0}\{1}",
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
#if !DataDllBuild
                    Properties.Settings.Default.DefMyDocsSubfolder);
#else
                    "OneStory");
#endif
            }
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
