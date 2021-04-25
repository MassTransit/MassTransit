namespace MassTransit.GrpcTransport.Fabric
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;


    public class MessageTopicExchange :
        IMessageExchange
    {
        readonly TopicNode _root;

        public MessageTopicExchange(string name, StringComparer comparer = default)
        {
            Name = name;

            _root = new TopicNode(comparer ?? StringComparer.Ordinal);
        }

        public IEnumerable<IMessageSink<GrpcTransportMessage>> Sinks => _root.Sinks;

        public string Name { get; }

        public Task Deliver(DeliveryContext<GrpcTransportMessage> context)
        {
            var routingKey = context.Message.RoutingKey;

            return _root.Deliver(context, routingKey);
        }

        public ConnectHandle Connect(IMessageSink<GrpcTransportMessage> sink, string routingKey)
        {
            return _root.Add(sink, routingKey);
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
