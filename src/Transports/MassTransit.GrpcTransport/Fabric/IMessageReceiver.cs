namespace MassTransit.GrpcTransport.Fabric
{
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;


    /// <summary>
    /// Receives messages from a queue
    /// </summary>
    public interface IMessageReceiver :
        IProbeSite
    {
        Task Deliver(GrpcTransportMessage message, CancellationToken cancellationToken);
    }
}
