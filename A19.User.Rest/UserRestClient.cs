using System.Threading.Tasks;
using A19.Messaging.Rest;
using A19.User.Common;

namespace A19.User.Rest
{
    public class UserRestClient
    {

        private readonly string _connectionUrl;

        public UserRestClient(
            string connectionUrl)
        {
            _connectionUrl = connectionUrl;
        }
        
    }
}