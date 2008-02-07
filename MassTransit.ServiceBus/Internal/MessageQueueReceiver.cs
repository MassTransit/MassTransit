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
using System.Messaging;
using System.Runtime.Serialization;
using System.Threading;
using log4net;
using MassTransit.ServiceBus.Exceptions;

namespace MassTransit.ServiceBus.Internal
{
	/// <summary>
	/// Receives envelopes from a message queue
	/// </summary>
	public class MessageQueueReceiver :
		IMessageReceiver
	{
		private static readonly object _locker = new object();
		private static readonly ILog _log = LogManager.GetLogger(typeof (MessageQueueReceiver));
		private static readonly ILog _messageLog = LogManager.GetLogger("MassTransit.Messages");
		private readonly IMessageQueueEndpoint _endpoint;

		private IEnvelopeConsumer _consumer;

		private Thread _monitorThread;
		private IMsmqQueue _queue;

		/// <summary>
		/// Initializes a MessageQueueReceiver
		/// </summary>
		/// <param name="endpoint">The endpoint where the receiver should be attached</param>
		public MessageQueueReceiver(IMessageQueueEndpoint endpoint)
		{
			_endpoint = endpoint;
		}

		#region IMessageReceiver Members

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

		#endregion

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
					while (enumerator.MoveNext(TimeSpan.FromSeconds(1)))
					{
						Message msg = enumerator.Current;
						if (msg != null)
						{
							if (_log.IsDebugEnabled)
								_log.DebugFormat("Queue: {0} Received Message Id {1}", _queue.Path, msg.Id);

							if (_consumer != null)
							{
								try
								{
									IEnvelope e = EnvelopeMessageMapper.MapFrom(msg);

									if (_consumer.IsHandled(e))
									{
										Message received = enumerator.RemoveCurrent(TimeSpan.FromSeconds(1));
										if (received.Id == msg.Id)
										{
											if (_messageLog.IsInfoEnabled)
												_messageLog.InfoFormat("Received message {0} from {1}", e.Messages[0].GetType(), e.ReturnEndpoint.Uri);

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
					}

					enumerator.Close();
				}
				catch (MessageQueueException ex)
				{
					if (ex.MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout)
					{
						//we don't log this because it isn't exceptional
					}
					else if (ex.MessageQueueErrorCode == MessageQueueErrorCode.QueueNotFound)
					{
						if(_log.IsErrorEnabled)
							_log.Error("The message queue does not exist", ex);

						ThreadPool.QueueUserWorkItem(delegate { Stop(); });
					}
					else if (ex.MessageQueueErrorCode == MessageQueueErrorCode.MessageAlreadyReceived)
					{
						// we are competing with another consumer, no reason to report an error since
						// the message has already been handled.
						if(_log.IsDebugEnabled)
							_log.DebugFormat("Message received by another receiver before it could be retrieved");
					}
					else
					{
						if (_log.IsErrorEnabled)
							_log.Error("An error occured while communicating with the queue", ex);
					}
				}
				catch(ThreadAbortException ex)
				{
					if(_log.IsInfoEnabled)
						_log.Info("Thread aborted by receiver", ex);
				}
				catch (Exception ex)
				{
					if (_log.IsErrorEnabled)
						_log.Error("An unknown exception occured", ex);
				}
			}
		}
	}
}