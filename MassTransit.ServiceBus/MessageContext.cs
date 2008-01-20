/// Copyright 2007-2008 The Apache Software Foundation.
/// 
/// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
/// this file except in compliance with the License. You may obtain a copy of the 
/// License at 
/// 
///   http://www.apache.org/licenses/LICENSE-2.0 
/// 
/// Unless required by applicable law or agreed to in writing, software distributed 
/// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
/// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
/// specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using log4net;

namespace MassTransit.ServiceBus
{
    /// <summary>
    /// A message context contains the participants in a message exchange that is
    /// received on the service bus.
    /// </summary>
    /// <typeparam name="T">The message type</typeparam>
    public class MessageContext<T> :
        EventArgs, IMessageContext<T> where T : IMessage
    {
        private readonly IServiceBus _bus;
        private readonly IEnvelope _envelope;
        private readonly ILog _log = LogManager.GetLogger(typeof (MessageContext<T>));
        private readonly T _message;

        /// <summary>
        /// Initializes an instance of the <c ref="MessageContext" /> class
        /// </summary>
        /// <param name="bus">The service bus on which the message was received</param>
        /// <param name="envelope">The message envelope received</param>
        /// <param name="message">The individual message from the envelope being received</param>
        public MessageContext(IServiceBus bus, IEnvelope envelope, T message)
        {
            _envelope = envelope;
            _bus = bus;
            _message = message;
        }

        /// <summary>
        /// The envelope containing the message
        /// </summary>
        public IEnvelope Envelope
        {
            get { return _envelope; }
        }

        /// <summary>
        /// The actual message being delivered
        /// </summary>
        public T Message
        {
            get { return _message; }
        }

        /// <summary>
        /// The service bus on which the message was received
        /// </summary>
        public IServiceBus Bus
        {
            get { return _bus; }
        }

        /// <summary>
        /// Builds an envelope with the correlation id set to the id of the incoming envelope
        /// </summary>
        /// <param name="messages">The messages to include with the reply</param>
        public void Reply(params IMessage[] messages)
        {
            IEndpoint replyEndpoint = Envelope.ReturnEndpoint;

            IEnvelope envelope = new Envelope(Bus.Endpoint, messages);
            envelope.CorrelationId = Envelope.Id;

            replyEndpoint.Sender.Send(envelope);
        }

        /// <summary>
        /// Moves the specified messages to the back of the list of available 
        /// messages so they can be handled later. Could screw up message order.
        /// </summary>
        public void HandleMessagesLater(params IMessage[] messages)
        {
            Bus.Endpoint.Sender.Send(this.Envelope);
        }

        /// <summary>
        /// Marks the whole context as poison
        /// </summary>
        public void MarkPoison()
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Envelope {0} Was Marked Poisonous", _envelope.Id);

            Bus.PoisonEndpoint.Sender.Send(_envelope);
        }

        /// <summary>
        /// Marks a specific message as poison
        /// </summary>
        public void MarkPoison(IMessage msg)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("A Message (Index:{1}) in Envelope {0} Was Marked Poisonous", _envelope.Id, new List<IMessage>(Envelope.Messages).IndexOf(msg));

            IEnvelope env = (IEnvelope) Envelope.Clone(); //Should this be cloned?
            env.Messages = new IMessage[] {Message};

            Bus.PoisonEndpoint.Sender.Send(env);
        }
    }
}