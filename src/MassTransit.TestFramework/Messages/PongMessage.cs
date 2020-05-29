namespace MassTransit.TestFramework.Messages
{
    using System;


    [Serializable]
    public class PongMessage :
        IEquatable<PongMessage>,
        CorrelatedBy<Guid>
    {
        Guid _id;

        public PongMessage()
        {
            _id = Guid.NewGuid();
        }

        public PongMessage(Guid correlationId)
        {
            _id = correlationId;
        }

        public Guid CorrelationId
        {
            get => _id;
            set => _id = value;
        }

        public bool Equals(PongMessage obj)
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
            if (obj.GetType() != typeof(PongMessage))
                return false;
            return Equals((PongMessage)obj);
        }

        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }
    }
}
