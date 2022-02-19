#nullable enable
namespace MassTransit.Transports.Fabric
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Util;


    public class MessageDirectExchange<T> :
        IMessageExchange<T>
        where T : class
    {
        readonly ConcurrentDictionary<string, Connectable<IMessageSink<T>>> _sinks;

        public MessageDirectExchange(string name, StringComparer? comparer = default)
        {
            Name = name;

            _sinks = new ConcurrentDictionary<string, Connectable<IMessageSink<T>>>(comparer ?? StringComparer.Ordinal);
        }

        public IEnumerable<IMessageSink<T>> Sinks
        {
            get
            {
                var sinks = new List<IMessageSink<T>>();

                foreach (KeyValuePair<string, Connectable<IMessageSink<T>>> sink in _sinks)
                    sink.Value.ForEach(s => sinks.Add(s));

                return sinks;
            }
        }

        public string Name { get; }

        public async Task Deliver(DeliveryContext<T> context)
        {
            if (_sinks.TryGetValue(context.RoutingKey ?? "", out Connectable<IMessageSink<T>> forKey))
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

        public ConnectHandle Connect(IMessageSink<T> sink, string? routingKey)
        {
            Connectable<IMessageSink<T>> forKey = _sinks.GetOrAdd(routingKey ?? "", key => new Connectable<IMessageSink<T>>());

            return forKey.Connect(sink);
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("exchange");
            scope.Add("name", Name);
            scope.Add("type", "direct");

            var sinkScope = scope.CreateScope("keys");

            foreach (KeyValuePair<string, Connectable<IMessageSink<T>>> sink in _sinks)
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
