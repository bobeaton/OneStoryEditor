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

    // edits (typing, paste, cut, drop) -- the box's plain value
    document.addEventListener('input', function (e) {
        var ta = e.target;
        if (isTextarea(ta) && !ta.readOnly)
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
    document.addEventListener('dragover', function (e) {
        if (dropTarget(e.target)) {
            e.preventDefault();
            if (e.dataTransfer)
                e.dataTransfer.dropEffect = 'copy';
        }
    });
    document.addEventListener('drop', function (e) {
        var target = dropTarget(e.target);
        if (!target)
            return;
        e.preventDefault();
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
        ose.send('textChanged', { id: ta.id, value: ta.value });
    });

    ose.on('appendText', function (m) {
        var ta = textarea(m.id);
        if (!ta)
            return;
        ta.value += m.text;
        ose.send('textChanged', { id: ta.id, value: ta.value });
        if (m.focus)
            ta.focus();
    });

    ose.on('appendHtml', function (m) {
        var el = document.getElementById(m.id);
        if (el)
            el.insertAdjacentHTML('beforeend', m.html);
    });

    ose.on('setHtml', function (m) {
        var el = document.getElementById(m.id);
        if (el)
            el.innerHTML = m.html;
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

    // the selection C# asks about is in the box it names (or the last one focused); a box keeps its selection when
    //  focus moves to the app (e.g. the Find dialog)
    ose.on('getSelection', function (m) {
        var ta = textarea(m && m.id) || textarea(lastTextareaId);
        if (ta && (ta.selectionEnd > ta.selectionStart))
            return { selType: 'text', text: ta.value.substring(ta.selectionStart, ta.selectionEnd) };
        var sel = window.getSelection();
        if (sel && !sel.isCollapsed)
            return { selType: 'text', text: sel.toString() };
        return { selType: 'none', text: '' };
    });

    // replaces box 'id''s selection with text; returns the new end point (0 = failed) and the box's whole value
    function replaceSelectionIn(id, text) {
        var ta = textarea(id);
        if (!ta)
            return 0;
        ta.setRangeText(text, ta.selectionStart, ta.selectionEnd, 'end');
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
})();
