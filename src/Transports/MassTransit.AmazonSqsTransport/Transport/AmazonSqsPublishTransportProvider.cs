namespace MassTransit.AmazonSqsTransport.Transport
{
    using System;
    using System.Threading.Tasks;
    using Transports;


    public class AmazonSqsPublishTransportProvider :
        IPublishTransportProvider
    {
        readonly IAmazonSqsHostControl _host;

        public AmazonSqsPublishTransportProvider(IAmazonSqsHostControl host)
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
