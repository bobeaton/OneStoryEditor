function OnBibRefJump(btn) {
    var spans = document.getElementsByTagName("div");
    var arr = jQuery.makeArray(spans);
    window.external.PassArray();
    window.external.OnBibRefJump(btn.id);
    return false; // cause the href navigation to not happen
}
function OnLineOptionsButton(btn)
{
    window.external.OnLineOptionsButton(btn.id);
    return false;
}
function OnVerseLineJump(link)
{
    window.external.OnVerseLineJump(link.name);
    return false; // cause the href navigation to not happen
}
function DisplayHtml(strFunction) {
    var curVal = $('#status').val();
    $('#status').val(curVal + " " + strFunction);
}
function removeSelection(jqtextarea) {
    if (jqtextarea.attr('selectedText')) {
        jqtextarea.removeAttr("selectionStart");
        jqtextarea.removeAttr("selectionEnd");
        jqtextarea.removeAttr("selectedText");
    }
}
// the following code used to be in onblur (and works just
//  fine that way in IE). But for some reason, in a 
//  WebBrowser in a WindowsForm, the onblur is triggered
//  even when I'm still in focus. this is called by the
//  onfocus handler (before getting to the work of the new
//  textarea in focus) as well as by my WindowsForm app
//  if the WebBrowser itself loses focus (e.g. to go to some
//  other control in the app).
function TriggerMyBlur() {
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
            $(oldThis).html(pre + "<span class='highlighted'>" + oldThis.selectedText + "</span>" + post);
        }
    }
    window.oseConfig.idLastTextareaToBlur = null;
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
        /*
        else if (this.selectedText) {
        var text = $(this).val();
        var pre = (this.selectionStart > 0)
        ? text.substring(0, this.selectionStart)
        : "";
        var post = (this.selectionEnd < (text.length - 1))
        ? text.substring(this.selectionEnd)
        : "";
        $(this).html(pre + "<span class='highlighted'>" + this.selectedText + "</span>" + post);
        }
        */
        DisplayHtml(event.type);
    }).focus(function (event) {
        if ($(this).attr('placeholder') != '' && $(this).val() == $(this).attr('placeholder')) {
            $(this).val('').removeClass('hasPlaceholder');
        }
        // for some reason, in IE, the following code works
        //  fine in onblur, but in a WebBrowser in IE
        TriggerMyBlur();

        // now if we previously added the span (what we do
        //  during TriggerMyBlur but for this control now)
        //  then it has to be removed when we begin editing
        //  this textarea again
        if ($(this).has("span").length) {
            // setting the html with only the value will 
            //  remove the span element.
            $(this).html($(this).val());

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
    });
});
window.oseConfig =
{
    idLastTextareaToBlur: null,
    idLastTextareaToFocus: null
};
