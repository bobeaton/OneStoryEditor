# Sub-project R: Remove the old .NET text-box view — Design

Status: approved in conversation (2026-10-03) · Branch: `DecoupleWebBrowser` (on top of sub-project A)

## Context and goal

StoryEditor still carries an alternative Story/BT pane built from WinForms text boxes ("old style",
toggled by the Advanced menu item `advancedUseOldStyleStoryBtPaneMenu` / property
`StoryEditor.UsingHtmlForStoryBtPane`, persisted as the user setting `UsingHtmlForStoryBtPane`).
Nobody uses it (user decision). It doubles the code paths that sub-projects B and C must refactor.

**Goal:** remove the old view and everything that exists only for it, leaving the HTML panes'
behaviour unchanged.

**Success:** the solution builds, the existing 69 tests pass, and the HTML panes behave exactly as
before (manual smoke test below). Pure removal: no new features.

## Scope (from the removal inventory)

**Delete these files** (SDK-style csproj with globbing, so no csproj edits are needed):
- `CtrlTextBox.cs`
- `VerseBtControl.*`
- `StoryLineControl.*`
- `TestingQuestionControl.*`
- `MultiLineControl.*`
- `LineFlowLayoutPanel.*` (also defines `VerseBtLineFlowLayoutPanel`)
- `ResizableControl.*` (also defines `VerseControl`)
- `AnchorControl.*`
- `VerseEditorForm.*` (its only use is a commented-out block in `HtmlStoryBtControl`)

Keep `DynamicTableLayoutPanel` (used by GlossingControl and NetBibleViewer) and `TextPaster`.

**Flatten** every `UsingHtmlForStoryBtPane` branch to its HTML (true) side, in:
- `StoryEditor.cs` (~45 sites)
- `CheckEndOfStateTransition.cs`
- `ProjectFacilitatorRequirementsCheck.cs`
- `HtmlConNoteControl.DoFind`

Remove `#define UsingHtmlDisplayForConNotes` (top of `StoryEditor.cs` and `StoryEditor.Designer.cs`)
and keep only the `#if` sides. The `#else` sides are already dead code that no longer compiles: the
non-HTML note panes were deleted earlier.

**Remove members that exist only for the old view:**
- **`StoryEditor.cs`:**
  - verse-control builders (`InitVerseControls`, `CreateVerseBtControl`, `AddDropTargetToFlowLayout`, drop-target handlers and light/dim helpers)
  - the `CtrlTextBox`-based selection, copy and paste helpers (`GetSelectedLanguageTextNetCtrls` and the like), the `NavigateTo(…, CtrlTextBox)` overload, and `AddNoteAbout`
  - the `splitContainerLeftRight` Panel1/Panel2 `SizeChanged` handlers and their wiring
  - the `CheckBiblePaneCursorPositionMouseMove` wrapper (the HTML pane calls `CheckBiblePaneCursorPosition()` directly)
  - `InitStoryBtPaneControl`, the menu click handler, the `UsingHtmlForStoryBtPane` property
  - the `CtrlTextBox._inTextBox` / `_nLastVerse` resets that currently run even in HTML mode
- **`StringTransfer`:** `_tb`, `SetAssociation(CtrlTextBox)`, `TextBox`, plus the code that uses `_tb`.
- **`VerseData`:** `ExistingTextBox`.
- **`TextPaster`:** only the `CtrlTextBox` overloads and branches.
- **`SearchForm`:** only its `CtrlTextBox` paths. The `HtmlPane` / `HtmlConNoteControl` paths stay.
- **Callers:** `CheckEndOfStateTransition.ShowErrorFocus(StoryEditor, CtrlTextBox, string)` and its old-view callers.

Rule: a member that *might* have another caller (e.g. `IsTestQuestionBox`, `IsRetellingBox`,
`IsTqAnswerBox`, `GetTesterId`, `Panel1_Width`, `Panel2_Width`, `DoMove`, `DoPasteVerse`) is
deleted only if a search shows no remaining live caller. The overloads the HTML pane uses
(`GetTqAnswerData(answers, string)`, `ChangeAnswerBoxUns(tq, answers, answer)`) stay.
`ReInitVerseControls` stays (~18 callers) with only its HTML body.

**Move constants first.** `HtmlStoryBtControl` uses `VerseBtControl.CstrMenuLabelHide` /
`CstrMenuLabelUnhide` and `AnchorControl.CstrNullAnchor`. Move them, with their `Localizer.Str`
strings unchanged, into `HtmlStoryBtControl` before deleting the classes. Also delete the
commented-out `VerseEditorForm` block in `HtmlStoryBtControl`.

**Designer (`StoryEditor.Designer.cs`):**
- remove `advancedUseOldStyleStoryBtPaneMenu` (creation, add to the Advanced menu, configuration, field)
- remove `flowLayoutPanelVerses` (creation, add to its parent, configuration including the MouseMove wiring, field)
- remove the `ConNoteFlowLayoutPanel` class
- remove the Panel1/Panel2 `SizeChanged` wiring

**Search (user decision):** hide the Edit menu's Find, Find Next and Replace items (they already do
nothing in the HTML view). Keep their handlers' HTML-safe shells and `SearchForm` with its HTML-pane
code, so search can be re-enabled later (ideally in sub-project C). "Hidden" means `Visible = false`
in the designer, with a comment saying why.

**Settings:** remove `UsingHtmlForStoryBtPane` from `Properties/Settings.settings`,
`Properties/Settings.Designer.cs` and `app.config`. Users who had the old view on simply get the
HTML view. .NET ignores the leftover `user.config` entry and drops it on the next save. No migration.

**Localization:** remove the `OneStoryProjectEditor.StoryEditor.advancedUseOldStyleStoryBtPaneMenu.*`,
`OneStoryProjectEditor.VerseBtControl.*` and `OneStoryProjectEditor.AnchorControl.*` entries from
`LocData/*.xml`. Keep the entries of any string that moved into `HtmlStoryBtControl` that is looked up
by key. The moved constants use `Localizer.Str("…")` with the same text, so lookups by text keep working.

## Out of scope

- Re-enabling search for the HTML panes, and fixing the notes' broken Ctrl+F (`DoFind` called with no argument).
- Any change to HTML-pane behaviour, the JS, or the data model.
- Unrelated `#if` symbols (`GoBackToOriginal`, `UseAutoUpgrade`, `UsingHtmlDisplayForStoryBt` in the csproj, …).

## Approach

Staged, with the build green after each stage, so a break is easy to locate:

1. Move the constants and delete the commented-out block.
2. Flatten the switch and the `#if`s.
3. Delete the now-unreachable members.
4. Delete the files and edit the designer (including hiding the search menu items).
5. Remove the setting and the translation entries.

## Testing

- After every stage: the solution builds (Debug|x86) and the 69 tests pass.
- No new unit tests: this is pure removal of UI code.
- **Manual smoke test (the user, in the dev build):**
  - load a story; edit vernacular, BT and retelling boxes; save and reopen
  - copy and paste (Edit menu and context menu)
  - add a consultant note and a coach note, including "create note" from selections
  - hide/unhide a line (menu labels show "Hide line"/"Unhide line")
  - anchor actions, including the null anchor
  - a state transition whose check focuses an offending field
  - the Advanced menu has no "old style" item
  - the Edit menu has no Find/Find Next/Replace
  - switching languages (localization) still works
