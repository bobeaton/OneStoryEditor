using NUnit.Framework;

namespace OneStoryProjectEditor.Tests
{
    // Runs once before any test in this assembly. The first NetLoc Localizer.Str call (e.g. VersesData.LinePrefix) happens
    //  lazily; if that first call comes while a browser form is live, running one browser fixture on its own crashes the test
    //  host after the run (AppDomainUnloadedException) and loses the results. The full suite never hit it because earlier
    //  fixtures made that first call. Do it here, before any form exists.
    [SetUpFixture]
    public class TestAssemblySetUp
    {
        [OneTimeSetUp]
        public void WarmUpLocalizer()
        {
            var strDummy = VersesData.LinePrefix;
            Assert.That(strDummy, Is.Not.Null);
        }
    }
}
