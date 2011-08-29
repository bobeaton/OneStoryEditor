using System;
using System.Text;
using System.Net;
using System.Web;
using System.IO;

using Gapi.Core;
using Gapi.Json;

namespace Gapi.Language
{
    public static class Translator
    {
        const string LanguageTranslateUrl = "http://ajax.googleapis.com/ajax/services/language/translate?v={0}&q={1}&langpair={2}|{3}";
        const string LanguageTranslateUrlWithKey = "http://ajax.googleapis.com/ajax/services/language/translate?v={0}&q={1}&langpair={2}|{3}&key={4}";
        const string LanguageDetectUrl = "http://ajax.googleapis.com/ajax/services/language/detect?v={0}&q={1}";
        const string LanguageApiVersion = "1.0";

        public static string ApiKey = null;

        public static string Translate(string phrase, Language sourceLanguage, Language targetLanguage)
        {
            return Translate(phrase, ref sourceLanguage, targetLanguage);
        }

        public static string Translate(string phrase, Language targetLanguage)
        {
            Language sourceLanguage = Language.Unknown;
            return Translate(phrase, ref sourceLanguage, targetLanguage);
        }

        public static string Translate(string phrase, Language targetLanguage, out Language detectedSourceLanguage)
        {
            detectedSourceLanguage = Language.Unknown;
            return Translate(phrase, ref detectedSourceLanguage, targetLanguage);
        }

        private static string Translate(string phrase, ref Language sourceLanguage, Language targetLanguage)
        {
            if ((phrase == null)||(phrase == ""))
                return "";

            string url;
            if (ApiKey == null)
            {
                url = string.Format(LanguageTranslateUrl, LanguageApiVersion,
                    Uri.EscapeDataString(phrase),
                    LanguageHelper.GetLanguageString(sourceLanguage),
                    LanguageHelper.GetLanguageString(targetLanguage));
            }
            else
            {
                url = string.Format(LanguageTranslateUrl, LanguageApiVersion,
                    Uri.EscapeDataString(phrase),
                    LanguageHelper.GetLanguageString(sourceLanguage),
                    LanguageHelper.GetLanguageString(targetLanguage), 
                    ApiKey);
            }

            string responseData = CoreHelper.PerformRequest(url);

            JsonObject jsonObject = CoreHelper.ParseGoogleAjaxAPIResponse(responseData);

            // Translation response validation
            // Get 'responseData'
            if (jsonObject.ContainsKey("responseData") == false)
                throw new GapiException("Invalid response - no responseData: " + responseData);

            if (!(jsonObject["responseData"] is JsonObject))
                throw new GapiException("Invalid response - responseData is not JsonObject: " + responseData);

            // Get 'translatedText'
            JsonObject responseContent = (JsonObject)jsonObject["responseData"];
            if (responseContent.ContainsKey("translatedText") == false)
                throw new GapiException("Invalid response - no translatedText: " + responseData);

            if (!(responseContent["translatedText"] is JsonString))
                throw new GapiException("Invalid response - translatedText is not JsonString: " + responseData);

            string translatedPhrase = ((JsonString)responseContent["translatedText"]).Value;

            // If there's a detected language - return it
            if ( (responseContent.ContainsKey("detectedSourceLanguage") == true)&&
                 (responseContent["detectedSourceLanguage"] is JsonString))
            {
                JsonString detectedSourceLanguage = (JsonString)responseContent["detectedSourceLanguage"];
                sourceLanguage = LanguageHelper.GetLanguage(detectedSourceLanguage.Value);
            }

            return Uri.UnescapeDataString(translatedPhrase);
        }

        public static Language Detect(string phrase)
        {
            bool isReliable;
            double confidence;

            return Detect(phrase, out isReliable, out confidence);
        }

        public static Language Detect(string phrase, out bool isReliable, out double confidence)
        {
            if ((phrase == null) || (phrase == ""))
                throw new GapiException("No phrase to detect from");

            string url = string.Format(LanguageDetectUrl, 
                LanguageApiVersion,
                Uri.EscapeDataString(phrase));

            string responseData = CoreHelper.PerformRequest(url);

            JsonObject jsonObject = CoreHelper.ParseGoogleAjaxAPIResponse(responseData);

            // Translation response validation
            // Get 'responseData'
            if (jsonObject.ContainsKey("responseData") == false)
                throw new GapiException("Invalid response - no responseData: " + responseData);

            if (!(jsonObject["responseData"] is JsonObject))
                throw new GapiException("Invalid response - responseData is not JsonObject: " + responseData);

            // Get 'translatedText'
            JsonObject responseContent = (JsonObject)jsonObject["responseData"];
            if (responseContent.ContainsKey("language") == false)
                throw new GapiException("Invalid response - no language: " + responseData);

            if (!(responseContent["language"] is JsonString))
                throw new GapiException("Invalid response - language is not JsonString: " + responseData);

            string language = ((JsonString)responseContent["language"]).Value;

            isReliable = false;
            confidence = 0;

            if ((responseContent.ContainsKey("isReliable") == true) &&
                 (responseContent["isReliable"] is JsonBoolean))
            {
                isReliable = ((JsonBoolean)responseContent["isReliable"]).Value;
            }

            if ((responseContent.ContainsKey("confidence") == true) &&
                 (responseContent["confidence"] is JsonNumber))
            {
                confidence = ((JsonNumber)responseContent["confidence"]).DoubleValue;
            }

            return LanguageHelper.GetLanguage(language);
        }
    }
}
