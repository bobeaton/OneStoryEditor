using System;
using System.Collections.Generic;

namespace OneStoryProjectEditor
{
    /// <summary>
    /// builds a new note's referring text from the highlighted selections ("Add note on selected text"),
    /// exactly as HtmlStoryBtControl.AddNote did from IE's span elements
    /// </summary>
    internal static class ReferringTextBuilder
    {
        // false if an item isn't in a recognisable textarea (AddNote gave up then, and still does)
        public static bool TryBuild(IEnumerable<HighlightedText> items, out string strReferringText)
        {
            var nLastSubItemIndex = -1;
            string strLastFieldReference = null;
            strReferringText = null;

            foreach (var item in items)
            {
                if (!HtmlStoryBtControl.TryGetTextAreaId(item.TextareaId, out var textAreaIdentifierParent))
                {
                    strReferringText = null;
                    return false;
                }

                // (this compares a reference name with a type name, so every item starts a new " vs: " part;
                //  kept as it was so notes come out the same)
                if (strLastFieldReference != textAreaIdentifierParent.FieldTypeName)
                {
                    if (!String.IsNullOrEmpty(strLastFieldReference))
                        strReferringText += " vs: ";

                    strLastFieldReference = textAreaIdentifierParent.FieldReferenceName;
                    strReferringText += strLastFieldReference;
                }
                else if (textAreaIdentifierParent.SubItemIndex != nLastSubItemIndex)
                {
                    if (nLastSubItemIndex != -1)
                        strReferringText += " &";
                    nLastSubItemIndex = textAreaIdentifierParent.SubItemIndex;
                }
                strReferringText += " " + SpanHtml(item);
            }

            // remove the highlight class so it isn't highlighted in the connote pane
            if (strReferringText != null)
            {
                strReferringText = strReferringText.Replace(" highlight", null);
                strReferringText = strReferringText.Replace(" readonly", null);
            }
            return true;
        }

        // what IE's span.OuterHtml gave (lower-case tag; NoteHtmlSanitizer keeps only span + Lang* classes anyway)
        public static string SpanHtml(HighlightedText item)
        {
            return "<span class=\"" + item.ClassName + "\">" +
                   HtmlText.LineBreaksToBr(HtmlText.Encode(item.Text ?? String.Empty)) +
                   "</span>";
        }
    }
}
