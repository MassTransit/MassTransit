namespace MassTransit.GrpcTransport.Fabric
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using GreenPipes.Util;


    public class GrpcDirectExchange :
        IGrpcExchange
    {
        readonly ConcurrentDictionary<string, Connectable<IMessageSink<GrpcTransportMessage>>> _sinks;

        public GrpcDirectExchange(string name, StringComparer comparer = default)
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
                {
                    sink.Value.All(s =>
                    {
                        sinks.Add(s);
                        return true;
                    });
                }

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

        public override string ToString()
        {
            return $"Exchange({Name})";
        }
    }
}
