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
namespace MassTransit.ServiceBus.NMS
{
	using System;
	using System.IO;
	using System.Runtime.Serialization;
	using System.Runtime.Serialization.Formatters.Binary;
	using Apache.NMS;
	using Apache.NMS.ActiveMQ;
	using Exceptions;
	using log4net;

	public class NmsEndpoint :
		INmsEndpoint
	{
		private static readonly IFormatter _formatter = new BinaryFormatter();
		private static readonly ILog _log = LogManager.GetLogger(typeof (NmsEndpoint));
		private static readonly ILog _messageLog = LogManager.GetLogger("MassTransit.Messages");
		private readonly IConnectionFactory _factory;
		private readonly string _queueName;
		private readonly Uri _uri;
		private readonly IConnection _connection;

		public NmsEndpoint(Uri uri)
		{
			_uri = uri;

			UriBuilder queueUri = new UriBuilder("tcp", Uri.Host, Uri.Port);

			_queueName = Uri.AbsolutePath.Substring(1);

			_factory = new ConnectionFactory(queueUri.Uri);

			_connection = _factory.CreateConnection();
			_connection.ExceptionListener += ConnectionExceptionListener;
		}

		void ConnectionExceptionListener(Exception ex)
		{
			_log.Error("NMS threw an exception: ", ex);
		}

		public NmsEndpoint(string uriString)
			: this(new Uri(uriString))
		{
		}

		public Uri Uri
		{
			get { return _uri; }
		}

		public static string Scheme
		{
			get { return "activemq"; }
		}


		public void Send<T>(T message) where T : class
		{
			Send(message, NMSConstants.defaultTimeToLive);
		}

		public void Send<T>(T message, TimeSpan timeToLive) where T : class
		{
			Type messageType = typeof (T);

			using (ISession session = _connection.CreateSession())
			{
				IBytesMessage bm = session.CreateBytesMessage();

				MemoryStream mem = new MemoryStream();
				_formatter.Serialize(mem, message);

				bm.Content = new byte[mem.Length];
				mem.Seek(0, SeekOrigin.Begin);
				mem.Read(bm.Content, 0, (int) mem.Length);

				if (timeToLive < NMSConstants.defaultTimeToLive)
					bm.NMSTimeToLive = timeToLive;

				bm.NMSPersistent = true;

				IDestination destination = session.GetQueue(_queueName);

				using (IMessageProducer producer = session.CreateProducer(destination))
				{
					try
					{
						producer.Send(bm);

						if (_messageLog.IsInfoEnabled)
							_messageLog.InfoFormat("Message {0} Sent To {1}", messageType, Uri);
					}
					catch (Exception ex)
					{
						throw new EndpointException(this, "Problem with " + Uri, ex);
					}

					if (_log.IsDebugEnabled)
						_log.DebugFormat("Message Sent: Id = {0}, Message Type = {1}", bm.NMSMessageId, messageType);
				}
			}
		}

		public object Receive()
		{
			return Receive(TimeSpan.MaxValue);
		}

		public object Receive(TimeSpan timeout)
		{
			try
			{
				using (ISession session = _connection.CreateSession())
				{
					IDestination destination = session.GetQueue(_queueName);
					using (IMessageConsumer consumer = session.CreateConsumer(destination))
					{
						IMessage message = consumer.Receive(timeout);
						if (message == null)
							return null;

						IBytesMessage bm = message as IBytesMessage;
						if (bm == null)
							throw new MessageException(message.GetType(), "Message not a IBytesMessage");

						try
						{
							MemoryStream mem = new MemoryStream(bm.Content, false);

							object obj = _formatter.Deserialize(mem);

							return obj;
						}
						catch (SerializationException ex)
						{
							throw new MessageException(message.GetType(), "An error occurred deserializing a message", ex);
						}
					}
				}
			}
			catch (Exception ex)
			{
				throw new EndpointException(this, "Receive error occured", ex);
			}
		}

		public object Receive(Predicate<object> accept)
		{
			return Receive(TimeSpan.MaxValue, accept);
		}

		public object Receive(TimeSpan timeout, Predicate<object> accept)
		{
			using (ISession session = _connection.CreateSession())
			{
				IDestination destination = session.GetQueue(_queueName);
				using (IMessageConsumer consumer = session.CreateConsumer(destination))
				{
					IMessage message = consumer.Receive(timeout);
					if (message == null)
						return null;

					IBytesMessage bm = message as IBytesMessage;
					if (bm == null)
						throw new MessageException(message.GetType(), "Message not a IBytesMessage");

					try
					{
						MemoryStream mem = new MemoryStream(bm.Content, false);

						object obj = _formatter.Deserialize(mem);

						if (accept(obj))
						{
							if (_log.IsDebugEnabled)
								_log.DebugFormat("Queue: {0} Received Message Id {1}", _queueName, message.NMSMessageId);

							if (_messageLog.IsInfoEnabled)
								_messageLog.InfoFormat("RECV:{0}:System.Object:{1}", _queueName, message.NMSMessageId);

							return obj;
						}
						else
						{
							if (_log.IsDebugEnabled)
								_log.DebugFormat("Queue: {0} Skipped Message Id {1}", _queueName, message.NMSMessageId);
						}
					}
					catch (SerializationException ex)
					{
						throw new MessageException(typeof (object), "An error occurred deserializing a message", ex);
					}

					return null;
				}
			}
		}

		public T Receive<T>() where T : class
		{
			return Receive<T>(TimeSpan.MaxValue);
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
			return Receive(TimeSpan.MaxValue, accept);
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
			if (_connection != null)
			{
				_connection.ExceptionListener -= ConnectionExceptionListener;
				_connection.Close();
				_connection.Dispose();
			}
		}
	}
}