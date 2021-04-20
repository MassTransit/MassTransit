namespace MassTransit.GrpcTransport.Fabric
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using GreenPipes.Util;


    public class GrpcFanOutExchange :
        IGrpcExchange
    {
        readonly Connectable<IMessageSink<GrpcTransportMessage>> _sinks;

        public GrpcFanOutExchange(string name)
        {
            Name = name;

            _sinks = new Connectable<IMessageSink<GrpcTransportMessage>>();
        }

        public IEnumerable<IMessageSink<GrpcTransportMessage>> Sinks
        {
            get
            {
                var sinks = new List<IMessageSink<GrpcTransportMessage>>();
                _sinks.All(s =>
                {
                    sinks.Add(s);
                    return true;
                });

                return sinks;
            }
        }

        public string Name { get; }

        public async Task Deliver(DeliveryContext<GrpcTransportMessage> context)
        {
            await _sinks.ForEachAsync(async sink =>
            {
                if (context.WasAlreadyDelivered(sink))
                    return;

                await sink.Deliver(context).ConfigureAwait(false);

                context.Delivered(sink);
            }).ConfigureAwait(false);
        }

        public Task Send(GrpcTransportMessage message, CancellationToken cancellationToken)
        {
            var deliveryContext = new GrpcDeliveryContext(message, cancellationToken);

            return Deliver(deliveryContext);
        }

        public ConnectHandle Connect(IMessageSink<GrpcTransportMessage> sink, string routingKey)
        {
            return _sinks.Connect(sink);
        }

        public override string ToString()
        {
            return $"Exchange({Name})";
        }
    }
}
