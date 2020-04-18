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
        IDispatchMetrics,
        IReceiveObserverConnector,
        IProbeSite
    {
        Task Dispatch(ReceiveContext context, ReceiveLockContext receiveLock = default);
    }
}
