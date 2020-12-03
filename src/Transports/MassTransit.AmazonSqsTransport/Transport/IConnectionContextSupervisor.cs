namespace MassTransit.AmazonSqsTransport.Transport
{
    using Transports;


    public interface IConnectionContextSupervisor :
        ITransportSupervisor<ConnectionContext>,
        ISendTransportProvider,
        IPublishTransportProvider
    {
    }
}
