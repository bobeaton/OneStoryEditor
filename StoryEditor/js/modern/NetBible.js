// modern/NetBible.js: the Bible pane (NetBibleViewer) on WebView2. Footnote links show a tooltip; verse buttons can be
//  clicked (show commentary) or dragged (HTML5 drag) onto an anchor cell or a note box in another pane.
(function () {
    'use strict';
    function isButton(el) { return el.nodeName === 'BUTTON'; }
    // the real verse buttons have no value attribute; IE (quirks) reported their text as the value, so do the same
    function refOf(btn) { return btn.getAttribute('value') || btn.textContent; }

    document.addEventListener('click', function (e) {
        var link = ose.closest(e.target, function (el) { return el.nodeName === 'A'; });
        if (!link)
            return;
        e.preventDefault();
        ose.send('hoverRef', { ref: link.getAttribute('href').substr(6) });
    });

    document.addEventListener('mousedown', function (e) {
        if (!ose.closest(e.target, isButton))
            return;
        ose.send('refMouseDown');
    });

    document.addEventListener('mouseup', function (e) {
        var btn = ose.closest(e.target, isButton);
        if (!btn)
            return;
        ose.send('refMouseUp', { target: btn.id, ref: refOf(btn) });
    });

    document.addEventListener('dragstart', function (e) {
        var btn = ose.closest(e.target, isButton);
        if (!btn)
            return;
        e.dataTransfer.setData('text/plain', refOf(btn));
        e.dataTransfer.effectAllowed = 'copy';
        ose.send('refDragStart', { target: btn.id, ref: refOf(btn) });
    });

    document.addEventListener('dragend', function (e) {
        if (ose.closest(e.target, isButton))
            ose.send('refDragEnd');
    });

    window.addEventListener('DOMContentLoaded', function () {
        var buttons = document.getElementsByTagName('button');
        for (var i = 0; i < buttons.length; i++)
            buttons[i].draggable = true;
    });
})();
