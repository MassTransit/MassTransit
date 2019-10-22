namespace MassTransit.RabbitMqTransport.Transport
{
    using System;
    using System.Threading.Tasks;
    using Transports;


    public interface IRabbitMqHostControl :
        IRabbitMqHost,
        IBusHostControl
    {
        Task<ISendTransport> CreateSendTransport(Uri address);

        Task<ISendTransport> CreatePublishTransport<T>()
            where T : class;
    }
}
