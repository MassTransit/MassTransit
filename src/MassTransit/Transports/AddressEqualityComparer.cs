namespace MassTransit.Transports
{
    using System;
    using System.Collections.Generic;


    public class AddressEqualityComparer :
        IEqualityComparer<Uri>
    {
        public static readonly IEqualityComparer<Uri> Comparer = new AddressEqualityComparer();

        public bool Equals(Uri x, Uri y)
        {
            return ReferenceEquals(x, y)
                || (x != null
                    && y != null
                    && x.Scheme.Equals(y.Scheme, StringComparison.OrdinalIgnoreCase)
                    && x.Host.Equals(y.Host, StringComparison.OrdinalIgnoreCase)
                    && x.Port.Equals(y.Port)
                    && x.AbsolutePath.Equals(y.AbsolutePath, StringComparison.OrdinalIgnoreCase));
        }

        public int GetHashCode(Uri obj)
        {
            return obj.AbsolutePath.GetHashCode();
        }
    }
}
