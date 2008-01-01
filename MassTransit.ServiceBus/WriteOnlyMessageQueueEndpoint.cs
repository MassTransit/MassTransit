namespace MassTransit.ServiceBus
{
    using System;
    using System.IO;
    using System.Messaging;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Transactions;
    using log4net;

    public class WriteOnlyMessageQueueEndpoint :
        IEndpoint
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(WriteOnlyMessageQueueEndpoint));

        private readonly BinaryFormatter _formatter;

        private string _queueName;
        private readonly MessageQueue _queue;


        public WriteOnlyMessageQueueEndpoint(string queueName)
        {
            _queue = new MessageQueue(queueName, QueueAccessMode.ReceiveAndAdmin);

            _queueName = NormalizeQueueName(_queue);

            MessagePropertyFilter mpf = new MessagePropertyFilter();
            mpf.SetAll();

            _queue.MessageReadPropertyFilter = mpf;

            _formatter = new BinaryFormatter();
        }

        //TODO: Duplicate Code
        private static string NormalizeQueueName(MessageQueue queue)
        {
            string machineName = queue.MachineName;
            if (machineName == "." || string.Compare(machineName, "localhost", true) == 0)
            {
                queue.MachineName = Environment.MachineName;
            }

            return queue.Path;
        }

        public void Send(IEnvelope envelope)
        {
            using (MessageQueue q = new MessageQueue(_queueName, QueueAccessMode.Send))
            {
                Message msg = new Message();

                if (envelope.Messages != null && envelope.Messages.Length > 0)
                {
                    SerializeMessages(msg.BodyStream, envelope.Messages);
                }

                msg.ResponseQueue = new MessageQueue(envelope.ReturnTo.Address);

                MessageQueueTransactionType tt = MessageQueueTransactionType.None;
                if (q.Transactional)
                {
                    //TODO: move this into the check util class?
                    if (Transaction.Current == null)
                        throw new Exception(string.Format("The current queue {0} is transactional and this MessageQueueEndpoint is not running in a transaction.", this._queueName));

                    tt = MessageQueueTransactionType.Automatic;
                }

                q.Send(msg, tt);

                envelope.Id = msg.Id;

                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Message Sent: Id = {0}, Message Type = {1}", msg.Id,
                                     envelope.Messages != null ? envelope.Messages[0].GetType().ToString() : "");
            }
        }

        //TODO: Duplicate Code
        private void SerializeMessages(Stream stream, IMessage[] messages)
        {
            _formatter.Serialize(stream, messages);
        }

        /// <summary>
        /// This event will never fire.
        /// </summary>
        public event EventHandler<EnvelopeReceivedEventArgs> EnvelopeReceived;

        public string Address
        {
            get { return _queueName; }
        }

        public IEndpoint Poison
        {
            get { throw new NotImplementedException("Write Only Queues don't have poison queues"); }
        }
    }
}