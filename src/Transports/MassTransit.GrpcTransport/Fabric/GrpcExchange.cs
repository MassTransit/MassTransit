namespace MassTransit.GrpcTransport.Fabric
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;


    public class GrpcExchange :
        IGrpcExchange
    {
        readonly HashSet<IMessageSink<GrpcTransportMessage>> _sinks;

        public GrpcExchange(string name)
        {
            Name = name;
            _sinks = new HashSet<IMessageSink<GrpcTransportMessage>>();
        }

        public IEnumerable<IMessageSink<GrpcTransportMessage>> Sinks => _sinks;

        public string Name { get; }

        public async Task Deliver(DeliveryContext<GrpcTransportMessage> context)
        {
            foreach (IMessageSink<GrpcTransportMessage> sink in _sinks)
            {
                if (context.WasAlreadyDelivered(sink))
                    continue;

                await sink.Deliver(context).ConfigureAwait(false);

                context.Delivered(sink);
            }
        }

        public void Connect(IMessageSink<GrpcTransportMessage> sink)
        {
            _sinks.Add(sink);
        }

        public Task Send(GrpcTransportMessage message, CancellationToken cancellationToken)
        {
            var deliveryContext = new GrpcDeliveryContext(message, cancellationToken);

            return Deliver(deliveryContext);
        }

        public override string ToString()
        {
            return $"Exchange({Name})";
        }
    }
}