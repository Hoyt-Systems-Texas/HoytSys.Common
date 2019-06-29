using Mrh.Messaging.Json;
using Mrh.Messaging.NetMq;

namespace ServiceApplicationTester
{
    public class JsonSettings : IJsonSetting
    {
        public int MaxFrameSize => 0x10000 - MessageSettings.HEADER_SIZE;
    }
}