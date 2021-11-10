function DisplayHtml(str) {
    window.external.LogMessage(str.replace(/\r\n/gm, "<nl>"));
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

function OnBibRefJump(link) {
    window.external.OnBibRefJump(link.name);
    return false; // cause the href navigation to not happen
}
function OnVerseLineJump(link) {
    window.external.OnVerseLineJump(link.name);
    return false; // cause the href navigation to not happen
}

var s_key = 83;
var f5_key = 116;
function OnKeyDown() {
    if (window.event.keyCode == f5_key) {
        // let the form handle it
        window.external.LoadDocument();

        // disable the propagation of the F5 event
        window.event.keyCode = 0;
        window.event.returnValue = false;
        return false;
    }
    else if (window.event.ctrlKey && (window.event.keyCode == 70)) {
        if (window.event.stopPropagation) {
            window.event.stopPropagation();
        }
        else {
            window.event.cancelBubble = true;
            window.event.returnValue = false;
            window.event.keyCode = 0;
        }
        window.external.DoFind();
        return false;
    }
    else if (window.event.ctrlKey && (window.event.keyCode == s_key)) {
        if (window.event.stopPropagation) {
            window.event.stopPropagation();
        }
        else {
            window.event.cancelBubble = true;
            window.event.returnValue = false;
            window.event.keyCode = 0;
        }
        window.external.OnSaveDocument();
        return false;
    }
}
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
function textboxSetSelection(strId, iStart, iLen) {
    var oTextbox = document.getElementById(strId);
    var oRange = oTextbox.createTextRange();
    oRange.moveStart("character", iStart);
    oRange.moveEnd("character", -oTextbox.value.length + iStart + iLen);
    oRange.select();
}
function textboxSetSelectionTextReturnEndPosition(strId, strNewValue) {
    var oTextbox = document.getElementById(strId);
    var rangeSelection = document.selection.createRange();
    var rangeElement = rangeSelection.duplicate();
    rangeElement.moveToElementText(oTextbox);
    var nEndPoint = 0;
    if (rangeElement.inRange(rangeSelection)) {
        rangeSelection.text = strNewValue;
        rangeSelection.select();
        while (rangeElement.compareEndPoints('StartToEnd', rangeSelection) < 0) {
            rangeElement.moveStart('character', 1);
            nEndPoint++;
        }
    }
    return nEndPoint;
}
function OnMouseUp() {
    // if the user right-clicks, then ask the ConNote pane to show the context menu
    if (window.event.button == 2)
        window.external.ShowContextMenu();
}
function OnTextAreaKeyDown() {
    if (window.event.ctrlKey && (window.event.keyCode == 66)) {
        transformText("$")
    } else if (window.event.ctrlKey && (window.event.keyCode == 73)) {
        transformText("*")
    }
}

function transformText(type) {
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
    window.event.returnValue = false;
    window.event.keyCode = 0;
}

function startsWith(str, word) {
    return str.lastIndexOf(word, 0) === 0;
}
function endsWith(str, word) {
    return str.indexOf(word, str.length - word.length) !== -1;
}
