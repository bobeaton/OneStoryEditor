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
