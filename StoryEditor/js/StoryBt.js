function OnBibRefJump(btn) {
    if (event.button == 2)
        window.external.OnAnchorButton(btn.id);
    else
        window.external.OnBibRefJump(btn.name);
    return false; // cause the href navigation to not happen
}
function OnLineOptionsButton(btn) {
    // capture the last textarea selected before it loses focus to do a context menu
    TriggerMyBlur(true);
    
    var bIsRightButton = (event.button == 2);
    window.external.OnLineOptionsButton(btn.id, bIsRightButton);
    return false;
}
function OnVerseLineJump(link)
{
    window.external.OnVerseLineJump(link.name);
    return false; // cause the href navigation to not happen
}
function OnKeyDown() {
    if (window.event.keyCode == 116) {
        // let the form handle it
        window.external.LoadDocument();

        // disable the propagation of the F5 event
        window.event.keyCode = 0;
        window.event.returnValue = false;
        return false;
    }
    /* this is for searching... not implemented yet
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
    */
}
function DisplayHtml(strFunction) {
    var debugWindow = $('#osedebughtmlwindow');
    if (debugWindow) {
        var curVal = debugWindow.val();
        $('#osedebughtmlwindow').val(curVal + " " + strFunction);
    }
}
function removeSelection(jqtextarea) {
    if (jqtextarea.attr('selectedText')) {
        jqtextarea.removeAttr("selectionStart");
        jqtextarea.removeAttr("selectionEnd");
        jqtextarea.removeAttr("selectedText");
        removeSpan(jqtextarea);    // remove the highlighting
    }
}

function removeSpan(jqtextarea) {
    jqtextarea.html(jqtextarea.val());
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
function TriggerMyBlur(bTriggeredFromHere) {
    if (window.oseConfig.idLastTextareaToBlur) {
        var oldThis = document.getElementById(window.oseConfig.idLastTextareaToBlur);
        if (oldThis.selectedText) {
            var text = oldThis.value;
            var pre = (oldThis.selectionStart > 0)
                ? text.substring(0, oldThis.selectionStart)
                : "";
            var post = (oldThis.selectionEnd < (text.length - 1))
                ? text.substring(oldThis.selectionEnd)
                : "";

            // if this is being called by the app, it's possible that the selection
            //  here is still selected. Then the process of replacing the 'html' is likely
            //  to cause all the text in the textarea to be selected. So before doing
            //  this replacement, go ahead and collapse the selected text. (but only
            //  if not triggered from this html (i.e. only if triggered from the app)
            if (!bTriggeredFromHere)
                document.selection.empty();

            $(oldThis).html(pre + "<span class='" + oldThis.className + " highlight'>" + oldThis.selectedText + "</span>" + post);
        }
        window.oseConfig.idLastTextareaToBlur = null;
    }
}
$(document).ready(function () {
    $('textarea').select(function (event) {
        var range = document.selection.createRange();
        if (range.text.length > 0) {
            var storedRange = range.duplicate();
            storedRange.moveToElementText(this);
            storedRange.setEndPoint('EndToEnd', range);
            this.selectionStart = storedRange.text.length - range.text.length;
            this.selectionEnd = this.selectionStart + range.text.length;
            this.selectedText = range.text;
        }
        else {
            removeSelection($(this));
        }
        DisplayHtml(event.type);
    }).blur(function (event) {
        if ($(this).attr('placeholder') != '' && ($(this).val() == '' || $(this).val() == $(this).attr('placeholder'))) {
            $(this).val($(this).attr('placeholder')).addClass('hasPlaceholder');
        }
        window.oseConfig.idLastTextareaToBlur = this.id;
        DisplayHtml(event.type);
    }).focus(function (event) {
        if ($(this).attr('placeholder') != '' && $(this).val() == $(this).attr('placeholder')) {
            $(this).val('').removeClass('hasPlaceholder');
        }
        // for some reason, in the WebBrowser, we continue to get
        //  focus events even after the textarea is in focus...
        if (window.oseConfig.idLastTextareaToFocus == this.id)
            return;

        window.oseConfig.idLastTextareaToFocus = this.id;

        // for some reason, in IE, the following code works
        //  fine in onblur, but in a WebBrowser in IE
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
                var range = this.createTextRange();
                range.collapse(true);
                range.moveStart("character", this.selectionStart);
                range.moveEnd("character", this.selectionEnd - this.selectionStart);
                range.select();
            }
        }
        // window.external.TextareaOnFocus(this.id);
        DisplayHtml(event.type);
    }).mouseup(function (event) {
        if (event.button == 2)  // right-clicked... 
        {
            // tell app to show the context menu for this control
            window.external.ShowContextMenu(this.id);
            // return false;
        }
        window.external.TextareaMouseUp(this.id);
        return true;
    }).mousedown(function (event) {
        window.external.OnTextareaMouseDown(this.id, this.value);
        if (event.button == 1)
            removeSelection($(this));
    });
    $('.readonly').attr('readonly', 'readonly');
});
window.oseConfig =
{
    idLastTextareaToBlur: null,
    idLastTextareaToFocus: null
};
