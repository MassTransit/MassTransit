namespace MassTransit.GrpcTransport.Fabric
{
    using System.Threading;
    using System.Threading.Tasks;


    public interface IGrpcQueueConsumer
    {
        Task Consume(GrpcTransportMessage message, CancellationToken cancellationToken);
    }
}
