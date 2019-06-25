namespace Mrh.Messaging
{
    public enum MessageResultType
    {
        /// <summary>
        ///     Message was processed successfully.
        /// </summary>
        Success = 0,
        /// <summary>
        ///     An unexpected error has occurred.
        /// </summary>
        Error = 10,
        /// <summary>
        ///     The server is to busy to process the request.
        /// </summary>
        Busy = 11,
        /// <summary>
        ///     Access was denied to the user making the request
        /// </summary>
        AccessDenied = 12
    }
}