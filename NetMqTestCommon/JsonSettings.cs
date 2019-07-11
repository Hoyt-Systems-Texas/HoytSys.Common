using A19.Messaging.NetMq;
using Mrh.Messaging.Json;

namespace ServiceApplicationTester
{
    public class JsonSettings : IJsonSetting
    {
        public int MaxFrameSize => 0x10000 - MessageSettings.HEADER_SIZE;
    }
}