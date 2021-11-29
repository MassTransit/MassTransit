namespace MassTransit.GrpcTransport
{
    using System.Net.Mime;
    using Fabric;
    using Transports;


    public class GrpcReceiveContext :
        BaseReceiveContext
    {
        public GrpcReceiveContext(GrpcTransportMessage message, GrpcReceiveEndpointContext receiveEndpointContext)
            : base(false, receiveEndpointContext)
        {
            Message = message;

            Body = new BytesMessageBody(message.Body);

            HeaderProvider = new GrpcHeaderProvider(message.Headers);
        }

        public GrpcTransportMessage Message { get; }

        protected override IHeaderProvider HeaderProvider { get; }

        public override MessageBody Body { get; }

        protected override ContentType GetContentType()
        {
            return ConvertToContentType(Message.ContentType);
        }
    }
}
