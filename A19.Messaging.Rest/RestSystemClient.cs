using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using A19.Messaging.Common;
using Mrh.Messaging.Json;
using Mrh.Monad;

namespace A19.Messaging.Rest
{
    public class RestSystemClient : IRestClient
    {
        private static readonly HttpClient HttpClient = new HttpClient();
        private readonly string _url;

        public RestSystemClient(
            string url)
        {
            _url = url;
        }

        public async Task<IResultMonad<TR>> PostAsync<TBody, TR>(
            string service,
            string action,
            TBody body)
        {
            var bodyS = JsonHelper.Encode(body);
            using (var requestMessage = new HttpRequestMessage(
                HttpMethod.Post,
                $"{_url}/{service}/{action}"))
            {
                requestMessage.Content = new StringContent(bodyS, Encoding.UTF8, "application/json");
                var result = await HttpClient.SendAsync(requestMessage);
                if (result.StatusCode == HttpStatusCode.OK)
                {
                    var stringR = await result.Content.ReadAsStringAsync();
                    var r = JsonHelper.Decode<TextMessageResult>(stringR);
                    return r.ResultType.ToResultMonad<TR>(stringR);
                }
                else
                {
                    return this.HandleErrors<TR>(result);
                }
            }
        }

        private IResultMonad<TR> HandleErrors<TR>(HttpResponseMessage result)
        {
            if (result.StatusCode == HttpStatusCode.Forbidden)
            {
                // TODO need to reauthenticate.
                return new ResultAccessDenied<TR>(new List<string>
                {
                    "Access denied.  Need to reauthenticate."
                });
            }
            else if (result.StatusCode == HttpStatusCode.Unauthorized)
            {
                return new ResultAccessDenied<TR>(new List<string>
                {
                    "Access denied."
                });
            }
            else
            {
                return new ResultError<TR>(
                    new List<string>
                    {
                        "Unexpected error has occurred."
                    });
            }
        }

        public class TextMessageResult : IMessageResult<object>
        {
            public MessageResultType ResultType { get; set; }
        }
    }
}