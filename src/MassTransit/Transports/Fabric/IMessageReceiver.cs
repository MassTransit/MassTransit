namespace MassTransit.Transports.Fabric
{
    using System.Threading;
    using System.Threading.Tasks;


    /// <summary>
    /// Receives messages from a queue
    /// </summary>
    public interface IMessageReceiver<in T> :
        IProbeSite
        where T : class
    {
        Task Deliver(T message, CancellationToken cancellationToken);
    }
}
