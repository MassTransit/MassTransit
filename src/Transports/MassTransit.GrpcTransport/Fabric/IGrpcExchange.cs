namespace MassTransit.GrpcTransport.Fabric
{
    using System.Threading;
    using System.Threading.Tasks;


    public interface IGrpcExchange :
        IMessageSink<GrpcTransportMessage>,
        IMessageSource<GrpcTransportMessage>
    {
        string Name { get; }

        Task Send(GrpcTransportMessage message, CancellationToken cancellationToken);
    }
}