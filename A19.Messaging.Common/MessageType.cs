namespace A19.Messaging.Common
{
    public enum MessageType
    {
        /// <summary>
        ///     Send a pong to a server after receiving a ping.
        /// </summary>
        Pong = -2, 
        
        /// <summary>
        ///     A ping set to the server to see if it's still up.
        /// </summary>
        Ping = -1,
        
        /// <summary>
        ///     A message making a request.
        /// </summary>
        Request = 0,
        
        /// <summary>
        ///     The reply to a message.
        /// </summary>
        Reply = 1,
        
        /// <summary>
        ///     An event being sent.
        /// </summary>
        Event = 2,
        
        /// <summary>
        ///     Requesting the status of a message.
        /// </summary>
        Status = 3
    }
}