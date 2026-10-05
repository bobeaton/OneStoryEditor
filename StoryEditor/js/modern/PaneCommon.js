// modern/PaneCommon.js: what the Story/BT and note pages share, on WebView2. Same messages and commands as
//  js/PaneCommon.js (the IE one), with standard DOM APIs. Inlined after modern/bridge.js and before the page's own
//  script, which may replace a handler with ose.on (the last registration wins).
(function () {
    'use strict';
    var lastTextareaId = null;

    function isTextarea(el) { return !!el && el.nodeName === 'TEXTAREA'; }
    function textarea(id) { var el = id ? document.getElementById(id) : null; return isTextarea(el) ? el : null; }

    document.addEventListener('focusin', function (e) {
        if (isTextarea(e.target))
            lastTextareaId = e.target.id;
    });

    // boxes grow to fit their text, as they did in IE's quirks mode (overflow:visible). The page's style
    //  (PageScriptSet.CstrModernStyle) has field-sizing:content for that; this does it where the runtime doesn't, and
    //  then lets the page re-lay out anything drawn over the boxes (ose.afterAutoSize: StoryBt's mirrors)
    function borders(ta) {
        var cs = window.getComputedStyle(ta);
        return (parseFloat(cs.borderTopWidth) || 0) + (parseFloat(cs.borderBottomWidth) || 0);
    }
    function sizeBoxes(list) {
        var shown = [], i;
        for (i = 0; i < list.length; i++)
            if (list[i].offsetParent !== null)      // (a hidden box has no height to fit)
                shown.push(list[i]);
        if (!shown.length)
            return;
        var x = window.pageXOffset, y = window.pageYOffset, heights = [];
        for (i = 0; i < shown.length; i++)
            shown[i].style.height = 'auto';
        for (i = 0; i < shown.length; i++)          // (one layout for all the reads, then all the writes)
            heights.push(shown[i].scrollHeight + borders(shown[i]));
        for (i = 0; i < shown.length; i++)
            shown[i].style.height = heights[i] + 'px';
        if ((window.pageXOffset !== x) || (window.pageYOffset !== y))
            window.scrollTo(x, y);
        if (typeof ose.afterAutoSize === 'function')
            ose.afterAutoSize();
    }
    function autoSize(ta) {
        if (isTextarea(ta))
            sizeBoxes([ta]);
    }
    function autoSizeAll() {
        sizeBoxes(Array.prototype.slice.call(document.getElementsByTagName('textarea')));
    }
    var resizePending = false;
    window.addEventListener('resize', function () {
        if (resizePending)
            return;
        resizePending = true;
        window.requestAnimationFrame(function () {
            resizePending = false;
            autoSizeAll();
        });
    });
    document.addEventListener('DOMContentLoaded', autoSizeAll);
    window.addEventListener('load', function () {
        autoSizeAll();
        if (document.fonts && document.fonts.ready)
            document.fonts.ready.then(autoSizeAll);
    });

    // edits (typing, paste, cut, drop) -- the box's plain value
    document.addEventListener('input', function (e) {
        var ta = e.target;
        if (!isTextarea(ta))
            return;
        autoSize(ta);
        if (!ta.readOnly)
            ose.send('textChanged', { id: ta.id, value: ta.value });
    });

    // never let a click navigate the pane away; the links we make are sent to C# instead
    document.addEventListener('click', function (e) {
        var link = ose.closest(e.target, function (el) { return el.nodeName === 'A'; });
        if (link) {
            e.preventDefault();
            var href = link.getAttribute('href') || '';
            if (href === 'bibleViewer.setReference')
                ose.send('bibRefJump', { ref: link.getAttribute('name') });
            else if (href === 'conNote.jumpToLine')
                ose.send('verseLineJump', { index: link.getAttribute('name') });
            else if (/^https?:/i.test(href))
                ose.send('openUrl', { url: href });
            return;
        }
        var btn = ose.closest(e.target, function (el) { return !!el.getAttribute('data-action'); });
        if (btn) {
            e.preventDefault();
            ose.send('action', { name: btn.getAttribute('data-action'), id: btn.id, arg: btn.getAttribute('data-arg') });
        }
    });

    document.addEventListener('keydown', function (e) {
        if (e.ctrlKey && (e.keyCode === 83)) {          // Ctrl+S
            e.preventDefault();
            ose.send('save');
        }
        else if (e.keyCode === 116) {                   // F5 (Ctrl+F5 also realigns the lines)
            e.preventDefault();
            ose.send(e.ctrlKey ? 'realign' : 'reload');
        }
        else if (e.ctrlKey && ((e.keyCode === 82) || (e.keyCode === 80))) {
            e.preventDefault();                         // the browser's own reload (Ctrl+R) and print (Ctrl+P)
        }
    });

    // a reference dragged from the Bible pane onto an anchor cell or a note box
    function dropTarget(el) {
        return ose.closest(el, function (x) { return x.getAttribute('data-drop') === 'scripture'; });
    }
    // anything else dragged in from outside this page (e.g. a verse dropped on a story box) mustn't be typed into a
    //  box, as IE never did; text dragged within the page (from one box to another) still moves as usual
    var dragFromThisPage = false;
    document.addEventListener('dragstart', function () { dragFromThisPage = true; });
    document.addEventListener('dragend', function () { dragFromThisPage = false; });
    function refuseOutsideDrop(e) {
        if (dragFromThisPage || !isTextarea(e.target))
            return false;
        e.preventDefault();
        if (e.dataTransfer)
            e.dataTransfer.dropEffect = 'none';
        return true;
    }
    document.addEventListener('dragover', function (e) {
        if (dropTarget(e.target)) {
            e.preventDefault();
            if (e.dataTransfer)
                e.dataTransfer.dropEffect = 'copy';
        }
        else
            refuseOutsideDrop(e);
    });
    document.addEventListener('drop', function (e) {
        var target = dropTarget(e.target);
        if (!target) {
            refuseOutsideDrop(e);
            dragFromThisPage = false;
            return;
        }
        e.preventDefault();
        dragFromThisPage = false;
        ose.send('scriptureDropped', { id: target.id });
    });

    // which line is at the top (same rule as the IE page)
    function topOf(el) {
        var y = 0;
        while (el) {
            y += el.offsetTop;
            el = el.offsetParent;
        }
        return y;
    }
    var scrollTimer = null;
    function reportScroll() {
        scrollTimer = null;
        var scrollTop = window.pageYOffset || document.documentElement.scrollTop;
        var cells = document.getElementsByTagName('td');
        var top = null, line = null;
        for (var i = 0; i < cells.length; i++) {
            var td = cells[i];
            if (!td.id)
                continue;
            if (topOf(td) > scrollTop + 1)
                break;
            top = td;
            if (td.id.indexOf('ln_') === 0)
                line = td;
        }
        var msg = { topId: top ? top.id : null, topLabel: line ? line.innerText : null, prevId: null, nextId: null };
        if (line) {
            var n = parseInt(line.id.substr(3), 10);
            if (document.getElementById('ln_' + (n - 1)))
                msg.prevId = 'ln_' + (n - 1);
            if (document.getElementById('ln_' + (n + 1)))
                msg.nextId = 'ln_' + (n + 1);
        }
        ose.send('scrolled', msg);
    }
    window.addEventListener('scroll', function () {
        if (!scrollTimer)
            scrollTimer = setTimeout(reportScroll, 50);
    });
    window.addEventListener('load', reportScroll);

    // commands from C#
    ose.on('setText', function (m) {
        var ta = textarea(m.id);
        if (!ta)
            return;
        ta.value = m.text;
        autoSize(ta);
        ose.send('textChanged', { id: ta.id, value: ta.value });
    });

    ose.on('appendText', function (m) {
        var ta = textarea(m.id);
        if (!ta)
            return;
        ta.value += m.text;
        autoSize(ta);
        ose.send('textChanged', { id: ta.id, value: ta.value });
        if (m.focus)
            ta.focus();
    });

    ose.on('appendHtml', function (m) {
        var el = document.getElementById(m.id);
        if (el) {
            el.insertAdjacentHTML('beforeend', m.html);
            autoSizeAll();      // (the new HTML may have boxes, or change the width of some)
        }
    });

    ose.on('setHtml', function (m) {
        var el = document.getElementById(m.id);
        if (el) {
            el.innerHTML = m.html;
            autoSizeAll();
        }
    });

    ose.on('hasElement', function (m) {
        return { found: !!document.getElementById(m.id) };
    });

    ose.on('selectRange', function (m) {
        var ta = textarea(m.id);
        if (!ta)
            return;
        ta.focus();
        ta.setSelectionRange(m.start, m.start + m.length);
    });

    ose.on('clearSelection', function () {
        var ta = textarea(lastTextareaId);
        if (ta)
            ta.setSelectionRange(ta.selectionEnd, ta.selectionEnd);
        var sel = window.getSelection();
        if (sel)
            sel.removeAllRanges();
    });

    // the selection C# asks about is in the box it names (a box keeps its selection when focus moves to the app, e.g.
    //  the Find dialog): none if that box has none (as IE, whose selection was never in the named box then). With no
    //  box named: the last one focused, or else the page's selection
    ose.on('getSelection', function (m) {
        var bNamed = !!(m && m.id);
        var ta = bNamed ? textarea(m.id) : textarea(lastTextareaId);
        if (ta && (ta.selectionEnd > ta.selectionStart))
            return { selType: 'text', text: ta.value.substring(ta.selectionStart, ta.selectionEnd) };
        if (bNamed)
            return { selType: 'none', text: '' };
        var sel = window.getSelection();
        if (sel && !sel.isCollapsed)
            return { selType: 'text', text: sel.toString() };
        return { selType: 'none', text: '' };
    });

    // replaces box 'id''s selection with text; returns the new end point (0 = failed, e.g. the box has no selection)
    function replaceSelectionIn(id, text) {
        var ta = textarea(id);
        if (!ta || !(ta.selectionEnd > ta.selectionStart))
            return 0;
        ta.setRangeText(text, ta.selectionStart, ta.selectionEnd, 'end');
        autoSize(ta);
        return ta.selectionEnd;
    }

    ose.on('replaceSelection', function (m) {
        var nEndPoint = replaceSelectionIn(m.id, m.text);
        var ta = textarea(m.id);
        return { endPoint: nEndPoint, value: ta ? ta.value : null };
    });

    // send the last-focused box's text (it may have changed without an input event, e.g. set by script)
    ose.on('flush', function () {
        var ta = textarea(lastTextareaId);
        if (ta && !ta.readOnly)
            ose.send('textChanged', { id: ta.id, value: ta.value, quiet: true });
        return {};
    });

    ose.lastTextareaId = function () { return lastTextareaId; };
    ose.replaceSelectionIn = replaceSelectionIn;
    ose.autoSize = autoSize;
    ose.autoSizeAll = autoSizeAll;
})();
