namespace MassTransit.GrpcTransport.Contexts
{
    using System.IO;
    using System.Net.Mime;
    using Context;
    using Fabric;


    public class GrpcReceiveContext :
        BaseReceiveContext
    {
        readonly byte[] _body;

        public GrpcReceiveContext(GrpcTransportMessage message, GrpcReceiveEndpointContext receiveEndpointContext)
            : base(false, receiveEndpointContext)
        {
            _body = message.Body;
            Message = message;

            HeaderProvider = new GrpcHeaderProvider(message.Headers);
        }

        public GrpcTransportMessage Message { get; }

        protected override IHeaderProvider HeaderProvider { get; }

        public override byte[] GetBody()
        {
            return _body;
        }

        public override Stream GetBodyStream()
        {
            return new MemoryStream(_body, 0, _body.Length, false);
        }

        protected override ContentType GetContentType()
        {
            return ConvertToContentType(Message.ContentType);
        }
    }
}
