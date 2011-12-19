function OnBibRefJump(btn) {
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
function CallOnFocus(textarea) {
    return window.external.TextareaOnFocus(textarea.id);
}
function CallOnBlur(textarea) {
    return window.external.TextareaOnBlur(textarea.id, textarea.value);
}
function CallTextareaOnSelect(textarea) 
{
    if (!document.selection) {
        alert("document.selection is null");
        return false;
    }
    if (!textarea.contentWindow) {
        alert("textarea.contentWindow is null");
        return false;
    }
    var allText = textarea.contentWindow.document.selection.createRange().parentElement().innerText;
    var selText = textarea.contentWindow.document.selection.createRange().text;
    return window.external.TextareaOnSelect(textarea.id, allText.indexof(selText), selText.length);
}
jQuery(function () {
    jQuery.support.placeholder = false;
    var test = document.createElement('textarea');
    if ('placeholder' in test) jQuery.support.placeholder = true;
});
$(function () {
    if (!$.support.placeholder) {
        var active = document.activeElement;
        $('textarea').focus(function () {
            if ($(this).attr('placeholder') != '' && $(this).val() == $(this).attr('placeholder')) {
                $(this).val('').removeClass('hasPlaceholder');
            }
        }).blur(function () {
            if ($(this).attr('placeholder') != '' && ($(this).val() == '' || $(this).val() == $(this).attr('placeholder'))) {
                $(this).val($(this).attr('placeholder')).addClass('hasPlaceholder');
            }
        });
        $('textarea').blur();
        $(active).focus();
        $('form').submit(function () {
            $(this).find('.hasPlaceholder').each(function () { $(this).val(''); });
        });
    }
});
