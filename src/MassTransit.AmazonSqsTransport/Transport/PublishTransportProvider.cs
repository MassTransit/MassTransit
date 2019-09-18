namespace MassTransit.AmazonSqsTransport.Transport
{
    using System;
    using System.Threading.Tasks;
    using Transports;


    public class PublishTransportProvider :
        IPublishTransportProvider
    {
        readonly IAmazonSqsHostControl _host;

        public PublishTransportProvider(IAmazonSqsHostControl host)
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
