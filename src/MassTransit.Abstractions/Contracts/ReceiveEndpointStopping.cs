namespace MassTransit
{
    public interface ReceiveEndpointStopping :
        ReceiveEndpointEvent
    {
        bool Removed { get; }
    }
}
