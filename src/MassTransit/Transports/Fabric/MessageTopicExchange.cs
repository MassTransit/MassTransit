#nullable enable
namespace MassTransit.Transports.Fabric
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;


    public class MessageTopicExchange<T> :
        IMessageExchange<T>
        where T : class
    {
        readonly TopicNode<T> _root;

        public MessageTopicExchange(string name, StringComparer? comparer = default)
        {
            Name = name;

            _root = new TopicNode<T>(comparer ?? StringComparer.Ordinal);
        }

        public IEnumerable<IMessageSink<T>> Sinks => _root.Sinks;

        public string Name { get; }

        public Task Deliver(DeliveryContext<T> context)
        {
            var routingKey = context.RoutingKey;

            return _root.Deliver(context, routingKey);
        }

        public ConnectHandle Connect(IMessageSink<T> sink, string? routingKey)
        {
            return _root.Add(sink, routingKey);
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("exchange");
            scope.Add("name", Name);
            scope.Add("type", "topic");

            var topicScope = scope.CreateScope("topics");

            _root.Probe(topicScope);
        }

        public override string ToString()
        {
            return $"Exchange({Name})";
        }
    }
}
