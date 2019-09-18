namespace MassTransit.HttpTransport.Transport
{
    using System;
    using System.Threading.Tasks;
    using Transports;


    public class PublishTransportProvider :
        IPublishTransportProvider
    {
        readonly ISendTransportProvider _sendTransportProvider;

        public PublishTransportProvider(ISendTransportProvider sendTransportProvider)
        {
            _sendTransportProvider = sendTransportProvider;
        }

        public Task<ISendTransport> GetPublishTransport<T>(Uri publishAddress)
            where T : class
        {
            return _sendTransportProvider.GetSendTransport(publishAddress);
        }
    }
}
