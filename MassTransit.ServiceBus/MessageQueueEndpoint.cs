using System.Messaging;
using log4net;
using MassTransit.ServiceBus.Util;

namespace MassTransit.ServiceBus
{
    public class MessageQueueEndpoint :
        IEndpoint
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (MessageQueueEndpoint));

        private readonly string _queueName;

        private MessageQueueEndpoint _poisonEndpoint;
        private MessageQueue _queue;

        public MessageQueueEndpoint(string queueName)
        {
            _queue = new MessageQueue(queueName, QueueAccessMode.Send);

            _queueName = MsmqUtilities.NormalizeQueueName(_queue);
        }

        #region IEndpoint Members

        public string Address
        {
            get { return _queueName; }
        }

        public void Dispose()
        {
            _queue.Close();
            _queue.Dispose();
            _queue = null;
        }

        #endregion

        private MessageQueueTransactionType GetTransactionType()
        {
            MessageQueueTransactionType tt = MessageQueueTransactionType.None;
            if (_queue.Transactional)
            {
                Check.RequireTransaction(
                    string.Format(
                        "The current queue {0} is transactional and this MessageQueueEndpoint is not running in a transaction.",
                        _queueName));

                tt = MessageQueueTransactionType.Automatic;
            }
            return tt;
        }

        public static implicit operator MessageQueueEndpoint(string queuePath)
        {
            return new MessageQueueEndpoint(queuePath);
        }

        public static implicit operator string(MessageQueueEndpoint endpoint)
        {
            return endpoint.Address;
        }
    }
}