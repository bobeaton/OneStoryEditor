// ConNoteDomPrefix.js: the Consultant/Coach note panes. Inlined after bridge.js and PaneCommon.js (which handle the
//  links, buttons, keys, drops, scrolling and the textarea commands).
function DisplayHtml(str) {
    if (window.oseDebug)
        ose.send('log', { text: str.replace(/\r\n/gm, "<nl>") });
}

function regexRemoveSpan(html) {
    return html.replace(/<span class=".*?">(.*?)<\/span>/gmi, "$1");
}

function ToNewLines(html) {
    return html.replace(/(<br>+)/gmi, "\r\n");
}

if (typeof String.prototype.trim !== 'function') {
    String.prototype.trim = function () {
        return this.replace(/^\s+|\s+$/g, '');
    }
}

// (unchanged from before, apart from being called by the dblclick listener below)
function OnDoubleClick(elem) {
    DisplayHtml("OnDoubleClick: start: typeof elem (<p>): '" + (typeof elem) + "'");
    if (document.selection) {
        var sel = document.selection;
        var rng = sel.createRange();
        rng.expand("word");

        // if the user dblclicks on the last word in the textarea (or <p>) and it isn't followed by a punctuation char (in IE),
        //  then the range will be empty
        if (!rng.text) {
            if (!elem.id) {
                rng.moveToElementText(elem);            // set the range to the entire contents of the text area
                DisplayHtml("OnDoubleClick: rng.text: '" + rng.text + "'");

                /* none of this works, because anytime you double click on a <p> (non-editable portion of the con notes pane)
                    it returns !rng.text, which means you can't tell which portion was selected. So doubleclick will select
                    everything in the cell (from moveToElementText above) and if they need to select a single (set of) words
                    then they'll need to click and drag
                var fullText = rng.text;
                if (fullText.length > 0) {
                    var words = fullText.trim().split(' ');        //  and split by words
                    if (words.length >= 2) {                // if there is at least 2
                        var lastWord = words[words.length - 1]; // get the (length of the) last word

                        // now have to find that word's start and end in the innerhtml (the real string in the paragraph)
                        var realText = elem.innerHTML;
                        if (realText) {
                            var startIndex = rng.text.indexOf(lastWord);
                            rng.collapse(true);
                            rng.moveStart("character", startIndex);
                            rng.moveEnd("character", lastWord.length);
                        }
                        DisplayHtml("OnDoubleClick: lastWord: '" + lastWord + "', rng.text: '" + rng.text + "', startIndex: '" + startIndex + "', end: '" + (startIndex + lastWord.length) + "', elem.innerHTML: '" + elem.innerHTML + "', words: '" + words + "'");
                    }
                }
                /*
                // means it's a readonly <p> element
                var fullText = ToNewLines(regexRemoveSpan(elem.innerHTML));
                DisplayHtml("OnDoubleClick: typeof elem (<p>): '" + (typeof elem) + "', fullText: '" + fullText + "'");
                */
            }
            else {
                var fullText = elem.value;                // get the string of the contents...
                DisplayHtml("OnDoubleClick: typeof elem: '" + (typeof elem) + "', elem.id: '" + elem.id + "', fullText: '" + fullText + "'");
                if (fullText.length > 0) {
                    rng.moveToElementText(elem);        // set the range to the entire contents of the text area
                    var words = fullText.split(' ');        //  and split by words
                    if (words.length >= 2) {                // if there is at least 2
                        var lastWord = words[words.length - 1]; // get the (length of the) last word
                        rng.moveStart('character', fullText.length - lastWord.length);  // move the start pos to the beginning of the last word
                    }
                }
            }
        }
        else {
            // if there is no punctuation after the word, IE automatically selects the space afterwards, which I find annoying
            while (rng.text.slice(-1) == ' ') {
                rng.moveEnd('character', -1);
            }
        }

        if (rng.text) {
            rng.select();
        }
        rng.parentElement().focus();    // gotta focus or typing afterwards won't replace the selected text
    }
}
function transformText(e, type) {
    if (document.selection) {
        var rangeSelection = document.selection.createRange();
        var selectVal = rangeSelection.text;
        selectVal = selectVal.trim();
        if (selectVal) {
            if (!startsWith(selectVal, type) || !endsWith(selectVal, type)) {
                selectVal = type + selectVal + type + " ";
                rangeSelection.text = selectVal;
                rangeSelection.select();
            }
        }
    }
    try { window.event.keyCode = 0; } catch (ex) { }
    ose.cancel(e);
}

function startsWith(str, word) {
    return str.lastIndexOf(word, 0) === 0;
}
function endsWith(str, word) {
    return str.indexOf(word, str.length - word.length) !== -1;
}

// double-click selects a word in the note box or the referring text (data-note on both)
ose.listen(document, 'dblclick', function (e) {
    var el = ose.closest(e.target, function (x) { return !!x.getAttribute('data-note'); });
    if (el)
        OnDoubleClick(el);
}, false);

// Ctrl+B / Ctrl+I in the note box wrap the selection in $...$ / *...*
ose.listen(document, 'keydown', function (e) {
    if (!e.ctrlKey || !ose.closest(e.target, function (x) { return x.getAttribute('data-note') == 'edit'; }))
        return;
    if (e.keyCode == 66)
        transformText(e, "$");
    else if (e.keyCode == 73)
        transformText(e, "*");
}, false);

// right-click anywhere asks C# for the note context menu (window.event.button, as the old body onmouseup used)
document.onmouseup = function () {
    if (window.event.button == 2)
        ose.send('contextMenu', {});
};

// the note box's own events (this replaces the HTML_Script_AddTextareaMouseDown block; same event properties, so
//  the values sent are the same as before)
ose.listen(window, 'load', function () {
    var textareas = document.getElementsByTagName("textarea");
    for (var i = 0; i < textareas.length; i++) {
        textareas[i].onmousedown = function () { ose.send('textareaMouseDown', { id: this.id, value: this.value, button: window.event.button }); };
        textareas[i].onkeyup = function () { ose.send('textChanged', { id: this.id, value: this.value }); };
        textareas[i].onselect = function () { this.focus(); };
        textareas[i].onchange = function () { ose.send('textChanged', { id: this.id, value: this.value }); };
        textareas[i].onpaste = function () { ose.send('textChanged', { id: this.id, value: this.value }); };
    }
}, false);

// the selection in a read-only part of the pane, and the line it's on, for "Add note on selected text"
ose.on('getNoteSelection', function () {
    if (!document.selection)
        return {};
    var range = document.selection.createRange();
    if (!range || !range.htmlText)
        return {};
    var re = /id="?tp_(\d+?)_/;
    var elem = range.parentElement();
    while (elem && !re.test(elem.innerHTML))
        elem = elem.parentElement;
    if (!elem)
        return {};
    return { lineIndex: parseInt(re.exec(elem.innerHTML)[1], 10), html: range.htmlText };
});
