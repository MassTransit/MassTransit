namespace MassTransit.Transports
{
    public interface IDispatchMetrics
    {
        int ActiveDispatchCount { get; }
        long DispatchCount { get; }
        int MaxConcurrentDispatchCount { get; }

        event ZeroActiveDispatchHandler ZeroActivity;

        DeliveryMetrics GetMetrics();
    }
}
