var textareas = document.getElementsByTagName("textarea");
for (var i=0; i < textareas.length; i++)
{
    textareas[i].onchange = function() { return window.external.TextareaOnChange(this.id, this.value); };
    textareas[i].onselect = function() { return CallTextareaOnSelect(this); };
}
