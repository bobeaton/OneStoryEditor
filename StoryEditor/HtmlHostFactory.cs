namespace OneStoryProjectEditor
{
    public static class HtmlHostFactory
    {
        // sub-project C adds the setting that chooses the WebView2 host here
        public static IHtmlHost Create(HtmlHostOptions options = null)
        {
            return new IeHtmlHost(options ?? new HtmlHostOptions());
        }
    }
}
