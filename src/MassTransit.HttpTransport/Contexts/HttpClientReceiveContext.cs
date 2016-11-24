namespace MassTransit.HttpTransport.Contexts
{
    using System.IO;
    using System.Net.Http;
    using Context;


    public class HttpClientReceiveContext :
        BaseReceiveContext
    {
        readonly HttpResponseMessage _requestContext;

        public HttpClientReceiveContext(HttpResponseMessage requestContext, IHeaderProvider provider, bool redelivered, IReceiveObserver receiveObserver,
            ISendEndpointProvider sendEndpointProvider, IPublishEndpointProvider publishEndpointProvider)
            : base(requestContext.RequestMessage.RequestUri, redelivered, receiveObserver, sendEndpointProvider, publishEndpointProvider)
        {
            _requestContext = requestContext;
            HeaderProvider = provider;
        }

        protected override IHeaderProvider HeaderProvider { get; }

        public HttpResponseMessage RequestContext => _requestContext;

        protected override Stream GetBodyStream()
        {
            // TODO: Chris isn't going to like this.
            return _requestContext.Content.ReadAsStreamAsync().Result;
        }
    }
}