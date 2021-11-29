namespace MassTransit.AmazonSqsTransport.Topology
{
    using System;
    using System.Collections.Generic;


    public sealed class AmazonSqsHostEqualityComparer :
        IEqualityComparer<AmazonSqsHostSettings>
    {
        public static IEqualityComparer<AmazonSqsHostSettings> Default { get; } = new AmazonSqsHostEqualityComparer();

        public bool Equals(AmazonSqsHostSettings x, AmazonSqsHostSettings y)
        {
            if (ReferenceEquals(x, y))
                return true;

            if (ReferenceEquals(x, null))
                return false;

            if (ReferenceEquals(y, null))
                return false;

            return string.Equals(x.Region.SystemName, y.Region.SystemName, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(AmazonSqsHostSettings obj)
        {
            unchecked
            {
                var hashCode = obj.AccessKey?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ (obj.Region?.GetHashCode() ?? 0);
                return hashCode;
            }
        }
    }
}
