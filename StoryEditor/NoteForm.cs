using System.Drawing;
using System.Windows.Forms;
using Chorus;
using Chorus.UI.Notes.Bar;
using Chorus.UI.Notes.Browser;
using Chorus.Utilities;
using Palaso.Progress;

namespace OneStoryProjectEditor
{
    public partial class NoteForm : TopForm
    {
        private ChorusSystem _chorusSystem;
        private NotesBarView _notesBar;
        private ProjectSettings _projSettings;

        public NoteForm(ProjectSettings projSettings, string strUsername)
        {
            InitializeComponent();
            _projSettings = projSettings;

            _chorusSystem = new ChorusSystem(projSettings.ProjectFolder);

            // create an object to tie notes to particular things. In our case, it's
            //  just tied to the projectname
            var notesToRecordMapping = new NotesToRecordMapping
            {
                FunctionToGetCurrentUrlForNewNotes = GetCurrentUrlForNewNotes,
                FunctionToGoFromObjectToItsId = GetIdForObject
            };

            _notesBar = _chorusSystem.WinForms.CreateNotesBar(projSettings.ProjectFilePath,
                notesToRecordMapping, new NullProgress());
            tableLayoutPanel.Controls.Add(_notesBar, 0, 0);

            NotesBrowserPage browser = _chorusSystem.WinForms.CreateNotesBrowser();
            browser.Dock = DockStyle.Fill;

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

        private void NoteForm_Load(object sender, System.EventArgs e)
        {
            _notesBar.SetTargetObject(_projSettings);
        }
    }
}
