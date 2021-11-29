namespace MassTransit.ActiveMqTransport.Topology
{
    using System;
    using System.Collections.Generic;


    public sealed class ActiveMqHostEqualityComparer :
        IEqualityComparer<ActiveMqHostSettings>
    {
        public static IEqualityComparer<ActiveMqHostSettings> Default { get; } = new ActiveMqHostEqualityComparer();

        public bool Equals(ActiveMqHostSettings x, ActiveMqHostSettings y)
        {
            if (ReferenceEquals(x, y))
                return true;

            if (ReferenceEquals(x, null))
                return false;

            if (ReferenceEquals(y, null))
                return false;

            return string.Equals(x.Host, y.Host, StringComparison.OrdinalIgnoreCase) && x.Port == y.Port;
        }

        public int GetHashCode(ActiveMqHostSettings obj)
        {
            unchecked
            {
                var hashCode = obj.Host?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ obj.Port;
                return hashCode;
            }
        }
    }
}
