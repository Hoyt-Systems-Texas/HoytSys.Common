using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Mrh.Messaging.Json
{
    /// <summary>
    ///     Used to serialize the JSON.
    /// </summary>
    public static class JsonHelper
    {
        private static readonly DefaultContractResolver defaultContractResolver;

        private static readonly JsonSerializerSettings serializerSettings;

        static JsonHelper()
        {
            defaultContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy(),
            };
            serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = defaultContractResolver
            };
        }

        public static string Encode<T>(T body)
        {
            return JsonConvert.SerializeObject(body, serializerSettings);
        }

        public static T Decode<T>(string body)
        {
            return JsonConvert.DeserializeObject<T>(body);
        }
    }
}