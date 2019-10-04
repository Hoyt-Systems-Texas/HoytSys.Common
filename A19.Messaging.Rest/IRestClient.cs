using System.Collections.Generic;
using System.Threading.Tasks;
using Mrh.Monad;

namespace A19.Messaging.Rest
{
    public interface IRestClient
    {
        Task<IResultMonad<TR>> PostAsync<TBody, TR>(string service,
            string action,
            TBody body, string serviceKey = null);

        Task<IResultMonad<TR>> GetAsync<TR>(
            string service,
            string action,
            IEnumerable<KeyValuePair<string, string>> queryString = null);
    }
}
