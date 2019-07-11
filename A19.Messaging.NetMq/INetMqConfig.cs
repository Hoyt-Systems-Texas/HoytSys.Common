namespace A19.Messaging.NetMq
{
    public interface INetMqConfig
    {
        string IncomingConnection { get; }
        
        string OutgoingConnection { get;  }
    }
}