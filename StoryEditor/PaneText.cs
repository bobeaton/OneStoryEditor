namespace OneStoryProjectEditor
{
    internal static class PaneText
    {
        // true when text from a pane is what the model already holds. Then nothing changed, so the project must not
        //  become Modified (a flush re-sends the focused box's text, which mustn't cause a "save changes?" prompt)
        public static bool IsSame(StringTransfer st, string strNewText)
        {
            var strNew = StoryData.NormalizeLineEndings(strNewText) ?? string.Empty;
            return strNew == (st.ToString() ?? string.Empty);
        }
    }
}
