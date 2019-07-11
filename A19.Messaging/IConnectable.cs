using System;
using System.Threading.Tasks;

namespace A19.Messaging
{
    public interface IConnectable : IDisposable
    {
        Task Connect();

        Task Reconnect();
    }
}