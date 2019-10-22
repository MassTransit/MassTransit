namespace MassTransit.HttpTransport.Builders
{
    using Configuration;
    using Contexts;
    using MassTransit.Builders;
    using Transport;


    public class HttpReceiveEndpointBuilder :
        ReceiveEndpointBuilder,
        IHttpReceiveEndpointBuilder
    {
        readonly IHttpReceiveEndpointConfiguration _configuration;
        readonly IHttpHostControl _host;

        public HttpReceiveEndpointBuilder(IHttpHostControl host, IHttpReceiveEndpointConfiguration configuration)
            : base(configuration)
        {
            _configuration = configuration;
            _host = host;
        }

        public HttpReceiveEndpointContext CreateReceiveEndpointContext()
        {
            return new HttpTransportReceiveEndpointContext(_host, _configuration);
        }
    }
}
