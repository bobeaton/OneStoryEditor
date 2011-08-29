using System;
using System.Text;
using System.Net;
using Gapi.Json;
using System.IO;

namespace Gapi.Core
{
    internal class CoreHelper
    {
        public static IWebProxy Proxy = null;

        public static void SetProxy(IWebProxy newProxy)
        {
            CoreHelper.Proxy = newProxy;
        }

        public static WebRequest BuildWebRequest(string url)
        {
            WebRequest webRequest = WebRequest.Create(url);

            if (CoreHelper.Proxy != null)
                webRequest.Proxy = CoreHelper.Proxy;
            else
            {
                IWebProxy proxy = webRequest.Proxy;
                if (proxy != null)
                    proxy.Credentials = CredentialCache.DefaultCredentials;
            }

            return webRequest;
        }

        public static string PerformRequest(string url)
        {
            WebRequest request = CoreHelper.BuildWebRequest(url);

            using (WebResponse response = request.GetResponse())
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                return reader.ReadToEnd();
            }
        }

        public static JsonObject ParseGoogleAjaxAPIResponse(string responseData)
        {
            // Common response validation
            // Validate 'responseStatus'
            JsonObject jsonObject = JsonObject.Parse(responseData);
            if (jsonObject == null)
                throw new GapiException("No JsonObject found in the response: " + responseData);

            string responseDetails = null;
            if ((jsonObject.ContainsKey("responseDetails")) &&
                 (jsonObject["responseDetails"] is JsonString))
                responseDetails = ((JsonString)jsonObject["responseDetails"]).Value;

            JsonHelper.ValidateJsonField(jsonObject, "responseStatus", typeof(JsonNumber));

            if (((JsonNumber)jsonObject["responseStatus"]).IntValue != 200)
            {
                if (responseDetails == null)
                    throw new GapiException("ResponseStatus: " + ((JsonNumber)jsonObject["responseStatus"]).IntValue.ToString() + ", Response data: " + responseData);
                else
                    throw new GapiException(string.Format("ResponseStatus: {0}, Reason: {1}, Response data: {2}",
                        ((JsonNumber)jsonObject["responseStatus"]).IntValue,
                        responseDetails,
                        responseData));
            }

            return jsonObject;
        }
    }
}
