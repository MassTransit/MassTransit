namespace MassTransit.Tests.ContainerTests.Common_Tests
{
    using System;


    public class MyId :
        IEquatable<MyId>
    {
        readonly Guid _id;

        public MyId(Guid id)
        {
            _id = id;
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

        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }

        public override string ToString()
        {
            return _id.ToString();
        }
    }
}
