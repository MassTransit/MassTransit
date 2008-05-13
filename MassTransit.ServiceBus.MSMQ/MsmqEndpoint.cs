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
	using System.Transactions;
	using Exceptions;
	using Formatters;
	using Internal;
	using log4net;

	/// <summary>
	/// A MessageQueueEndpoint is an implementation of an endpoint using the Microsoft Message Queue service.
	/// </summary>
	public class MsmqEndpoint :
		IMsmqEndpoint
	{
		private static readonly IBodyFormatter _formatter = new BinaryBodyFormatter();
		private static readonly ILog _log = LogManager.GetLogger(typeof (MsmqEndpoint));
		private static readonly ILog _messageLog = LogManager.GetLogger("MassTransit.Messages");
		private readonly string _queuePath;
		private readonly Uri _uri;
		private IMessageReceiver _receiver;

		private bool _reliableMessaging = true;
		private IMessageSender _sender;

		/// <summary>
		/// Initializes a <c ref="MessageQueueEndpoint" /> instance with the specified URI string.
		/// </summary>
		/// <param name="uriString">The URI for the endpoint</param>
		public MsmqEndpoint(string uriString)
			: this(new Uri(uriString))
		{
		}

		/// <summary>
		/// Initializes a <c ref="MessageQueueEndpoint" /> instance with the specified URI.
		/// </summary>
		/// <param name="uri">The URI for the endpoint</param>
		public MsmqEndpoint(Uri uri)
		{
			_uri = uri;

			if (_uri.AbsolutePath.IndexOf("/", 1) >= 0)
			{
				throw new EndpointException(this, "Queue Endpoints can't have a child folder unless it is 'public'. Good: 'msmq://machinename/queue_name' or 'msmq://machinename/public/queue_name' - Bad: msmq://machinename/queue_name/bad_form");
			}

			string hostName = _uri.Host;
			if (string.Compare(hostName, ".") == 0 || string.Compare(hostName, "localhost", true) == 0)
			{
				hostName = Environment.MachineName.ToLowerInvariant();
			}

			if (string.Compare(_uri.Host, "localhost", true) == 0)
			{
				_uri = new Uri("msmq://" + Environment.MachineName.ToLowerInvariant() + _uri.AbsolutePath);
			}

			_queuePath = string.Format(@"FormatName:DIRECT=OS:{0}\private$\{1}", hostName, _uri.AbsolutePath.Substring(1));
		}

		/// <summary>
		/// Creates an instance of the <c ref="MessageQueueEndpoint" /> class using the specified queue
		/// </summary>
		/// <param name="queue">A Microsoft Message Queue</param>
		public MsmqEndpoint(MessageQueue queue)
		{
			string path = queue.Path;
			const string prefix = "FormatName:DIRECT=OS:";

			if (path.Length > prefix.Length && path.Substring(0, prefix.Length).ToUpperInvariant() == prefix.ToUpperInvariant())
				path = path.Substring(prefix.Length);

			string[] parts = path.Split('\\');

			if (parts.Length != 3)
				throw new ArgumentException("Invalid Queue Path Specified");

			//Validate parts[1] = private$
			if (string.Compare(parts[1], "private$", true) != 0)
				throw new ArgumentException("Invalid Queue Path Specified");

			if (parts[0] == ".")
				parts[0] = Environment.MachineName;

			parts[0] = parts[0].ToLowerInvariant();

			_queuePath = string.Format("{0}{1}\\{2}\\{3}", prefix, parts[0], parts[1], parts[2]);
			_uri = new Uri(string.Format("msmq://{0}/{1}", parts[0], parts[2]));
		}

		#region IMessageQueueEndpoint Members

		public bool ReliableMessaging
		{
			get { return _reliableMessaging; }
			set { _reliableMessaging = value; }
		}

		/// <summary>
		/// The path of the message queue for the endpoint. Suitable for use with <c ref="MessageQueue" />.Open
		/// to access a message queue.
		/// </summary>
		public string QueuePath
		{
			get { return _queuePath; }
		}

		public MessageQueue Open(QueueAccessMode mode)
		{
			MessageQueue queue = new MessageQueue(QueuePath, mode);

			MessagePropertyFilter mpf = new MessagePropertyFilter();
			mpf.SetAll();

			queue.MessageReadPropertyFilter = mpf;

			return queue;
		}

		/// <summary>
		/// The address of the endpoint, in URI format
		/// </summary>
		public Uri Uri
		{
			get { return _uri; }
		}

		public IMessageSender Sender
		{
			get
			{
				lock (this)
				{
					if (_sender == null)
						_sender = new MsmqMessageSender(this);
				}
				return _sender;
			}
		}

		public IMessageReceiver Receiver
		{
			get
			{
				lock (this)
				{
					if (_receiver == null)
						_receiver = new MsmqMessageReceiver(this);
				}

				return _receiver;
			}
		}

		public void Send<T>(T message) where T : class
		{
			Send(message, MessageQueue.InfiniteTimeout);
		}

		public void Send<T>(T message, TimeSpan timeToLive) where T : class
		{
			Type messageType = typeof (T);

			if (!messageType.IsSerializable)
				throw new MessageException(messageType, string.Format("The type {0} must be serializable", messageType.FullName));

			Message msg = new Message();

			_formatter.Serialize(new MsmqFormattedBody(msg), message);

			if (timeToLive < MessageQueue.InfiniteTimeout)
				msg.TimeToBeReceived = timeToLive;

			msg.Label = messageType.AssemblyQualifiedName;

			msg.Recoverable = _reliableMessaging;

			try
			{
				if (_messageLog.IsInfoEnabled)
					_messageLog.InfoFormat("Message {0} Sent To {1}", messageType, Uri);

				using (MessageQueue queue = Open(QueueAccessMode.SendAndReceive))
				{
					MessageQueueTransactionType tt = MessageQueueTransactionType.None;

					if (queue.Transactional)
					{
						EnsureThereIsATransactionScope();

						tt = MessageQueueTransactionType.Automatic;
					}

					queue.Send(msg, tt);
				}
			}
			catch (MessageQueueException ex)
			{
				throw new EndpointException(this, "Problem with " + QueuePath, ex);
			}

			if (_log.IsDebugEnabled)
				_log.DebugFormat("Message Sent: Id = {0}, Message Type = {1}", msg.Id, messageType);
		}

		public object Receive()
		{
			using (MessageQueue queue = Open(QueueAccessMode.ReceiveAndAdmin))
			{
				try
				{
					MessageQueueTransactionType tt = MessageQueueTransactionType.None;

					if (queue.Transactional)
					{
						EnsureThereIsATransactionScope();

						tt = MessageQueueTransactionType.Automatic;
					}

					Message msg = queue.Receive(tt);
					Type messageType = Type.GetType(msg.Label);

					try
					{
						object obj = _formatter.Deserialize(new MsmqFormattedBody(msg));

						return obj;
					}
					catch (SerializationException ex)
					{
						throw new MessageException(messageType, string.Format("An error occurred serializing a message of type {0}", messageType.FullName), ex);
					}
				}
				catch (MessageQueueException ex)
				{
					HandleVariousErrorCodes(ex.MessageQueueErrorCode, ex);
					throw;
				}
			}
		}

		public T Receive<T>() where T : class
		{
			using (MessageQueue queue = Open(QueueAccessMode.ReceiveAndAdmin))
			{
				try
				{
					MessageQueueTransactionType tt = MessageQueueTransactionType.None;

					if (queue.Transactional)
					{
						EnsureThereIsATransactionScope();

						tt = MessageQueueTransactionType.Automatic;
					}

					MessageEnumerator enumerator = queue.GetMessageEnumerator2();
					while (enumerator.MoveNext())
					{
						Message msg = enumerator.Current;
						Type messageType = Type.GetType(msg.Label);

						if (messageType == typeof (T))
						{
							Message received = enumerator.RemoveCurrent(TimeSpan.FromSeconds(1), tt);
							if (received.Id == msg.Id)
							{
								try
								{
									T obj = _formatter.Deserialize<T>(new MsmqFormattedBody(msg));

									if (_log.IsDebugEnabled)
										_log.DebugFormat("Queue: {0} Received Message Id {1}", queue.Path, msg.Id);

									if (_messageLog.IsInfoEnabled)
										_messageLog.InfoFormat("RECV:{0}:{1}:{3}", queue.Path, messageType, msg.Id);

									return obj;
								}
								catch (SerializationException ex)
								{
									throw new MessageException(messageType, string.Format("An error occurred serializing a message of type {0}", messageType.FullName), ex);
								}
							}
							else
							{
								if (_log.IsDebugEnabled)
									_log.DebugFormat("Queue: {0} Unmatched Message Id {1}", queue.Path, msg.Id);

								enumerator.Close();
								enumerator = queue.GetMessageEnumerator2();
							}
						}
						else
						{
							if (_log.IsDebugEnabled)
								_log.DebugFormat("Queue: {0} Skipped Message Id {1}", queue.Path, msg.Id);
						}
					}
					enumerator.Close();
				}
				catch (MessageQueueException ex)
				{
					HandleVariousErrorCodes(ex.MessageQueueErrorCode, ex);
					throw;
				}

				return null;
			}
		}

		public T Receive<T>(Predicate<T> accept) where T : class
		{
			T result;
			while ((result = Receive<T>()) != null)
			{
				if (accept(result))
					return result;
			}

			return result;
		}

		public void Dispose()
		{
			if (_receiver != null)
				_receiver.Dispose();

			if (_sender != null)
				_sender.Dispose();
		}

		private static void HandleVariousErrorCodes(MessageQueueErrorCode code, Exception ex)
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
					break;

				case MessageQueueErrorCode.QueueNotFound:
				case MessageQueueErrorCode.IllegalFormatName:
				case MessageQueueErrorCode.MachineNotFound:
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

		private void EnsureThereIsATransactionScope()
		{
			if (Transaction.Current == null)
			{
				throw new EndpointException(this, "This is a transactional endpoint, and requires a TransactionScope to be open");
			}
		}

		#endregion

		/// <summary>
		/// Implicitly creates a <c ref="MsmqEndpoint" />.
		/// </summary>
		/// <param name="queueUri">A string identifying the URI of the message queue (ex. msmq://localhost/my_queue)</param>
		/// <returns>An instance of the MessageQueueEndpoint class</returns>
		public static implicit operator MsmqEndpoint(string queueUri)
		{
			return new MsmqEndpoint(queueUri);
		}

		/// <summary>
		/// Returns the URI string for the message queue endpoint.
		/// </summary>
		/// <param name="endpoint">The endpoint to use to generate the URI string</param>
		/// <returns>A URI string that identifies the message queue endpoint</returns>
		public static implicit operator string(MsmqEndpoint endpoint)
		{
			return endpoint.Uri.AbsoluteUri;
		}
	}
}