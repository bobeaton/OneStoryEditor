// this one is called from the anchor buttons
function OnBibRefJump(btn) {
    if (event.button == 2) {
        window.external.OnAnchorButton(btn.id);

        // prevent the OnEmptyAnchorClick from happening too
        event.cancelBubble = true;
    }
    else
        window.external.OnBibRefJump(btn.name);
    return false; // cause the href navigation to not happen
}

if (typeof String.prototype.trim !== 'function') {
    String.prototype.trim = function () {
        return this.replace(/^\s+|\s+$/g, '');
    }
}

// this one is called from the empty cell where the buttons go (for right-click to add Null Button)
function OnEmptyAnchorClick(id) {
    if (event.button == 2)
        window.external.OnAnchorButton(id);
}

function OnLineOptionsButton(btn) {
    // capture the last textarea selected before it loses focus to do a context menu
    DisplayHtml("Calling TriggerMyBlur from OnLineOptionsButton");
    TriggerMyBlur(true);
    
    var bIsRightButton = (event.button == 2);
    window.external.OnLineOptionsButton(btn.id, bIsRightButton);
    return false;
}
function OnVerseLineJump(link) {
    window.external.OnVerseLineJump(link.name);
    return false; // cause the href navigation to not happen
}
function textboxSetSelectionTextReturnEndPosition(strId, strNewValue) {
    var oTextbox = document.getElementById(strId);

    // if we had turned our selection into a span earlier, then convert it back
    if (oTextbox.selectedText) {
        var range = oTextbox.createTextRange();
        range.collapse(true);
        range.moveStart("character", oTextbox.selectionStart);
        range.moveEnd("character", oTextbox.selectionEnd - oTextbox.selectionStart);
        range.select();
    }

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

function DisplayHtml(str) {
    var debugWindow = $('#osedebughtmlwindow');
    if (debugWindow) {
        window.external.LogMessage(str.replace(/\r\n/gm, "<nl>"));
    }
}
function removeSelection(jqtextarea) {
    if (jqtextarea.attr('selectedText')) {
        jqtextarea.removeAttr("selectionStart");
        jqtextarea.removeAttr("selectionEnd");
        jqtextarea.removeAttr("selectionAdjusted");
        jqtextarea.removeAttr("selectedText");
        removeSpan(jqtextarea);    // also remove the highlighting
    }
}
function CheckRemovePlaceHolder(jqtextarea) {
    if (jqtextarea.attr('placeholder') != '' && jqtextarea.val() == jqtextarea.attr('placeholder')) {
        jqtextarea.val('').removeClass('hasPlaceholder');
    }
}
function removeSpan(jqtextarea) {
    // jqtextarea.html(jqtextarea.val());
    //  The above works mostly, bkz 'val()' strips out any markup. But it also strips out new lines (even if I had converted them to '<br>'s).
    //  I've tried various things here, but the one that keeps "<br>"s is using Regex to strip out the spans.
    var html = jqtextarea.html();
    var html2 = NewLinesToBRs(regexRemoveSpan(html))
    DisplayHtml("removeSpan: '" + html2 + "'");
    jqtextarea.html(html2);
    DisplayHtml("removeSpan2: '" + jqtextarea.html() + "'");
}

function regexRemoveSpan(html) {
    return html.replace(/<span class=".*?">(.*?)<\/span>/gmi, "$1");
}

function ClearSelectionSpan(strId) {
    var oTextbox = document.getElementById(strId);
    removeSelection($(oTextbox));
}

function NewLinesToBRs(str) {
    return str.replace(/(\r\n|\ufffd+)/gm, "<br>");
}

function NewLinesToSpecialChars(html) {
    var addTwo = false;
    var arry = [];

    // use a function for the replacement so we can get the offsets as well, which we need to determine
    //  whether the start and end include newlines that might need to be adjusted
    var newStr = html.replace(/(\r\n|<br>)/gmi, function (match, group1, offset) {

        // put one \ufffd for each character being replaces (2 for \r\n or 4 for '<br>') so we know
        //  how much to adjust the start and end points
        for (var str = ''; str.length < match.length; str += "\ufffd") { }
        addTwo = (match.length > 2);    // if we're switching from \r\n to <br>, we'll need to adjust the selectionStart and End by 2
        arry.push(offset);
        DisplayHtml("match: (" + match.length + " at [" + offset + "]): '" + match + "', str: '" + str + "'");
        return str;
    });

    return [newStr, arry, addTwo];
}

function ToNewLines(html) {
    return html.replace(/(<br>+)/gmi, "\r\n");
}

// the following code used to be in onblur (and works just
//  fine that way in IE). But for some reason, in a 
//  WebBrowser in a WindowsForm, the onblur is triggered
//  even when the textarea is still in focus. So to work
//  around this problem, what was in onblur is now called by 
//  the onfocus handler (before getting to the work of the 
//  textarea newly in focus) as well as by my WindowsForm app
//  if the WebBrowser itself loses focus (e.g. to go to some
//  other control in the app).
function TriggerMyBlur(bDontEmptySelection) {
    DisplayHtml("TriggerMyBlur start w/ bDontEmptySelection: '" + bDontEmptySelection + "', idLastTextareaToBlur: '" + window.oseConfig.idLastTextareaToBlur + "'");
    if (window.oseConfig.idLastTextareaToBlur) {
        var oldThis = document.getElementById(window.oseConfig.idLastTextareaToBlur);
        if (oldThis.selectedText) {

            // first remove a span if it's there
            if ($(oldThis).has("span").length) {
                removeSpan($(oldThis));
            }

            var html = $(oldThis).html();
            DisplayHtml("TriggerMyBlur: seltext: '" + oldThis.selectedText + "', html: '" + html + "'");
            var tuple = NewLinesToSpecialChars(html);
            var htmlWithNewLines = tuple[0];
            var offsets = tuple[1];
            var addTwo = tuple[2];
            DisplayHtml("TriggerMyBlur: offsets: '" + offsets + "', addTwo: '" + addTwo + "'");

            // if they differ, then update the textarea with the version that has the special chars that won't get ignored by selection points
            if (html != htmlWithNewLines) {
                $(oldThis).html(htmlWithNewLines);
                DisplayHtml("TriggerMyBlur: htmlWithNewLines: '" + htmlWithNewLines + "', newHtml: '" + $(oldThis).html() + "', .value: '" + oldThis.value + "'");
            }

            // if this text had the longer "<br>" in it (rather than the shorter '\r\n's), then we possibly need to 
            //  adjust the start and end points 
            if (addTwo && !oldThis.selectionAdjusted) {
                oldThis.selectionAdjusted = true;   // (but only once)

                //  adjust each index carefully: if the adjustment is earlier in the text
                //  than the start index, then both start and end get bumped, else if the adjustment
                //  is earlier than the end, then only the end index gets bumped, else if it's after 
                //  the end then neither get bumped
                for (var i = 0; i < offsets.length; i++) {
                    if (offsets[i] < oldThis.selectionStart) {
                        oldThis.selectionStart = oldThis.selectionStart + 2;
                        oldThis.selectionEnd = oldThis.selectionEnd + 2;
                    }
                    else if (offsets[i] < oldThis.selectionEnd) {
                        oldThis.selectionEnd = oldThis.selectionEnd + 2;
                    }
                }
                DisplayHtml("TriggerMyBlur: now start: '" + oldThis.selectionStart + "', end: '" + oldThis.selectionEnd + "'");
            }

            var text = oldThis.value;
            DisplayHtml("TriggerMyBlur: text(" + text.length + "): '" + text + "', addTwo: '" + addTwo + "', start: '" + oldThis.selectionStart + "', end: '" + oldThis.selectionEnd + "'");

            // calculate the portion of the string before the selection start point and after the end point
            var pre = (oldThis.selectionStart > 0)
                ? text.substring(0, oldThis.selectionStart)
                : "";
            var post = (oldThis.selectionEnd < text.length)
                ? text.substring(oldThis.selectionEnd)
                : "";

            DisplayHtml("TriggerMyBlur: pre: '" + pre + "', sel: '" + oldThis.selectedText + "', post: '" + post + "'");
            
            // if this is being called by the C# app, it's possible that the selection
            //  here is still selected. Then the process of replacing the 'html' is likely
            //  to cause all the text in the textarea to be selected. So before doing
            //  this replacement, go ahead and collapse the selected text. (but only
            //  if not triggered from this html (i.e. only if triggered from the app)
            // UPDATE: I think the cause of the 'all the text in the textarea to be selected' was 
            //  having the start and end index incorrect. Hopefully that's resolves now (w/ the adjustments
            //  above), but this won't hurt to do anyway.
            if (!bDontEmptySelection)
                document.selection.empty();

            var newHtml = NewLinesToBRs(pre + "<span class='" + oldThis.className + " highlight'>" + oldThis.selectedText + "</span>" + post);
            $(oldThis).html(newHtml);
            DisplayHtml("myblur: before loading: newHtml: '" + newHtml + "', after loading: html: '" + $(oldThis).html() + "'");
        }
        window.oseConfig.idLastTextareaToBlur = null;
    }
    DisplayHtml("TriggerMyBlur end");
}
$(document).ready(function () {
    $('textarea').select(function (event) {
        DisplayHtml(".select start: id: '" + this.id + "'");

        // this is for when box 1 has something selected, and the user clicks in another box.
        //  we want to remember where the selection started and ended in the original box.
        // UPDATE: No... I think this also happens when the user clicks in a box
        var range = document.selection.createRange();
        if (range.htmlText.length > 0) {
            DisplayHtml(".select range.htmlText: '" + range.htmlText + "', selText: '" + this.selectedText + "', parentId: '" + range.parentElement().id + "'");
            var storedRange = range.duplicate();
            storedRange.moveToElementText(this);
            storedRange.setEndPoint('EndToEnd', range);
            DisplayHtml(".select storedRange.htmlText.length: '" + storedRange.htmlText.length + "', range.htmlText.length: '" + range.htmlText.length + "'");
            this.selectionStart = storedRange.htmlText.length - range.htmlText.length;
            this.selectionEnd = this.selectionStart + range.htmlText.length;
            this.selectionAdjusted = $(this).has("br").length;
            this.selectedText = range.htmlText;
            DisplayHtml(".select2 range.htmlText: '" + range.htmlText + "', selText: '" + this.selectedText + "', start: '" + this.selectionStart + "', end: '" + this.selectionEnd + "', selectionAdjusted: " + this.selectionAdjusted);
        }
        else {
            DisplayHtml(".select removeSelection: selText: '" + this.selectedText + "'");
            removeSelection($(this));
        }

        DisplayHtml(".select end id: '" + this.id + "'");
    }).blur(function (event) {
        DisplayHtml(".blur start id: '" + this.id + "'");
       if ($(this).attr('placeholder') != '' && ($(this).val() == '' || $(this).val() == $(this).attr('placeholder'))) {
            $(this).val($(this).attr('placeholder')).addClass('hasPlaceholder');
        }
        window.oseConfig.idLastTextareaToBlur = this.id;
        window.external.TextareaOnBlur(this.id);
        DisplayHtml(".blur end id: '" + this.id + "'");
    }).focus(function (event) {
        DisplayHtml(".focus start id: '" + this.id + "'");
        CheckRemovePlaceHolder($(this));

        // for some reason, in the WebBrowser, we continue to get
        //  focus events even after the textarea is in focus...
        if (window.oseConfig.idLastTextareaToFocus != this.id) {
            window.oseConfig.idLastTextareaToFocus = this.id;

            // for some reason, in IE, the following code works
            //  fine in onblur, but not in a WebBrowser in IE. So we have
            //  to trigger an onblur from here before dealing with focus
            //  this will turn the selected text from the last control
            //  to have focus into a span.
            DisplayHtml("Calling TriggerMyBlur from .focus");
            TriggerMyBlur(true);

            // now if we previously added the span (what we do
            //  during TriggerMyBlur but for this control now)
            //  then it has to be removed when we begin editing
            //  this textarea again
            if ($(this).has("span").length) {
                // setting the html with only the value will 
                //  remove the span element.
                removeSpan($(this));

                // and if we had previously a selected portion in
                //  this textarea, select it again
                if (this.selectedText) {
                    // moveStart and moveEnd below move to a position that's related to the 'text' value
                    //  of a textarea; not the htmlText value. So if the textarea now contains <br>s, they 
                    //  will be lost when we do the following. But our special character replacement is 
                    //  transparent to 'text', so before setting the select start and end, replace the
                    //  <br>s w/ the special character
                    // First, replace the html with the new characters...
                    var html = $(this).html();
                    var tuple = NewLinesToSpecialChars(html);
                    var htmlWithSpecialChars = tuple[0];
                    $(this).html(htmlWithSpecialChars);

                    // now get a range for it and do the moveStart, etc.
                    var range = this.createTextRange();
                    range.collapse(true);
                    range.moveStart("character", this.selectionStart);
                    range.moveEnd("character", this.selectionEnd - this.selectionStart);
                    var bookmark = range.getBookmark();

                    // finally, replace the original text back into the textarea and set the bookmark on it
                    $(this).html(html);
                    range.moveToBookmark(bookmark);
                    DisplayHtml(".focus .selected '" + range.text + "', start: '" + this.selectionStart + "', end: '" + this.selectionEnd + "', htmlWithSpecialChars: '" + htmlWithSpecialChars + "'");
                    // range.select();   apparently not needed
                }
            }
            window.external.TextareaOnFocus(this.id);
        }

        DisplayHtml(".focus end id: '" + this.id + "'");
    }).mouseup(function (event) {
        DisplayHtml(".mouseup start id: '" + $(this).id + "', w/ btn: '" + event.button + "'");
        if (event.button == 2)  // right-clicked... 
        {
            DisplayHtml("mu: lst2blr: " + window.oseConfig.idLastTextareaToBlur + ", this: " + this.id);
            // if the user right-clicks as the first event in a new textarea, it will get
            //  focus event first, which will turn it's span into a selection (which we
            //  don't want, because right-click means we're probably going to need the 
            //  spans during the subsequent call to, e.g. add note on selected text). 
            // So in the case of a right-click, undo the selection and go back to a span
            //  (whether or not we empty the selection first depends on whether this is 
            //  a new textarea or not. If it's a new text area, then last2blur will be
            //  null and then bDontEmptySelection will be false which will trigger the
            //  collapse of the selection (not sure... but it works...)
            window.oseConfig.idLastTextareaToBlur = this.id;
            // var bDontEmptySelection = !(window.oseConfig.idLastTextareaToBlur == this.id);
            // TriggerMyBlur(bDontEmptySelection);

            // tell app to show the context menu for this control
            window.external.ShowContextMenu(this.id);
            // return false;
        }
        window.external.TextareaMouseUp(this.id);
        DisplayHtml(".mouseup end id: '" + this.id + "'");
        return true;
    }).mousedown(function (event) {
        DisplayHtml(".mousedown start id: '" + this.id + "'");
        CheckRemovePlaceHolder($(this));    // remove the place holder in case this is TextPaster (or the name of the languages is thought to be text)
        window.external.OnTextareaMouseDown(this.id, this.value, event.button);
        if (event.button == 1) {
            removeSelection($(this));
        }
        DisplayHtml(".mousedown end id: '" + this.id + "'");
    }).mousemove(function () {
        window.external.OnMouseMove();
    }).keyup(function (event) {
        DisplayHtml(".keyup start id: '" + this.id + "'");
        // if we had something selected and the user presses delete or backspace, 
        // UPDATE (10/5/13): if the user types *anything* after selecting, we need to remove the selection...
        // then we have to clear out the selection (so it doesn't reoccur if we trigger blur)
        $(this).removeAttr("selectedText");

        // then there are certain keys we don't want to trigger this for
        if (ctrl_down && ((event.keyCode == ctrl_key) ||   // ignore this one... (it's the control key)
                          (event.keyCode == c_key) ||   // copy
                          (event.keyCode == a_key) ||   // select all
                          (event.keyCode == f_key) ||   // find
                          (event.keyCode == h_key) ||   // replace
                          (event.keyCode == s_key))) {
            return true;
        }
        DisplayHtml(".keyup end id: '" + this.id + "', this.value: '" + this.value + "' vs, html: '" + $(this).html() + "'");
        return window.external.TextareaOnKeyUp(this.id, this.value);
    }).keydown(function (event) {
        DisplayHtml(".keydown start id: '" + this.id + "'");
        if (ctrl_down && ((event.keyCode == v_key) ||   // paste
                          (event.keyCode == x_key))) {  // cut
            $(this).removeAttr("selectedText"); // cut or paste means we no longer have a selection
        }
        DisplayHtml(".keydown end id: '" + this.id + "'");
    }).dblclick(function (event) {
        DisplayHtml(".dblclick start id: '" + this.id + "'");
        var sel = document.selection;
        var range = sel.createRange();
        range.expand("word");

        // if the user dblclicks on the last word in the textarea and it isn't followed by a punctuation char (in IE), 
        //  then the range will be empty (don't ask... 40 man hours be here...)
        if (!range.text) {
            // First, get the html and since we're going to force it have BRs later (or the if it has \r\n, they'll be stripped out by something),
            //  put the BRs in now, so that the start and end indices will be based on there being 4 chars (<br>) rather than 2 (\r\n)
            //  (but see note below)
            var html = NewLinesToBRs($(this).html());
            if (html.length > 0) {
                var words = this.value.trim().split(' ');          // split by words (here we can use the 'value', which strips out the html bits -- so we don't see "<br>" as a word)
                if (words.length >= 2) {                    //  and if there are at least 2...
                    var lastWord = words[words.length - 1]; // get the (length of the) last word

                    // moveStart and moveEnd below move to a position that's related to the 'text' value
                    //  of a textarea; not the htmlText value. So if the textarea now contains <br>s or \r\n, they 
                    //  will be lost when we do the following. But our special character replacement is 
                    //  transparent to 'text', so before setting the select start and end, replace the
                    //  <br>s w/ the special character
                    // UPDATE: but this is all about getting the final word... so if I know how many offsets there 
                    //  are, then I worked out that there will be 3 * # of offsets that we have to reduce the start 
                    //  and end indices by (bkz the br is somehow treated in html as 3 less than the 4 chars in "<br>"??? 
                    //  maybe as a single character vs 4? anyway, not sure why, but empirically this works)
                    var tuple = NewLinesToSpecialChars(html);
                    var offsets = tuple[1];
                    var extraNegIndexOffset = 3 * offsets.length;

                    // finally, replace the original text back into the textarea...
                    $(this).html(html);

                    range = this.createTextRange();
                    range.collapse(true);
                    range.moveStart("character", html.length - lastWord.length - extraNegIndexOffset);
                    range.moveEnd("character", html.length - extraNegIndexOffset);
                    DisplayHtml(".dblclick7: range.text: '" + range.text + "'");
                }
            }
        }
        else {
            // if there is no punctuation after the word, IE automatically selects the space afterwards, which I find annoying
            while (range.text.slice(-1) == ' ') {
                range.moveEnd('character', -1);
            }
        }

        if (range.text) {
            range.select();
        }

        range.parentElement().focus();    // gotta focus or typing afterwards won't replace the selected text
        DisplayHtml(".dblclick end id: '" + this.id + "'");
    });
    $('.readonly').attr('readonly', 'readonly');
});
window.oseConfig =
{
    idLastTextareaToBlur: null,
    idLastTextareaToFocus: null
};

var ctrl_down = false;
var ctrl_key = 17;
var a_key = 65;
var c_key = 67;
var f_key = 70;
var h_key = 72;
var s_key = 83;
var v_key = 86;
var x_key = 88;
var f5_key = 116;

$(document).keydown(function (e) {
    if (e.keyCode == ctrl_key) ctrl_down = true;
}).keyup(function (e) {
    if (e.keyCode == ctrl_key) ctrl_down = false;
});

$(document).keydown(function (e) {
    if (ctrl_down && (e.keyCode == s_key)) {
        window.external.OnSaveDocument();
        // Your code
        e.preventDefault();
        return false;
    }
    else if (e.keyCode == f5_key) {
        if (ctrl_down) {
            window.external.TriggerCtrlF5();
        }

        // let the form handle it
        window.external.LoadDocument();
        // doesn't work... e.preventDefault();
        return true;
    }
}); 