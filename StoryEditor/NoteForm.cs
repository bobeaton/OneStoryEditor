using System.Drawing;
using System.Windows.Forms;
using Chorus;
using Chorus.UI.Notes.Bar;
using Chorus.Utilities;

namespace OneStoryProjectEditor
{
    public partial class NoteForm : Form
    {
        private ChorusSystem _chorusSystem;
        private NotesBarView _notesBar;

        public NoteForm(ProjectSettings projSettings, string strUsername)
        {
            InitializeComponent();

            // create an object to tie notes to particular things. In our case, it's
            //  just tied to the projectname
            var notesToRecordMapping = new NotesToRecordMapping
            {
                FunctionToGetCurrentUrlForNewNotes = GetCurrentUrlForNewNotes,
                FunctionToGoFromObjectToItsId = GetIdForObject
            };

            _chorusSystem = new ChorusSystem(projSettings.ProjectFolder, strUsername);
            _notesBar = _chorusSystem.WinForms.CreateNotesBar(projSettings.ProjectFilePath,
                notesToRecordMapping, new NullProgress());
            _notesBar.Width = 60;
            _notesBar.Dock = DockStyle.Right | DockStyle.Top;
            _notesBar.SetTargetObject(projSettings);
            _notesBar.Location = new Point(0, 0);
            tableLayoutPanel.Controls.Add(_notesBar, 0, 0);

            Chorus.UI.Notes.Browser.NotesBrowserPage browser = _chorusSystem.WinForms.CreateNotesBrowser();
            browser.Location = new Point(0, _notesBar.Height);
            Size sz = browser.Bounds.Size;
            sz.Height += _notesBar.Height;
            ClientSize = sz;
            tableLayoutPanel.Controls.Add(browser, 0, 1);
        }

        private static string GetIdForObject(object targetofannotation)
        {
            var projSettings = targetofannotation as ProjectSettings;
            return OneStoryUrlBuilder.UrlProjectNote(projSettings.ProjectName);
        }

        private static string GetCurrentUrlForNewNotes(object target, string escapedid)
        {
            var projSettings = target as ProjectSettings;
            return OneStoryUrlBuilder.UrlProjectNote(projSettings.ProjectName);
        }
    }
}
