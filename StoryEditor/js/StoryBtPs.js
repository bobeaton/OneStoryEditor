var textareas = document.getElementsByTagName("textarea");
for (var i = 0; i < textareas.length; i++) {
    // keep this one in this format (as opposed to jscript) since the C# code occasionally calls InvokeMember("onchange")
    //  and if we don't have it in regular java, then C# invoke can't access it (I think)
    textareas[i].onchange = function() { return window.external.TextareaOnChange(this.id, this.value); };
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
