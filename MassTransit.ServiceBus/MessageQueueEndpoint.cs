using System;
using System.Collections.Generic;
using System.Messaging;
using System.Threading;
using log4net;
using MassTransit.ServiceBus.Util;

namespace MassTransit.ServiceBus
{
    public class MessageQueueEndpoint :
        IReadWriteEndpoint
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (MessageQueueEndpoint));

        private static readonly Dictionary<string, MessageQueueEndpoint> _transportCache =
            new Dictionary<string, MessageQueueEndpoint>();

        private readonly List<IEnvelopeConsumer> _consumers = new List<IEnvelopeConsumer>();

        private readonly Cursor _peekCursor;
        private readonly string _queueName;

        private IAsyncResult _peekAsyncResult;
        private IEndpoint _poisonEndpoint;
        private MessageQueue _queue;

        public MessageQueueEndpoint(string queueName)
        {
            _queue = new MessageQueue(queueName, QueueAccessMode.SendAndReceive);

            _queueName = MsmqUtilities.NormalizeQueueName(_queue);

            MessagePropertyFilter mpf = new MessagePropertyFilter();
            mpf.SetAll();

            _queue.MessageReadPropertyFilter = mpf;

            _peekCursor = _queue.CreateCursor();
        }

        #region IReadWriteEndpoint Members

        public string Address
        {
            get { return _queueName; }
        }

        public void Send(IEnvelope envelope)
        {
            Message msg = EnvelopeMessageMapper.MapFrom(envelope);

            _queue.Send(msg, GetTransactionType());

            envelope.Id = msg.Id;

            if (_log.IsDebugEnabled)
                _log.DebugFormat("Message Sent: Id = {0}, Message Type = {1}", msg.Id,
                                 envelope.Messages != null ? envelope.Messages[0].GetType().ToString() : "");
        }

        public void Subscribe(IEnvelopeConsumer consumer)
        {
            if (!_consumers.Contains(consumer))
            {
                _consumers.Add(consumer);
            }

            if (_peekAsyncResult == null)
            {
                _peekAsyncResult =
                    _queue.BeginPeek(TimeSpan.FromHours(24), _peekCursor, PeekAction.Current, this,
                                     Queue_PeekCompleted);
            }
        }

        public IEndpoint PoisonEndpoint
        {
            get
            {
                if (_poisonEndpoint == null)
                {
                    _poisonEndpoint = Open(_queueName + "_poison");
                }

                return _poisonEndpoint;
            }
        }

        public void Dispose()
        {
            Remove(_queueName);

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

        //TODO: Need to make this public so it can be tested
        public void ProcessMessage(object obj)
        {
            Message msg = obj as Message;
            if (msg == null)
                return;

            if (_log.IsDebugEnabled)
                _log.DebugFormat("Queue: {0} Received Message Id {1}", _queue.Path, msg.Id);

            if (_consumers.Count > 0)
            {
                IEnvelope e = EnvelopeMessageMapper.MapFrom(msg);

                try
                {
                    bool handleMessage = false;
                    foreach (IEnvelopeConsumer consumer in _consumers)
                    {
                        if (consumer.MeetsCriteria(e))
                        {
                            handleMessage = true;
                            break;
                        }
                    }

                    if (handleMessage)
                    {
                        if (_log.IsDebugEnabled)
                            _log.DebugFormat("Delivering Envelope {0} by {1}", e.Id, GetHashCode());

                        _consumers.ForEach(delegate(IEnvelopeConsumer consumer) { consumer.Deliver(e); });
                    }
                }
                catch (Exception ex)
                {
                    if (_log.IsErrorEnabled)
                        _log.Error("Envelope Exception", ex);
                }
            }
        }

        private void Queue_PeekCompleted(IAsyncResult asyncResult)
        {
            try
            {
                if (_queue == null)
                    return;

                Message msg = _queue.EndPeek(asyncResult);

                if (msg != null)
                {
                    ThreadPool.QueueUserWorkItem(ProcessMessage, msg);
                }
            }
            catch (MessageQueueException ex)
            {
                if ((uint) ex.MessageQueueErrorCode == 0xC0000120 ||
                    ex.MessageQueueErrorCode == MessageQueueErrorCode.IllegalCursorAction)
                {
                    if (_log.IsInfoEnabled)
                        _log.InfoFormat("The queue ({0}) was closed during an asynchronous operation", _queueName);

                    return;
                }

                if (ex.MessageQueueErrorCode != MessageQueueErrorCode.IOTimeout)
                {
                    if (_log.IsErrorEnabled)
                        _log.ErrorFormat("Queue_PeekCompleted Exception ({0}): {1} ", ex.Message,
                                         ex.MessageQueueErrorCode);

                    return;
                }
            }

            _queue.BeginPeek(TimeSpan.FromHours(24), _peekCursor, PeekAction.Next, this, Queue_PeekCompleted);
        }

        public static MessageQueueEndpoint Open(string queuePath)
        {
            string key;
            string queueName = NormalizeQueueName(queuePath, out key);

            lock (_transportCache)
            {
                if (!_transportCache.ContainsKey(key))
                    _transportCache.Add(key, new MessageQueueEndpoint(queueName));

                return _transportCache[key];
            }
        }

        //TODO: If one is open should the other be close?
        public static void Remove(string queuePath)
        {
            string key;
            NormalizeQueueName(queuePath, out key);

            lock (_transportCache)
            {
                if (_transportCache.ContainsKey(key))
                    _transportCache.Remove(key);
            }
        }

        private static string NormalizeQueueName(string queuePath, out string key)
        {
            using (MessageQueue queue = new MessageQueue(queuePath))
            {
                key = MsmqUtilities.NormalizeQueueName(queue).Replace("FORMATNAME:DIRECT=OS:", "");

                return queue.Path;
            }
        }

        public static implicit operator MessageQueueEndpoint(string queuePath)
        {
            return Open(queuePath);
        }

        public static implicit operator string(MessageQueueEndpoint endpoint)
        {
            return endpoint.Address;
        }
    }
}
