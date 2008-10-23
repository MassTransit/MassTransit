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
		private readonly string _queuePath;
		private readonly Uri _uri;
		private readonly string _machineName = Environment.MachineName;
		private MessageQueue _queue;
		private bool _reliableMessaging = true;
	    private bool _isLocal;
	    private MessageQueueTransactionType _sendTransactionType;
	    private MessageQueueTransactionType _receiveTransactionType;

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

            string localhost = Environment.MachineName.ToLowerInvariant();

			string hostName = _uri.Host;
		    if (string.Compare(hostName, ".") == 0 || string.Compare(hostName, "localhost", true) == 0)
			{
			    _uri = new Uri("msmq://" + localhost + _uri.AbsolutePath);
			    _isLocal = true;
			}
            else
		    {
		        _isLocal = string.Compare(_uri.Host, localhost, true) == 0;
		    }

			_queuePath = string.Format(@"FormatName:DIRECT=OS:{0}\private$\{1}", localhost, _uri.AbsolutePath.Substring(1));

			_queue = Open(QueueAccessMode.SendAndReceive);

		    Initialize();
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

            string localhost = Environment.MachineName.ToLowerInvariant();

            if (parts[0] == "." || string.Compare("localhost", parts[0], true) == 0)
            {
                parts[0] = localhost;
                _isLocal = true;
            }
            else
            {
                parts[0] = parts[0].ToLowerInvariant();
                _isLocal = string.Compare(localhost, parts[0], true) == 0;
            }

		    _queuePath = string.Format("{0}{1}\\{2}\\{3}", prefix, parts[0], parts[1], parts[2]);
			_uri = new Uri(string.Format("msmq://{0}/{1}", parts[0], parts[2]));

			_queue = Open(QueueAccessMode.SendAndReceive);

		    Initialize();
		}

        private void Initialize()
        {
            _sendTransactionType = _isLocal && _queue.Transactional ? MessageQueueTransactionType.Automatic : MessageQueueTransactionType.None;

            _receiveTransactionType = _isLocal && _queue.Transactional ? MessageQueueTransactionType.Automatic : MessageQueueTransactionType.None;
        }

		#region IMessageQueueEndpoint Members

		public bool ReliableMessaging
		{
			get { return _reliableMessaging; }
			set { _reliableMessaging = value; }
		}

		public static string Scheme
		{
			get { return "msmq"; }
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

			if (!queue.CanRead)
				throw new EndpointException(this, "The endpoint could not be found: " + QueuePath);

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
			Send(message, TimeSpan.MaxValue);
		}

		public void Send<T>(T message, TimeSpan timeToLive) where T : class
		{
			if (!_queue.CanWrite)
				throw new EndpointException(this, "Not allowed to write to endpoint " + _uri);

			Type messageType = typeof (T);

		    Message msg = BuildMessage(timeToLive, messageType, message);

			try
			{
				if (SpecialLoggers.Messages.IsInfoEnabled)
                    SpecialLoggers.Messages.InfoFormat("SEND:{0}:{1}", Uri, messageType.Name);

				if(_sendTransactionType == MessageQueueTransactionType.Automatic)
				{
					if(Transaction.Current == null)
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
				_log.DebugFormat("Message Sent: Id = {0}, Message Type = {1}", msg.Id, messageType.Name);
		}

	    private Message BuildMessage(TimeSpan timeToLive, Type messageType, object message)
	    {
            var msg = new Message();

            _formatter.Serialize(new MsmqFormattedBody(msg), message);

            if (timeToLive < TimeSpan.MaxValue)
                msg.TimeToBeReceived = timeToLive;

            msg.Label = messageType.AssemblyQualifiedName;

            msg.Recoverable = _reliableMessaging;

	        return msg;
	    }

	    public object Receive(TimeSpan timeout)
		{
			try
			{
				Message msg = _queue.Receive(timeout, _receiveTransactionType);

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

	    public object Receive(TimeSpan timeout, Predicate<object> accept)
		{
			if (!_queue.CanRead)
				throw new EndpointException(this, "Not allowed to read from endpoint");

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

							try
							{
								object obj = _formatter.Deserialize(new MsmqFormattedBody(msg));

								if (accept(obj))
								{
									Message received = enumerator.RemoveCurrent(TimeSpan.FromSeconds(10), _receiveTransactionType);
									if (received.Id != msg.Id)
										throw new MessageException(obj.GetType(), "The message removed does not match the original message");

									if (_log.IsDebugEnabled)
										_log.DebugFormat("Queue: {0} Received Message Id {1}", _queue.Path, msg.Id);

                                    if (SpecialLoggers.Messages.IsInfoEnabled)
                                        SpecialLoggers.Messages.InfoFormat("RECV:{0}:{1}", _uri, obj.GetType().Name);

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
									_log.Error("Unable to purge message id " + msg.Id, ex2);
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

	    public void Dispose()
		{
			if (_queue != null)
				_queue.Dispose();
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