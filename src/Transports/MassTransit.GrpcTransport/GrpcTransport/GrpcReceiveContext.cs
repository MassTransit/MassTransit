namespace MassTransit.GrpcTransport
{
    using System;
    using System.Collections.Generic;
    using System.Net.Mime;
    using Context;
    using Fabric;
    using Transports;


    public class GrpcReceiveContext :
        BaseReceiveContext,
        TransportReceiveContext
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

        public IDictionary<string, object> GetTransportProperties()
        {
            var properties = new Lazy<Dictionary<string, object>>(() => new Dictionary<string, object>());

            if (!string.IsNullOrWhiteSpace(Message.RoutingKey))
                properties.Value[GrpcTransportPropertyNames.RoutingKey] = Message.RoutingKey;

            return properties.IsValueCreated ? properties.Value : null;
        }
    }
}
