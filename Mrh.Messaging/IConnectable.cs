using System;
using System.Threading.Tasks;

namespace Mrh.Messaging
{
    public interface IConnectable : IDisposable
    {
        Task Connect();

        Task Reconnect();
    }
}