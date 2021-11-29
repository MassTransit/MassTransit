namespace MassTransit.Transports
{
    using System.Threading.Tasks;


    /// <summary>
    /// Dispatches a prepared <see cref="ReceiveContext" /> to a <see cref="IReceivePipe" />.
    /// </summary>
    public interface IReceivePipeDispatcher :
        IConsumePipeConnector,
        IConsumeObserverConnector,
        IConsumeMessageObserverConnector,
        IRequestPipeConnector,
        IDispatchMetrics,
        IReceiveObserverConnector,
        IProbeSite
    {
        Task Dispatch(ReceiveContext context, ReceiveLockContext receiveLock = default);
    }
}
