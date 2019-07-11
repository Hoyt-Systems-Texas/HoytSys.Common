using System;

namespace A19.Messaging.NetMq.Client
{
    public class SubscriptionTypeMismatchedException : Exception
    {
        public SubscriptionTypeMismatchedException(string msg) : base(msg)
        {
            
        }
    }
}