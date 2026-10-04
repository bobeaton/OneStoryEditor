// sends a textarea's text the way the onchange always did: IE's htmlText (which keeps the line breaks that
//  'value' loses once we've put <br>s and highlight spans in the box), minus spans, <br> -> \r\n
function oseSendChange(ta, bQuiet) {
    if (ta.readOnly)    // if edits aren't allowed, then our job is done here!
        return;

    // an empty box shows its language's name (with class 'hasPlaceholder'), but that isn't data
    if ($(ta).hasClass('hasPlaceholder')) {
        ose.send('textChanged', { id: ta.id, value: '', quiet: bQuiet });
        return;
    }

    var range = ta.createTextRange();
    ose.send('textChanged', { id: ta.id, ieHtml: ToNewLines(regexRemoveSpan(range.htmlText)), quiet: bQuiet });
}

var textareas = document.getElementsByTagName("textarea");
for (var i = 0; i < textareas.length; i++) {
    textareas[i].onchange = function () { oseSendChange(this, false); };
}

// the Story/BT flush sends the last-focused box the onchange way (replaces PaneCommon's)
ose.on('flush', function () {
    var ta = document.getElementById(ose.lastTextareaId() || '');
    if (ta && (ta.nodeName == 'TEXTAREA'))
        oseSendChange(ta, true);
    return {};
});

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

// IE9 mode doesn't show 'placeholder' itself, so (as the blur handler does) show the language name
//  grayed in boxes that start out empty
$('textarea').each(function () {
    if ($(this).attr('placeholder') != '' && $(this).val() == '') {
        $(this).val($(this).attr('placeholder')).addClass('hasPlaceholder');
    }
});
