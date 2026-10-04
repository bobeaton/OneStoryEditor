// PaneCommon.js: what the Story/BT pane and the Consultant/Coach note panes have in common. Inlined after bridge.js
//  and before the pane's own script, which may replace any of these handlers with ose.on (the last one wins).
(function () {
    var lastTextareaId = null;

    function isTextarea(el) { return !!el && (el.nodeName == 'TEXTAREA'); }
    function textarea(id) { var el = id ? document.getElementById(id) : null; return isTextarea(el) ? el : null; }

    // focus doesn't bubble, but it can be captured
    document.addEventListener('focus', function (e) {
        if (isTextarea(e.target))
            lastTextareaId = e.target.id;
    }, true);

    // never let a click navigate the pane away; the links we make are sent to C# instead
    document.addEventListener('click', function (e) {
        var link = ose.closest(e.target, function (el) { return el.nodeName == 'A'; });
        if (link) {
            ose.cancel(e);
            var href = link.getAttribute('href') || '';
            if (href == 'bibleViewer.setReference')
                ose.send('bibRefJump', { ref: link.getAttribute('name') });
            else if (href == 'conNote.jumpToLine')
                ose.send('verseLineJump', { index: link.getAttribute('name') });
            else if (/^https?:/i.test(href))
                ose.send('openUrl', { url: href });
            return;
        }
        var btn = ose.closest(e.target, function (el) { return !!el.getAttribute('data-action'); });
        if (btn) {
            ose.cancel(e);
            ose.send('action', { name: btn.getAttribute('data-action'), id: btn.id, arg: btn.getAttribute('data-arg') });
        }
    }, false);

    document.addEventListener('keydown', function (e) {
        if (e.ctrlKey && (e.keyCode == 83)) {           // Ctrl+S
            ose.cancel(e);
            ose.send('save');
        }
        else if (e.keyCode == 116) {                    // F5 (Ctrl+F5 also realigns the lines)
            ose.send(e.ctrlKey ? 'realign' : 'reload');
            try { window.event.keyCode = 0; } catch (ex) { }   // the only way to stop IE's own refresh
            ose.cancel(e);
        }
    }, false);

    // a reference dragged from the Bible pane onto an anchor cell or a note box
    function dropTarget(el) {
        return ose.closest(el, function (x) { return x.getAttribute('data-drop') == 'scripture'; });
    }
    document.addEventListener('dragover', function (e) {
        if (dropTarget(e.target))
            ose.cancel(e);
    }, false);
    document.addEventListener('drop', function (e) {
        var target = dropTarget(e.target);
        if (!target)
            return;
        ose.cancel(e);
        ose.send('scriptureDropped', { id: target.id });
    }, false);

    // which line is at the top, for the line-number link and for coming back to the same place after a reload
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
        var scrollTop = Math.max(document.documentElement.scrollTop, document.body.scrollTop);
        var cells = document.getElementsByTagName('td');
        var top = null, line = null;
        for (var i = 0; i < cells.length; i++) {
            var td = cells[i];
            if (!td.id)
                continue;
            if (topOf(td) > scrollTop + 1)
                break;
            top = td;
            if (td.id.indexOf('ln_') == 0)
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
    }, false);
    window.addEventListener('load', reportScroll, false);

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
        if (m.focus) {
            try { ta.focus(); } catch (e) { }
        }
    });

    ose.on('appendHtml', function (m) {
        var el = document.getElementById(m.id);
        if (el)
            el.insertAdjacentHTML('beforeEnd', m.html);
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
        var range = ta.createTextRange();
        range.moveStart('character', m.start);
        range.moveEnd('character', -ta.value.length + m.start + m.length);
        range.select();
    });

    ose.on('clearSelection', function () {
        if (document.selection)
            document.selection.empty();
    });

    ose.on('getSelection', function () {
        if (!document.selection)
            return { selType: 'none', text: '' };
        var selType = String(document.selection.type).toLowerCase();
        return { selType: selType, text: (selType == 'text') ? document.selection.createRange().text : '' };
    });

    // replaces the selection (which must be in textarea 'id') with text; returns the new end point (0 = failed)
    function replaceSelectionIn(id, text) {
        var ta = textarea(id);
        if (!ta || !document.selection)
            return 0;
        var rangeSelection = document.selection.createRange();
        var rangeElement = rangeSelection.duplicate();
        rangeElement.moveToElementText(ta);
        var nEndPoint = 0;
        if (rangeElement.inRange(rangeSelection)) {
            rangeSelection.text = text;
            rangeSelection.select();
            while (rangeElement.compareEndPoints('StartToEnd', rangeSelection) < 0) {
                rangeElement.moveStart('character', 1);
                nEndPoint++;
            }
        }
        return nEndPoint;
    }

    ose.on('replaceSelection', function (m) {
        var nEndPoint = replaceSelectionIn(m.id, m.text);
        var ta = textarea(m.id);
        return { endPoint: nEndPoint, ieHtml: ta ? ta.innerHTML : null };
    });

    // send the last-focused box's text (it may have changed without a keyup, e.g. pasted with the mouse)
    ose.on('flush', function () {
        var ta = textarea(lastTextareaId);
        if (ta && !ta.readOnly)
            ose.send('textChanged', { id: ta.id, value: ta.value, quiet: true });
        return {};
    });

    ose.lastTextareaId = function () { return lastTextareaId; };
    ose.replaceSelectionIn = replaceSelectionIn;
})();
