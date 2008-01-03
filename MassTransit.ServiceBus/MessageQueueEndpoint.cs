using System;
using System.IO;
using System.Messaging;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using log4net;

namespace MassTransit.ServiceBus
{
    using Util;

    public partial class MessageQueueEndpoint :
        IEndpoint, IEnvelopeFactory
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (MessageQueueEndpoint));

        private readonly BinaryFormatter _formatter;
        private readonly string _queueName;

        private readonly MessageQueue _queue;
        private readonly IEndpoint _poisonEnpoint;

        private readonly object _eventLock = new object();

        protected MessageQueueEndpoint(string queueName)
        {
            _queue = new MessageQueue(queueName, QueueAccessMode.ReceiveAndAdmin);

            _queueName = MsmqUtilities.NormalizeQueueName(_queue);

            MessagePropertyFilter mpf = new MessagePropertyFilter();
            mpf.SetAll();

            _queue.MessageReadPropertyFilter = mpf;

            _formatter = new BinaryFormatter();

            _poisonEnpoint = new WriteOnlyMessageQueueEndpoint(queueName + "_poison");

            //_queue.PeekCompleted += Queue_PeekCompleted;

            _peekCursor = _queue.CreateCursor();

            _queue.BeginPeek(TimeSpan.FromHours(24), _peekCursor, PeekAction.Current, this, Queue_PeekCompleted);
            
            // TODO this may scale at some point to multiple receives althought not sure if that works
        }

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

        public void AcceptEnvelope(string id)
        {
            try
            {
                Message msg = _queue.ReceiveById(id);
            }
            catch (Exception ex)
            {
                _log.Error("Receive Exception", ex);
            }
        }


        private void Queue_PeekCompleted(IAsyncResult asyncResult)
        {
            try
            {
                Message msg = _queue.EndPeek(asyncResult);

                if (msg != null)
                {
                    _log.DebugFormat("Queue: {0} Received Message Id {1}", _queue.Path, msg.Id);

                    IEnvelope e;

                    if (msg.ResponseQueue != null)
                        e = EnvelopeFactory.NewEnvelope((MessageQueueEndpoint) msg.ResponseQueue.Path);
                    else
                        e = EnvelopeFactory.NewEnvelope(this);

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
                if ((uint)ex.MessageQueueErrorCode == 0xC0000120)
                    return;

                if (ex.MessageQueueErrorCode == MessageQueueErrorCode.IllegalCursorAction)
                    return;

                if (ex.MessageQueueErrorCode != MessageQueueErrorCode.IOTimeout)
                {
                    _log.ErrorFormat("Queue_PeekCompleted Exception ({0}): {1} ", ex.Message, ex.MessageQueueErrorCode);
                }
            }

            if(_queue.CanRead)
                _queue.BeginPeek(TimeSpan.FromHours(24), _peekCursor, PeekAction.Next, this, Queue_PeekCompleted);
        }

        public string Address
        {
            get { return _queueName; }
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
                    Check.RequireTransaction(string.Format( "The current queue {0} is transactional and this MessageQueueEndpoint is not running in a transaction.", _queueName));

                    tt = MessageQueueTransactionType.Automatic;
                }

                q.Send(msg, tt);

                envelope.Id = msg.Id;

                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Message Sent: Id = {0}, Message Type = {1}", msg.Id,
                                     envelope.Messages != null ? envelope.Messages[0].GetType().ToString() : "");
            }
        }

        private EventHandler<EnvelopeReceivedEventArgs> _onEnvelopeReceived;
        private Cursor _peekCursor;

        public event EventHandler<EnvelopeReceivedEventArgs> EnvelopeReceived
        {
            add
            {
                lock (_eventLock)
                    _onEnvelopeReceived =
                        (EventHandler<EnvelopeReceivedEventArgs>) Delegate.Combine(_onEnvelopeReceived, value);
            }
            remove
            {
                lock (_eventLock)
                    _onEnvelopeReceived =
                        (EventHandler<EnvelopeReceivedEventArgs>) Delegate.Remove(_onEnvelopeReceived, value);
            }
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

        public IEndpoint Poison
        {
            get { return _poisonEnpoint; }
        }

        public void Dispose()
        {
            _queue.Close();
        }

        public static implicit operator MessageQueueEndpoint(string queuePath)
        {
            return MessageQueueEndpointFactory.Instance.Resolve(queuePath);
        }

        public static implicit operator string(MessageQueueEndpoint endpoint)
        {
            return endpoint.Address;
        }

        public IEnvelope NewEnvelope(params IMessage[] messages)
        {
            return EnvelopeFactory.NewEnvelope(messages);
        }

        public IEnvelope NewEnvelope(IEndpoint returnTo, params IMessage[] messages)
        {
            return EnvelopeFactory.NewEnvelope(returnTo, messages);
        }
    }
}