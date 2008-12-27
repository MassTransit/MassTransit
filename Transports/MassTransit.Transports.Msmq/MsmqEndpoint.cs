// Copyright 2007-2008 The Apache Software Foundation.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.Transports.Msmq
{
	using System;
	using System.Collections.Generic;
	using System.Messaging;
	using System.Runtime.Serialization;
	using System.Threading;
	using System.Transactions;
	using Configuration;
	using Exceptions;
	using Internal;
	using log4net;
	using Serialization;

	/// <summary>
	/// A MessageQueueEndpoint is an implementation of an endpoint using the Microsoft Message Queue service.
	/// </summary>
	public class MsmqEndpoint :
		IEndpoint
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (MsmqEndpoint));
		private readonly QueueAddress _queueAddress;
		private readonly IMessageSerializer _serializer;
		private volatile bool _disposed;
		private MessageQueue _queue;
		private MessageQueueTransactionType _receiveTransactionType;

		public MessageQueueTransactionType SendTransactionType
		{
			get { return _sendTransactionType; }
		}

		public MessageQueueTransactionType ReceiveTransactionType
		{
			get { return _receiveTransactionType; }
		}

		private MessageQueueTransactionType _sendTransactionType;

		/// <summary>
		/// Initializes a <c ref="MessageQueueEndpoint" /> instance with the specified URI string.
		/// </summary>
		/// <param name="uriString">The URI for the endpoint</param>
		public MsmqEndpoint(string uriString)
			: this(new Uri(uriString), new BinaryMessageSerializer())
		{
		}

		/// <summary>
		/// Initializes a <c ref="MessageQueueEndpoint" /> instance with the specified URI.
		/// </summary>
		/// <param name="uri">The URI for the endpoint</param>
		public MsmqEndpoint(Uri uri)
			: this(uri, new BinaryMessageSerializer())
		{
		}

		/// <summary>
		/// Initializes a <c ref="MessageQueueEndpoint" /> instance with the specified URI.
		/// </summary>
		/// <param name="uri">The URI for the endpoint</param>
		/// <param name="serializer">The serializer to use for the endpoint</param>
		public MsmqEndpoint(Uri uri, IMessageSerializer serializer)
		{
			ReliableMessaging = true;

			_serializer = serializer;

			_queueAddress = new QueueAddress(uri);

			_queue = Open(QueueAccessMode.SendAndReceive);

			Initialize();
		}

		public static string Scheme
		{
			get { return "msmq"; }
		}

		public bool ReliableMessaging { get; set; }


		/// <summary>
		/// The path of the message queue for the endpoint. Suitable for use with <c ref="MessageQueue" />.Open
		/// to access a message queue.
		/// </summary>
		public string QueuePath
		{
			get { return _queueAddress.FormatName; }
		}

		/// <summary>
		/// The address of the endpoint, in URI format
		/// </summary>
		public Uri Uri
		{
			get { return _queueAddress.ActualUri; }
		}

		public void Send<T>(T message) where T : class
		{
			Send(message, TimeSpan.MaxValue);
		}

		public void Send<T>(T message, TimeSpan timeToLive) where T : class
		{
			if (_disposed) throw new ObjectDisposedException("The object has been disposed");

			if (!_queue.CanWrite)
				throw new EndpointException(this, "Not allowed to write to endpoint " + _queueAddress.ActualUri);

			Type messageType = typeof (T);

			Message msg = BuildMessage(timeToLive, messageType, message);

			try
			{
				if (SpecialLoggers.Messages.IsInfoEnabled)
					SpecialLoggers.Messages.InfoFormat("SEND:{0}:{1}", Uri, messageType.Name);

				if (_sendTransactionType == MessageQueueTransactionType.Automatic)
				{
					if (Transaction.Current == null)
					{
						_queue.Send(msg, MessageQueueTransactionType.Single);
					}
					else
					{
						_queue.Send(msg, _sendTransactionType);
					}
				}
				else
				{
					_queue.Send(msg, _sendTransactionType);
				}
			}
			catch (MessageQueueException ex)
			{
				throw new EndpointException(this, "Problem with " + QueuePath, ex);
			}

			if (_log.IsDebugEnabled)
				_log.DebugFormat("Sent {0} from {1} [{2}]", messageType.FullName, Uri, msg.Id);
		}


		public void Receive(TimeSpan timeout, Func<object, Func<object, bool>, bool> receiver)
		{
			if (!_queue.CanRead)
				throw new EndpointException(this, string.Format("Not allowed to read from endpoint: '{0}'", _queueAddress.ActualUri));

			try
			{
				using (MessageEnumerator enumerator = _queue.GetMessageEnumerator2())
				{
					while (enumerator.MoveNext(timeout))
					{
						Message msmqMessage = enumerator.Current;
						if (msmqMessage == null)
							continue;

						object message = DeserializeMessage(msmqMessage);
						if (message == null)
							continue;

						if (receiver(message, x =>
							{
								Message received = enumerator.RemoveCurrent(timeout, _receiveTransactionType);
								if (received == null)
									throw new MessageException(message.GetType(), "The message could not be removed from the queue");

								if (received.Id != msmqMessage.Id)
									throw new MessageException(message.GetType(), "The message removed does not match the original message");

								if (_log.IsDebugEnabled)
									_log.DebugFormat("Queue: {0} Received Message Id {1}", _queue.Path, msmqMessage.Id);

								if (SpecialLoggers.Messages.IsInfoEnabled)
									SpecialLoggers.Messages.InfoFormat("RECV:{0}:{1}", _queueAddress.ActualUri, message.GetType().Name);

								return true;
							}))
							return;

						if (_log.IsDebugEnabled)
							_log.DebugFormat("Queue: {0} Skipped Message Id {1}", _queue.Path, msmqMessage.Id);
					}
					enumerator.Close();
				}
			}
			catch (MessageQueueException ex)
			{
				HandleVariousErrorCodes(ex.MessageQueueErrorCode, ex);
			}
		}

		public void Dispose()
		{
			if (_queue != null)
				_queue.Dispose();
		}

		public IEnumerable<IMessageSelector> SelectiveReceive(TimeSpan timeout)
		{
			if (_disposed) throw new ObjectDisposedException("The object has been disposed");

			if (!_queue.CanRead)
				throw new EndpointException(this, string.Format("Not allowed to read from endpoint: '{0}'", _queueAddress.ActualUri));

			using (MessageEnumerator enumerator = _queue.GetMessageEnumerator2())
			{
				while (enumerator.MoveNext(timeout))
				{
					using (MsmqMessageSelector selector = new MsmqMessageSelector(this, enumerator, _serializer))
					{
						yield
							return selector;
					}
				}
				enumerator.Close();
			}
		}

		/// <summary>
		/// Opens a message queue
		/// </summary>
		/// <param name="mode">The access mode for the queue</param>
		/// <returns>An open <c ref="MessageQueue" /> object</returns>
		public MessageQueue Open(QueueAccessMode mode)
		{
			MessageQueue queue = new MessageQueue(QueuePath, mode);

			MessagePropertyFilter mpf = new MessagePropertyFilter();
			mpf.SetAll();

			queue.MessageReadPropertyFilter = mpf;

			if (!queue.CanRead)
				throw new EndpointException(this, "The endpoint could not be found: " + QueuePath);

			return queue;
		}

		public object Receive(TimeSpan timeout)
		{
			try
			{
				Message msg = _queue.Receive(timeout, _receiveTransactionType);

				try
				{
					if (msg == null)
						throw new MessageException(typeof (object), string.Format("Endpoint '{0}' just fed us a null Msmq Message", this._queueAddress.ActualUri));

					//TODO: What do we want to do if the message body stream is null?
					object obj = _serializer.Deserialize(msg.BodyStream);

					return obj;
				}
				catch (SerializationException ex)
				{
					string messageName = msg == null ? "UNKNOWN" : msg.Label;
					string exceptionMessage = string.Format("An error occurred serializing a message of type '{0}'", messageName);

					throw new MessageException(typeof (Object), exceptionMessage, ex);
				}
			}
			catch (MessageQueueException ex)
			{
				HandleVariousErrorCodes(ex.MessageQueueErrorCode, ex);
			}

			return null;
		}

		public object Receive(TimeSpan timeout, Predicate<object> accept)
		{
			if (!_queue.CanRead)
				throw new EndpointException(this, string.Format("Not allowed to read from endpoint: '{0}'", _queueAddress.ActualUri));

			try
			{
				DateTime started = DateTime.Now;
				while (started + timeout > DateTime.Now)
				{
					using (MessageEnumerator enumerator = _queue.GetMessageEnumerator2())
					{
						// account for the fact that we might be doing multiple reads on the enumerator
						while (enumerator.MoveNext(timeout))
						{
							Message msg = enumerator.Current;
							if (msg == null)
								throw new MessageException(typeof (object), string.Format("Received a null Msmq Message while enumerating the queue '{0}'", this.Uri));

							try
							{
								object obj = _serializer.Deserialize(msg.BodyStream);

								if (accept(obj))
								{
									Message received = enumerator.RemoveCurrent(TimeSpan.FromSeconds(10), _receiveTransactionType);
									if (received == null)
										throw new MessageException(typeof (object), string.Format("Received a null Msmq Message while enumerating the queue '{0}' post accept", this.Uri));

									if (received.Id != msg.Id)
										throw new MessageException(obj.GetType(), "The message removed does not match the original message");

									if (_log.IsDebugEnabled)
										_log.DebugFormat("Queue: {0} Received Message Id {1}", _queue.Path, msg.Id);

									if (SpecialLoggers.Messages.IsInfoEnabled)
										SpecialLoggers.Messages.InfoFormat("RECV:{0}:{1}", _queueAddress.ActualUri, obj.GetType().Name);

									return obj;
								}

								if (_log.IsDebugEnabled)
									_log.DebugFormat("Queue: {0} Skipped Message Id {1}", _queue.Path, msg.Id);
							}
							catch (SerializationException ex)
							{
								// if we get a message we cannot serialize, we need to do something about it or it will 
								// hang the service bus forever

								try
								{
									Message discard = _queue.ReceiveById(msg.Id, _receiveTransactionType);

									_log.Error("Discarded message " + discard.Id + " due to a serialization error", ex);
								}
								catch (Exception ex2)
								{
									_log.Error(string.Format("Unable to purge message id '{0}'", msg.Id), ex2);
								}

								throw new MessageException(typeof (object), "An error occurred deserializing a message", ex);
							}
						}
						enumerator.Close();
					}
				}
			}
			catch (MessageQueueException ex)
			{
				HandleVariousErrorCodes(ex.MessageQueueErrorCode, ex);
			}

			return null;
		}

		public void DiscardMessage(string messageId, string message)
		{
			try
			{
				using (Message discarded = _queue.ReceiveById(messageId, MessageQueueTransactionType.Single))
				{
				}

				if (_log.IsWarnEnabled)
					_log.WarnFormat("Discarding {1} from {0}", _queueAddress.ActualUri, messageId);
			}
			catch (Exception ex)
			{
				_log.Error("Unable to purge message id " + messageId, ex);
			}
		}

		private void Initialize()
		{
			_sendTransactionType = _queueAddress.IsLocal && _queue.Transactional ? MessageQueueTransactionType.Automatic : MessageQueueTransactionType.None;

			_receiveTransactionType = _queueAddress.IsLocal && _queue.Transactional ? MessageQueueTransactionType.Automatic : MessageQueueTransactionType.None;
		}

		private Message BuildMessage(TimeSpan timeToLive, Type messageType, object message)
		{
			var msg = new Message();

			_serializer.Serialize(msg.BodyStream, message);

			if (timeToLive < TimeSpan.MaxValue)
				msg.TimeToBeReceived = timeToLive;

			msg.Label = messageType.Name;

			msg.Recoverable = ReliableMessaging;

			return msg;
		}


		private object DeserializeMessage(Message msg)
		{
			try
			{
				object obj = _serializer.Deserialize(msg.BodyStream);

				return obj;
			}
			catch (SerializationException ex)
			{
				// if we get a message we cannot serialize, we need to do something about it or it will 
				// hang the service bus for..ev..er..

				try
				{
					Message discard = _queue.ReceiveById(msg.Id, _receiveTransactionType);

					_log.Error("Discarded message " + discard.Id + " due to a serialization error", ex);
				}
				catch (Exception ex2)
				{
					_log.Error("Unable to purge message id " + msg.Id, ex2);
				}

				throw new MessageException(typeof (object), "An error occurred deserializing a message", ex);
			}
		}

		private void HandleVariousErrorCodes(MessageQueueErrorCode code, Exception ex)
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
					if (_log.IsErrorEnabled)
						_log.Error("There was a problem accessing the queue", ex);


					// reopen the queue in case for some reason it disappeared
					_queue = Open(QueueAccessMode.SendAndReceive);

					// we don't want to spin the CPU completely
					Thread.Sleep(2000);
					break;

				case MessageQueueErrorCode.QueueNotFound:
				case MessageQueueErrorCode.IllegalFormatName:
				case MessageQueueErrorCode.MachineNotFound:
					if (_log.IsErrorEnabled)
						_log.Error("The message queue does not exist", ex);

					// we don't want to spin the CPU completely
					Thread.Sleep(2000);
					break;

				case MessageQueueErrorCode.MessageAlreadyReceived:
					// we are competing with another consumer, no reason to report an error since
					// the message has already been handled.
					if (_log.IsDebugEnabled)
						_log.DebugFormat("Message received by another receiver before it could be retrieved");
					break;

				case MessageQueueErrorCode.InvalidHandle:
					// reopen the queue in case for some reason it is lost (maybe msmq was restarted)
					_queue = Open(QueueAccessMode.SendAndReceive);

					// we don't want to spin the CPU completely
					Thread.Sleep(1000);
					break;

				default:
					if (_log.IsErrorEnabled)
						_log.Error("An error occured while communicating with the queue", ex);
					break;
			}
		}

		public static IEndpoint ConfigureEndpoint(Uri uri, Action<IEndpointConfigurator> configurator)
		{
			if (uri.Scheme.ToLowerInvariant() == "msmq")
			{
				IEndpoint endpoint = MsmqEndpointConfigurator.New(x =>
					{
						x.SetUri(uri);

						configurator(x);
					});

				return endpoint;
			}

			return null;
		}
	}
}