namespace MassTransit.Context
{
    public interface ConsumerFaultInfo
    {
        string MessageType { get; }
        string ConsumerType { get; }
    }
}