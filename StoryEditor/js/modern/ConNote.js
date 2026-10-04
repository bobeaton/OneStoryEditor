// modern/ConNote.js: the Consultant/Coach note pages on WebView2. Same messages as js/ConNoteDomPrefix.js (the IE one).
//  Inlined after modern/bridge.js and modern/PaneCommon.js (links, buttons, keys, drops, scrolling, edits).
(function () {
    'use strict';
    function legacyButton(b) { return (b === 0) ? 1 : (b === 2) ? 2 : 4; }    // the values C# (and IE) use
    function noteBox(el) { return ose.closest(el, function (x) { return x.getAttribute('data-note') === 'edit'; }); }

    // Ctrl+B / Ctrl+I in the note box wrap the selection in $...$ / *...*
    document.addEventListener('keydown', function (e) {
        var ta = noteBox(e.target);
        if (!ta || !e.ctrlKey)
            return;
        var mark = (e.keyCode === 66) ? '$' : (e.keyCode === 73) ? '*' : null;
        if (!mark)
            return;
        e.preventDefault();
        var sel = ta.value.substring(ta.selectionStart, ta.selectionEnd).trim();
        if (!sel)
            return;
        var bWrapped = (sel.lastIndexOf(mark, 0) === 0) && (sel.indexOf(mark, sel.length - mark.length) !== -1);
        if (!bWrapped) {
            ta.setRangeText(mark + sel + mark + ' ', ta.selectionStart, ta.selectionEnd, 'end');
            ta.dispatchEvent(new Event('input', { bubbles: true }));    // so PaneCommon sends the new text
        }
    });

    document.addEventListener('mousedown', function (e) {
        var ta = e.target;
        if (ta && ta.nodeName === 'TEXTAREA')
            ose.send('textareaMouseDown', { id: ta.id, value: ta.value, button: legacyButton(e.button) });
    });

    // right-click anywhere asks C# for the note context menu
    document.addEventListener('contextmenu', function (e) {
        e.preventDefault();
        ose.send('contextMenu', {});
    });

    // the selection in a read-only part of the pane, and the line it's on, for "Add note on selected text"
    ose.on('getNoteSelection', function () {
        var sel = window.getSelection();
        if (!sel || sel.isCollapsed || !sel.rangeCount)
            return {};
        var range = sel.getRangeAt(0);
        var el = range.commonAncestorContainer;
        if (el.nodeType !== 1)
            el = el.parentNode;
        var re = /^tp_(\d+)_/;
        for (var x = el; x && x.nodeType === 1; x = x.parentNode) {
            var match = re.exec(x.id || '');
            if (!match) {
                var inner = x.querySelector('[id^="tp_"]');
                match = inner ? re.exec(inner.id) : null;
            }
            if (match) {
                var div = document.createElement('div');
                div.appendChild(range.cloneContents());
                return { lineIndex: parseInt(match[1], 10), html: div.innerHTML };
            }
        }
        return {};
    });
})();
