namespace MassTransit.TestFramework.Messages
{
    using System;


    [Serializable]
    public class PingNotSupported :
        IEquatable<PingNotSupported>,
        CorrelatedBy<Guid>
    {
        Guid _id;

        public PingNotSupported()
        {
            _id = Guid.NewGuid();
        }

        public PingNotSupported(Guid correlationId)
        {
            _id = correlationId;
        }

        public Guid CorrelationId
        {
            get => _id;
            set => _id = value;
        }

        public bool Equals(PingNotSupported obj)
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
            if (obj.GetType() != typeof(PingNotSupported))
                return false;
            return Equals((PingNotSupported)obj);
        }

        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }
    }
}
