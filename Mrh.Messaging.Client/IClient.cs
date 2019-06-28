using System;
using System.Threading.Tasks;
using Mrh.Monad;

namespace Mrh.Messaging.Client
{
    public interface IClient<TPayloadType, TBody> where TPayloadType: struct
    {
        void Connect();

        void Disconnect();

        /// <summary>
        ///     Used to subscribe to and event.
        /// </summary>
        /// <param name="eventType">The type of event to subscribe to.</param>
        /// <returns>The observable that gets called when the vent fires.</returns>
        IObservable<T> Subscribe<T>(TPayloadType eventType);

        /// <summary>
        ///     Sends a message.
        /// </summary>
        /// <typeparam name="TR">The type to return.</typeparam>
        /// <typeparam name="T">The type you are sending.</typeparam>
        /// <returns>The result as an result monad.</returns>
        Task<IResultMonad<TR>> Send<T, TR>(
            TPayloadType payloadType,
            T message);
    }
}