namespace MassTransit.HttpTransport.Transport
{
    using System;
    using System.Threading.Tasks;
    using Context;
    using Transports;


    public class HttpSendTransportProvider :
        ISendTransportProvider
    {
        readonly IHttpHostControl _host;
        readonly ReceiveEndpointContext _receiveEndpointContext;

        public HttpSendTransportProvider(IHttpHostControl host, ReceiveEndpointContext receiveEndpointContext)
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

    }
}
