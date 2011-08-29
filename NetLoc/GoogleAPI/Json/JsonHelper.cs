using System;
using System.Collections.Generic;
using System.Text;

namespace Gapi.Json
{
    public static class JsonHelper
    {
        public static string GetJsonStringAsString(JsonObject jsonObject, string fieldName)
        {
            return GetJsonStringAsString(jsonObject, fieldName, null);
        }

        public static string GetJsonStringAsString(JsonObject jsonObject, string fieldName, string defaultValue)
        {
            if (jsonObject.ContainsKey(fieldName) &&
                (jsonObject[fieldName] is JsonString))
                return ((JsonString)jsonObject[fieldName]).Value;
            else
                return defaultValue;
        }

        public static void ValidateJsonField(JsonObject jsonObject, string fieldName, Type fieldType)
        {
            if (jsonObject == null)
                throw new NullReferenceException("jsonObject");

            if (jsonObject.ContainsKey(fieldName) == false)
                throw new JsonException(string.Format("Could not find key: '{0}' in JSON object", fieldName));

            if ((jsonObject[fieldName] == null)&&(fieldType != null))
                throw new NullReferenceException(string.Format("jsonObject field '{0}' == NULL", fieldName));

            if (jsonObject[fieldName].GetType() != fieldType)
                throw new JsonException(string.Format("JSON field: '{0}', type is '{1}', but expecting: '{2}'", fieldName, jsonObject[fieldName].GetType(), fieldType)); 
        }
    }
}
