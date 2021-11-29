namespace MassTransit.RabbitMqTransport.Topology
{
    using System.Collections.Generic;
    using System.Linq;


    public class ExchangeBindingEntity :
        ExchangeToExchangeBinding,
        ExchangeBindingHandle
    {
        readonly ExchangeEntity _destination;

        readonly ExchangeEntity _source;

        public ExchangeBindingEntity(long id, ExchangeEntity source, ExchangeEntity destination, string routingKey, IDictionary<string, object> arguments)
        {
            Id = id;
            RoutingKey = routingKey;
            Arguments = arguments ?? new Dictionary<string, object>();
            _source = source;
            _destination = destination;
        }

        public static IEqualityComparer<ExchangeBindingEntity> EntityComparer { get; } = new ExchangeBindingEntityEqualityComparer();
        public long Id { get; }
        public ExchangeToExchangeBinding Binding => this;

        public Exchange Source => _source.Exchange;
        public Exchange Destination => _destination.Exchange;
        public string RoutingKey { get; }
        public IDictionary<string, object> Arguments { get; }

        public override string ToString()
        {
            return string.Join(", ",
                new[]
                {
                    $"source: {Source.ExchangeName}",
                    $"destination: {Destination.ExchangeName}",
                    string.IsNullOrWhiteSpace(RoutingKey) ? "" : $"routing-key: {RoutingKey}",
                    string.Join(", ", Arguments.Select(x => $"{x.Key}: {x.Value}"))
                }.Where(x => !string.IsNullOrWhiteSpace(x)));
        }


        sealed class ExchangeBindingEntityEqualityComparer : IEqualityComparer<ExchangeBindingEntity>
        {
            public bool Equals(ExchangeBindingEntity x, ExchangeBindingEntity y)
            {
                if (ReferenceEquals(x, y))
                    return true;
                if (ReferenceEquals(x, null))
                    return false;
                if (ReferenceEquals(y, null))
                    return false;
                if (x.GetType() != y.GetType())
                    return false;
                return x._source.Equals(y._source) && x._destination.Equals(y._destination) && string.Equals(x.RoutingKey, y.RoutingKey)
                    && x.Arguments.All(a => y.Arguments.TryGetValue(a.Key, out var value) && a.Value.Equals(value));
            }

            public int GetHashCode(ExchangeBindingEntity obj)
            {
                unchecked
                {
                    var hashCode = obj._source.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj._destination.GetHashCode();
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
