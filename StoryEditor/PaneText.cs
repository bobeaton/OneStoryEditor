namespace OneStoryProjectEditor
{
    internal static class PaneText
    {
        private static readonly char[] EdgeLineBreaks = { '\r', '\n' };

        // true when text from a pane is what the model already holds. Then nothing changed, so the project must not
        //  become Modified (a flush re-sends the focused box's text, which mustn't cause a "save changes?" prompt)
        public static bool IsSame(StringTransfer st, string strNewText)
        {
            return IsSame(st, strNewText, false);
        }

        // bIgnoreEdgeLineBreaks is for quiet (flush) text only: IE loses a leading line break from both a
        //  textarea's value and its htmlText (the HTML parser drops it), and htmlText also drops a trailing one.
        //  So a flush of a box the user merely focused can differ from the stored value only by leading/trailing
        //  CR/LF characters; that mustn't set Modified (or strip the stored line breaks).
        public static bool IsSame(StringTransfer st, string strNewText, bool bIgnoreEdgeLineBreaks)
        {
            var strNew = StoryData.NormalizeLineEndings(strNewText) ?? string.Empty;
            var strOld = st.ToString() ?? string.Empty;
            if (strNew == strOld)
                return true;

            if (!bIgnoreEdgeLineBreaks)
                return false;

            var strOldNormalized = StoryData.NormalizeLineEndings(strOld) ?? string.Empty;
            return strNew.Trim(EdgeLineBreaks) == strOldNormalized.Trim(EdgeLineBreaks);
        }
    }
}
