namespace MassTransit.Azure.ServiceBus.Core
{
    using System.Threading.Tasks;
    using Transports;


    public interface IServiceBusHostControl :
        IServiceBusHost,
        IBusHostControl
    {
        Task<ISendTransport> CreateSendTransport(ServiceBusEndpointAddress address);

        Task<ISendTransport> CreatePublishTransport<T>()
            where T : class;
    }
}
