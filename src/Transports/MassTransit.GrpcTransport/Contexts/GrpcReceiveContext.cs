namespace MassTransit.GrpcTransport.Contexts
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Fabric;


    public class GrpcReceiveContext :
        BaseReceiveContext,
        IAsyncDisposable
    {
        readonly byte[] _body;
        readonly CancellationTokenRegistration _registration;

        public GrpcReceiveContext(GrpcTransportMessage message, GrpcReceiveEndpointContext receiveEndpointContext, CancellationToken cancellationToken)
            : base(false, receiveEndpointContext)
        {
            _body = message.Body;
            Message = message;

            HeaderProvider = new GrpcHeaderProvider(message.Headers);

            if (cancellationToken.CanBeCanceled)
                _registration = cancellationToken.Register(() => Cancel());
        }

        public GrpcTransportMessage Message { get; }

        protected override IHeaderProvider HeaderProvider { get; }

        public ValueTask DisposeAsync()
        {
            return _registration.DisposeAsync();
        }

        public override byte[] GetBody()
        {
            return _body;
        }

        public override Stream GetBodyStream()
        {
            return new MemoryStream(_body, 0, _body.Length, false);
        }
    }
}
