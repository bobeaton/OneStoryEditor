using System.ComponentModel;
using NUnit.Framework;

namespace OneStoryProjectEditor.Tests
{
    [TestFixture]
    public class FormResourceTests
    {
        // the SDK-style project names embedded .resx resources after the *file*, but a form's
        //  InitializeComponent looks them up by its *class* name. HtmlDisplayForm.resx belongs to
        //  RevisionHistoryForm, so it needs an explicit LogicalName.
        [Test]
        public void RevisionHistoryForm_ResourcesAreFoundByClassName()
        {
            var resources = new ComponentResourceManager(typeof(RevisionHistoryForm));
            Assert.That(resources.GetString("radioButtonShowAllWithState.ToolTip"), Is.Not.Null);
        }
    }
}
