namespace MassTransit.Courier
{
    using System;
    using System.Threading;
    using Context;
    using MassTransit.Serialization;
    using Metadata;
    using Util;


    public class RoutingSlipSendContext<T> :
        MessageSendContext<T>
        where T : class
    {
        public RoutingSlipSendContext(T message, CancellationToken cancellationToken, Uri destinationAddress)
            : base(message, cancellationToken)
        {
            DestinationAddress = destinationAddress;

            Serializer = new JsonMessageSerializer();
        }

        public MessageEnvelope GetMessageEnvelope()
        {
            var envelope = new JsonMessageEnvelope(this, Message, TypeMetadataCache<T>.MessageTypeNames);

            return envelope;
        }
    }
}
