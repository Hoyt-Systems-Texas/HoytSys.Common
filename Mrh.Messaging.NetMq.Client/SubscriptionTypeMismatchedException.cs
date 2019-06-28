using System;

namespace Mrh.Messaging.NetMq.Client
{
    public class SubscriptionTypeMismatchedException : Exception
    {
        public SubscriptionTypeMismatchedException(string msg) : base(msg)
        {
            
        }
    }
}