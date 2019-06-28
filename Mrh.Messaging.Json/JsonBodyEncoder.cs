using Mrh.Messaging.Common;

namespace Mrh.Messaging.Json
{
    /// <summary>
    ///     The json body encoder.
    /// </summary>
    public class JsonBodyEncoder : IBodyEncoder<string>
    {
        public T Decode<T>(string body)
        {
            return JsonHelper.Decode<T>(body);
        }

        public string Encode<T>(T value)
        {
            return JsonHelper.Encode(value);
        }
    }
}