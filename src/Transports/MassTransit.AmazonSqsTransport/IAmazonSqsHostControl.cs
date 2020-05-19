namespace MassTransit.AmazonSqsTransport
{
    using System.Threading.Tasks;
    using Transports;


    public interface IAmazonSqsHostControl :
        IBusHostControl,
        IAmazonSqsHost
    {
        Task<ISendTransport> CreateSendTransport(AmazonSqsEndpointAddress address);

        Task<ISendTransport> CreatePublishTransport<T>()
            where T : class;
    }
}
