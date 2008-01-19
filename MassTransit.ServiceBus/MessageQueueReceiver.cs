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
using System.Messaging;
using System.Threading;
using log4net;

namespace MassTransit.ServiceBus
{
    using Exceptions;

    /// <summary>
    /// Receives envelopes from a message queue
    /// </summary>
    public class MessageQueueReceiver :
        IMessageReceiver
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (MessageQueueReceiver));

        private readonly List<IEnvelopeConsumer> _consumers = new List<IEnvelopeConsumer>();
        private readonly Cursor _cursor;

        private IAsyncResult _peekAsyncResult;

        private MessageQueue _queue;

        /// <summary>
        /// Initializes a MessageQueueReceiver
        /// </summary>
        /// <param name="endpoint">The endpoint where the receiver should be attached</param>
        public MessageQueueReceiver(IMessageQueueEndpoint endpoint)
        {
            _queue = new MessageQueue(endpoint.QueueName, QueueAccessMode.SendAndReceive);

            MessagePropertyFilter mpf = new MessagePropertyFilter();
            mpf.SetAll();

            _queue.MessageReadPropertyFilter = mpf;

            try
            {
                _cursor = _queue.CreateCursor();
            }
            catch(MessageQueueException ex)
            {
                if(_log.IsErrorEnabled)
                    _log.Error("There is an issue with the queue " + endpoint.Uri, ex);

                throw new EndpointException(endpoint, string.Format("There are issues with the queue '{0}'", endpoint.Uri), ex);
            }
            
        }

        #region IMessageReceiver Members

        ///<summary>
        ///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        ///</summary>
        ///<filterpriority>2</filterpriority>
        public void Dispose()
        {
            _queue.Close();
            _queue.Dispose();
            _queue = null;
        }

        /// <summary>
        /// Adds a consumer to the message receiver
        /// </summary>
        /// <param name="consumer">The consumer to add</param>
        public void Subscribe(IEnvelopeConsumer consumer)
        {
            if (!_consumers.Contains(consumer))
            {
                _consumers.Add(consumer);
            }

            if (_peekAsyncResult == null)
            {
                _peekAsyncResult = _queue.BeginPeek(TimeSpan.FromHours(24), _cursor, PeekAction.Current, this, Queue_PeekCompleted);
            }
        }

        #endregion

        /// <summary>
        /// Called by the thread pool to process a message that has been seen on the queue (via Peek)
        /// 
        /// The message is checked to see if it will handled by a consumer and if so, the message
        /// is received from the queue and send to the consumer(s) that will handle it.
        /// </summary>
        /// <param name="obj">An instance of <c ref="Message" /> that was seen on the queue</param>
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
                    bool foundAConsumerThatCares = false;

                    foreach (IEnvelopeConsumer consumer in _consumers)
                    {
                        if (consumer.IsHandled(e))
                        {
                            foundAConsumerThatCares = true;

                            //we have found someone that cares take it off the queue
                            _queue.ReceiveById(msg.Id);
                            break;
                        }
                    }

                    if (foundAConsumerThatCares)
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

                    throw;
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
                        _log.InfoFormat("The queue '{0}' was closed during an asynchronous operation", this._queue.QueueName);

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

            _peekAsyncResult = _queue.BeginPeek(TimeSpan.FromHours(24), _cursor, PeekAction.Next, this, Queue_PeekCompleted);
        }
    }
}
