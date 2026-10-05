using NUnit.Framework;

namespace OneStoryProjectEditor.Tests
{
    [TestFixture]
    public class HtmlHostFactoryTests
    {
        [Test]
        public void UsesWebView2_WhenRuntimePresent_SettingOff_NoFailure()
        {
            Assert.That(HtmlHostFactory.WhyNotWebView2(false, "130.0.2849.80", ""), Is.Null);
        }

        [Test]
        public void Setting_ForcesIe()
        {
            Assert.That(HtmlHostFactory.WhyNotWebView2(true, "130.0.2849.80", ""), Does.Contain("setting"));
        }

        [TestCase(null)]
        [TestCase("")]
        public void NoRuntime_UsesIe(string strRuntime)
        {
            Assert.That(HtmlHostFactory.WhyNotWebView2(false, strRuntime, ""), Does.Contain("isn't installed"));
        }

        [Test]
        public void FailedLastTime_OnThisRuntime_UsesIe()
        {
            Assert.That(HtmlHostFactory.WhyNotWebView2(false, "130.0.2849.80", "130.0.2849.80"), Does.Contain("failed"));
        }

        [Test, Apartment(System.Threading.ApartmentState.STA)]
        public void MissingWebView2Assembly_FallsBackToIe_WithNotice()
        {
            var creator = HtmlHostFactory.WebView2Creator;
            var use = HtmlHostFactory.UseWebView2WhenAvailable;
            try
            {
                HtmlHostFactory.ResetNoticeForTests();
                HtmlHostFactory.UseWebView2WhenAvailable = true;
                HtmlHostFactory.WebView2Creator = o => throw new System.IO.FileNotFoundException("Microsoft.Web.WebView2.Core");
                var host = HtmlHostFactory.Create();
                try
                {
                    Assert.That(host, Is.InstanceOf<IeHtmlHost>());
                    Assert.That(HtmlHostFactory.LastNotice, Does.Contain("WebView2 components could not be loaded"));
                }
                finally
                {
                    (host as System.IDisposable)?.Dispose();
                }
            }
            finally
            {
                HtmlHostFactory.WebView2Creator = creator;
                HtmlHostFactory.UseWebView2WhenAvailable = use;
                HtmlHostFactory.ResetNoticeForTests();
            }
        }

        [Test]
        public void FailedOnAnOlderRuntime_TriesWebView2Again()
        {
            Assert.That(HtmlHostFactory.WhyNotWebView2(false, "131.0.1.1", "130.0.2849.80"), Is.Null);
        }
    }
}
