// modern/HoverLinks.js: the commentary/info popup (HtmlForm) on WebView2. Clicking a link shows that reference in the
//  Bible pane.
document.addEventListener('click', function (e) {
    var link = ose.closest(e.target, function (el) { return el.nodeName === 'A'; });
    if (!link)
        return;
    e.preventDefault();
    ose.send('hoverRef', { ref: link.innerHTML });
});
