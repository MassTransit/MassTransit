namespace MassTransit.RabbitMqTransport.Transport
{
    using System;
    using System.Threading.Tasks;
    using Transports;


    public class PublishTransportProvider :
        IPublishTransportProvider
    {
        readonly IRabbitMqHostControl _host;

        public PublishTransportProvider(IRabbitMqHostControl host)
        {
            _host = host;
        }

        public Task<ISendTransport> GetPublishTransport<T>(Uri publishAddress)
            where T : class
        {
            return _host.CreatePublishTransport<T>();
        }
    }
}
