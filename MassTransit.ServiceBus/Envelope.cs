using System;

namespace MassTransit.ServiceBus
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    [Serializable]
    public class Envelope : 
        IEnvelope, IEquatable<Envelope>
    {
        private IMessage[] _messages;
        private IEndpoint _returnTo;
        private string _id;
    	private string _correlationId;
    	private bool _recoverable;
        private TimeSpan _timeToBeReceived = TimeSpan.MaxValue;
    	private DateTime _sentTime;
    	private DateTime _arrivedTime;
    	private string _label;

        public Envelope(IEndpoint returnTo, params IMessage[] messages)
        {
            _returnTo = returnTo;
            _messages = messages;
        }

        public Envelope()
    	{
    	}

        /// <remarks>
        /// Since the XmlSerializer doesn't work well with interfaces,
        /// we ask it to ignore this data and synchronize with the <see cref="_messagesObj"/> field.
        /// </remarks>
        [XmlIgnore]
    	public IMessage[] Messages
        {
            get { return _messages; }
            set {
                _messages = value;
                _messagesObj = new List<object>(_messages);
            }
        }

        private List<object> _messagesObj;

        /// <summary>
        /// Gets/sets the list of messages in the message bundle.
        /// </summary>
        public List<object> MessagesObj
        {
            get { return _messagesObj; }
            set { _messagesObj = value; }
        }


        public IEndpoint ReturnTo
        {
            get { return _returnTo; }
            set { _returnTo = value; }
        }

        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

    	public string CorrelationId
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


        public bool Equals(Envelope envelope)
        {
            if (envelope == null) return false;
            return Equals(_id, envelope._id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as Envelope);
        }

        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }


        public object Clone()
        {
            Envelope env = new Envelope(this.ReturnTo, this.Messages);
            env.ArrivedTime = this.ArrivedTime;
            env.CorrelationId = this.CorrelationId;
            //id?
            env.Label = this.Label;
            env.Messages = this.Messages;
            env.MessagesObj = this.MessagesObj;
            env.Recoverable = this.Recoverable;
            env.SentTime = this.SentTime;
            env.TimeToBeReceived = this.TimeToBeReceived;

            return env;
        }
    }
}