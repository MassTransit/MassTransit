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
            _queue = new MessageQueue(endpoint.QueueName, QueueAccessMode.SendAndReceive);

            MessagePropertyFilter mpf = new MessagePropertyFilter();
            mpf.SetAll();

            _queue.MessageReadPropertyFilter = mpf;
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
			    throw new EndpointException(_endpoint, "Problem with " + _endpoint.QueueName);
			}

            envelope.Id = msg.Id;

            if (_log.IsDebugEnabled)
                _log.DebugFormat("Message Sent: Id = {0}, Message Type = {1}", msg.Id,
                    envelope.Messages != null ? envelope.Messages[0].GetType().ToString() : "");
        }

        #endregion
    }
}