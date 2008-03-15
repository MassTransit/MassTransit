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

namespace MassTransit.ServiceBus.MSMQ
{
	using System;
	using System.Messaging;
	using System.Runtime.Serialization;
	using System.Threading;
	using Exceptions;
	using Internal;
	using log4net;

	/// <summary>
	/// Receives envelopes from a message queue
	/// </summary>
	public class MsmqMessageReceiver :
		IMessageReceiver
	{
		private static readonly object _locker = new object();
		private static readonly ILog _log = LogManager.GetLogger(typeof (MsmqMessageReceiver));
		private static readonly ILog _messageLog = LogManager.GetLogger("MassTransit.Messages");
		private readonly IMsmqEndpoint _endpoint;
		private readonly TimeSpan _readTimeout = TimeSpan.FromSeconds(4);
	    private IEnvelopeMapper<Message> _mapper;

		private IEnvelopeConsumer _consumer;

		private Thread _monitorThread;
		private MessageQueue _queue;

		/// <summary>
		/// Initializes a MessageQueueReceiver
		/// </summary>
		/// <param name="endpoint">The endpoint where the receiver should be attached</param>
		public MsmqMessageReceiver(IMsmqEndpoint endpoint)
		{
			_endpoint = endpoint;
            _mapper = new MsmqEnvelopeMapper();
		}

		///<summary>
		///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		///</summary>
		///<filterpriority>2</filterpriority>
		public void Dispose()
		{
			Stop();
		}

		/// <summary>
		/// Adds a consumer to the message receiver
		/// </summary>
		/// <param name="consumer">The consumer to add</param>
		public void Subscribe(IEnvelopeConsumer consumer)
		{
			lock (_locker)
			{
				if (_consumer == null)
				{
					_consumer = consumer;

					Restart();
				}
				else if (_consumer != consumer)
				{
					throw new EndpointException(_endpoint, "Only one consumer can be registered for a message receiver");
				}
			}
		}

	    public IEnvelopeConsumer Consumer
	    {
            get
            {
                if(_consumer == null) throw new EndpointException(_endpoint, "No consumer has been registered");
                return _consumer;
            }
	    }

		private void Restart()
		{
			lock (this)
			{
				Stop();

				Start();
			}
		}

		private void Start()
		{
			lock (this)
			{
				_queue = _endpoint.Open(QueueAccessMode.SendAndReceive);

				_monitorThread = new Thread(MonitorQueue);
				_monitorThread.Start();
			}
		}

		private void Stop()
		{
			lock (this)
			{
				if (_queue != null)
				{
					_queue.Close();
					_queue.Dispose();
					_queue = null;
				}

				if (_monitorThread != null)
				{
					_monitorThread.Join(TimeSpan.FromSeconds(10));
					_monitorThread = null;
				}
			}
		}

		/// <summary>
		/// Called by the thread pool to process a message that has been seen on the queue (via Peek)
		/// 
		/// The message is checked to see if it will handled by a consumer and if so, the message
		/// is received from the queue and send to the consumer(s) that will handle it.
		/// </summary>
		/// <param name="obj">An instance of <c ref="Message" /> that was seen on the queue</param>
		public void ProcessMessage(object obj)
		{
			IEnvelope e = obj as IEnvelope;
			if (e == null)
				return;
			try
			{
				_consumer.Deliver(e);

				e.ReturnEndpoint.Dispose();
			}
			catch (Exception ex)
			{
				if (_log.IsErrorEnabled)
					_log.Error(string.Format("An exception occured delivering envelope {0}", e.Id), ex);
			}
		}

		private void MonitorQueue()
		{
			while (_queue != null)
			{
				try
				{
					MessageEnumerator enumerator = _queue.GetMessageEnumerator2();
					while (enumerator.MoveNext(_readTimeout))
					{
						Message msg = enumerator.Current;
                        if (msg != null) //TODO: will this ever happen?
                        {
                            if (_log.IsDebugEnabled)
                                _log.DebugFormat("Queue: {0} Received Message Id {1}", _queue.Path, msg.Id);

                            try
                            {
                                IEnvelope e = _mapper.ToEnvelope(msg);

                                //TODO: Should this be 'CanHandle' or 'WantsToHandle' or 'IsInterested'
                                if (this.Consumer.IsHandled(e))
                                {
                                    Message received = enumerator.RemoveCurrent(TimeSpan.FromSeconds(1));
                                    if (received.Id == msg.Id)
                                    {
                                        if (_messageLog.IsInfoEnabled)
                                            _messageLog.InfoFormat("Received message {0} from {1}",
                                                                   e.Messages[0].GetType(), e.ReturnEndpoint.Uri);

                                        ThreadPool.QueueUserWorkItem(ProcessMessage, e);
                                        break;
                                    }
                                }
                            }
                            catch (SerializationException ex)
                            {
                                Message discard = enumerator.RemoveCurrent(TimeSpan.FromSeconds(5));
                                _log.Error("Discarding unknown message " + discard.Id, ex);
                            }
                        }
					}

					enumerator.Close();
				}
				catch (MessageQueueException ex)
				{
					HandleVariousErrorCodes(ex.MessageQueueErrorCode);
				}
				catch (ThreadAbortException ex)
				{
					if (_log.IsInfoEnabled)
						_log.Info("Thread aborted by receiver", ex);
				}
				catch (Exception ex)
				{
					if (_log.IsErrorEnabled)
						_log.Error("An unknown exception occured", ex);
				}
			}
		}

        private void HandleVariousErrorCodes(MessageQueueErrorCode code)
        {
            switch (code)
            {
                case MessageQueueErrorCode.IOTimeout:
                    // this is OK its just a normal timeout
                    break;

                case MessageQueueErrorCode.AccessDenied:
                case MessageQueueErrorCode.QueueNotAvailable:
                case MessageQueueErrorCode.ServiceNotAvailable:
                case MessageQueueErrorCode.QueueDeleted:
                    ThreadPool.QueueUserWorkItem(delegate { Stop(); });
                    if (_log.IsErrorEnabled)
                        _log.Error("There was a problem accessing the queue", ex);
                    break;

                case MessageQueueErrorCode.QueueNotFound:
                case MessageQueueErrorCode.IllegalFormatName:
                case MessageQueueErrorCode.MachineNotFound:
                    ThreadPool.QueueUserWorkItem(delegate { Stop(); });
                    if (_log.IsErrorEnabled)
                        _log.Error("The message queue does not exist", ex);
                    break;

                case MessageQueueErrorCode.MessageAlreadyReceived:
                    // we are competing with another consumer, no reason to report an error since
                    // the message has already been handled.
                    if (_log.IsDebugEnabled)
                        _log.DebugFormat("Message received by another receiver before it could be retrieved");
                    break;

                default:
                    if (_log.IsErrorEnabled)
                        _log.Error("An error occured while communicating with the queue", ex);
                    break;
            }
        }
	}
}