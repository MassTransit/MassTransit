namespace MassTransit.RabbitMqTransport.Transport
{
    using System.Threading.Tasks;
    using Integration;
    using Transports;


    public interface IRabbitMqHostControl :
        IRabbitMqHost,
        IBusHostControl
    {
        Task<ISendTransport> CreateSendTransport(RabbitMqEndpointAddress address);

        Task<ISendTransport> CreateSendTransport(RabbitMqEndpointAddress address, IModelContextSupervisor modelContextSupervisor);

        Task<ISendTransport> CreatePublishTransport<T>()
            where T : class;
    }
}
