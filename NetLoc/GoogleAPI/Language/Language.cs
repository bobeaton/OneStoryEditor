using System;
using System.Collections;
using System.Text;

namespace Gapi.Language
{
    public enum Language
    {
        Afrikaans,
        Albanian,
        Amharic,
        Arabic,
        Armenian,
        Azerbaijani,
        Basque,
        Belarusion,
        Bengali,
        Bihari,
        Bulgarian,
        Burmese,
        Catalan,
        Cherokee,
        Chinese,
        ChineseSimplified,
        ChineseTraditiona,
        Croatian,
        Czech,
        Danish,
        Dhivehi,
        Dutch,
        English,
        Esperanto,
        Estonian,
        Filipino,
        Finnish,
        French,
        Galician,
        Georgian,
        German,
        Greek,
        Guarani,
        Gujarati,
        Hebrew,
        Hindi, 
        Hungarian,
        Icelandic,
        Indonesian,
        Inuktitut,
        Italian,
        Japanese,
        Kannada,
        Kazakh,
        Khmer,
        Korean,
        Kurdish,
        Kyrgyz,
        Laothian,
        Latvian,
        Lithuanian,
        Macedonian,
        Malay,
        Malayalam,
        Maltese,
        Marathi,
        Mongolian,
        Nepali,
        Norwegian,
        Oriya,
        Pashto,
        Persian,
        Polish,
        Portuguese,
        Punjabi,
        Romanian, 
        Russian,
        Sanskrit,
        Serbian,
        Sindhi,
        Sinhalese,
        Slovak,
        Slovenian,
        Spanish,
        Swahili,
        Swedish,
        Tajik,
        Tamil,
        //Tagalog,
        Telugu,
        Thai,
        Tibetan,
        Turkish,
        Ukrainian,
        Urdu,
        Uzbek,
        Uighur,
        Vietnamese,
        Unknown
    }

    public static class LanguageHelper
    {
        static Hashtable _stringToLang = new Hashtable();

        static LanguageHelper()
        {
            _stringToLang = new Hashtable();
            foreach (Language lang in Enum.GetValues(typeof(Language)))
            {
                _stringToLang.Add(GetLanguageString(lang), lang);
            }
        }

        public static Language GetLanguage(string lang)
        {
            if (lang == null)
                throw new NullReferenceException("lang");

            if (_stringToLang.ContainsKey(lang))
                return (Language)_stringToLang[lang];

            throw new NotSupportedException("Unsupported language string: " + lang);
        }

        public static string GetLanguageString(Language lang)
        {
            switch (lang)
            {
                case Language.Afrikaans:
                    return "af";
                case Language.Albanian:
                    return "sq";
                case Language.Amharic:
                    return "am";
                case Language.Arabic:
                    return "ar";
                case Language.Armenian:
                    return "hy";
                case Language.Azerbaijani:
                    return "az";
                case Language.Basque:
                    return "eu";
                case Language.Belarusion:
                    return "be";
                case Language.Bengali:
                    return "bn";
                case Language.Bihari:
                    return "bh";
                case Language.Bulgarian:
                    return "bg";
                case Language.Burmese:
                    return "my";
                case Language.Catalan:
                    return "ca";
                case Language.Cherokee:
                    return "chr";
                case Language.Chinese:
                    return "zh";
                case Language.ChineseSimplified:
                    return "zh-CN";
                case Language.ChineseTraditiona:
                    return "zh-TW";
                case Language.Croatian:
                    return "hr";
                case Language.Czech:
                    return "cs";
                case Language.Danish:
                    return "da";
                case Language.Dhivehi:
                    return "dv";
                case Language.Dutch:
                    return "nl";
                case Language.English:
                    return "en";
                case Language.Esperanto:
                    return "eo";
                case Language.Estonian:
                    return "et";
                case Language.Filipino:
                    return "tl";
                case Language.Finnish:
                    return "fi";
                case Language.French:
                    return "fr";
                case Language.Galician:
                    return "gl";
                case Language.Georgian:
                    return "ka";
                case Language.German:
                    return "de";
                case Language.Greek:
                    return "el";
                case Language.Guarani:
                    return "gn";
                case Language.Gujarati:
                    return "gu";
                case Language.Hebrew:
                    return "iw";
                case Language.Hindi:
                    return "hi";
                case Language.Hungarian:
                    return "hu";
                case Language.Icelandic:
                    return "is";
                case Language.Indonesian:
                    return "id";
                case Language.Inuktitut:
                    return "iu";
                case Language.Italian:
                    return "it";
                case Language.Japanese:
                    return "ja";
                case Language.Kannada:
                    return "kn";
                case Language.Kazakh:
                    return "kk";
                case Language.Khmer:
                    return "km";
                case Language.Korean:
                    return "ko";
                case Language.Kurdish:
                    return "ku";
                case Language.Kyrgyz:
                    return "ky";
                case Language.Laothian:
                    return "lo";
                case Language.Latvian:
                    return "lv";
                case Language.Lithuanian:
                    return "lt";
                case Language.Macedonian:
                    return "mk";
                case Language.Malay:
                    return "ms";
                case Language.Malayalam:
                    return "ml";
                case Language.Maltese:
                    return "mt";
                case Language.Marathi:
                    return "mr";
                case Language.Mongolian:
                    return "mn";
                case Language.Nepali:
                    return "ne";
                case Language.Norwegian:
                    return "no";
                case Language.Oriya:
                    return "or";
                case Language.Pashto:
                    return "ps";
                case Language.Persian:
                    return "fa";
                case Language.Polish:
                    return "pl";
                case Language.Portuguese:
                    return "pt-PT";
                case Language.Punjabi:
                    return "pa";
                case Language.Romanian:
                    return "ro";
                case Language.Russian:
                    return "ru";
                case Language.Sanskrit:
                    return "sa";
                case Language.Serbian:
                    return "sr";
                case Language.Sindhi:
                    return "sd";
                case Language.Sinhalese:
                    return "si";
                case Language.Slovak:
                    return "sk";
                case Language.Slovenian:
                    return "sl";
                case Language.Spanish:
                    return "es";
                case Language.Swahili:
                    return "sw";
                case Language.Swedish:
                    return "sv";
                //case Language.Tagalog:
                //    return "tl";
                case Language.Tajik:
                    return "tg";
                case Language.Tamil:
                    return "ta";
                case Language.Telugu:
                    return "te";
                case Language.Thai:
                    return "th";
                case Language.Tibetan:
                    return "bo";
                case Language.Turkish:
                    return "tr";
                case Language.Uighur:
                    return "ug";
                case Language.Ukrainian:
                    return "uk";
                case Language.Urdu:
                    return "ur";
                case Language.Uzbek:
                    return "uz";
                case Language.Vietnamese:
                    return "vi";
                case Language.Unknown:
                    return "";
            }

            throw new NotSupportedException("Language not supported: " + lang);
        }
    }
}
