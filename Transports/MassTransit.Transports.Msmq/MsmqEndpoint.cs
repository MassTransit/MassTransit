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

		private MessageQueueTransactionType _sendTransactionType;

		/// <summary>
		/// Initializes a <c ref="MessageQueueEndpoint" /> instance with the specified URI string.
		/// </summary>
		/// <param name="uriString">The URI for the endpoint</param>
		public MsmqEndpoint(string uriString)
			: this(new Uri(uriString), new XmlMessageSerializer())
		{
		}

		/// <summary>
		/// Initializes a <c ref="MessageQueueEndpoint" /> instance with the specified URI.
		/// </summary>
		/// <param name="uri">The URI for the endpoint</param>
		public MsmqEndpoint(Uri uri)
			: this(uri, new XmlMessageSerializer())
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

		public MessageQueueTransactionType SendTransactionType
		{
			get { return _sendTransactionType; }
		}

		public MessageQueueTransactionType ReceiveTransactionType
		{
			get { return _receiveTransactionType; }
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
				throw new EndpointException(this.Uri, "Not allowed to write to endpoint " + _queueAddress.ActualUri);

			Type messageType = typeof (T);

			OutboundMessage.Set(headers =>
				{
					headers.SetMessageType(messageType);
					headers.SetDestinationAddress(Uri);
				});

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
                HandleVariousErrorCodes(ex.MessageQueueErrorCode, ex);
				throw new EndpointException(this.Uri, "Problem with " + QueuePath, ex);
			}

			if (_log.IsDebugEnabled)
				_log.DebugFormat("Sent {0} from {1} [{2}]", messageType.FullName, Uri, msg.Id);
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
				throw new EndpointException(this.Uri, string.Format("Not allowed to read from endpoint: '{0}'", _queueAddress.ActualUri));

                using (MessageEnumerator enumerator = _queue.GetMessageEnumerator2())
                {
                    _log.DebugFormat("Enumerating endpoint: {0} ({1}ms)", Uri, timeout.ToString());

                    bool hasNext = true;
                    while (hasNext)
                    {
                        try
                        {
                            hasNext = enumerator.MoveNext(timeout);
                        }
                        catch(MessageQueueException ex)
                        {
                            HandleVariousErrorCodes(ex.MessageQueueErrorCode, ex);
                            throw;
                        }

                        _log.DebugFormat("Moved Next on {0}", Uri);

                        using (MsmqMessageSelector selector = new MsmqMessageSelector(this, enumerator, _serializer))
                        {
                            yield return selector;
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
				throw new EndpointException(this.Uri, "The endpoint could not be found: " + QueuePath);

			return queue;
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

		private Message BuildMessage<T>(TimeSpan timeToLive, Type messageType, T message)
		{
			var msg = new Message();

			_serializer.Serialize(msg.BodyStream, message);

			if (timeToLive < TimeSpan.MaxValue)
				msg.TimeToBeReceived = timeToLive;

			msg.Label = messageType.Name;

			msg.Recoverable = ReliableMessaging;

			return msg;
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
                case MessageQueueErrorCode.StaleHandle:
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