using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using MassTransit.ServiceBus.Util;

namespace MassTransit.ServiceBus
{
    [Serializable]
    public class Envelope :
        IEnvelope, IEquatable<Envelope>
    {
        private DateTime _arrivedTime;
        private MessageId _correlationId = MessageId.Empty;
        private MessageId _id = MessageId.Empty;
        private string _label;
        private IMessage[] _messages;
        private List<object> _messagesObj;
        private bool _recoverable;
        private IEndpoint _returnTo;
        private DateTime _sentTime;
        private TimeSpan _timeToBeReceived = TimeSpan.MaxValue;

        public Envelope(IEndpoint returnTo, params IMessage[] messages)
        {
            _returnTo = returnTo;
            _messages = messages;
        }

        public Envelope(params IMessage[] messages)
        {
            _messages = messages;
        }

        /// <summary>
        /// Gets/sets the list of messages in the message bundle.
        /// </summary>
        public List<object> MessagesObj
        {
            get { return _messagesObj; }
            set { _messagesObj = value; }
        }

        #region IEnvelope Members

        /// <remarks>
        /// Since the XmlSerializer doesn't work well with interfaces,
        /// we ask it to ignore this data and synchronize with the <see cref="_messagesObj"/> field.
        /// </remarks>
        [XmlIgnore]
        public IMessage[] Messages
        {
            get { return _messages; }
            set
            {
                _messages = value;
                _messagesObj = new List<object>(_messages);
            }
        }


        public IEndpoint ReturnTo
        {
            get { return _returnTo; }
            set { _returnTo = value; }
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
            Envelope env = new Envelope(ReturnTo, Messages);
            env.ArrivedTime = ArrivedTime;
            env.CorrelationId = CorrelationId;
            //id?
            env.Label = Label;
            env.Messages = Messages;
            env.MessagesObj = MessagesObj;
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