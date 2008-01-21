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

using System.Messaging;
using log4net;
using MassTransit.ServiceBus.Exceptions;

namespace MassTransit.ServiceBus
{
    /// <summary>
    /// Send envelopes on a message queue
    /// </summary>
    public class MessageQueueSender :
        IMessageSender
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (MessageQueueSender));

        private MessageQueue _queue;
        private IMessageQueueEndpoint _endpoint;

        /// <summary>
        /// Initializes an instance of the <c ref="MessageQueueSender" /> class
        /// </summary>
        /// <param name="endpoint">The destination endpoint for messages to be sent</param>
        public MessageQueueSender(IMessageQueueEndpoint endpoint)
        {
            _endpoint = endpoint;

            _queue = endpoint.Open(QueueAccessMode.SendAndReceive);
        }

        #region IMessageSender Members

        public void Dispose()
        {
            _queue.Close();
            _queue.Dispose();
            _queue = null;
        }

        public void Send(IEnvelope envelope)
        {
            Message msg = EnvelopeMessageMapper.MapFrom(envelope);

            try
			{
                _queue.Send(msg);
            }
			catch(MessageQueueException ex)
			{
			    throw new EndpointException(_endpoint, "Problem with " + _endpoint.QueuePath, ex);
			}

            envelope.Id = msg.Id;

            if (_log.IsDebugEnabled)
                _log.DebugFormat("Message Sent: Id = {0}, Message Type = {1}", msg.Id,
                    envelope.Messages != null ? envelope.Messages[0].GetType().ToString() : "");
        }

        #endregion
    }
}