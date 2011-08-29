using System;
using System.Collections.Generic;
using System.Text;
using NetLoc.com.microsofttranslator.api;
using System.Diagnostics;
using Gapi.Language;

namespace NetLoc
{
    public class AutoTranslator
    {
        static readonly string appId = "9CBC27D3318918EF9A7AB7CBBE08A47318C38FA7";      // Paratext Microsoft App Id

        public enum Engines { Microsoft, Google };

        /// <summary>
        /// Current translation engine
        /// </summary>
        public static Engines Engine = Engines.Google;

        public static string TranslateText(string fromLang, string toLang, string text)
        {
            if (Engine == Engines.Google)
                return TranslateTextGoogle(fromLang, toLang, text);
            if (Engine == Engines.Microsoft)
                return TranslateTextMicrosoft(fromLang, toLang, text);
            throw new ArgumentException("Unknown engine");
        }

        public static string TranslateTextMicrosoft(string fromLang, string toLang, string text)
        {
            // Handle special cases
            if (fromLang == "zh-Hans") fromLang = "zh-CHS";
            if (toLang == "zh-Hans") toLang = "zh-CHS";
            if (fromLang == "zh-Hant") fromLang = "zh-CHT";
            if (toLang == "zh-Hant") toLang = "zh-CHT";

            // Create an instance of the translator service
            Soap soapClient = new Soap();

            // Strip initial whitespace
            string initialWs = "";
            string restText = text;
            for (int i = 0; i < text.Length; i++)
            {
                if (!Char.IsWhiteSpace(text[i]))
                {
                    restText = text.Substring(i);
                    break;
                }
                initialWs += text[i];
            }

            return initialWs + soapClient.Translate(appId, restText, fromLang, toLang);
        }

        /// <summary>
        /// Google translation
        /// </summary>
        /// <param name="fromLang"></param>
        /// <param name="toLang"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string TranslateTextGoogle(string fromLang, string toLang, string text)
        {
            // Handle special cases
            if (fromLang == "zh-Hans") fromLang = "zh-CN";
            if (toLang == "zh-Hans") toLang = "zh-CN";
            if (fromLang == "zh-Hant") fromLang = "zh-TW";
            if (toLang == "zh-Hant") toLang = "zh-TW";

            Translator.ApiKey = "ABQIAAAAmToBmc1o2prRsdsE8HO6DRQD65lbrLyKyRpXCyZ8lvJRRigzQBS2hlAcSFL7FEwJwDqOgYk6jgYi1Q";

            Language fromLanguage = LanguageHelper.GetLanguage(fromLang);
            Language toLanguage = LanguageHelper.GetLanguage(toLang);

            return Translator.Translate(text, fromLanguage, toLanguage);
        }
    
    }
}
