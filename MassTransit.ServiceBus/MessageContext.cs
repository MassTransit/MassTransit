using System;

namespace MassTransit.ServiceBus
{
    using System.Collections.Generic;
    using log4net;

    public class MessageContext<T> :
        EventArgs where T : IMessage
    {
        private IEnvelope _envelope;
        private T _message;
        private IServiceBus _bus;
        private ILog _log = LogManager.GetLogger(typeof (MessageContext<T>));

        public MessageContext(IServiceBus bus, IEnvelope envelope, T message)
        {
            _envelope = envelope;
            _bus = bus;
            _message = message;
        }

        public IEnvelope Envelope
        {
            get { return _envelope; }
            set { _envelope = value; }
        }

        public T Message
        {
            get { return _message; }
            set { _message = value; }
        }

        public IServiceBus Bus
        {
            get { return _bus; }
            set { _bus = value; }
        }

        private bool _accepted = false;

        public bool WasAccepted
        {
            get { return _accepted; }
        }

        public bool Accept()
        {
            if (!_accepted)
            {
                if ( Bus.Endpoint.AcceptEnvelope(_envelope.Id) )
                    _accepted = true;
            }

            return _accepted;
        }

        /// <summary>
        /// Builds an envelope with the correlation id set to the id of the incoming envelope
        /// </summary>
        /// <param name="messages">The messages to include with the reply</param>
        public void Reply(params IMessage[] messages)
        {
            IEndpoint replyEndpoint = Envelope.ReturnTo;

            IEnvelope envelope = new Envelope(Bus.Endpoint, messages);
            envelope.CorrelationId = Envelope.Id;

            replyEndpoint.Send(envelope);
        }

        /// <summary>
        /// Moves the specified messages to the back of the list of available 
        /// messages so they can be handled later. Could screw up message order.
        /// </summary>
        public void HandleMessagesLater(params IMessage[] messages)
        {
            Bus.Send(Bus.Endpoint, messages);
        }

        /// <summary>
        /// Marks the whole context as poison
        /// </summary>
        public void MarkPoison()
        {
           // if (_log.IsDebugEnabled)
             //   _log.DebugFormat("Envelope {0} Was Marked Poisonous", this._envelope.Id);
            Bus.Endpoint.PoisonEndpoint.Send(Envelope);
        }

        /// <summary>
        /// Marks a specific message as poison
        /// </summary>
        public void MarkPoison(IMessage msg)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("A Message (Index:{1}) in Envelope {0} Was Marked Poisonous", this._envelope.Id, new List<IMessage>(Envelope.Messages).IndexOf(msg));

            IEnvelope env = (IEnvelope) this.Envelope.Clone(); //Should this be cloned?
            env.Messages = new IMessage[] {this.Message};
            
            Bus.Endpoint.PoisonEndpoint.Send(env);
        }
    }
}