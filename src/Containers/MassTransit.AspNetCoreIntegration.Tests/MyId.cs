namespace MassTransit.AspNetCoreIntegration.Tests
{
    using System;


    public class MyId : IEquatable<MyId>
    {
        readonly Guid _id;

        public MyId()
        {
            _id = Guid.NewGuid();
        }

        public bool Equals(MyId other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return _id.Equals(other._id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;
            return Equals((MyId)obj);
        }

        public override int GetHashCode() => _id.GetHashCode();
    }
}
