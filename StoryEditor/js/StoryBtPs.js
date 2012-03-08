var textareas = document.getElementsByTagName("textarea");
for (var i=0; i < textareas.length; i++)
{
    textareas[i].onchange = function() { return window.external.TextareaOnChange(this.id, this.value); };
    textareas[i].onkeyup = function () { return window.external.TextareaOnKeyUp(this.id, this.value); };
    textareas[i].onmousedown = function () { return window.external.OnTextareaMouseDown(this.id, this.value); };
}
$('textarea').attr('placeholder', function () {
    if ($(this).hasClass('LangVernacular'))
        return VernacularLanguageName();
    else if ($(this).hasClass('LangNationalBt'))
        return NationalBtLanguageName();
    else if ($(this).hasClass('LangInternationalBt'))
        return InternationalBtLanguageName();
    else if ($(this).hasClass('LangFreeTranslation'))
        return FreeTranslationLanguageName();
    else
        return "error in StoryBtPs.js";
});
