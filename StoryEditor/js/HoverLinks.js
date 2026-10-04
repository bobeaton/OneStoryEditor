// HoverLinks.js: the commentary/info popup (HtmlForm). Clicking a link shows that reference in the Bible pane.
ose.listen(document, 'click', function (e) {
    var link = ose.closest(e.target, function (el) { return el.nodeName == 'A'; });
    if (!link)
        return;
    ose.cancel(e);
    ose.send('hoverRef', { ref: link.innerHTML });
}, false);
