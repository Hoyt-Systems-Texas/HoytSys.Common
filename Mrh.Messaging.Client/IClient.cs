using System;

namespace Mrh.Messaging.Client
{
    public interface IClient<TPayloadType, TBody, TCtx> where TPayloadType: struct where TCtx: Message<TPayloadType, TBody>
    {
        void Connect();

        void Disconnect();

        /// <summary>
        ///     Used to subscribe to and event.
        /// </summary>
        /// <param name="eventType">The type of event to subscribe to.</param>
        /// <returns>The observable that gets called when the vent fires.</returns>
        IObservable<TCtx> Subscribe(TPayloadType eventType);
    }
}