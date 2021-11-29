namespace MassTransit.RabbitMqTransport.Topology
{
    using System.Collections.Generic;
    using System.Linq;


    public class QueueBindingEntity :
        ExchangeToQueueBinding,
        QueueBindingHandle
    {
        readonly ExchangeEntity _exchange;
        readonly QueueEntity _queue;

        public QueueBindingEntity(long id, ExchangeEntity exchange, QueueEntity queue, string routingKey, IDictionary<string, object> arguments)
        {
            Id = id;
            RoutingKey = routingKey;
            Arguments = arguments ?? new Dictionary<string, object>();
            _exchange = exchange;
            _queue = queue;
        }

        public static IEqualityComparer<QueueBindingEntity> EntityComparer { get; } = new QueueBindingEntityEqualityComparer();

        public Exchange Source => _exchange.Exchange;
        public Queue Destination => _queue.Queue;
        public string RoutingKey { get; }
        public IDictionary<string, object> Arguments { get; }

        public long Id { get; }
        public ExchangeToQueueBinding Binding => this;

        public override string ToString()
        {
            return string.Join(", ",
                new[]
                {
                    $"source: {Source.ExchangeName}",
                    $"destination: {Destination.QueueName}",
                    string.IsNullOrWhiteSpace(RoutingKey) ? "" : $"routing-key: {RoutingKey}",
                    string.Join(", ", Arguments.Select(x => $"{x.Key}: {x.Value}"))
                }.Where(x => !string.IsNullOrWhiteSpace(x)));
        }


        sealed class QueueBindingEntityEqualityComparer : IEqualityComparer<QueueBindingEntity>
        {
            public bool Equals(QueueBindingEntity x, QueueBindingEntity y)
            {
                if (ReferenceEquals(x, y))
                    return true;
                if (ReferenceEquals(x, null))
                    return false;
                if (ReferenceEquals(y, null))
                    return false;
                if (x.GetType() != y.GetType())
                    return false;
                return x._exchange.Equals(y._exchange) && x._queue.Equals(y._queue) && string.Equals(x.RoutingKey, y.RoutingKey)
                    && x.Arguments.All(a => y.Arguments.TryGetValue(a.Key, out var value) && a.Value.Equals(value));
            }

            public int GetHashCode(QueueBindingEntity obj)
            {
                unchecked
                {
                    var hashCode = obj._exchange.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj._queue.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.RoutingKey.GetHashCode();
                    foreach (KeyValuePair<string, object> keyValuePair in obj.Arguments)
                    {
                        hashCode = (hashCode * 397) ^ keyValuePair.Key.GetHashCode();
                        hashCode = (hashCode * 397) ^ keyValuePair.Value.GetHashCode();
                    }

                    return hashCode;
                }
            }
        }
    }
}
