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

        [Test]
        public void FailedOnAnOlderRuntime_TriesWebView2Again()
        {
            Assert.That(HtmlHostFactory.WhyNotWebView2(false, "131.0.1.1", "130.0.2849.80"), Is.Null);
        }
    }
}
