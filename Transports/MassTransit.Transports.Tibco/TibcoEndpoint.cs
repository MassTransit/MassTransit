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
namespace MassTransit.Transports.Tibco
{
	using System;
	using System.IO;
	using System.Runtime.Serialization;
	using Exceptions;
	using Internal;
	using log4net;
	using Serialization;
	using TIBCO.EMS;

	public class TibcoEndpoint :
		IEndpoint,
		IExceptionListener
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (TibcoEndpoint));
		private static readonly IMessageSerializer _serializer = new BinaryMessageSerializer();

		private readonly Uri _uri;
		private int _deliveryMode = DeliveryMode.RELIABLE_DELIVERY;
		private bool _disposed;
		private ConnectionFactory _factory;
		private string _queueName;

		public TibcoEndpoint(string uriString)
			: this(new Uri(uriString))
		{
		}

		public TibcoEndpoint(Uri uri)
		{
			_uri = uri;

			_queueName = Uri.AbsolutePath.Substring(1);

			string url = _uri.Host + ":" + _uri.Port;

			_factory = new ConnectionFactory(url);
		}

		public static string Scheme
		{
			get { return "tibco"; }
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

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
			Type messageType = typeof (T);

			try
			{
				Connection connection = OpenConnection();

				Session session = connection.CreateSession(false, SessionMode.AutoAcknowledge);

				BytesMessage bm = session.CreateBytesMessage();

				using (MemoryStream mem = new MemoryStream())
				{
					_serializer.Serialize(mem, message);

					bm.WriteBytes(mem.ToArray());
				}

				Destination destination = session.CreateQueue(_queueName);

				MessageProducer producer = session.CreateProducer(destination);

				if (timeToLive < TimeSpan.MaxValue)
					producer.TimeToLive = (long) timeToLive.TotalMilliseconds;

				producer.DeliveryMode = _deliveryMode;

				producer.Send(bm);

				if (SpecialLoggers.Messages.IsInfoEnabled)
					SpecialLoggers.Messages.InfoFormat("Message {0} Sent To {1}", messageType, Uri);

				if (_log.IsDebugEnabled)
					_log.DebugFormat("Message Sent: Id = {0}, Message Type = {1}", bm.MessageID, messageType);
			}
			catch (Exception ex)
			{
				throw new EndpointException(this, "Problem with " + Uri, ex);
			}
		}

		private Connection OpenConnection()
		{
			Connection connection = _factory.CreateConnection();

			connection.ExceptionListener = this;

			connection.Start();

			return connection;
		}

		public object Receive(TimeSpan timeout)
		{
			try
			{
				Connection connection = OpenConnection();

				Session session = connection.CreateSession(false, SessionMode.NoAcknowledge);

				Destination destination = session.CreateQueue(_queueName);

				MessageConsumer consumer = session.CreateConsumer(destination);

				Message message = consumer.Receive((long) timeout.TotalMilliseconds);
				if (message == null)
					return null;

				BytesMessage bm = message as BytesMessage;
				if (bm == null)
					throw new MessageException(message.GetType(), "Message not a IBytesMessage");

				byte[] bytes = new byte[bm.BodyLength];
				bm.ReadBytes(bytes);

				try
				{
					using (MemoryStream mem = new MemoryStream(bytes, false))
					{
						object obj = _serializer.Deserialize(mem);

						return obj;
					}
				}
				catch (SerializationException ex)
				{
					throw new MessageException(message.GetType(), "An error occurred deserializing a message", ex);
				}
			}
			catch (Exception ex)
			{
				throw new EndpointException(this, "Receive error occured", ex);
			}
		}

		public object Receive(TimeSpan timeout, Predicate<object> accept)
		{
			try
			{
				Connection connection = OpenConnection();

				Session session = connection.CreateSession(false, SessionMode.NoAcknowledge);

				Destination destination = session.CreateQueue(_queueName);

				MessageConsumer consumer = session.CreateConsumer(destination);

				Message message = consumer.Receive((long) timeout.TotalMilliseconds);
				if (message == null)
					return null;

				BytesMessage bm = message as BytesMessage;
				if (bm == null)
					throw new MessageException(message.GetType(), "Message not a IBytesMessage");

				byte[] bytes = new byte[bm.BodyLength];
				bm.ReadBytes(bytes);

				try
				{
					using (MemoryStream mem = new MemoryStream(bytes, false))
					{
						object obj = _serializer.Deserialize(mem);

						if (accept(obj))
						{
							if (_log.IsDebugEnabled)
								_log.DebugFormat("Queue: {0} Received Message Id {1}", _queueName, message.MessageID);

							if (SpecialLoggers.Messages.IsInfoEnabled)
								SpecialLoggers.Messages.InfoFormat("RECV:{0}:{1}", _uri, obj.GetType().Name);

							return obj;
						}

						if (_log.IsDebugEnabled)
							_log.DebugFormat("Queue: {0} Skipped Message Id {1}", _queueName, message.MessageID);

						return null;
					}
				}
				catch (SerializationException ex)
				{
					try
					{
						message.Acknowledge();

						_log.Error("Discarded message " + message.MessageID + " due to a serialization error", ex);
					}
					catch (Exception ex2)
					{
						_log.Error("Unable to purge message id " + message.MessageID, ex2);
					}

					throw new MessageException(typeof (object), "An error occurred deserializing a message", ex);
				}
			}
			catch (Exception ex)
			{
				throw new EndpointException(this, "Receive error occured", ex);
			}
		}

		public void OnException(EMSException exception)
		{
			_log.Error("An exception occurred on the endpoint. Uri = " + _uri, exception);
		}

		~TibcoEndpoint()
		{
			Dispose(false);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
			}
			_disposed = true;
		}
	}
}