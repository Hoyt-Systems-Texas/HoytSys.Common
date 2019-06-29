namespace Mrh.Messaging
{
    public interface IMessageSetting
    {
        short ServerId { get; }
        
        int ShiftNumber { get; }
        
        int MaxFrameSize { get; }
    }
}