using System;
using System.Collections.Generic;
using System.IO;
using System.Messaging;
using System.Runtime.Serialization.Formatters.Binary;
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

        private readonly object _eventLock = new object();

        private readonly BinaryFormatter _formatter;
        private readonly string _queueName;

        private EventHandler<EnvelopeReceivedEventArgs> _onEnvelopeReceived;
        private IAsyncResult _peekAsyncResult;
        private Cursor _peekCursor;
        private IEndpoint _poisonEndpoint;
        private MessageQueue _queue;

        public MessageQueueEndpoint(string queueName)
        {
            _queue = new MessageQueue(queueName, QueueAccessMode.SendAndReceive);

            _queueName = MsmqUtilities.NormalizeQueueName(_queue);

            MessagePropertyFilter mpf = new MessagePropertyFilter();
            mpf.SetAll();

            _queue.MessageReadPropertyFilter = mpf;

            _formatter = new BinaryFormatter();
        }

        #region IReadWriteEndpoint Members

        public bool AcceptEnvelope(string id)
        {
            try
            {
                _queue.ReceiveById(id);

                return true;
            }
            catch (Exception ex)
            {
                _log.Error("Receive Exception", ex);
            }

            return false;
        }

        public string Address
        {
            get { return _queueName; }
        }

        public void Send(IEnvelope envelope)
        {
            Message msg = new Message();

            if (envelope.Messages != null && envelope.Messages.Length > 0)
            {
                SerializeMessages(msg.BodyStream, envelope.Messages);
            }

            msg.ResponseQueue = new MessageQueue(envelope.ReturnTo.Address);

            if (envelope.TimeToBeReceived < MessageQueue.InfiniteTimeout)
                msg.TimeToBeReceived = envelope.TimeToBeReceived;

            if (!string.IsNullOrEmpty(envelope.Label))
                msg.Label = envelope.Label;

            msg.Recoverable = envelope.Recoverable;

            if (!string.IsNullOrEmpty(envelope.CorrelationId))
                msg.CorrelationId = envelope.CorrelationId;

            MessageQueueTransactionType tt = MessageQueueTransactionType.None;
            if (_queue.Transactional)
            {
                Check.RequireTransaction(
                    string.Format(
                        "The current queue {0} is transactional and this MessageQueueEndpoint is not running in a transaction.",
                        _queueName));

                tt = MessageQueueTransactionType.Automatic;
            }

            _queue.Send(msg, tt);

            envelope.Id = msg.Id;

            if (_log.IsDebugEnabled)
                _log.DebugFormat("Message Sent: Id = {0}, Message Type = {1}", msg.Id,
                                 envelope.Messages != null ? envelope.Messages[0].GetType().ToString() : "");
        }

        public event EventHandler<EnvelopeReceivedEventArgs> EnvelopeReceived
        {
            add
            {
                lock (_eventLock)
                    _onEnvelopeReceived =
                        (EventHandler<EnvelopeReceivedEventArgs>) Delegate.Combine(_onEnvelopeReceived, value);

                if (_peekAsyncResult == null)
                {
                    _peekCursor = _queue.CreateCursor();

                    _peekAsyncResult =
                        _queue.BeginPeek(TimeSpan.FromHours(24), _peekCursor, PeekAction.Current, this,
                                         Queue_PeekCompleted);
                }
            }
            remove
            {
                lock (_eventLock)
                    _onEnvelopeReceived =
                        (EventHandler<EnvelopeReceivedEventArgs>) Delegate.Remove(_onEnvelopeReceived, value);
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

        private void Receive(object obj)
        {
            IEnvelope envelope = obj as Envelope;
            if (envelope == null)
                return;

            try
            {
                NotifyHandlers(new EnvelopeReceivedEventArgs(envelope));
            }
            catch (Exception ex)
            {
                _log.Error("Envelope Exception", ex);
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
                    _log.DebugFormat("Queue: {0} Received Message Id {1}", _queue.Path, msg.Id);

                    IEnvelope e;

                    if (msg.ResponseQueue != null)
                        e = new Envelope(Open(msg.ResponseQueue.Path));
                    else
                        e = new Envelope(this);

                    e.Id = msg.Id;
                    e.CorrelationId = (msg.CorrelationId == "00000000-0000-0000-0000-000000000000\\0"
                                           ? null
                                           : msg.CorrelationId);

                    e.TimeToBeReceived = msg.TimeToBeReceived;
                    e.Recoverable = msg.Recoverable;
                    e.SentTime = msg.SentTime;
                    e.ArrivedTime = msg.ArrivedTime;
                    e.Label = msg.Label;


                    IMessage[] messages = _formatter.Deserialize(msg.BodyStream) as IMessage[];
                    if (messages != null)
                    {
                        e.Messages = messages;
                    }

                    try
                    {
                        ThreadPool.QueueUserWorkItem(Receive, e);
                    }
                    catch (Exception ex)
                    {
                        _log.Error("Exception from Envelope Received: ", ex);
                    }
                }
            }
            catch (MessageQueueException ex)
            {
                if ((uint) ex.MessageQueueErrorCode == 0xC0000120)
                    return;

                if (ex.MessageQueueErrorCode == MessageQueueErrorCode.IllegalCursorAction)
                    return;

                if (ex.MessageQueueErrorCode != MessageQueueErrorCode.IOTimeout)
                {
                    _log.ErrorFormat("Queue_PeekCompleted Exception ({0}): {1} ", ex.Message, ex.MessageQueueErrorCode);
                }
            }

            if (_queue.CanRead)
                _queue.BeginPeek(TimeSpan.FromHours(24), _peekCursor, PeekAction.Next, this, Queue_PeekCompleted);
        }

        private void NotifyHandlers(EnvelopeReceivedEventArgs e)
        {
            if (_onEnvelopeReceived != null)
            {
                _log.DebugFormat("Delivering Envelope {0} by {1}", e.Envelope.Id, GetHashCode());
                _onEnvelopeReceived(this, e);
            }
            else
                _log.DebugFormat("Envelope {0} dropped by {1}", e.Envelope.Id, GetHashCode());
        }

        //TODO: Duplicated Code
        private void SerializeMessages(Stream stream, IMessage[] messages)
        {
            _formatter.Serialize(stream, messages);
        }


        public static MessageQueueEndpoint Open(string queuePath)
        {
            string key;
            string queueName = NormalizeQueueName(queuePath, out key);

            lock (_transportCache)
            {
                if (_transportCache.ContainsKey(key))
                    return _transportCache[key];

                MessageQueueEndpoint transport = new MessageQueueEndpoint(queueName);

                _transportCache.Add(key, transport);

                return transport;
            }
        }

        public static void Remove(string queuePath)
        {
            string key;
            string queueName = NormalizeQueueName(queuePath, out key);

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