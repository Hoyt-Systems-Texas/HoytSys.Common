using Mrh.Messaging.NetMq;

namespace ServiceApplicationTester
{
    public class NetMqConfig : INetMqConfig
    {
        public string IncomingConnection => "tcp://localhost:5004";
        public string OutgoingConnection => "tcp://localhost:5003";
    }
}