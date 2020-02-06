namespace MassTransit.HttpTransport.Contexts
{
    using System.IO;
    using Hosting;
    using Context;
    using Microsoft.AspNetCore.Http;


    public class HttpReceiveContext :
        BaseReceiveContext
    {
        readonly HttpContext _httpContext;
        byte[] _body;

        public HttpReceiveContext(HttpContext httpContext, bool redelivered, ReceiveEndpointContext topology)
            : base(redelivered, topology)
        {
            _httpContext = httpContext;

            HeaderProvider = new HttpHeaderProvider(httpContext.Request.Headers);
        }

        protected override IHeaderProvider HeaderProvider { get; }

        public HttpContext HttpContext => _httpContext;

        public override byte[] GetBody()
        {
            if (_body == null)
                GetBodyAsByteArray();

            return _body;
        }

        public override Stream GetBodyStream()
        {
            if (_body == null)
                GetBodyAsByteArray();

            return new MemoryStream(_body, false);
        }

        void GetBodyAsByteArray()
        {
            using (var ms = new MemoryStream())
            {
                _httpContext.Request.Body.CopyTo(ms);

                _body = ms.ToArray();
            }
        }
    }
}
