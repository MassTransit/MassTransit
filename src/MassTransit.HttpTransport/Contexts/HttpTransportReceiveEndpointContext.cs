namespace MassTransit.HttpTransport.Contexts
{
    using Configuration;
    using Context;
    using Microsoft.AspNetCore.Http;
    using Topology;
    using Transport;


    public class HttpTransportReceiveEndpointContext :
        BaseReceiveEndpointContext,
        HttpReceiveEndpointContext
    {
        readonly HttpTransportProvider _transportProvider;

        public HttpTransportReceiveEndpointContext(IHttpHostControl host, IHttpReceiveEndpointConfiguration configuration)
            : base(configuration)
        {
            _transportProvider = new HttpTransportProvider(host, this);
        }

        public ReceiveEndpointContext CreateResponseEndpointContext(HttpContext httpContext)
        {
            return new HttpResponseReceiveEndpointContext(this, httpContext);
        }

        protected override ISendTransportProvider CreateSendTransportProvider()
        {
            return _transportProvider;
        }

        protected override IPublishTransportProvider CreatePublishTransportProvider()
        {
            return _transportProvider;
        }
    }
}
