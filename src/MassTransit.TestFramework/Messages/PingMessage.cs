namespace MassTransit.TestFramework.Messages
{
    using System;


    [Serializable]
    public class PingMessage :
        IEquatable<PingMessage>,
        CorrelatedBy<Guid>
    {
        Guid _id = new Guid("D62C9B1C-8E31-4D54-ADD7-C624D56085A4");

        public PingMessage()
        {
        }

        public PingMessage(Guid id)
        {
            _id = id;
        }

        public Guid CorrelationId
        {
            get => _id;
            set => _id = value;
        }

        public bool Equals(PingMessage obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            return obj._id.Equals(_id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != typeof(PingMessage))
                return false;
            return Equals((PingMessage)obj);
        }

        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }
    }
}
