using NUnit.Framework;

namespace OneStoryProjectEditor.Tests
{
    [TestFixture]
    public class PageScriptsTests
    {
        [Test]
        public void Ie_IsTodaysScripts()
        {
            var ie = PageScripts.For(HtmlEngine.Ie);
            Assert.That(ie.Bridge, Does.Contain("window.external"));
            Assert.That(ie.PaneCommon, Is.EqualTo(PageScripts.Get("PaneCommon.js")));
            Assert.That(ie.StoryBt, Is.EqualTo(Properties.Resources.StoryBtJs));
            Assert.That(ie.JQuery, Is.EqualTo(Properties.Resources.jquery_min));
            const string strHtml = "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\">\r\n<html></html>";
            Assert.That(ie.ApplyDoctype(strHtml), Is.EqualTo(strHtml), "IE pages are left exactly as they are");
        }

        [Test]
        public void WebView2_IsTheModernScripts_InStandardsMode()
        {
            var modern = PageScripts.For(HtmlEngine.WebView2);
            Assert.That(modern.Bridge, Does.Contain("chrome.webview"));
            Assert.That(modern.Bridge, Does.Not.Contain("window.external"));
            Assert.That(modern.JQuery, Is.Empty);
            Assert.That(modern.StoryBtEnd, Is.Empty);
            Assert.That(modern.ApplyDoctype("<!DOCTYPE HTML PUBLIC \"x\">\r\n<html></html>"),
                        Is.EqualTo("<!DOCTYPE html>\r\n" + PageScriptSet.CstrModernStyle + "<html></html>"));
            Assert.That(modern.ApplyDoctype("<html></html>"), Is.EqualTo("<!DOCTYPE html>\r\n" + PageScriptSet.CstrModernStyle + "<html></html>"));
            // (last in the head, so it wins over the page's own textarea rule)
            Assert.That(modern.ApplyDoctype("<html><head><style>x</style></head><body></body></html>"),
                        Is.EqualTo("<!DOCTYPE html>\r\n<html><head><style>x</style>" + PageScriptSet.CstrModernStyle + "</head><body></body></html>"));
        }

        // quirks mode matched class names whatever their case; standards mode doesn't, so on a modern page every class
        //  an element uses that a style rule names must be spelt the same
        [Test]
        public void ModernPages_ClassNamesMatchTheirStylesExactly()
        {
            foreach (var strPage in new[] { PaneTestData.StoryBtPage(HtmlEngine.WebView2), PaneTestData.ConsultantNotesPage(HtmlEngine.WebView2) })
            {
                var selectors = new System.Collections.Generic.HashSet<string>();
                foreach (System.Text.RegularExpressions.Match style in
                         System.Text.RegularExpressions.Regex.Matches(strPage, @"<style[^>]*>(.*?)</style>", System.Text.RegularExpressions.RegexOptions.Singleline))
                    foreach (System.Text.RegularExpressions.Match sel in
                             System.Text.RegularExpressions.Regex.Matches(style.Groups[1].Value, @"\.([A-Za-z][\w-]*)"))
                        selectors.Add(sel.Groups[1].Value);
                Assert.That(selectors, Does.Contain(StoryData.CstrLangInternationalBtStyleClassName), "(the scan found the styles)");

                var mismatches = new System.Collections.Generic.SortedSet<string>();
                foreach (System.Text.RegularExpressions.Match cls in
                         System.Text.RegularExpressions.Regex.Matches(strPage, "class=[\"']([^\"']*)[\"']", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                    foreach (var strToken in cls.Groups[1].Value.Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries))
                        if (!selectors.Contains(strToken) &&
                            System.Linq.Enumerable.Any(selectors, s => System.String.Equals(s, strToken, System.StringComparison.OrdinalIgnoreCase)))
                            mismatches.Add(strToken);
                Assert.That(mismatches, Is.Empty, "classes used with a different case from their style rule");
            }
        }

        [Test]
        public void IsModernPage_TellsTheEnginesPagesApart()
        {
            var project = PaneTestData.LoadProject();
            Assert.That(PageScripts.IsModernPage(StoryData.AddHtmlHtmlDocOutside("<p>x</p>", project.ProjSettings, HtmlEngine.WebView2)), Is.True);
            Assert.That(PageScripts.IsModernPage(StoryData.AddHtmlHtmlDocOutside("<p>x</p>", project.ProjSettings, HtmlEngine.Ie)), Is.False);
            Assert.That(PageScripts.IsModernPage(PaneTestData.ConsultantNotesPage(HtmlEngine.Ie)), Is.False);
            Assert.That(PageScripts.IsModernPage("<p>a fragment</p>"), Is.False);
            Assert.That(PageScripts.IsModernPage(null), Is.False);
            Assert.That(PageScripts.IsModernPage(""), Is.False);
        }

        [Test]
        public void Css_Ie_IsUnchanged()
        {
            const string strCss = ".a { font-size: 12 pt; } .b { font-size: 10,5 pt; } <style> body  { margin:1 } </style>";
            Assert.That(PageScripts.For(HtmlEngine.Ie).Css(strCss), Is.EqualTo(strCss));
        }

        [Test]
        public void Css_WebView2_GivesStandardsModeLengths()
        {
            var modern = PageScripts.For(HtmlEngine.WebView2);
            Assert.That(modern.Css(".a {\r\n      font-size: 12 pt;\r\n  }"), Is.EqualTo(".a {\r\n      font-size: 12pt;\r\n  }"));
            Assert.That(modern.Css(".a { font-size: 10.5 pt; }"), Is.EqualTo(".a { font-size: 10.5pt; }"));
            Assert.That(modern.Css(".a { font-size: 10,5 pt; }"), Is.EqualTo(".a { font-size: 10.5pt; }"));
            Assert.That(modern.Css("<style> body  { margin:1 } </style>"), Is.EqualTo("<style> body  { margin:1px } </style>"));
            Assert.That(modern.Css("body { margin:0 } table { font-size: small; }"), Is.EqualTo("body { margin:0 } table { font-size: small; }"));
        }

        // the IE pages stay byte-identical: the engine-aware StylePrefix gives IE exactly what the old one did
        [Test]
        public void StylePrefix_Ie_IsUnchanged_WebView2_HasNoSpacedUnits()
        {
            var project = PaneTestData.LoadProject();
            var strIe = StoryData.StylePrefix(project.ProjSettings, null, null);
            Assert.That(StoryData.StylePrefix(project.ProjSettings, null, null, HtmlEngine.Ie), Is.EqualTo(strIe));
            Assert.That(strIe, Does.Match(@"font-size: \d+(\.\d+)? pt;"), "the IE styles keep their quirks-mode form");
            Assert.That(StoryData.StylePrefix(project.ProjSettings, "Arial", "14", HtmlEngine.Ie),
                        Is.EqualTo(StoryData.StylePrefix(project.ProjSettings, "Arial", "14")));

            var strModern = StoryData.StylePrefix(project.ProjSettings, "Arial", "14", HtmlEngine.WebView2);
            Assert.That(strModern, Does.Not.Match(@"\d\s+pt"));
            Assert.That(strModern, Does.Contain("font-size: 14pt;"));
        }

        [Test]
        public void IePresentationPage_IsUnchangedByTheEngineParameter()
        {
            var project = PaneTestData.LoadProject();
            var html = StoryData.AddHtmlHtmlDocOutside("<p>x</p>", project.ProjSettings, HtmlEngine.Ie);
            Assert.That(html, Does.StartWith("<!DOCTYPE HTML PUBLIC"));
            Assert.That(html, Does.Contain("window.external"));      // the IE bridge
            // (the IE bridge itself still mentions chrome.webview in a fallback branch, so check for a modern-only marker)
            Assert.That(html, Does.Not.Contain("webview.addEventListener"));
        }

        [Test]
        public void WebView2PresentationPage_HasModernScripts_AndNoJQuery()
        {
            var project = PaneTestData.LoadProject();
            var html = StoryData.AddHtmlHtmlDocOutside("<p>x</p>", project.ProjSettings, HtmlEngine.WebView2);
            Assert.That(html, Does.StartWith("<!DOCTYPE html>"));
            Assert.That(html, Does.Contain("chrome.webview"));
            Assert.That(html, Does.Not.Contain("window.external"));
            Assert.That(html, Does.Not.Contain("jQuery"));
        }
    }
}
