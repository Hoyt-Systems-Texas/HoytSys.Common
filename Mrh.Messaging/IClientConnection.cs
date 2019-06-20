namespace Mrh.Messaging
{
    public interface IClientConnection<TPayloadType, TBodyType> where TPayloadType: struct
    {
        /// <summary>
        ///     The message to send.
        /// </summary>
        /// <param name="msg">The message we are sending to the client.</param>
        void Send(Message<TPayloadType, TBodyType> msg);
    }
}