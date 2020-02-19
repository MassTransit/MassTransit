namespace MassTransit.Transports
{
    using System.Threading.Tasks;
    using GreenPipes;
    using Metrics;
    using Pipeline;


    /// <summary>
    /// Dispatches a prepared <see cref="ReceiveContext"/> to a <see cref="IReceivePipe"/>.
    /// </summary>
    public interface IReceivePipeDispatcher :
        IReceiveObserverConnector,
        IProbeSite
    {
        Task Dispatch(ReceiveContext context, ReceiveLockContext receiveLock = default);

        event ZeroActiveDispatchHandler ZeroActivity;

        long DispatchCount { get; }
        int ActiveDispatchCount { get; }
        int MaxConcurrentDispatchCount { get; }

        DeliveryMetrics GetDeliveryMetrics();
    }
}
