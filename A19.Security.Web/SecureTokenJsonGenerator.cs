using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using A19.Core;
using Mrh.Messaging.Json;
using Mrh.Monad;

namespace A19.Security.Web
{
    /// <summary>
    ///     A signed token for passing values around that the user can't modify.'
    ///  
    /// </summary>
    public class SecureTokenJsonGenerator
    {
        private readonly byte[] _sharedSecret;
        private readonly ISecureKeyGenerator _secureKeyGenerator;

        private const int TOKEN_LENGTH = 64;
        private const int EXPIRE_DATE_START = TOKEN_LENGTH;
        private const int EXPIRE_DATE_LENGTH = 8;
        private const int RANDOM_NUMBER_START = EXPIRE_DATE_START + EXPIRE_DATE_LENGTH;
        private const int RANDOM_NUMBER_LENGTH = 8;
        private const int HEADER_LENGTH = EXPIRE_DATE_LENGTH + RANDOM_NUMBER_LENGTH + 8 + TOKEN_LENGTH;
        private const int BODY_START = HEADER_LENGTH;

        public SecureTokenJsonGenerator(
            ISecureKeyGenerator secureKeyGenerator,
            byte[] sharedSecret)
        {
            _sharedSecret = sharedSecret;
            _secureKeyGenerator = secureKeyGenerator;
        }

        public Span<byte> CreateToken<T>(
            T msg,
            DateTime expires)
        {
            var json = JsonHelper.Encode(msg);
            var rawArray = new byte[TotalLength(json.Length)];
            var resultEncoding = new Span<byte>(rawArray);
            var date = expires.ToUnix();
            if (
                BitConverter.TryWriteBytes(resultEncoding.Slice(EXPIRE_DATE_START, EXPIRE_DATE_LENGTH), date))
            {
                _secureKeyGenerator.Generate(resultEncoding.Slice(RANDOM_NUMBER_START, RANDOM_NUMBER_LENGTH));
                var num = Encoding.UTF8.GetBytes(json, resultEncoding.Slice(
                    BODY_START,
                    json.Length));
                var token = SecurityHelpers.Hmac512(
                    _sharedSecret,
                    rawArray,
                    TOKEN_LENGTH,
                    rawArray.Length - TOKEN_LENGTH);
                if (token.TryCopyTo(resultEncoding.Slice(0, TOKEN_LENGTH)))
                {
                    return resultEncoding;
                }
            }
            return null;
        }

        public IResultMonad<T> DecodeToken<T>(byte[] value)
        {
            var valueSpan = new Span<byte>(value);
            var token = valueSpan.Slice(0, TOKEN_LENGTH);
            var compareToken = SecurityHelpers.Hmac512(
                _sharedSecret,
                value,
                TOKEN_LENGTH,
                value.Length - TOKEN_LENGTH);
            if (compareToken.SpanByteCompare(token))
            {
                var currentDate = DateTime.Now.ToUniversalTime();
                var expires =
                    DateTimeExt.FromUnix(BitConverter.ToInt64(valueSpan.Slice(EXPIRE_DATE_START, EXPIRE_DATE_LENGTH)));
                if (currentDate < expires)
                {
                    var bodyS = valueSpan.Slice(BODY_START);
                    var json = Encoding.UTF8.GetString(bodyS);
                    return JsonHelper.Decode<T>(json).ToResultMonad();
                }
                else
                {
                    return new ResultError<T>(new List<string>
                    {
                        "Token has expired."
                    });
                }
            }
            else
            {
                return new ResultError<T>(new List<string>
                {
                    "The tokens do not match."
                });
            }
        }

        public int TotalLength(int jsonLength)
        {
            return HEADER_LENGTH + jsonLength;
        }
    }
}