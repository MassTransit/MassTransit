namespace MassTransit.HttpTransport.Hosting
{
    using System;
    using System.IO;
    using Context;


    public class HttpReceiveContext :
        BaseReceiveContext
    {
        readonly Stream _body;

        public HttpReceiveContext(Uri inputAddress, Stream body, IHeaderProvider provider, bool redelivered, IReceiveObserver receiveObserver)
            : base(inputAddress, redelivered, receiveObserver)
        {
            _body = body;
            HeaderProvider = provider;
        }

        protected override IHeaderProvider HeaderProvider { get; }

        protected override Stream GetBodyStream()
        {
            return _body;
        }
    }
}