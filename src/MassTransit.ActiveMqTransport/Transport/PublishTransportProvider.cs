namespace MassTransit.ActiveMqTransport.Transport
{
    using System;
    using System.Threading.Tasks;
    using Transports;


    public class PublishTransportProvider :
        IPublishTransportProvider
    {
        readonly IActiveMqHostControl _host;

        public PublishTransportProvider(IActiveMqHostControl host)
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
