using System;
using System.Collections.Generic;
using System.Messaging;
using System.Threading;
using log4net;

namespace MassTransit.ServiceBus
{
    public class MessageQueueReceiver :
        IMessageReceiver
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (MessageQueueReceiver));

        private readonly List<IEnvelopeConsumer> _consumers = new List<IEnvelopeConsumer>();
        private readonly Cursor _cursor;

        private IAsyncResult _peekAsyncResult;

        private MessageQueue _queue;

        public MessageQueueReceiver(IEndpoint endpoint)
        {
            _queue = new MessageQueue(endpoint.Address, QueueAccessMode.SendAndReceive);

            MessagePropertyFilter mpf = new MessagePropertyFilter();
            mpf.SetAll();

            _queue.MessageReadPropertyFilter = mpf;

            _cursor = _queue.CreateCursor();
        }

        #region IMessageReceiver Members

        public void Dispose()
        {
            _queue.Close();
            _queue.Dispose();
            _queue = null;
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
                    _queue.BeginPeek(TimeSpan.FromHours(24), _cursor, PeekAction.Current, this,
                                     Queue_PeekCompleted);
            }
        }

        #endregion

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
                        _log.InfoFormat("The queue was closed during an asynchronous operation");

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

            _peekAsyncResult =
                _queue.BeginPeek(TimeSpan.FromHours(24), _cursor, PeekAction.Next, this, Queue_PeekCompleted);
        }
    }
}