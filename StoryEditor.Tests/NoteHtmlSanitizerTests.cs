using NUnit.Framework;

namespace OneStoryProjectEditor.Tests
{
    [TestFixture]
    public class NoteHtmlSanitizerTests
    {
        [Test]
        public void KeepsCreateNoteSpans()
        {
            const string note = "Cerita : <SPAN class=\"LangVernacular StoryLine\">idop</SPAN> vs: Cerita : <SPAN class=\"LangNationalBt StoryLine\">hidup</SPAN>";
            Assert.That(NoteHtmlSanitizer.Sanitize(note),
                        Is.EqualTo("Cerita : <span class=\"LangVernacular StoryLine\">idop</span> vs: Cerita : <span class=\"LangNationalBt StoryLine\">hidup</span>"));
        }

        [Test]
        public void KeepsSimpleFormatting()
        {
            Assert.That(NoteHtmlSanitizer.Sanitize("<p><i>Ttg: Catatan Kons:</i></p>save <B>only</B> <EM>so</EM>"),
                        Is.EqualTo("<p><i>Ttg: Catatan Kons:</i></p>save <b>only</b> <em>so</em>"));
        }

        [Test]
        public void RemovesScriptWithItsContent()
        {
            const string note = "Ln: 11 Add Note\n<SCRIPT type=text/javascript>\n  var textareas = document.getElementsByTagName(\"textarea\");\n  for (var i = 0; i < textareas.length; i++) { textareas[i].onkeyup = function () { return window.external.TextareaOnKeyUp(this.id, this.value); }; }\n</SCRIPT>\nafter";
            var result = NoteHtmlSanitizer.Sanitize(note);
            Assert.That(result, Does.Not.Contain("script").IgnoreCase);
            Assert.That(result, Does.Not.Contain("window.external"));
            Assert.That(result, Does.Contain("after"));
        }

        [Test]
        public void UnwrapsDivAndDropsEventHandlersAndIds()
        {
            const string note = "<DIV id=tp_1_0_0 class=TextAreaStyle>\n<P ondblclick=OnDoubleClick(this) id=tp_1_0_1 class=LangInternationalBT>ok</P></DIV>";
            var result = NoteHtmlSanitizer.Sanitize(note);
            Assert.That(result, Does.Not.Contain("div").IgnoreCase);
            Assert.That(result, Does.Not.Contain("ondblclick").IgnoreCase);
            Assert.That(result, Does.Not.Contain("id=").IgnoreCase);
            Assert.That(result, Does.Contain("<p class=\"LangInternationalBT\">ok</p>"));
        }

        [Test]
        public void DropsUnknownClasses()
        {
            Assert.That(NoteHtmlSanitizer.Sanitize("<span class=\"LangVernacular highlight readonly\">x</span>"),
                        Is.EqualTo("<span class=\"LangVernacular\">x</span>"));
        }

        [Test]
        public void UnwrapsStoredInternalLinksToTheirText()
        {
            const string note = "masukkan <A onclick=\"return OnBibRefJump(this);\" \nhref=\"bibleViewer.setReference\" name=\"Luk 3:23\">Luk 3:23</A> sebagai DA. ke <A \nonclick=\"return OnVerseLineJump(this);\" class=LocalizationStyle \nhref=\"conNote.jumpToLine\" name=4>baris 4</A>:";
            Assert.That(NoteHtmlSanitizer.Sanitize(note), Is.EqualTo("masukkan Luk 3:23 sebagai DA. ke baris 4:"));
        }

        [Test]
        public void HttpLinksAreUnwrappedToTextAndUrl()
        {
            Assert.That(NoteHtmlSanitizer.Sanitize("<a href=\"https://example.org/a?b=1\" onclick=\"evil()\">here</a>"),
                        Is.EqualTo("here https://example.org/a?b=1"));
            Assert.That(NoteHtmlSanitizer.Sanitize("<a href=\"https://example.org/a\">https://example.org/a</a>"),
                        Is.EqualTo("https://example.org/a"));
        }

        [Test]
        public void SanitizedOutputHasNoAnchorsOrHrefs()
        {
            var result = NoteHtmlSanitizer.Sanitize("<a href=\"ftp://x\">a</a> <a name=n>b</a> <a href=\"https://x/y\">c</a>");
            Assert.That(result, Does.Not.Contain("<a").And.Not.Contain("href"));
        }

        [TestCase("<a href=\"  JaVa&#x09;Script:alert(1)\">x</a>")]
        [TestCase("<a href=\"data:text/html,x\">x</a>")]
        [TestCase("<p style=\"width:expression(alert(1))\">x</p>")]
        [TestCase("<img src=x onerror=alert(1)>")]
        [TestCase("<iframe src=//e></iframe>")]
        [TestCase("<!--[if IE]><script>alert(1)</script><![endif]-->")]
        [TestCase("<!-- <script>x</script> -->")]
        [TestCase("<a href=\"https://x/?Luk 3:23<img src=x onerror=alert(1)>\">t</a>")]
        public void OutputHasNothingExecutable(string note)
        {
            AssertSafe(NoteHtmlSanitizer.Sanitize(note));
            AssertSafe(NoteHtmlSanitizer.ToReadOnlyHtml(note));
        }

        [Test]
        public void HyperlinkingSanitizedNoteInjectsNothing()
        {
            const string note = "<a href=\"https://x/?Luk 3:23<img src=x onerror=alert(1)>\">t</a>";
            // (SetHyperlinks adds its own onClick attributes to the links it makes, so only look for ours)
            var result = ConsultNoteDataConverter.SetHyperlinks(NoteHtmlSanitizer.ToReadOnlyHtml(note));
            Assert.That(result, Does.Not.Contain("onerror").IgnoreCase);
            Assert.That(result, Does.Not.Contain("<img").IgnoreCase);
        }

        private static void AssertSafe(string html)
        {
            Assert.That(html, Does.Not.Contain("<script").IgnoreCase);
            Assert.That(html, Does.Not.Contain("javascript:").IgnoreCase);
            Assert.That(html, Does.Not.Contain("vbscript:").IgnoreCase);
            Assert.That(html, Does.Not.Contain("data:").IgnoreCase);
            Assert.That(html, Does.Not.Contain("expression(").IgnoreCase);
            Assert.That(html, Does.Not.Contain("<img").IgnoreCase);
            Assert.That(html, Does.Not.Contain("<iframe").IgnoreCase);
            Assert.That(html, Does.Not.Contain("<object").IgnoreCase);
            Assert.That(html, Does.Not.Contain("<embed").IgnoreCase);
            Assert.That(html, Does.Not.Contain("<style").IgnoreCase);
            Assert.That(html, Does.Not.Contain("onerror").IgnoreCase);
            Assert.That(System.Text.RegularExpressions.Regex.IsMatch(html, @"\son\w+\s*=",
                            System.Text.RegularExpressions.RegexOptions.IgnoreCase), Is.False, html);
        }

        [TestCase("JH: Re: <RTL>  Your retellings", "JH: Re: &lt;RTL&gt;  Your retellings")]
        [TestCase("parmatma said <malti  see cont note>that", "parmatma said &lt;malti  see cont note&gt;that")]
        [TestCase("use a <space> rather than a <dot>.", "use a &lt;space&gt; rather than a &lt;dot&gt;.")]
        public void ShowsUserPseudoTagsAsText(string note, string expected)
        {
            Assert.That(NoteHtmlSanitizer.Sanitize(note), Is.EqualTo(expected));
        }

        [Test]
        public void KeepsBoldItalicMarkersAndEntities()
        {
            Assert.That(NoteHtmlSanitizer.Sanitize("$bold$ *it* [B&amp;B] a&nbsp;b"),
                        Is.EqualTo("$bold$ *it* [B&amp;B] a&nbsp;b"));
        }

        [Test]
        public void ToReadOnlyHtml_KeepsLineBreaks()
        {
            Assert.That(NoteHtmlSanitizer.ToReadOnlyHtml("line 1\r\nline <B>2</B>\r\n\r\nline 3"),
                        Is.EqualTo("line 1<br />line <b>2</b><br /><br />line 3"));
        }

        [Test]
        public void ToReadOnlyHtml_NullAndEmpty()
        {
            Assert.That(NoteHtmlSanitizer.ToReadOnlyHtml(null), Is.Null);
            Assert.That(NoteHtmlSanitizer.ToReadOnlyHtml(""), Is.EqualTo(""));
        }

        [Test]
        public void ToReadOnlyHtmlWithHighlight_HighlightsPlainRange()
        {
            Assert.That(NoteHtmlSanitizer.ToReadOnlyHtmlWithHighlight("find the word here", 9, 4, "<mark>", "</mark>"),
                        Is.EqualTo("find the <mark>word</mark> here"));
        }

        [Test]
        public void ToReadOnlyHtmlWithHighlight_RangeInsideMarkupFallsBackWithoutSentinels()
        {
            // the found range is inside the tag "<B>" itself (e.g. a search for "B>"), which the
            //  sanitizer rewrites; no private-use sentinel characters may leak into the page
            var result = NoteHtmlSanitizer.ToReadOnlyHtmlWithHighlight("x <B>y</B>", 3, 2, "<mark>", "</mark>");
            Assert.That(result, Does.Not.Contain("\uE000").And.Not.Contain("\uE001"));
            Assert.That(result, Does.Contain("<b>y</b>"));
        }

        [Test]
        public void ToReadOnlyHtmlWithHighlight_OutOfRangeIsIgnored()
        {
            Assert.That(NoteHtmlSanitizer.ToReadOnlyHtmlWithHighlight("abc", 2, 5, "<mark>", "</mark>"),
                        Is.EqualTo("abc"));
        }
    }
}
