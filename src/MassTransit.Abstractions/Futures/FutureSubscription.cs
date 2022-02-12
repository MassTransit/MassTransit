#nullable disable
namespace MassTransit
{
    using System;
    using System.Collections.Generic;


    public class FutureSubscription :
        IEquatable<FutureSubscription>
    {
        public FutureSubscription(Uri address, Guid? requestId = default)
        {
            RequestId = requestId;
            Address = address;
        }

        public static IEqualityComparer<FutureSubscription> Comparer { get; } = new EqualityComparer();

        public Guid? RequestId { get; }
        public Uri Address { get; }

        public bool Equals(FutureSubscription other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return Nullable.Equals(RequestId, other.RequestId) && Equals(Address, other.Address);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;
            return Equals((FutureSubscription)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (RequestId.GetHashCode() * 397) ^ (Address != null ? Address.GetHashCode() : 0);
            }
        }

        public static bool operator ==(FutureSubscription left, FutureSubscription right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(FutureSubscription left, FutureSubscription right)
        {
            return !Equals(left, right);
        }


        sealed class EqualityComparer :
            IEqualityComparer<FutureSubscription>
        {
            public bool Equals(FutureSubscription x, FutureSubscription y)
            {
                if (ReferenceEquals(x, y))
                    return true;
                if (ReferenceEquals(x, null))
                    return false;
                if (ReferenceEquals(y, null))
                    return false;
                return Nullable.Equals(x.RequestId, y.RequestId) && Equals(x.Address, y.Address);
            }

            public int GetHashCode(FutureSubscription obj)
            {
                unchecked
                {
                    return (obj.RequestId.GetHashCode() * 397) ^ (obj.Address != null ? obj.Address.GetHashCode() : 0);
                }
            }
        }
    }
}
