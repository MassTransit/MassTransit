#nullable enable
namespace MassTransit.GrpcTransport.Fabric
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Contracts;
    using Transports.Fabric;


    public class GrpcDeliveryContext :
        DeliveryContext<GrpcTransportMessage>
    {
        readonly HashSet<IMessageSink<GrpcTransportMessage>> _delivered;

        public GrpcDeliveryContext(GrpcTransportMessage message, CancellationToken cancellationToken)
        {
            Message = message;
            CancellationToken = cancellationToken;

            _delivered = new HashSet<IMessageSink<GrpcTransportMessage>>();
        }

        public CancellationToken CancellationToken { get; }

        public GrpcTransportMessage Message { get; }
        public string? RoutingKey => Message.RoutingKey;
        public DateTime? EnqueueTime => Message.EnqueueTime;

        public long? ReceiverId =>
            Message.Message.Deliver.DestinationCase == Deliver.DestinationOneofCase.Receiver
                ? Message.Message.Deliver.Receiver.ReceiverId
                : default(long?);

        public bool WasAlreadyDelivered(IMessageSink<GrpcTransportMessage> sink)
        {
            return _delivered.Contains(sink);
        }

        public void Delivered(IMessageSink<GrpcTransportMessage> sink)
        {
            _delivered.Add(sink);
        }
    }
}
