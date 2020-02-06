namespace MassTransit.HttpTransport.Contexts
{
    using System.IO;
    using System.Net.Http;
    using Hosting;
    using Context;


    public class HttpClientReceiveContext :
        BaseReceiveContext
    {
        readonly Stream _responseStream;
        byte[] _body;

        public HttpClientReceiveContext(HttpResponseMessage responseMessage, Stream responseStream, bool redelivered, ReceiveEndpointContext context)
            : base(redelivered, context)
        {
            _responseStream = responseStream;

            HeaderProvider = new HttpClientHeaderProvider(responseMessage.Headers);
        }

        protected override IHeaderProvider HeaderProvider { get; }

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
                _responseStream.CopyTo(ms);

                _body = ms.ToArray();
            }
        }
    }
}
