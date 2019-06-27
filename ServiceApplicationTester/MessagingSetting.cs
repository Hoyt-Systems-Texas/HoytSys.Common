using Mrh.Messaging;

namespace ServiceApplicationTester
{
    public class MessagingSetting : IMessageSetting
    {
        public short ServerId => 0;
        
        public int ShiftNumber => 40;
    }
}