# Remove the Old .NET Text-Box View (Sub-project R) Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Delete StoryEditor's unused WinForms text-box Story/BT view and everything that exists only for it, leaving the HTML panes' behaviour unchanged.

**Architecture:** This is a staged removal, and the build stays green after every task:
1. Relocate the two constants the HTML pane borrows.
2. Flatten the view switch to its HTML side.
3. Delete the old-view class files, and let the compiler list every dependent member to remove.
4. Hide the search menu items, which do nothing.
5. Remove the setting and the translation entries.

**Tech Stack:** C# / .NET Framework 4.8 (SDK-style csproj with globbing, x86), WinForms + IE WebBrowser, NUnit (`StoryEditor.Tests`, 69 tests).

**Spec:** `docs/superpowers/specs/2026-10-03-remove-old-dotnet-view-design.md`

## Global Constraints

- Branch `DecoupleWebBrowser`, on top of sub-project A. Commit after every task. A commit trailer naming the model that wrote the commit is fine, e.g. `Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>`.
- **Pure removal.** HTML-pane behaviour, JS, data model, file format, `RobustFile` usage and the save sequence must not change. The only allowed behaviour changes are the ones this plan names: the two state-transition checks in Task 3 now also scroll to the line, and Find/Find Next/Replace are hidden in Task 4.
- **Keep:** `DynamicTableLayoutPanel`, `TextPaster`, `SearchForm` (and its `HtmlPane`/`HtmlConNoteControl` code paths), `ReInitVerseControls` (HTML body only), `GetTqAnswerData(answers, string)`, `ChangeAnswerBoxUns(tq, answers, answer)`.
- **Deletion rule:** delete a member only if a search (Grep across `StoryEditor\*.cs`, `StoryEditor\js\*.js` and `StoryEditor\Properties\Resources.resx`) shows no remaining live caller. If in doubt, keep it and note it in the report.
- Unrelated `#if` symbols (`GoBackToOriginal`, `UseAutoUpgrade`, `DataDllBuild`, the csproj's `UsingHtmlDisplayForStoryBt`) stay as they are.
- Edit files with the Write/Edit tools, not shell heredocs/sed. In this environment, escape sequences have been turned into literal characters before. Use Bash only for git, MSBuild and vstest.
- `StoryEditor.cs` (~8,500 lines) and `StoryEditor.Designer.cs` are large. Use Grep with line numbers and read only the regions you edit. Line numbers below are from commit `c63fa06` and will drift as you edit, so locate code by the quoted text or method name.
- Build and test (Git Bash):
  ```bash
  MSBUILD="/c/Program Files/Microsoft Visual Studio/18/Insiders/MSBuild/Current/Bin/MSBuild.exe"
  VSTEST="/c/Program Files/Microsoft Visual Studio/18/Insiders/Common7/IDE/Extensions/TestPlatform/vstest.console.exe"
  "$MSBUILD" "StoryEditor 2017.sln" -t:StoryEditor_Tests -restore -p:Configuration=Debug -p:Platform=x86 -v:m -nologo
  "$VSTEST" StoryEditor.Tests/bin/x86/Debug/StoryEditor.Tests.dll /Platform:x86
  ```
  "Green" means zero build errors and the suite passes (69 tests: 68 run plus 1 skipped Explicit corpus test, or as many as the runner reports for that dll, with zero failed). Building the csproj on its own fails on `$(SolutionDir)`; always build through the solution.
- **No new unit tests.** This is removal of UI code with no test seam. Verification is the green build and suite after each task, plus the manual smoke test in Task 5.

## Review Focus

These are UI-level risks with no automated test seam. Each one is a manual smoke-test item in Task 5, and the per-task reviewer checks the code path it names:

1. **A user whose `user.config` has `UsingHtmlForStoryBtPane=False`** must still start straight into the HTML pane without an error. (Task 2 removes the only reader; Task 5 removes the setting.)
2. **Edit-menu Copy and Paste in the Story/BT pane** must still work: `editCopySelection`, `pasteToolStripMenuItem_Click` and the Edit-menu enabling logic, flattened in Task 2.
3. **Hide/unhide a line** must still show the localized "&Hide line"/"&Unhide line" labels, and adding a null anchor must still work. These use the constants moved in Task 1.
4. **The state-transition checks for a missing back-translation, English BT and Free translation** must show the error and scroll to the line (Task 3).
5. **Switching the UI language** must still reset the HTML pane's context menu (`OnLocalizationChange`, flattened in Task 2).

---

### Task 1: Move borrowed constants into `HtmlStoryBtControl`; drop the commented-out `VerseEditorForm` block

**Files:**
- Modify: `StoryEditor/HtmlStoryBtControl.cs` (uses at ~:1726 and ~:1764; the comment block at ~:413-428)

**Interfaces:**
- Produces: `public static string HtmlStoryBtControl.CstrMenuLabelHide`, `public static string HtmlStoryBtControl.CstrMenuLabelUnhide`, `public const string HtmlStoryBtControl.CstrNullAnchor`. Task 3 deletes the originals in `VerseBtControl`/`AnchorControl`.

- [ ] **Step 1: Add the members to `HtmlStoryBtControl`**

Put them near the top of the class, next to the other constants or fields. The text is unchanged from `VerseBtControl.cs:329-337` and `AnchorControl.cs:268`, so localization lookups by text keep working:
```csharp
        public const string CstrNullAnchor = "No Anchor";

        public static string CstrMenuLabelHide
        {
            get { return Localizer.Str("&Hide line"); }
        }

        public static string CstrMenuLabelUnhide
        {
            get { return Localizer.Str("&Unhide line"); }
        }
```
(Check that `HtmlStoryBtControl.cs` already has `using NetLoc;` for `Localizer`. Add it if not.)

- [ ] **Step 2: Point the two uses at them**

```csharp
            verseData.Anchors.AddAnchorData(CstrNullAnchor, CstrNullAnchor);
```
```csharp
            hideVerseToolStripMenuItem.Text = (verseData.IsVisible) ? CstrMenuLabelHide : CstrMenuLabelUnhide;
```

- [ ] **Step 3: Delete the commented-out block** in `OnLineOptionsButton`. It runs from the `/*` line before `HtmlElement elem;` through the `*/` after `contextMenuStrip.Show(MousePosition);`, and contains `TheSE.CreateVerseBtControl` and `new VerseEditorForm(verseNetCtrl)`. Keep the `return true;` that follows. The block contains a nested `/*` that the single closing `*/` ends, so remove all of it.

- [ ] **Step 4: Build and run the tests.** Expected: green.

- [ ] **Step 5: Commit**
```bash
git add StoryEditor/HtmlStoryBtControl.cs
git commit -m "Move hide/unhide and null-anchor constants into HtmlStoryBtControl; drop dead VerseEditorForm block"
```

---

### Task 2: Flatten the view switch and the `UsingHtmlDisplayForConNotes` conditionals

**Files:**
- Modify: `StoryEditor/StoryEditor.cs`, `StoryEditor/StoryEditor.Designer.cs`, `StoryEditor/CheckEndOfStateTransition.cs`, `StoryEditor/ProjectFacilitatorRequirementsCheck.cs`, `StoryEditor/HtmlConNoteControl.cs`

**Interfaces:**
- Consumes: nothing from Task 1.
- Produces: no `UsingHtmlForStoryBtPane` member, no `advancedUseOldStyleStoryBtPaneMenu`, no `InitStoryBtPaneControl`, and no `UsingHtmlDisplayForConNotes` symbol remain. The old-view classes still exist; Task 3 deletes them.

**Method.** Delete the property, then let the compiler list every site. Fix each one by keeping the HTML (true) side.

- [ ] **Step 1: Remove the `UsingHtmlDisplayForConNotes` conditionals**

1. Delete the line `#define UsingHtmlDisplayForConNotes` at the top of both `StoryEditor.cs` and `StoryEditor.Designer.cs`. In `StoryEditor.cs` the next line, `#define GoBackToOriginal`, stays.
2. For every `#if UsingHtmlDisplayForConNotes … #else … #endif` in those two files:
   - keep the `#if` body
   - delete the `#else` body and all three directive lines
   - for a block with an empty `#if` body, delete the whole block
3. The known blocks, in `StoryEditor.cs` at about :1642, 1671, 1706, 1850-1969, 2056, 2072, 2426-2466, 2473-2513, 2859 and 3112-3133, and in `StoryEditor.Designer.cs` at about :2288-2312 (the `ConNoteFlowLayoutPanel` class). Confirm there are none left:
   ```bash
   grep -n "UsingHtmlDisplayForConNotes" StoryEditor/*.cs
   ```
   Expected: no output.
4. The block at ~3112-3133 leaves `splitContainerLeftRight_Panel2_SizeChanged` with an empty body. Delete that method and its wiring in `StoryEditor.Designer.cs`: the line `this.splitContainerLeftRight.Panel2.SizeChanged += …splitContainerLeftRight_Panel2_SizeChanged…`.

Example of the transformation:
```csharp
// before
#if UsingHtmlDisplayForConNotes
            htmlConsultantNotesControl.ScrollToVerse(nVerseIndex);
#else
            ...flow-panel code...
#endif
// after
            htmlConsultantNotesControl.ScrollToVerse(nVerseIndex);
```

- [ ] **Step 2: Remove the toggle itself**

In `StoryEditor.cs`:
- Delete the property `public bool UsingHtmlForStoryBtPane { get { return !advancedUseOldStyleStoryBtPaneMenu.Checked; } set { … } }` (~:1486-1490).
- Delete `InitStoryBtPaneControl(...)` (~:7385-7398) and the `advancedUseOldStyleStoryBtPaneMenu_Click` handler (~:7400-7414).
- In the constructor (~:271), replace `InitStoryBtPaneControl(Settings.Default.UsingHtmlForStoryBtPane);` with the HTML side of what that method did, which is just:
  ```csharp
              htmlStoryBtControl.Visible = true;
  ```
  Read `InitStoryBtPaneControl` before deleting it, and carry over exactly the lines its `true` branch executes for `htmlStoryBtControl`. Do **not** carry over anything for `flowLayoutPanelVerses`; Task 3 removes that panel.

In `StoryEditor.Designer.cs`, remove the menu item `advancedUseOldStyleStoryBtPaneMenu`:
- its creation (`this.advancedUseOldStyleStoryBtPaneMenu = new …`)
- its entry in the Advanced menu's `DropDownItems.AddRange(...)` list
- its configuration block (Name, Size, Text, ToolTipText, Click)
- its field declaration

- [ ] **Step 3: Build, then flatten every remaining site**

Build. Every `UsingHtmlForStoryBtPane` reference now fails to compile; there are about 38 in `StoryEditor.cs` plus the files listed above. Fix each by keeping the true side:
```csharp
// if/else  →  keep the if-body
if (UsingHtmlForStoryBtPane) A; else B;           →  A;
// negated  →  keep the else-body (or nothing)
if (!UsingHtmlForStoryBtPane) B;                   →  (delete)
if (!UsingHtmlForStoryBtPane) B; else A;           →  A;
// early return  →  the method body after it is old-view-only
if (UsingHtmlForStoryBtPane) return; <rest>        →  (empty body; see the Find/Replace note)
// conditional expression
x = UsingHtmlForStoryBtPane ? a : b;               →  x = a;
// compound condition: substitute true and simplify
```

Notes on specific sites (from the inventory; verify each against the code):
- `ClearState` (~:885) also contains `CtrlTextBox._inTextBox = null;` and `LoadStory` (~:1568) `CtrlTextBox._nLastVerse = -1;`. These run in HTML mode too. **Leave them for Task 3**; they go when `CtrlTextBox` is deleted.
- `InitAllPanes` (~:1624-1727): keep the `htmlStoryBtControl` setup and `htmlStoryBtControl.LoadDocument()`. Drop the `flowLayoutPanelVerses` suspend/resume and the `InitVerseControls` / `AddDropTargetToFlowLayout` / `FocusOnVerse` / `stLast.TextBox.Focus()` branch.
- `InitializeTransliterators` (~:1742-1775) and `CurrentViewSettings` (~:5189-5200): keep the `HtmlStoryBtControl.Transliterator*` side.
- `ReInitVerseControls` (~:1800-1852): keep the HTML body (reload the document, scroll to `LastTextareaInFocusId`). The method itself stays.
- `FocusOnVerse` (~:2030-2052), `ClearFlowControls` (~:2849-2856), `TriggerSaveUpdates` (~:2904), `OnLocalizationChange` (~:7308), `SetViewSettings` (~:5137), `GetSelectedLanguageText` (~:6530): keep the HTML side.
- Edit-menu enabling (~:3871-3913), `editCopySelection` (~:5450), `pasteToolStripMenuItem_Click` (~:5463-5490): keep the HTML side exactly. These are **Review Focus #2**.
- `splitContainerLeftRight_Panel1_SizeChanged` (~:3138-3149): its whole body is the old-view branch. Delete the method and its wiring (`this.splitContainerLeftRight.Panel1.SizeChanged += …`) in the Designer.
- `editFindToolStripMenuItem_Click`, `findNextToolStripMenuItem_Click`, `replaceToolStripMenuItem_Click` (~:5870-5925): after flattening these become empty, because only the old view did anything. **Keep the empty handlers**, each with a one-line comment: `// search isn't wired to the HTML panes yet (menu item hidden; see sub-project C)`. Keep `LaunchSearchForm` and `LaunchReplaceForm`, which `HtmlConNoteControl` and `SearchForm` use. Task 4 hides the menu items.
- `CheckEndOfStateTransition.cs` (~:193, 764, 778, 1426) and `ProjectFacilitatorRequirementsCheck.cs` (~:403): keep the `ShowErrorFocus(theSE, nVerseNumber, <field>, msg)` side.
- `HtmlConNoteControl.DoFind` (~:517) starts with `if (TheSE.UsingHtmlForStoryBtPane) return;`. Substituting true makes the whole method a no-op. Replace the body with just `return;`, preceded by the same one-line search comment, and keep the method: JS calls `window.external.DoFind`.

Repeat build → fix until the build is green. Then confirm:
```bash
grep -n "UsingHtmlForStoryBtPane\|advancedUseOldStyleStoryBtPaneMenu\|InitStoryBtPaneControl" StoryEditor/*.cs
```
Expected: no output. `Settings.Designer.cs` may still define the setting property; Task 5 removes it.

- [ ] **Step 4: Run the tests.** Expected: green.

- [ ] **Step 5: Commit**
```bash
git add StoryEditor/StoryEditor.cs StoryEditor/StoryEditor.Designer.cs StoryEditor/CheckEndOfStateTransition.cs StoryEditor/ProjectFacilitatorRequirementsCheck.cs StoryEditor/HtmlConNoteControl.cs
git commit -m "Flatten the old-view switch and UsingHtmlDisplayForConNotes conditionals to the HTML side"
```

---

### Task 3: Delete the old-view classes and everything that depended on them

**Files:**
- Delete:
  - `StoryEditor/CtrlTextBox.cs`
  - `StoryEditor/VerseBtControl.cs`, `.Designer.cs`, `.resx`
  - `StoryEditor/StoryLineControl.cs`, `.Designer.cs`, `.resx`
  - `StoryEditor/TestingQuestionControl.cs`, `.Designer.cs`, `.resx`
  - `StoryEditor/MultiLineControl.cs`, `.Designer.cs`, `.resx`
  - `StoryEditor/LineFlowLayoutPanel.cs`, `.Designer.cs`, `.resx`
  - `StoryEditor/ResizableControl.cs`, `.Designer.cs`, `.resx`
  - `StoryEditor/AnchorControl.cs`, `.Designer.cs`, `.resx`
  - `StoryEditor/VerseEditorForm.cs`, `.Designer.cs`, `.resx`

  Use `git rm` and delete only files that exist; list what you deleted.
- Modify: `StoryEditor/StoryEditor.cs`, `StoryEditor/StoryEditor.Designer.cs`, `StoryEditor/StringTransfer.cs`, `StoryEditor/VerseData.cs`, `StoryEditor/TextPaster.cs`, `StoryEditor/SearchForm.cs`, `StoryEditor/CheckEndOfStateTransition.cs`

**Interfaces:**
- Consumes: Task 1's `HtmlStoryBtControl.CstrMenuLabelHide`/`CstrMenuLabelUnhide`/`CstrNullAnchor`, and Task 2's flattened code (there are no `UsingHtmlForStoryBtPane` branches left to keep these classes alive).
- Produces: none of `CtrlTextBox`, `VerseBtControl`, `VerseControl`, `StoryLineControl`, `TestingQuestionControl`, `MultiLineControl`, `LineFlowLayoutPanel`, `VerseBtLineFlowLayoutPanel`, `ResizableControl`, `AnchorControl`, `VerseEditorForm` or `flowLayoutPanelVerses` exists.

**Method.** Delete the files, then build. Every compile error is old-view code; remove it.

- [ ] **Step 1: Delete the files** (list above) with `git rm`.

- [ ] **Step 2: Remove the designer pieces** in `StoryEditor.Designer.cs`:
  - `flowLayoutPanelVerses`: its creation, its `Controls.Add` into its parent, its configuration block (including `MouseMove += …CheckBiblePaneCursorPositionMouseMove`), any `SuspendLayout`/`ResumeLayout`/`PerformLayout` lines for it, and its field declaration.
  - Any remaining reference to deleted control types.

  Then delete `StoryEditor.CheckBiblePaneCursorPositionMouseMove` (the `MouseEventArgs` wrapper). **Keep** `CheckBiblePaneCursorPosition()`, which `HtmlStoryBtControl.OnMouseMove` calls.

- [ ] **Step 3: Build and remove dependent code until green**

These are the known sites from the inventory. Verify each.
- **`StoryEditor.cs`:**
  - `CtrlTextBox._inTextBox = null;` in `ClearState` and `CtrlTextBox._nLastVerse = -1;` in `LoadStory`: delete the lines.
  - Delete the old-view-only methods: `InitVerseControls`, `CreateVerseBtControl`, `AddDropTargetToFlowLayout`, `buttonDropTarget_DragDrop`, `buttonDropTarget_DragEnter`, `LightUpDropTargetButtons`, `DimDropTargetButtons`, `AddNoteAbout`, `IsInStoryLine`, `AddExtraInfoBasedOnLabel`, `GetRetellingData`, `GetTqAnswerData(string, VerseBtControl, out …)`, `GetTestQuestionData`, `GetTestQuestionDataFromAnswerLabel`, `ChangeAnswerBoxUns(string, VerseBtControl)`, `GetSelectedLanguageTextNetCtrls`, `GetSelectedDataFromLineData`, `GrabTrimSelectedText`, and the `NavigateTo(…, CtrlTextBox …)` overload. Each one only if, after the deletions, nothing live calls it. Keep the HTML-pane overloads named in Global Constraints.
- **`StringTransfer.cs`:** delete `protected CtrlTextBox _tb`, `SetAssociation(CtrlTextBox …)`, the `TextBox` property, and their `#if !DataDllBuild` wrappers where those wrap only these members. `ExtractSelectedText(out string)` reads `_tb`; its only caller chain is `LineData.ExtractSelectedText` (`VerseData.cs` ~:179) ← `StoryLineControl` (deleted). Delete both `ExtractSelectedText` methods if no other caller remains.
- **`VerseData.cs`:** delete `ExistingTextBox` (~:144-157) and `LineData.ExtractSelectedText` (see above).
- **`TextPaster.cs`:** delete only the `CtrlTextBox` overloads and branches: `SetTextBoxText`, `GetTextBoxText`, `TriggerPaste(bool, CtrlTextBox)`, and the `is CtrlTextBox` branch in the undo stack. Keep the `HtmlElement` versions.
- **`SearchForm.cs`:** delete only the `CtrlTextBox` paths: finding the starting box via `CtrlTextBox._inTextBox`, `CaptureNextStartingCharIndex(CtrlTextBox)`, `stringTransfer.TextBox as CtrlTextBox`, `ctbStopWhereWeStarted`, the replace into `_inTextBox.SelectedText`, and the `CtrlTextBox` part of `UpdateEnableReplaceButton`. Keep the `HtmlPane`/`HtmlConNoteControl` branches. If removing a `CtrlTextBox` branch leaves an `if/else` whose other side is the HTML path, keep that side unconditionally.
- **`CheckEndOfStateTransition.cs`:**
  - Delete `ShowErrorFocus(StoryEditor, CtrlTextBox, string)`.
  - Two **unconditional** callers of it remain, the English-BT and Free-translation checks at ~:308 and ~:410. In the HTML view they passed a null text box, so they only showed the error. Convert them to the same overload their sibling checks use, so they also scroll to the line (Review Focus #4):
    ```csharp
                                ShowErrorFocus(theSE, nVerseNumber, StoryEditor.TextFields.InternationalBt,
                                               String.Format(
                                                   "Error: Line {0} doesn't have any English back-translation in it. Did you forget it?",
                                                   nVerseNumber));
    ```
    ```csharp
                                ShowErrorFocus(theSE, nVerseNumber, StoryEditor.TextFields.FreeTranslation,
                                               String.Format(
                                                   "Error: Line {0} doesn't have any Free translation in it. Did you forget it?",
                                                   nVerseNumber));
    ```
    Check that `nVerseNumber` is the variable the sibling calls in the same method pass. If the method uses a differently named line-number variable, use that one.

Repeat build → fix until the build is green.

- [ ] **Step 4: Sweep for members left unused**

These may now have no live caller: `IsTestQuestionBox`, `IsRetellingBox`, `IsTqAnswerBox`, `GetTesterId`, `Panel1_Width`, `Panel2_Width`, `DoMove`, `DoPasteVerse`. Grep each name across `StoryEditor\*.cs`, `StoryEditor\js\*.js` and `StoryEditor\Properties\Resources.resx`. Delete it only if there are zero references besides its definition. List each one in the report as deleted or kept (with why). Then confirm:
```bash
grep -n "CtrlTextBox\|VerseBtControl\|StoryLineControl\|TestingQuestionControl\|MultiLineControl\|LineFlowLayoutPanel\|ResizableControl\|AnchorControl\|VerseEditorForm\|flowLayoutPanelVerses\|VerseControl\b" StoryEditor/*.cs
```
Expected: no output. Comments mentioning the names are fine to delete as well.

- [ ] **Step 5: Build and run the tests.** Expected: green.

- [ ] **Step 6: Commit**
```bash
git add -A StoryEditor
git commit -m "Delete the old .NET text-box view controls and the code that only they used"
```

---

### Task 4: Hide the Find / Find Next / Replace menu items

**Files:**
- Modify: `StoryEditor/StoryEditor.Designer.cs`

**Interfaces:**
- Consumes: Task 2's empty `editFindToolStripMenuItem_Click`, `findNextToolStripMenuItem_Click` and `replaceToolStripMenuItem_Click` handlers.

- [ ] **Step 1: Hide the three items**

Find them through their Click wiring:
```bash
grep -n "editFindToolStripMenuItem_Click\|findNextToolStripMenuItem_Click\|replaceToolStripMenuItem_Click" StoryEditor/StoryEditor.Designer.cs
```
The items are `editFindMenu`, `editReplaceMenu` and the Find Next item. In each item's configuration block, add:
```csharp
            // search isn't wired to the HTML panes yet; hidden until sub-project C (SearchForm is kept)
            this.editFindMenu.Visible = false;
```
(Use the matching field name for each.) If a menu separator now sits next to nothing visible at either end of the Edit menu, hide it the same way, and say so in the report. Also check `StoryEditor.cs` for code that sets these items' `Visible` or `Enabled` at runtime (Grep the three field names). If something re-enables or shows them, make it leave them hidden, and report it.

- [ ] **Step 2: Build and run the tests.** Expected: green.

- [ ] **Step 3: Commit**
```bash
git add StoryEditor/StoryEditor.Designer.cs StoryEditor/StoryEditor.cs
git commit -m "Hide Find/Find Next/Replace (not wired to the HTML panes); keep SearchForm for later"
```

---

### Task 5: Remove the setting and the translation entries; smoke-test checklist

**Files:**
- Modify: `StoryEditor/Properties/Settings.settings` (~:209-211), `StoryEditor/Properties/Settings.Designer.cs` (~:827-837), `StoryEditor/app.config` (~:182-184)
- Modify: `StoryEditor/LocData/es.xml`, `fr.xml`, `id.xml`, `pt.xml`

- [ ] **Step 1: Remove the `UsingHtmlForStoryBtPane` setting** from all three files. That means the `<Setting Name="UsingHtmlForStoryBtPane" …>` element, the generated property in `Settings.Designer.cs`, and the `<setting name="UsingHtmlForStoryBtPane" …>` element under `userSettings` in `app.config`. Don't touch other settings or the files' structure. A user's leftover value in `user.config` is ignored by .NET (Review Focus #1).

- [ ] **Step 2: Remove the translation entries**

In each `LocData/*.xml`, delete the `<Entry …>` elements whose id starts with:
- `OneStoryProjectEditor.StoryEditor.advancedUseOldStyleStoryBtPaneMenu.`
- `OneStoryProjectEditor.VerseBtControl.`
- `OneStoryProjectEditor.AnchorControl.`

Look at one entry first to learn the element/attribute format, and remove each whole element, including any child elements. **Keep** entries keyed by text (e.g. `&Hide line`, `&Unhide line`) and every `OneStoryProjectEditor.HtmlStoryBtControl.*` entry. Count the entries removed per file and report the counts. The inventory expects about 46 in es/fr/id and 42 in pt. Check that each file is still well-formed XML:
```bash
for f in StoryEditor/LocData/*.xml; do python -c "import sys,xml.dom.minidom; xml.dom.minidom.parse(sys.argv[1])" "$f" && echo "ok $f"; done
```
If Python isn't available, use PowerShell: `[xml](Get-Content $f -Raw)`.

- [ ] **Step 3: Build and run the tests.** Expected: green. Also confirm:
```bash
grep -rn "UsingHtmlForStoryBtPane\|advancedUseOldStyleStoryBtPaneMenu" StoryEditor --include=*.cs --include=*.settings --include=*.config --include=*.xml
```
Expected: no output.

- [ ] **Step 4: Commit**
```bash
git add StoryEditor/Properties/Settings.settings StoryEditor/Properties/Settings.Designer.cs StoryEditor/app.config StoryEditor/LocData
git commit -m "Remove the UsingHtmlForStoryBtPane setting and old-view translation entries"
```

- [ ] **Step 5: Manual smoke test (for the human, in the Debug build `output\Debug\StoryEditor.exe`, on a copy of a project).** Report results; don't just tick.
  1. Start the app. If you once had the old view on, it opens in the HTML view without an error.
  2. Load a story. Edit vernacular, BT and retelling boxes; save; reopen. The edits are there.
  3. Edit-menu Copy and Paste in a Story/BT box. Context-menu copy/paste.
  4. Add a consultant note and a coach note. Use "create note" from selections in two boxes.
  5. Hide/unhide a line. The menu shows "Hide line"/"Unhide line" (localized if the UI isn't English).
  6. Anchor actions, including adding a null anchor ("No Anchor").
  7. Try a state transition with a missing English BT or Free translation on a line. The error appears and the pane scrolls to that line.
  8. The Advanced menu has no "old style" item. The Edit menu has no Find/Find Next/Replace.
  9. Switch the UI language. The Story/BT context menu shows the new language.
