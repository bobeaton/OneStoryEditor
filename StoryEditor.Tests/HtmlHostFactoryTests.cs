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

        // Create with the given setting and WebView2 creator; returns the host's type and the notice
        private static void CreateWith(bool bUseIeSetting, System.Func<HtmlHostOptions, IHtmlHost> creator,
            out System.Type typeHost, out string strNotice)
        {
            CreateWith(bUseIeSetting, creator, out typeHost, out strNotice, out _);
        }

        // ... and whether IE was remembered as in use (the setting turned on, and IeInUse raised); the real setting is
        //  never touched
        private static void CreateWith(bool bUseIeSetting, System.Func<HtmlHostOptions, IHtmlHost> creator,
            out System.Type typeHost, out string strNotice, out bool bIeRemembered)
        {
            var oldCreator = HtmlHostFactory.WebView2Creator;
            var oldSetting = HtmlHostFactory.UseIeSetting;
            var oldRemember = HtmlHostFactory.RememberIeInUse;
            var use = HtmlHostFactory.UseWebView2WhenAvailable;
            var bRemembered = false;
            var bRaised = false;
            System.EventHandler onIeInUse = (s, e) => bRaised = true;
            try
            {
                HtmlHostFactory.ResetNoticeForTests();
                HtmlHostFactory.UseWebView2WhenAvailable = true;
                HtmlHostFactory.UseIeSetting = () => bUseIeSetting;
                HtmlHostFactory.WebView2Creator = creator;
                HtmlHostFactory.RememberIeInUse = () => bRemembered = true;
                HtmlHostFactory.IeInUse += onIeInUse;
                var host = HtmlHostFactory.Create();
                typeHost = host.GetType();
                strNotice = HtmlHostFactory.LastNotice;
                host.Dispose();
            }
            finally
            {
                HtmlHostFactory.IeInUse -= onIeInUse;
                HtmlHostFactory.WebView2Creator = oldCreator;
                HtmlHostFactory.UseIeSetting = oldSetting;
                HtmlHostFactory.RememberIeInUse = oldRemember;
                HtmlHostFactory.UseWebView2WhenAvailable = use;
                HtmlHostFactory.ResetNoticeForTests();
            }
            Assert.That(bRaised, Is.EqualTo(bRemembered), "IeInUse is raised exactly when IE is remembered");
            bIeRemembered = bRemembered;
        }

        // falling back to IE checks 'Use the IE browser panes', so the Advanced menu shows what's in use
        [Test, Apartment(System.Threading.ApartmentState.STA)]
        public void FallingBackToIe_TurnsOnTheIeSetting()
        {
            CreateWith(false, o => throw new System.Runtime.InteropServices.COMException("boom"), out _, out _, out var bThrown);
            Assert.That(bThrown, Is.True, "WebView2 threw");
            CreateWith(false, o => null, out var type, out _, out var bWhyNot);    // (CreateWebView2's 'why not' case)
            Assert.That(type, Is.EqualTo(typeof(IeHtmlHost)));
            Assert.That(bWhyNot, Is.True, "a reason not to use WebView2 (e.g. no runtime)");
            CreateWith(true, o => null, out _, out _, out var bSetting);
            Assert.That(bSetting, Is.False, "the setting is already on");
        }

        [Test, Apartment(System.Threading.ApartmentState.STA)]
        public void MissingWebView2Assembly_FallsBackToIe_WithNotice()
        {
            CreateWith(false, o => throw new System.IO.FileNotFoundException("Microsoft.Web.WebView2.Core"), out var type, out var strNotice);
            Assert.That(type, Is.EqualTo(typeof(IeHtmlHost)));
            Assert.That(strNotice, Does.Contain("WebView2 components could not be loaded"));
        }

        // start-up must never crash over WebView2, whatever it throws
        [Test, Apartment(System.Threading.ApartmentState.STA)]
        public void AnyWebView2CreationException_FallsBackToIe_WithNotice()
        {
            CreateWith(false, o => throw new System.Runtime.InteropServices.COMException("boom"), out var type, out var strNotice);
            Assert.That(type, Is.EqualTo(typeof(IeHtmlHost)));
            Assert.That(strNotice, Does.Contain("WebView2 could not be started").And.Contain("boom"));
        }

        // forcing IE mustn't touch WebView2 at all (not even load its assembly to ask for the runtime version)
        [Test, Apartment(System.Threading.ApartmentState.STA)]
        public void IeSetting_IsCheckedBeforeWebView2IsTouched()
        {
            var bCreatorCalled = false;
            CreateWith(true, o => { bCreatorCalled = true; return null; }, out var type, out var strNotice);
            Assert.That(bCreatorCalled, Is.False);
            Assert.That(type, Is.EqualTo(typeof(IeHtmlHost)));
            Assert.That(strNotice, Does.Contain("setting"));
        }

        [Test]
        public void FailedLastTime_NoticeSaysHowToRetry()
        {
            Assert.That(HtmlHostFactory.WhyNotWebView2(false, "130.0.2849.80", "130.0.2849.80"),
                        Does.Contain("to try WebView2 again").And.Contain("Use the IE browser panes"));
        }

        [Test]
        public void FailedOnAnOlderRuntime_TriesWebView2Again()
        {
            Assert.That(HtmlHostFactory.WhyNotWebView2(false, "131.0.1.1", "130.0.2849.80"), Is.Null);
        }
    }
}
