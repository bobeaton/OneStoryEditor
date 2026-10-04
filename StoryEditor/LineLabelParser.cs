using System;

namespace OneStoryProjectEditor
{
    /// <summary>
    /// turns the text of a pane's top line-header cell (e.g. "Ln: 5", "Ln: 5 (Hidden)", "Gen Qs:", "Story: ...")
    /// into the line-number link's label and line index (was inline in HtmlVerseControl.OnScroll)
    /// </summary>
    internal static class LineLabelParser
    {
        public static bool TryParse(string strLabel, out string strLinkText, out int nLineIndex)
        {
            strLinkText = null;
            nLineIndex = 0;
            if (String.IsNullOrEmpty(strLabel))
                return false;

            if (StoryEditor.IsFirstCharsEqual(strLabel, VersesData.CstrZerothLineNameConNotes,
                                              VersesData.CstrZerothLineNameConNotes.Length))
            {
                strLinkText = StoryEditor.CstrFirstVerse;
                return true;
            }

            if (StoryEditor.IsFirstCharsEqual(strLabel, VersesData.CstrZerothLineNameBtPane,
                                              VersesData.CstrZerothLineNameBtPane.Length))
            {
                strLinkText = VersesData.CstrZerothLineNameBtPane;
                return true;
            }

            if (!StoryEditor.IsFirstCharsEqual(strLabel, VersesData.LinePrefix, VersesData.LinePrefix.Length))
                return false;

            // e.g. "Ln: 1" (or for the French localization: "Ln : 1") or "Ln: 1 (Hidden)"
            int nIndex;
            if ((nIndex = strLabel.IndexOf(VersesData.HiddenStringSpace, StringComparison.Ordinal)) != -1)
                strLabel = strLabel.Substring(0, nIndex);

            // the line number is the last bit after the last space (French has a space before the colon)
            nIndex = strLabel.LastIndexOf(' ');
            if ((nIndex == -1) || !Int32.TryParse(strLabel.Substring(nIndex + 1), out nLineIndex))
                return false;

            strLinkText = strLabel;
            return true;
        }
    }
}
