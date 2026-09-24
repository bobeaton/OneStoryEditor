using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using NetLoc;

namespace OneStoryProjectEditor
{
    // one line of the existing story (for a retelling) or one test question (for the
    //  answers) that an imported line can go with
    public class AlignTarget
    {
        public string Label;
        public LineData SourceLine;         // the story line or test question (shown on the left)
        public VerseData Verse;             // for a retelling
        public TestQuestionData TestQuestion; // for an answer
    }

    // Lets the user line up the lines imported from another program (e.g. a retelling
    //  or the answers to the test questions) with the lines of the story (or its test
    //  questions), which won't normally match one to one: the UNS may skip lines, or say
    //  one line in several, and the recording was segmented on its own terms.
    // The imported lines can be joined or split (and blank rows inserted), but never
    //  deleted or moved: they always stay in the order they were said. Each tier is its
    //  own column, so a line can be split at a different place in each tier.
    public partial class AlignImportLinesForm : TopForm
    {
        private readonly List<AlignTarget> _targets;
        private readonly List<ImportMapping> _mappings;
        private readonly ProjectSettings _projSettings;
        private readonly bool _bAnswers;

        // the imported lines of each tier (i.e. each mapping), which can have different lengths
        private List<List<string>> _tiers;
        private readonly Stack<List<List<string>>> _undoStack = new Stack<List<List<string>>>();
        private List<List<string>> _snapshotAtBeginEdit;
        private bool _bIgnoreEndEdit;

        private const int CnColumnLine = 0;
        private const int CnColumnSource = 1;
        private const int CnFirstTierColumn = 2;

        private static readonly Color ColorNoCell = Color.Gainsboro;
        private static readonly Color ColorRagged = Color.LightYellow;
        private static readonly Color ColorOverflow = Color.MistyRose;

        // the aligned tiers (one line per target) when the user clicks Accept
        public List<ImportMapping> AlignedMappings { get; private set; }

        // version used by Localization
        private AlignImportLinesForm()
        {
            InitializeComponent();
            Localizer.Ctrl(this);
        }

        public AlignImportLinesForm(List<AlignTarget> targets, List<ImportMapping> mappings,
                                    ProjectSettings projSettings, bool bAnswers)
        {
            _targets = targets;
            _mappings = mappings;
            _projSettings = projSettings;
            _bAnswers = bAnswers;
            InitializeComponent();
            Localizer.Ctrl(this);

            _tiers = mappings.Select(m => m.Tier.Lines.Select(l => l ?? String.Empty).ToList()).ToList();

            if (bAnswers)
            {
                Text = Localizer.Str("Line up the imported answers with the test questions");
                ColumnLine.HeaderText = Localizer.Str("Question");
            }

            foreach (var mapping in mappings)
            {
                var column = new DataGridViewTextBoxColumn
                {
                    HeaderText = String.Format("{0}{1}({2})", FieldName(mapping.Field), Environment.NewLine, mapping.Tier.Name),
                    SortMode = DataGridViewColumnSortMode.NotSortable,
                    FillWeight = 100F
                };
                column.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                var font = LanguageInfo(mapping.Field)?.FontToUse;
                if (font != null)
                    column.DefaultCellStyle.Font = font;
                dataGridViewAlign.Columns.Add(column);
            }
            ColumnSource.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            ColumnSource.DefaultCellStyle.BackColor = SystemColors.Control;

            InitSourceChoices();
            RefreshGrid();
        }

        #region targets

        // the lines of the story that will get a retelling (or the test questions that will get an answer)
        public static List<AlignTarget> GetTargets(StoryData theStory, bool bAnswers)
        {
            var targets = new List<AlignTarget>();
            if (bAnswers)
            {
                AddQuestions(targets, theStory.Verses.FirstVerse, 0);
                var nLine = 0;
                foreach (VerseData aVerse in theStory.Verses)
                {
                    nLine++;
                    if (aVerse.IsVisible)
                        AddQuestions(targets, aVerse, nLine);
                }
            }
            else
            {
                var nLine = 0;
                foreach (VerseData aVerse in theStory.Verses)
                {
                    nLine++;
                    if (!aVerse.IsVisible)
                        continue;
                    targets.Add(new AlignTarget
                    {
                        Label = nLine.ToString(),
                        SourceLine = aVerse.StoryLine,
                        Verse = aVerse
                    });
                }
            }
            return targets;
        }

        private static void AddQuestions(List<AlignTarget> targets, VerseData aVerse, int nLine)
        {
            if (aVerse == null)
                return;

            var nQuestion = 0;
            foreach (TestQuestionData aTq in aVerse.TestQuestions)
            {
                nQuestion++;
                if (!aTq.IsVisible)
                    continue;
                targets.Add(new AlignTarget
                {
                    Label = (nLine == 0)
                                ? String.Format(Localizer.Str("General TQ {0}"), nQuestion)
                                : String.Format(Localizer.Str("Ln {0}, TQ {1}"), nLine, nQuestion),
                    SourceLine = aTq.TestQuestionLine,
                    Verse = aVerse,
                    TestQuestion = aTq
                });
            }
        }

        #endregion

        #region source language

        private class SourceChoice
        {
            public string Name;
            public StoryEditor.TextFields Field;
            public override string ToString() { return Name; }
        }

        private void InitSourceChoices()
        {
            var fields = new[]
            {
                StoryEditor.TextFields.Vernacular,
                StoryEditor.TextFields.NationalBt,
                StoryEditor.TextFields.InternationalBt,
                StoryEditor.TextFields.FreeTranslation
            };

            // only the languages of the project that have some data in these lines
            var choices = fields.Where(f => (LanguageInfo(f) != null) && LanguageInfo(f).HasData &&
                                            _targets.Any(t => !String.IsNullOrEmpty(SourceText(t, f))))
                                .Select(f => new SourceChoice {Name = FieldName(f), Field = f})
                                .ToList();
            if (!choices.Any())
                choices.Add(new SourceChoice
                {
                    Name = FieldName(StoryEditor.TextFields.Vernacular),
                    Field = StoryEditor.TextFields.Vernacular
                });

            toolStripComboBoxSource.Items.AddRange(choices.Cast<object>().ToArray());

            // by default, the same language as the first tier being imported (usually the story language)
            var firstField = _mappings.Select(m => m.Field).FirstOrDefault();
            var def = choices.FirstOrDefault(c => c.Field == firstField) ?? choices.First();
            toolStripComboBoxSource.SelectedItem = def;
        }

        private StoryEditor.TextFields SourceField
        {
            get
            {
                var choice = toolStripComboBoxSource.SelectedItem as SourceChoice;
                return (choice != null) ? choice.Field : StoryEditor.TextFields.Vernacular;
            }
        }

        private static string SourceText(AlignTarget target, StoryEditor.TextFields field)
        {
            var line = target.SourceLine;
            if (line == null)
                return null;

            StringTransfer st;
            switch (field)
            {
                case StoryEditor.TextFields.NationalBt:
                    st = line.NationalBt;
                    break;
                case StoryEditor.TextFields.InternationalBt:
                    st = line.InternationalBt;
                    break;
                case StoryEditor.TextFields.FreeTranslation:
                    st = line.FreeTranslation;
                    break;
                default:
                    st = line.Vernacular;
                    break;
            }
            return (st != null) ? st.ToString() : null;
        }

        private void ComboBoxSourceSelectedIndexChanged(object sender, EventArgs e)
        {
            var font = LanguageInfo(SourceField)?.FontToUse;
            if (font != null)
                ColumnSource.DefaultCellStyle.Font = font;
            if (_tiers != null)
                RefreshGrid();
        }

        private ProjectSettings.LanguageInfo LanguageInfo(StoryEditor.TextFields field)
        {
            switch (field)
            {
                case StoryEditor.TextFields.Vernacular:
                    return _projSettings.Vernacular;
                case StoryEditor.TextFields.NationalBt:
                    return _projSettings.NationalBT;
                case StoryEditor.TextFields.InternationalBt:
                    return _projSettings.InternationalBT;
                case StoryEditor.TextFields.FreeTranslation:
                    return _projSettings.FreeTranslation;
            }
            return null;
        }

        private string FieldName(StoryEditor.TextFields field)
        {
            var li = LanguageInfo(field);
            return ((li != null) && !String.IsNullOrEmpty(li.LangName))
                       ? li.LangName
                       : field.ToString();
        }

        #endregion

        #region display

        private int RowCount
        {
            get { return Math.Max(_targets.Count, _tiers.Max(t => t.Count)); }
        }

        private void RefreshGrid()
        {
            // keep the user's place
            var nFirstRow = dataGridViewAlign.FirstDisplayedScrollingRowIndex;
            var current = dataGridViewAlign.CurrentCell;
            int nCurRow = (current != null) ? current.RowIndex : 0,
                nCurCol = (current != null) ? current.ColumnIndex : CnFirstTierColumn;

            var sourceField = SourceField;
            dataGridViewAlign.SuspendLayout();
            dataGridViewAlign.Rows.Clear();
            var nRows = RowCount;
            if (nRows > 0)
                dataGridViewAlign.Rows.Add(nRows);

            for (var nRow = 0; nRow < nRows; nRow++)
            {
                var row = dataGridViewAlign.Rows[nRow];
                if (nRow >= _targets.Count)
                {
                    row.Cells[CnColumnLine].Value = null;
                    row.Cells[CnColumnSource].Value = Localizer.Str("(no line to go with: join with the row above)");
                }
                else
                {
                    row.Cells[CnColumnLine].Value = _targets[nRow].Label;
                    row.Cells[CnColumnSource].Value = SourceText(_targets[nRow], sourceField);
                }

                for (var nTier = 0; nTier < _tiers.Count; nTier++)
                {
                    var tier = _tiers[nTier];
                    row.Cells[CnFirstTierColumn + nTier].Value = (nRow < tier.Count) ? tier[nRow] : null;
                }

                StyleRow(nRow);
            }
            dataGridViewAlign.ResumeLayout();

            if (nRows > 0)
            {
                nCurRow = Math.Min(Math.Max(nCurRow, 0), nRows - 1);
                nCurCol = Math.Min(Math.Max(nCurCol, 0), dataGridViewAlign.ColumnCount - 1);
                dataGridViewAlign.CurrentCell = dataGridViewAlign.Rows[nCurRow].Cells[nCurCol];
                if ((nFirstRow >= 0) && (nFirstRow < nRows))
                    dataGridViewAlign.FirstDisplayedScrollingRowIndex = nFirstRow;
            }

            UpdateStatus();
        }

        // colors the rows past the last line (which need to be joined with the one above), the
        //  cells a tier doesn't have, and the empty cells in a row where other tiers have text
        //  (e.g. a line was split in one tier, but not (yet) in the others)
        private void StyleRow(int nRow)
        {
            var row = dataGridViewAlign.Rows[nRow];
            var bOverflow = (nRow >= _targets.Count);
            row.DefaultCellStyle.BackColor = bOverflow ? ColorOverflow : Color.Empty;
            row.Cells[CnColumnSource].Style.BackColor = bOverflow ? ColorOverflow : Color.Empty;

            var bAnyText = _tiers.Any(t => (nRow < t.Count) && !String.IsNullOrEmpty(t[nRow]));
            for (var nTier = 0; nTier < _tiers.Count; nTier++)
            {
                var tier = _tiers[nTier];
                var cell = row.Cells[CnFirstTierColumn + nTier];
                if (bOverflow)
                    cell.Style.BackColor = Color.Empty;
                else if (nRow >= tier.Count)
                    cell.Style.BackColor = ColorNoCell;
                else if (bAnyText && String.IsNullOrEmpty(tier[nRow]) && (_tiers.Count > 1))
                    cell.Style.BackColor = ColorRagged;
                else
                    cell.Style.BackColor = Color.Empty;
            }
        }

        private void UpdateStatus()
        {
            var counts = _tiers.Select(t => t.Count).ToList();
            var strStatus = String.Format(_bAnswers ? Localizer.Str("Questions: {0}") : Localizer.Str("Story lines: {0}"),
                                          _targets.Count);
            for (var i = 0; i < _mappings.Count; i++)
                strStatus += String.Format("   {0}: {1}", FieldName(_mappings[i].Field), counts[i]);

            var bTiersDiffer = counts.Distinct().Count() > 1;
            var bOverflow = counts.Any(c => c > _targets.Count);
            if (bTiersDiffer)
                strStatus += "   " + Localizer.Str("(the tiers don't have the same number of lines: did you split a line in one tier, but not the others?)");
            else if (bOverflow)
                strStatus += "   " + Localizer.Str("(there are more imported lines than lines to go with)");

            labelStatus.Text = strStatus;
            labelStatus.ForeColor = (bTiersDiffer || bOverflow) ? Color.Firebrick : SystemColors.ControlText;
            toolStripButtonUndo.Enabled = _undoStack.Any();
        }

        #endregion

        #region editing

        private List<List<string>> Snapshot()
        {
            return _tiers.Select(t => new List<string>(t)).ToList();
        }

        private void PushUndo(List<List<string>> snapshot = null)
        {
            _undoStack.Push(snapshot ?? Snapshot());
        }

        private int CurrentRow
        {
            get { return (dataGridViewAlign.CurrentCell != null) ? dataGridViewAlign.CurrentCell.RowIndex : -1; }
        }

        // the index of the tier of the current cell (or -1 if it isn't on one of the tiers)
        private int CurrentTier
        {
            get
            {
                return (dataGridViewAlign.CurrentCell != null)
                           ? dataGridViewAlign.CurrentCell.ColumnIndex - CnFirstTierColumn
                           : -1;
            }
        }

        private static string Combine(string str1, string str2)
        {
            return String.Join(" ", new[] {str1, str2}.Where(s => !String.IsNullOrEmpty(s)).Select(s => s.Trim()));
        }

        // joins line nRow + 1 of a tier onto the end of line nRow (if it has both)
        private static bool JoinWithNext(List<string> tier, int nRow)
        {
            if ((nRow < 0) || (nRow + 1 >= tier.Count))
                return false;
            tier[nRow] = Combine(tier[nRow], tier[nRow + 1]);
            tier.RemoveAt(nRow + 1);
            return true;
        }

        private void DoEdit(Func<bool> edit)
        {
            if (!CommitEdit())
                return;

            var snapshot = Snapshot();
            if (!edit())
                return;

            PushUndo(snapshot);
            RefreshGrid();
        }

        private bool CommitEdit()
        {
            return !dataGridViewAlign.IsCurrentCellInEditMode || dataGridViewAlign.EndEdit();
        }

        private void JoinRowWithNextClick(object sender, EventArgs e)
        {
            var nRow = CurrentRow;
            DoEdit(() => _tiers.Aggregate(false, (b, t) => JoinWithNext(t, nRow) | b));
        }

        private void JoinRowWithPreviousClick(object sender, EventArgs e)
        {
            var nRow = CurrentRow;
            if (nRow > 0)
                MoveToRow(nRow - 1);
            DoEdit(() => _tiers.Aggregate(false, (b, t) => JoinWithNext(t, nRow - 1) | b));
        }

        private void InsertBlankRowClick(object sender, EventArgs e)
        {
            var nRow = CurrentRow;
            DoEdit(() =>
            {
                if (nRow < 0)
                    return false;
                var bChanged = false;
                foreach (var tier in _tiers.Where(t => nRow <= t.Count))
                {
                    tier.Insert(nRow, String.Empty);
                    bChanged = true;
                }
                return bChanged;
            });
        }

        private void DeleteBlankRowClick(object sender, EventArgs e)
        {
            var nRow = CurrentRow;
            DoEdit(() =>
            {
                if (nRow < 0)
                    return false;
                if (_tiers.Any(t => (nRow < t.Count) && !String.IsNullOrEmpty(t[nRow])))
                {
                    ShowCantDelete();
                    return false;
                }
                var bChanged = false;
                foreach (var tier in _tiers.Where(t => nRow < t.Count))
                {
                    tier.RemoveAt(nRow);
                    bChanged = true;
                }
                return bChanged;
            });
        }

        private void JoinCellWithNextClick(object sender, EventArgs e)
        {
            int nRow = CurrentRow, nTier = CurrentTier;
            if (nTier >= 0)
                DoEdit(() => JoinWithNext(_tiers[nTier], nRow));
        }

        private void JoinCellWithPreviousClick(object sender, EventArgs e)
        {
            int nRow = CurrentRow, nTier = CurrentTier;
            if ((nTier < 0) || (nRow <= 0))
                return;
            MoveToRow(nRow - 1);
            DoEdit(() => JoinWithNext(_tiers[nTier], nRow - 1));
        }

        private void InsertBlankCellClick(object sender, EventArgs e)
        {
            int nRow = CurrentRow, nTier = CurrentTier;
            if ((nTier < 0) || (nRow < 0))
                return;
            DoEdit(() =>
            {
                var tier = _tiers[nTier];
                if (nRow > tier.Count)
                    return false;
                tier.Insert(nRow, String.Empty);
                return true;
            });
        }

        private void DeleteBlankCellClick(object sender, EventArgs e)
        {
            int nRow = CurrentRow, nTier = CurrentTier;
            if ((nTier < 0) || (nRow < 0))
                return;
            DoEdit(() =>
            {
                var tier = _tiers[nTier];
                if (nRow >= tier.Count)
                    return false;
                if (!String.IsNullOrEmpty(tier[nRow]))
                {
                    ShowCantDelete();
                    return false;
                }
                tier.RemoveAt(nRow);
                return true;
            });
        }

        private static void ShowCantDelete()
        {
            LocalizableMessageBox.Show(
                Localizer.Str("Imported lines can't be deleted. Join them with the line they go with instead (all of the imported text is kept, in the order it was said)."),
                StoryEditor.OseCaption);
        }

        private void UndoClick(object sender, EventArgs e)
        {
            if (dataGridViewAlign.IsCurrentCellInEditMode)
            {
                _bIgnoreEndEdit = true;
                dataGridViewAlign.CancelEdit();
                dataGridViewAlign.EndEdit();
                _bIgnoreEndEdit = false;
            }

            if (!_undoStack.Any())
                return;
            _tiers = _undoStack.Pop();
            RefreshGrid();
        }

        private void MoveToRow(int nRow)
        {
            var current = dataGridViewAlign.CurrentCell;
            if ((current == null) || (nRow < 0) || (nRow >= dataGridViewAlign.RowCount))
                return;
            CommitEdit();
            dataGridViewAlign.CurrentCell = dataGridViewAlign.Rows[nRow].Cells[current.ColumnIndex];
        }

        private void DataGridViewAlignCellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            // only the imported lines can be edited, and only the ones that have a line to go with
            if ((e.ColumnIndex < CnFirstTierColumn) ||
                ((e.RowIndex >= _targets.Count) && (e.RowIndex >= _tiers[e.ColumnIndex - CnFirstTierColumn].Count)))
            {
                e.Cancel = true;
                return;
            }
            _snapshotAtBeginEdit = Snapshot();
        }

        private void DataGridViewAlignCellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (_bIgnoreEndEdit || (e.ColumnIndex < CnFirstTierColumn))
                return;

            var tier = _tiers[e.ColumnIndex - CnFirstTierColumn];
            var value = (dataGridViewAlign.Rows[e.RowIndex].Cells[e.ColumnIndex].Value as string) ?? String.Empty;
            var oldValue = (e.RowIndex < tier.Count) ? tier[e.RowIndex] : String.Empty;
            if (value == oldValue)
                return;

            PushUndo(_snapshotAtBeginEdit);
            while (tier.Count <= e.RowIndex)
                tier.Add(String.Empty);
            tier[e.RowIndex] = value;

            // the number of rows doesn't change by typing, so just redo this row's colors
            StyleRow(e.RowIndex);
            UpdateStatus();
        }

        // splits the cell being edited at the cursor: the text after it goes into a new
        //  cell below it (in this tier only; the other tiers' cells don't move)
        private void SplitCurrentCell(TextBox tb)
        {
            int nRow = CurrentRow, nTier = CurrentTier;
            if ((nTier < 0) || (nRow < 0))
                return;

            var text = tb.Text ?? String.Empty;
            var nCaret = Math.Min(tb.SelectionStart, text.Length);
            var before = text.Substring(0, nCaret).Trim();
            var after = text.Substring(nCaret).Trim();

            var snapshot = _snapshotAtBeginEdit ?? Snapshot();
            _bIgnoreEndEdit = true;
            dataGridViewAlign.CancelEdit();
            dataGridViewAlign.EndEdit();
            _bIgnoreEndEdit = false;

            var tier = _tiers[nTier];
            while (tier.Count <= nRow)
                tier.Add(String.Empty);
            tier[nRow] = before;
            tier.Insert(nRow + 1, after);
            PushUndo(snapshot);
            RefreshGrid();

            // put the cursor at the start of the new cell, in case it needs splitting again
            if (nRow + 1 < dataGridViewAlign.RowCount)
            {
                dataGridViewAlign.CurrentCell = dataGridViewAlign.Rows[nRow + 1].Cells[CnFirstTierColumn + nTier];
                if (dataGridViewAlign.BeginEdit(false) && (dataGridViewAlign.EditingControl is TextBox tbNew))
                    tbNew.SelectionStart = 0;
            }
        }

        // the reverse of the split: Delete at the end of the cell being edited joins the next
        //  cell (of this tier) onto it, and Backspace at the start joins it onto the previous
        //  one. Returns false (so the key works as usual) if there's no cell to join with.
        private bool JoinCellWhileEditing(TextBox tb, bool bWithNext)
        {
            int nRow = CurrentRow, nTier = CurrentTier;
            if ((nTier < 0) || (nRow < 0))
                return false;

            var tier = _tiers[nTier];
            var nFirst = bWithNext ? nRow : nRow - 1;
            if ((nFirst < 0) || (nFirst + 1 >= Math.Max(tier.Count, nRow + 1)))
                return false;

            var snapshot = _snapshotAtBeginEdit ?? Snapshot();
            var text = tb.Text ?? String.Empty;
            _bIgnoreEndEdit = true;
            dataGridViewAlign.CancelEdit();
            dataGridViewAlign.EndEdit();
            _bIgnoreEndEdit = false;

            // keep whatever was typed into the cell being edited
            while (tier.Count <= nRow)
                tier.Add(String.Empty);
            tier[nRow] = text;

            // the cursor goes where the two were joined
            var nCaret = (tier[nFirst] ?? String.Empty).Trim().Length;
            JoinWithNext(tier, nFirst);
            PushUndo(snapshot);
            RefreshGrid();

            dataGridViewAlign.CurrentCell = dataGridViewAlign.Rows[nFirst].Cells[CnFirstTierColumn + nTier];
            if (dataGridViewAlign.BeginEdit(false) && (dataGridViewAlign.EditingControl is TextBox tbNew))
            {
                tbNew.SelectionStart = Math.Min(nCaret, tbNew.TextLength);
                tbNew.SelectionLength = 0;
            }
            return true;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            var tb = dataGridViewAlign.EditingControl as TextBox;
            switch (keyData)
            {
                case Keys.Enter:
                    if (tb != null)
                    {
                        SplitCurrentCell(tb);
                        return true;
                    }
                    break;
                case Keys.Delete:
                    if ((tb != null) && (tb.SelectionLength == 0) && (tb.SelectionStart >= tb.TextLength) &&
                        JoinCellWhileEditing(tb, true))
                        return true;
                    break;
                case Keys.Back:
                    if ((tb != null) && (tb.SelectionLength == 0) && (tb.SelectionStart == 0) &&
                        JoinCellWhileEditing(tb, false))
                        return true;
                    break;
                case Keys.Control | Keys.J:
                    JoinRowWithNextClick(null, EventArgs.Empty);
                    return true;
                case Keys.Control | Keys.Shift | Keys.J:
                    JoinRowWithPreviousClick(null, EventArgs.Empty);
                    return true;
                case Keys.Control | Keys.Insert:
                    InsertBlankRowClick(null, EventArgs.Empty);
                    return true;
                case Keys.Control | Keys.Delete:
                    if (tb == null)
                    {
                        DeleteBlankRowClick(null, EventArgs.Empty);
                        return true;
                    }
                    break;
                case Keys.Control | Keys.Z:
                    if (tb == null)  // otherwise, let the text box undo the typing
                    {
                        UndoClick(null, EventArgs.Empty);
                        return true;
                    }
                    break;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void DataGridViewAlignCellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            // right-click selects the cell, so the context menu applies to it
            if ((e.Button == MouseButtons.Right) && (e.RowIndex >= 0) && (e.ColumnIndex >= 0))
            {
                CommitEdit();
                dataGridViewAlign.CurrentCell = dataGridViewAlign.Rows[e.RowIndex].Cells[e.ColumnIndex];
            }
        }

        private void ContextMenuStripOpening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var bOnTier = (CurrentTier >= 0);
            joinCellWithNextMenu.Enabled =
                joinCellWithPreviousMenu.Enabled =
                insertBlankCellMenu.Enabled =
                deleteBlankCellMenu.Enabled = bOnTier;
            undoMenu.Enabled = _undoStack.Any();
        }

        #endregion

        private void ButtonAcceptClick(object sender, EventArgs e)
        {
            if (!CommitEdit())
                return;

            var nTargets = _targets.Count;
            var tiers = Snapshot();

            // imported lines after the last line to go with get added to the end of the last
            //  one (they can't be dropped or moved elsewhere, so the order is always kept)
            var nExtra = tiers.Max(t => t.Count) - nTargets;
            if (nExtra > 0)
            {
                var res = LocalizableMessageBox.Show(
                    String.Format(Localizer.Str("There are {0} imported line(s) after the last line they could go with. Click 'OK' to add them to the end of line {1}, or click 'Cancel' to go back and join them with the lines they belong with."),
                                  nExtra, _targets.Last().Label),
                    StoryEditor.OseCaption, MessageBoxButtons.OKCancel);
                if (res != DialogResult.OK)
                    return;

                foreach (var tier in tiers)
                    while (tier.Count > nTargets)
                        JoinWithNext(tier, nTargets - 1);
            }

            AlignedMappings = _mappings.Select((m, i) => new ImportMapping
            {
                Field = m.Field,
                Tier = new ImportedTier
                {
                    Name = m.Tier.Name,
                    LangCode = m.Tier.LangCode,
                    Kind = m.Tier.Kind,
                    Lines = Enumerable.Range(0, nTargets)
                                      .Select(n => (n < tiers[i].Count) ? tiers[i][n] : null)
                                      .ToList()
                }
            }).ToList();

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
