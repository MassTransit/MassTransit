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
        readonly IHttpHostControl _host;

        public HttpTransportReceiveEndpointContext(IHttpHostControl host, IHttpReceiveEndpointConfiguration configuration)
            : base(configuration)
        {
            _host = host;
        }

        public ReceiveEndpointContext CreateResponseEndpointContext(HttpContext httpContext)
        {
            return new HttpResponseReceiveEndpointContext(this, httpContext);
        }

        protected override ISendTransportProvider CreateSendTransportProvider()
        {
            return new HttpSendTransportProvider(_host, this);
        }

        protected override IPublishTransportProvider CreatePublishTransportProvider()
        {
            return new PublishTransportProvider(SendTransportProvider);
        }
    }
}
