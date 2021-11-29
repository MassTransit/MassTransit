namespace MassTransit.GrpcTransport.Fabric
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Util;


    public class MessageDirectExchange :
        IMessageExchange
    {
        readonly ConcurrentDictionary<string, Connectable<IMessageSink<GrpcTransportMessage>>> _sinks;

        public MessageDirectExchange(string name, StringComparer comparer = default)
        {
            Name = name;

            _sinks = new ConcurrentDictionary<string, Connectable<IMessageSink<GrpcTransportMessage>>>(comparer ?? StringComparer.Ordinal);
        }

        public IEnumerable<IMessageSink<GrpcTransportMessage>> Sinks
        {
            get
            {
                var sinks = new List<IMessageSink<GrpcTransportMessage>>();

                foreach (KeyValuePair<string, Connectable<IMessageSink<GrpcTransportMessage>>> sink in _sinks)
                    sink.Value.ForEach(s => sinks.Add(s));

                return sinks;
            }
        }

        public string Name { get; }

        public async Task Deliver(DeliveryContext<GrpcTransportMessage> context)
        {
            if (_sinks.TryGetValue(context.Message.RoutingKey ?? "", out Connectable<IMessageSink<GrpcTransportMessage>> forKey))
            {
                await forKey.ForEachAsync(async sink =>
                {
                    if (context.WasAlreadyDelivered(sink))
                        return;

                    await sink.Deliver(context).ConfigureAwait(false);

                    context.Delivered(sink);
                }).ConfigureAwait(false);
            }
        }

        public ConnectHandle Connect(IMessageSink<GrpcTransportMessage> sink, string routingKey)
        {
            Connectable<IMessageSink<GrpcTransportMessage>> forKey =
                _sinks.GetOrAdd(routingKey ?? "", key => new Connectable<IMessageSink<GrpcTransportMessage>>());

            return forKey.Connect(sink);
        }

        public Task Send(GrpcTransportMessage message, CancellationToken cancellationToken)
        {
            var deliveryContext = new GrpcDeliveryContext(message, cancellationToken);

            return Deliver(deliveryContext);
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("exchange");
            scope.Add("name", Name);
            scope.Add("type", "direct");

            var sinkScope = scope.CreateScope("keys");

            foreach (KeyValuePair<string, Connectable<IMessageSink<GrpcTransportMessage>>> sink in _sinks)
            {
                var routingKeyScope = sinkScope.CreateScope(string.IsNullOrWhiteSpace(sink.Key) ? "<empty>" : sink.Key);

                sink.Value.ForEach(s => s.Probe(routingKeyScope));
            }
        }

        public override string ToString()
        {
            return $"Exchange({Name})";
        }
    }
}
