// NetBible.js: the Bible pane (NetBibleViewer). Footnote links show a tooltip; verse buttons can be clicked
//  (show commentary) or dragged out (to drop a reference on an anchor cell or a note).
(function () {
    function isButton(el) { return el.nodeName == 'BUTTON'; }

    ose.listen(document, 'click', function (e) {
        var link = ose.closest(e.target, function (el) { return el.nodeName == 'A'; });
        if (!link)
            return;
        ose.cancel(e);
        ose.send('hoverRef', { ref: link.getAttribute('href').substr(6) });
    }, false);

    ose.listen(document, 'mousedown', function (e) {
        if (!ose.closest(e.target, isButton))
            return;
        ose.cancel(e);
        ose.send('refMouseDown');
    }, false);

    ose.listen(document, 'mouseup', function (e) {
        var btn = ose.closest(e.target, isButton);
        if (!btn)
            return;
        ose.cancel(e);
        ose.send('refMouseUp', { target: btn.id, ref: btn.getAttribute('value') });
    }, false);

    ose.listen(document, 'mouseout', function (e) {
        var btn = ose.closest(e.target, isButton);
        if (!btn)
            return;
        ose.send('refMouseOut', { target: btn.id, ref: btn.getAttribute('value') });
    }, false);
})();
