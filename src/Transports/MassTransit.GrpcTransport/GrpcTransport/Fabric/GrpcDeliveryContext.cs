namespace MassTransit.GrpcTransport.Fabric
{
    using System.Collections.Generic;
    using System.Threading;


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
