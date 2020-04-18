namespace MassTransit.Transports.Metrics
{
    public interface IDeliveryTracker
    {
        int ActiveDeliveryCount { get; }
        long DeliveryCount { get; }
        int MaxConcurrentDeliveryCount { get; }
        IDelivery BeginDelivery();
    }
}
