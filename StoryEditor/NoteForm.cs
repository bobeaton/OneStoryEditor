using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Chorus;
using Chorus.Utilities;

namespace OneStoryProjectEditor
{
    public partial class NoteForm : Form
    {
        public NoteForm(ProjectSettings projSettings)
        {
            InitializeComponent();

            var chorus = new ChorusSystem(projSettings.ProjectFolder);
            Chorus.UI.Notes.Browser.NotesBrowserPage browser = chorus.WinForms.CreateNotesBrowser(); 
            NotesToRecordMapping mapping = SimpleForTest(); 
            var bar = chorus.WinForms.CreateNotesBar(projSettings.ProjectFilePath,
                mapping, new NullProgress());
            bar.SetTargetObject(this);
            bar.Dock = DockStyle.Fill;
            // Controls.Add(bar);
            // Point ptBrowser = new Point(0, bar.Size.Height);
            // browser.Location = ptBrowser;
            Controls.Add(browser);
            
            /*
            // Chorus.UI.Notes.Browser.NotesBrowserPage browser = chorus.WinForms.CreateNotesBrowser();
            // NotesToRecordMapping mapping = SimpleForTest();
            Point ptBrowser = new Point(0, bar.Size.Height);
            browser.Location = ptBrowser;
            Controls.Add(browser);
            ClientSize = browser.Bounds.Size + bar.Bounds.Size;
             */
        }

        static internal NotesToRecordMapping SimpleForTest()
        {
            var m = new NotesToRecordMapping();
            m.FunctionToGoFromObjectToItsId = Bar;
            m.FunctionToGetCurrentUrlForNewNotes = Foo;
            return m;
        }

        static private string Foo(object target, string escapedId)
        {
            return "This is Foo";
        }

        static private string Bar(object targetOfAnnotation)
        {
            return "This is Bar";
        }
    }
}
