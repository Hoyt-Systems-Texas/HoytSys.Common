using Mrh.Messaging;
using Mrh.Messaging.NetMq;

namespace NetMqTestCommon
{
    public class MessagingSetting : IMessageSetting
    {
        public short ServerId => 0;
        
        public int ShiftNumber => 40;
        public int MaxFrameSize => 0x10000 - MessageSettings.HEADER_SIZE;
    }
}