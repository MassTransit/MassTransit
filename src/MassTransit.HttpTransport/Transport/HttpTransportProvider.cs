namespace MassTransit.HttpTransport.Transport
{
    using System;
    using System.Threading.Tasks;
    using Context;
    using Transports;


    public class HttpTransportProvider :
        ISendTransportProvider,
        IPublishTransportProvider
    {
        readonly IHttpHostControl _host;
        readonly ReceiveEndpointContext _receiveEndpointContext;

        public HttpTransportProvider(IHttpHostControl host, ReceiveEndpointContext receiveEndpointContext)
        {
            _host = host;
            _receiveEndpointContext = receiveEndpointContext;
        }

        public Task<ISendTransport> GetSendTransport(Uri address)
        {
            return _host.CreateSendTransport(address, _receiveEndpointContext);
        }

        public Uri NormalizeAddress(Uri address)
        {
            return address;
        }

        public Task<ISendTransport> GetPublishTransport<T>(Uri publishAddress)
            where T : class
        {
            return _host.CreateSendTransport(publishAddress, _receiveEndpointContext);
        }
    }
}
