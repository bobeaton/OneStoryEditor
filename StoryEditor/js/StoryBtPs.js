var textareas = document.getElementsByTagName("textarea");
for (var i = 0; i < textareas.length; i++) {
    // keep this one in this format (as opposed to jscript) since the C# code occasionally calls InvokeMember("onchange")
    //  and if we don't have it in regular java, then C# invoke can't access it (I think)
    // Now that we sometimes replace \r\n (which this.value can deal with), with things
    //  like <br>s (which this.value can't deal with), we have to switch them back before
    //  calling TextareaOnChange. Also remove any possible span.
    textareas[i].onchange = function () {
        if (this.readonly)  // if edits aren't allowed, then our job is done here!
        {
            DisplayHtml("onchange: but this.id: '" + this.id + "' is read-only, so not sending this.value: '" + this.value + "' to C#");
            return;
        }

        // we can get the html text (which includes the '<br>'s that we need to convert to \r\n or we lose the paragraph breaks on saving)
        //  if we use a text range (there is no 'this.htmlText'!?)
        var range = this.createTextRange();
        var newStr = ToNewLines(regexRemoveSpan(range.htmlText));
        DisplayHtml("onchange: this.id: '" + this.id + "', this.value: '" + this.value + "', range.htmlText: '" + range.htmlText + "', newStr: '" + newStr + "'");
        return window.external.TextareaOnChange(this.id, newStr);
    };
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
