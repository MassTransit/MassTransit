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
		private string _machineName = Environment.MachineName;
		private MessageQueue _queue;
		private bool _reliableMessaging = true;

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

			_queue = Open(QueueAccessMode.SendAndReceive);
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

			_queue = Open(QueueAccessMode.SendAndReceive);
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

		public void Send<T>(T message) where T : class
		{
			Send(message, MessageQueue.InfiniteTimeout);
		}

		public void Send<T>(T message, TimeSpan timeToLive) where T : class
		{
			Type messageType = typeof (T);

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

				_queue.Send(msg, GetTransactionType());
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
			return Receive(MessageQueue.InfiniteTimeout);
		}

		public object Receive(TimeSpan timeout)
		{
			try
			{
				Message msg = _queue.Receive(timeout, GetTransactionType());

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
			}

			return null;
		}

		public object Receive(Predicate<object> accept)
		{
			return Receive(MessageQueue.InfiniteTimeout, accept);
		}

		public object Receive(TimeSpan timeout, Predicate<object> accept)
		{
			try
			{
				MessageQueueTransactionType transactionType = GetTransactionType();

				DateTime started = DateTime.Now;
				while (started + timeout > DateTime.Now)
				{
					using (MessageEnumerator enumerator = _queue.GetMessageEnumerator2())
					{
						// account for the fact that we might be doing multiple reads on the enumerator
						while (enumerator.MoveNext(timeout))
						{
							Message msg = enumerator.Current;

							try
							{
								object obj = _formatter.Deserialize(new MsmqFormattedBody(msg));

								if (accept(obj))
								{
									Message received = enumerator.RemoveCurrent(TimeSpan.FromSeconds(10), transactionType);
									if (received.Id != msg.Id)
										throw new MessageException(obj.GetType(), "The message removed does not match the original message");

									if (_log.IsDebugEnabled)
										_log.DebugFormat("Queue: {0} Received Message Id {1}", _queue.Path, msg.Id);

									if (_messageLog.IsInfoEnabled)
										_messageLog.InfoFormat("RECV:{0}:System.Object:{1}", _queue.Path, msg.Id);

									return obj;
								}
								else
								{
									if (_log.IsDebugEnabled)
										_log.DebugFormat("Queue: {0} Skipped Message Id {1}", _queue.Path, msg.Id);
								}
							}
							catch (SerializationException ex)
							{
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

		public T Receive<T>() where T : class
		{
			return Receive<T>(MessageQueue.InfiniteTimeout);
		}

		public T Receive<T>(TimeSpan timeout) where T : class
		{
			return (T) Receive(timeout, delegate(object obj)
			                            	{
			                            		Type messageType = obj.GetType();

			                            		if (messageType != typeof (T))
			                            			return false;

			                            		return true;
			                            	});
		}

		public T Receive<T>(Predicate<T> accept) where T : class
		{
			return Receive(MessageQueue.InfiniteTimeout, accept);
		}

		public T Receive<T>(TimeSpan timeout, Predicate<T> accept) where T : class
		{
			return (T) Receive(timeout, delegate(object obj)
			                            	{
			                            		Type messageType = obj.GetType();

			                            		if (messageType != typeof (T))
			                            			return false;

			                            		T message = obj as T;
			                            		if (message == null)
			                            			return false;

			                            		return accept(message);
			                            	});
		}


		public void Dispose()
		{
			if (_queue != null)
				_queue.Dispose();
		}

		private MessageQueueTransactionType GetTransactionType()
		{
			MessageQueueTransactionType transactionType = MessageQueueTransactionType.None;
			// a local queue should be checked to see if it supports transactions
			if (string.Compare(Uri.Host, _machineName, true) == 0)
				if (_queue.Transactional)
				{
					if (Transaction.Current == null)
						throw new EndpointException(this, "This is a transactional endpoint that requires a TransactionScope to be open");

					transactionType = MessageQueueTransactionType.Automatic;
				}

			return transactionType;
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

				case MessageQueueErrorCode.InvalidHandle:
					break;

				default:
					if (_log.IsErrorEnabled)
						_log.Error("An error occured while communicating with the queue", ex);
					break;
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