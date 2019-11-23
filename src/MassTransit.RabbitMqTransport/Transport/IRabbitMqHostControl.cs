namespace MassTransit.RabbitMqTransport.Transport
{
    using System.Threading.Tasks;
    using Transports;


    public interface IRabbitMqHostControl :
        IRabbitMqHost,
        IBusHostControl
    {
        Task<ISendTransport> CreateSendTransport(RabbitMqEndpointAddress address);

        Task<ISendTransport> CreatePublishTransport<T>()
            where T : class;
    }
}
