namespace MassTransit.RabbitMqTransport.Topology
{
    using System;
    using System.Collections.Generic;


    public sealed class RabbitMqHostEqualityComparer :
        IEqualityComparer<RabbitMqHostSettings>
    {
        public static IEqualityComparer<RabbitMqHostSettings> Default { get; } = new RabbitMqHostEqualityComparer();

        public bool Equals(RabbitMqHostSettings x, RabbitMqHostSettings y)
        {
            if (ReferenceEquals(x, y))
                return true;

            if (ReferenceEquals(x, null))
                return false;

            if (ReferenceEquals(y, null))
                return false;

            if (!string.Equals(x.Host, y.Host, StringComparison.OrdinalIgnoreCase))
                return false;

            if (!string.Equals(x.VirtualHost, y.VirtualHost))
                return false;

            if (x.Port == y.Port)
                return true;

            // handle SSL mismatch betweeen clients
            if ((x.Port == 5671 || x.Port == 5672) && (y.Port == 5671 || y.Port == 5672))
                return true;

            return false;
        }

        public int GetHashCode(RabbitMqHostSettings obj)
        {
            unchecked
            {
                var hashCode = obj.Host?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ obj.Port;
                hashCode = (hashCode * 397) ^ (obj.VirtualHost?.GetHashCode() ?? 0);
                return hashCode;
            }
        }
    }
}
