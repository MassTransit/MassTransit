using System.Messaging;
using log4net;

namespace MassTransit.ServiceBus
{
    public class MessageQueueSender :
        IMessageSender
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (MessageQueueSender));

        private MessageQueue _queue;

        public MessageQueueSender(IMessageQueueEndpoint endpoint)
        {
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

            _queue.Send(msg);

            envelope.Id = msg.Id;

            if (_log.IsDebugEnabled)
                _log.DebugFormat("Message Sent: Id = {0}, Message Type = {1}", msg.Id,
                    envelope.Messages != null ? envelope.Messages[0].GetType().ToString() : "");
        }

        #endregion
    }
}