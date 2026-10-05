// modern/StoryBt.js: the Story/BT page on WebView2. A selection made in a box becomes that box's highlight (one per
//  box; highlights in several boxes add up) for "Add note on selected text". Chromium can't draw inside a textarea, so a
//  highlighted box gets a mirror layer behind it (same text metrics; the range in a <mark>) and goes transparent.
//  Same messages as js/StoryBt.js + StoryBtPs.js (the IE ones). Inlined after modern/bridge.js and modern/PaneCommon.js.
(function () {
    'use strict';
    var highlights = {};      // textarea id -> { start, end, value (the box's value then) }
    var mirrors = {};         // textarea id -> its mirror div
    var lastMouseMove = 0;
    var COPIED = ['fontFamily', 'fontSize', 'fontWeight', 'fontStyle', 'fontVariant', 'lineHeight', 'letterSpacing',
                  'wordSpacing', 'textAlign', 'textIndent', 'textTransform', 'direction', 'unicodeBidi', 'tabSize',
                  'overflowWrap', 'wordBreak',
                  'paddingTop', 'paddingRight', 'paddingBottom', 'paddingLeft',
                  'borderTopWidth', 'borderRightWidth', 'borderBottomWidth', 'borderLeftWidth'];

    function isTextarea(el) { return !!el && el.nodeName === 'TEXTAREA'; }
    function legacyButton(b) { return (b === 0) ? 1 : (b === 2) ? 2 : 4; }    // the values C# (and IE) use
    function esc(s) { return s.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/"/g, '&quot;'); }

    function placeholderFor(ta) {
        var c = ta.classList;
        if (c.contains('LangVernacular')) return VernacularLanguageName();
        if (c.contains('LangNationalBt')) return NationalBtLanguageName();
        if (c.contains('LangInternationalBt')) return InternationalBtLanguageName();
        if (c.contains('LangFreeTranslation')) return FreeTranslationLanguageName();
        return '';
    }

    // a box's current (non-empty) selection becomes its highlight
    function record(ta) {
        if (ta.selectionEnd > ta.selectionStart) {
            highlights[ta.id] = { start: ta.selectionStart, end: ta.selectionEnd, value: ta.value };
            drawMirror(ta);
        }
    }

    // a box's highlight, unless its value has changed since (e.g. by a script, with no input event): then the range
    //  no longer marks the text that was selected, so the highlight is dropped
    function current(ta) {
        var h = highlights[ta.id];
        if (h && (h.value !== ta.value)) {
            delete highlights[ta.id];
            h = null;
        }
        return h || null;
    }

    function clear(ta) {
        if (highlights[ta.id]) {
            delete highlights[ta.id];
            drawMirror(ta);
        }
    }

    function drawMirror(ta) {
        var h = current(ta), m = mirrors[ta.id];
        if (!h) {
            if (m) {
                m.parentNode.removeChild(m);
                delete mirrors[ta.id];
                ta.style.backgroundColor = ta.getAttribute('data-ose-bg') || '';
            }
            return;
        }
        var cs = window.getComputedStyle(ta);
        if (!m) {
            m = document.createElement('div');
            m.className = 'oseMirror';
            m.setAttribute('aria-hidden', 'true');
            m.style.backgroundColor = cs.backgroundColor;          // (taken before the box goes transparent)
            ta.setAttribute('data-ose-bg', ta.style.backgroundColor || '');
            ta.parentNode.insertBefore(m, ta);
            mirrors[ta.id] = m;
            if (window.getComputedStyle(ta.parentNode).position === 'static')
                ta.parentNode.style.position = 'relative';         // so the mirror and the box share a frame
            ta.style.position = 'relative';
            ta.style.zIndex = '1';
            ta.style.backgroundColor = 'transparent';
        }
        for (var i = 0; i < COPIED.length; i++)
            m.style[COPIED[i]] = cs[COPIED[i]];
        // the text wraps at the same width as the box's only if the mirror leaves room for the box's vertical
        //  scrollbar (which Chromium puts on the left when the box is right-to-left)
        var scrollbar = ta.offsetWidth - ta.clientWidth -
                        parseFloat(cs.borderLeftWidth) - parseFloat(cs.borderRightWidth);
        if (scrollbar > 0) {
            if (cs.direction === 'rtl')
                m.style.paddingLeft = (parseFloat(cs.paddingLeft) + scrollbar) + 'px';
            else
                m.style.paddingRight = (parseFloat(cs.paddingRight) + scrollbar) + 'px';
        }
        m.style.boxSizing = 'border-box';
        m.style.position = 'absolute';
        m.style.zIndex = '0';
        m.style.margin = '0';
        m.style.borderStyle = 'solid';
        m.style.borderColor = 'transparent';
        m.style.color = 'transparent';
        m.style.whiteSpace = 'pre-wrap';
        m.style.overflow = 'hidden';            // (still scrolled by script, to follow the box)
        m.style.pointerEvents = 'none';
        m.style.left = ta.offsetLeft + 'px';
        m.style.top = ta.offsetTop + 'px';
        m.style.width = ta.offsetWidth + 'px';
        m.style.height = ta.offsetHeight + 'px';
        var v = ta.value;
        m.innerHTML = esc(v.substring(0, h.start)) + '<mark class="' + esc(ta.className) + ' highlight">' +
                      esc(v.substring(h.start, h.end)) + '</mark>' + esc(v.substring(h.end)) + '\n';
        m.scrollTop = ta.scrollTop;
        m.scrollLeft = ta.scrollLeft;
        // while the box has focus, the browser shows its own selection (the mirror stays, as the box's background)
        m.querySelector('mark').style.visibility = (document.activeElement === ta) ? 'hidden' : 'visible';
    }

    function redrawAll() {
        for (var id in mirrors) {
            var ta = document.getElementById(id);
            if (ta)
                drawMirror(ta);
        }
    }

    document.addEventListener('DOMContentLoaded', function () {
        var style = document.createElement('style');
        style.textContent = '.oseMirror mark { color: transparent !important; }';
        document.head.appendChild(style);
        var boxes = document.getElementsByTagName('textarea');
        for (var i = 0; i < boxes.length; i++) {
            var ta = boxes[i];
            if (ta.classList.contains('readonly'))
                ta.readOnly = true;
            ta.placeholder = placeholderFor(ta);
            ta.addEventListener('scroll', function (e) {
                var mi = mirrors[e.target.id];
                if (mi) {
                    mi.scrollTop = e.target.scrollTop;
                    mi.scrollLeft = e.target.scrollLeft;
                }
            });
        }
    });
    window.addEventListener('resize', redrawAll);
    ose.afterAutoSize = redrawAll;      // (PaneCommon grew or shrank boxes to fit their text: the mirrors follow)

    document.addEventListener('focusin', function (e) {
        if (!isTextarea(e.target))
            return;
        drawMirror(e.target);
        ose.send('focus', { id: e.target.id });
    });

    document.addEventListener('focusout', function (e) {
        if (!isTextarea(e.target))
            return;
        record(e.target);       // a selection made and then left stays as the box's highlight
        drawMirror(e.target);
        ose.send('blur', { id: e.target.id });
    });

    document.addEventListener('mousedown', function (e) {
        var ta = e.target;
        if (!isTextarea(ta))
            return;
        if (e.button === 0)
            clear(ta);          // as before: a left-click in a box drops its highlight (a new drag makes a new one)
        ose.send('textareaMouseDown', { id: ta.id, value: ta.value, button: legacyButton(e.button) });
    });

    document.addEventListener('mouseup', function (e) {
        var ta = e.target;
        if (isTextarea(ta)) {
            record(ta);
            if (e.button === 2)
                ose.send('contextMenu', { id: ta.id });
            ose.send('textareaMouseUp', { id: ta.id });
            return;
        }
        // anchor buttons, the empty part of the anchor row, and the line-options buttons (data-mouseup)
        var el = ose.closest(e.target, function (x) { return !!x.getAttribute('data-mouseup'); });
        if (!el)
            return;
        var what = el.getAttribute('data-mouseup');
        var bRight = (e.button === 2);
        if (what === 'lineOptions') {
            var last = document.getElementById(ose.lastTextareaId() || '');
            if (isTextarea(last))
                record(last);   // (what TriggerMyBlur did before a line-options menu)
            ose.send('action', { name: 'lineOptions', id: el.id, arg: bRight ? 'right' : 'left' });
            e.preventDefault();
        }
        else if (what === 'anchor') {
            if (bRight)
                ose.send('action', { name: 'anchorMenu', id: el.id });
            else
                ose.send('bibRefJump', { ref: el.getAttribute('name') });
            e.preventDefault();
        }
        else if ((what === 'anchorCell') && bRight) {
            ose.send('action', { name: 'anchorMenu', id: el.id });
        }
    });

    document.addEventListener('keyup', function (e) {
        var ta = e.target;
        if (!isTextarea(ta))
            return;
        if (ta.selectionEnd > ta.selectionStart)
            record(ta);         // e.g. Shift+arrows
        else
            clear(ta);
    });

    document.addEventListener('input', function (e) {
        if (isTextarea(e.target))
            clear(e.target);    // the highlighted text may have changed (PaneCommon sends the new value)
    });

    document.addEventListener('dblclick', function (e) {
        var ta = e.target;
        if (!isTextarea(ta))
            return;
        // as before: no trailing space on a double-clicked word
        while ((ta.selectionEnd > ta.selectionStart) && (ta.value.charAt(ta.selectionEnd - 1) === ' '))
            ta.setSelectionRange(ta.selectionStart, ta.selectionEnd - 1);
        record(ta);
    });

    document.addEventListener('contextmenu', function (e) {
        if (isTextarea(e.target))
            e.preventDefault();
    });

    document.addEventListener('mousemove', function (e) {
        if (!isTextarea(e.target))
            return;
        var now = Date.now();
        if (now - lastMouseMove >= 100) {     // only the Bible pane's auto-hide listens
            lastMouseMove = now;
            ose.send('mouseMove');
        }
    });

    // the highlights in one line's table (after making the box being worked in's selection its highlight, as
    //  TriggerMyBlur did) -- the same items as the IE page, so the note's referring text comes out the same
    ose.on('getHighlights', function (m) {
        var last = document.getElementById(ose.lastTextareaId() || '');
        if (isTextarea(last))
            record(last);
        var items = [];
        var table = document.getElementById(m.tableId);
        if (table) {
            var boxes = table.getElementsByTagName('textarea');
            for (var i = 0; i < boxes.length; i++) {
                var h = current(boxes[i]);
                if (!h && mirrors[boxes[i].id])
                    drawMirror(boxes[i]);     // (a stale highlight's mirror goes too)
                if (h)
                    items.push({ textareaId: boxes[i].id, className: boxes[i].className + ' highlight',
                                 text: boxes[i].value.substring(h.start, h.end) });
            }
        }
        return { items: items };
    });

    // (the box's own selection goes too, as the IE page's selection expandos did, or the next getHighlights would
    //  turn it straight back into a highlight)
    ose.on('clearHighlight', function (m) {
        var ta = document.getElementById(m.id);
        if (!isTextarea(ta))
            return;
        clear(ta);
        ta.setSelectionRange(ta.selectionEnd, ta.selectionEnd);
    });

    // PaneCommon's commands that change a box's value by script (no input event), redone here so they drop the box's
    //  highlight first (the last ose.on registration wins); same replies as PaneCommon's
    function textarea(id) { var el = id ? document.getElementById(id) : null; return isTextarea(el) ? el : null; }

    ose.on('setText', function (m) {
        var ta = textarea(m.id);
        if (!ta)
            return;
        clear(ta);
        ta.value = m.text;
        ose.autoSize(ta);
        ose.send('textChanged', { id: ta.id, value: ta.value });
    });

    ose.on('appendText', function (m) {
        var ta = textarea(m.id);
        if (!ta)
            return;
        clear(ta);
        ta.value += m.text;
        ose.autoSize(ta);
        ose.send('textChanged', { id: ta.id, value: ta.value });
        if (m.focus)
            ta.focus();
    });

    ose.on('replaceSelection', function (m) {
        var ta = textarea(m.id);
        if (ta)
            clear(ta);
        var nEndPoint = ose.replaceSelectionIn(m.id, m.text);
        return { endPoint: nEndPoint, value: ta ? ta.value : null };
    });
})();
