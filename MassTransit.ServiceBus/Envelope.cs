using System;
using MassTransit.ServiceBus.Util;

namespace MassTransit.ServiceBus
{
    public class Envelope :
        IEnvelope, IEquatable<Envelope>
    {
        private DateTime _arrivedTime;
        private MessageId _correlationId = MessageId.Empty;
        private MessageId _id = MessageId.Empty;
        private string _label;
        private IMessage[] _messages;
        private bool _recoverable;
        private IEndpoint _ReturnEndpoint;
        private DateTime _sentTime;
        private TimeSpan _timeToBeReceived = TimeSpan.MaxValue;

        public Envelope(IEndpoint returnEndpoint, params IMessage[] messages)
        {
            _ReturnEndpoint = returnEndpoint;
            _messages = messages;
        }

        public Envelope(params IMessage[] messages)
        {
            _messages = messages;
        }

        #region IEnvelope Members

        public IMessage[] Messages
        {
            get { return _messages; }
            set { _messages = value; }
        }

        public IEndpoint ReturnEndpoint
        {
            get { return _ReturnEndpoint; }
            set { _ReturnEndpoint = value; }
        }

        public MessageId Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public MessageId CorrelationId
        {
            get { return _correlationId; }
            set { _correlationId = value; }
        }

        public bool Recoverable
        {
            get { return _recoverable; }
            set { _recoverable = value; }
        }

        public TimeSpan TimeToBeReceived
        {
            get { return _timeToBeReceived; }
            set { _timeToBeReceived = value; }
        }

        public DateTime SentTime
        {
            get { return _sentTime; }
            set { _sentTime = value; }
        }

        public DateTime ArrivedTime
        {
            get { return _arrivedTime; }
            set { _arrivedTime = value; }
        }

        public string Label
        {
            get { return _label; }
            set { _label = value; }
        }

        public object Clone()
        {
            Envelope env = new Envelope(ReturnEndpoint, Messages);
            env.ArrivedTime = ArrivedTime;
            env.Id = Id;
            env.CorrelationId = CorrelationId;
            env.Label = Label;
            env.Recoverable = Recoverable;
            env.SentTime = SentTime;
            env.TimeToBeReceived = TimeToBeReceived;

            return env;
        }

        #endregion

        #region IEquatable<Envelope> Members

        public bool Equals(Envelope envelope)
        {
            if (envelope == null) return false;
            return Equals(_id, envelope._id);
        }

        #endregion

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as Envelope);
        }

        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }
    }
}