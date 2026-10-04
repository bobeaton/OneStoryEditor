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
            Assert.That(modern.ApplyDoctype("<!DOCTYPE HTML PUBLIC \"x\">\r\n<html></html>"), Is.EqualTo("<!DOCTYPE html>\r\n<html></html>"));
            Assert.That(modern.ApplyDoctype("<html></html>"), Is.EqualTo("<!DOCTYPE html>\r\n<html></html>"));
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
