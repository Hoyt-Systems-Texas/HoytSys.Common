using System;
using System.Threading.Tasks;

namespace Mrh.Messaging.Client
{
    /// <summary>
    ///     A client to use when you want to forward the call to another protocol like REST.
    /// </summary>
    /// <typeparam name="TPayloadType">The type for the payload.</typeparam>
    /// <typeparam name="TBody">The type for the body.</typeparam>
    public interface IForwardingClient<TPayloadType, TBody> where TPayloadType: struct
    {
        void Connect();

        void Disconnect();

        Task<Message<TPayloadType, TBody>> Send<T>(
            TPayloadType payloadType,
            T body,
            Guid userId);
    }
}