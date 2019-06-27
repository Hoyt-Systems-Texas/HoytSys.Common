using System;

namespace Mrh.Messaging
{
    /// <summary>
    ///     Represents a unique identifier for a message.
    /// </summary>
    public struct MessageIdentifier
    {
        public readonly Guid ConnectionId;

        public readonly int CorrelationId;

        public MessageIdentifier(
            Guid connectionId,
            int correlationId)
        {
            this.ConnectionId = connectionId;
            this.CorrelationId = correlationId;
        }

        public bool Equals(MessageIdentifier other)
        {
            return string.Equals(ConnectionId, other.ConnectionId) && CorrelationId == other.CorrelationId;
        }

        public override bool Equals(object obj)
        {
            return obj is MessageIdentifier other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((ConnectionId != null ? ConnectionId.GetHashCode() : 0) * 397) ^ CorrelationId;
            }
        }
    }
}