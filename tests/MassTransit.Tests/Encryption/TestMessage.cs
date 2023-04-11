namespace MassTransit.Tests.Encryption
{
    using System;

    public class TestMessage
    {
        public Guid Id { get; }
        public string TestValue { get; }

        public TestMessage(Guid id, string testValue)
        {
            Id = id;
            TestValue = testValue;
        }

    #region Equality members

        protected bool Equals(TestMessage other)
        {
            return Id.Equals(other.Id) && TestValue == other.TestValue;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            return Equals((TestMessage)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Id.GetHashCode() * 397) ^ TestValue.GetHashCode();
            }
        }

    #endregion
    }
}
