using System;

namespace MassTransit.ServiceBus.Util
{
    public class MessageId : IEquatable<MessageId>
    {
        private static readonly MessageId _empty = new MessageId(Guid.Empty, 0);

        private readonly Guid _id;
        private readonly uint _sequence;

        protected MessageId(Guid id, uint sequence)
        {
            _id = id;
            _sequence = sequence;
        }

        public MessageId()
        {
            _id = _empty.Id;
            _sequence = _empty.Sequence;
        }

        public MessageId(MessageId other)
        {
            _id = other._id;
            _sequence = other._sequence;
        }

        public MessageId(string s)
            : this()
        {
            string[] parts = s.Split('\\');
            if (parts.Length == 2)
            {
                _id = new Guid(parts[0]);
                _sequence = uint.Parse(parts[1]);
            }
        }

        public Guid Id
        {
            get { return _id; }
        }

        public uint Sequence
        {
            get { return _sequence; }
        }

        public static MessageId Empty
        {
            get { return _empty; }
        }

        #region IEquatable<MessageId> Members

        public bool Equals(MessageId other)
        {
            if (_id != other._id)
                return false;

            if (_sequence != other._sequence)
                return false;

            return true;
        }

        #endregion

        public override string ToString()
        {
            return string.Format(@"{0}\{1}", _id, _sequence);
        }

        public override bool Equals(object obj)
        {
            if (obj is MessageId)
                return Equals((MessageId) obj);

            return false;
        }

        public override int GetHashCode()
        {
            return _id.GetHashCode() + 29*_sequence.GetHashCode();
        }

        public static implicit operator MessageId(string id)
        {
            return new MessageId(id);
        }

        public static implicit operator string(MessageId id)
        {
            return id.ToString();
        }

        public static bool operator ==(MessageId leftHandValue, MessageId rightHandValue)
        {
            return leftHandValue.Equals(rightHandValue);
        }

        public static bool operator !=(MessageId leftHandValue, MessageId rightHandValue)
        {
            return !leftHandValue.Equals(rightHandValue);
        }
    }
}