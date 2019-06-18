using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace TestApp.Hubs
{
    public class ChatHub : Hub
    {
        public async Task NewMessage(string username, string password)
        {
            
        }
    }
}