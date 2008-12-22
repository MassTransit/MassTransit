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
namespace MassTransit.Transports.Nms
{
	using System;
	using System.IO;
	using System.Runtime.Serialization;
	using Apache.NMS;
	using Apache.NMS.ActiveMQ;
	using Configuration;
	using Exceptions;
	using Internal;
	using log4net;
	using Serialization;

	public class NmsEndpoint :
		IEndpoint
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (NmsEndpoint));
		private readonly IMessageSerializer _serializer;
		private readonly IConnectionFactory _factory;
		private readonly string _queueName;
		private readonly Uri _uri;
		private bool _disposed;
		private IConnection _connection;

		public NmsEndpoint(Uri uri, IMessageSerializer serializer)
		{
			_uri = uri;
		    _serializer = serializer;
			UriBuilder queueUri = new UriBuilder("tcp", Uri.Host, Uri.Port);

			_queueName = Uri.AbsolutePath.Substring(1);

			_factory = new ConnectionFactory(queueUri.Uri);

			_connection = _factory.CreateConnection();
			_connection.ExceptionListener += ConnectionExceptionListener;

			_connection.Start();
		}

		public NmsEndpoint(string uriString, IMessageSerializer serializer)
			: this(new Uri(uriString), serializer)
		{
		}

		public static string Scheme
		{
			get { return "activemq"; }
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
			WithinProducerContext((session, producer) =>
			{
				Type messageType = typeof(T);

				IBytesMessage bm = session.CreateBytesMessage();

				using (MemoryStream mem = new MemoryStream())
				{
					_serializer.Serialize(mem, message);

					bm.Content = mem.ToArray();
				}

				if (timeToLive < NMSConstants.defaultTimeToLive)
					producer.TimeToLive = timeToLive;

				producer.Persistent = true;

				producer.Send(bm);

				if (SpecialLoggers.Messages.IsInfoEnabled)
					SpecialLoggers.Messages.InfoFormat("SEND {0} {1} {2}", messageType, Uri, bm.NMSMessageId);

				if (_log.IsDebugEnabled)
					_log.DebugFormat("Message Sent: Id = {0}, Message Type = {1}", bm.NMSMessageId, messageType);
			});
		}

		public object Receive(TimeSpan timeout)
		{
			return Receive(timeout, x => true);
		}

		public object Receive(TimeSpan timeout, Predicate<object> accept)
		{
			return WithinConsumerContext(consumer =>
			{
				IMessage message = consumer.Receive(timeout);
				if (message == null)
					return null;

				IBytesMessage bm = message as IBytesMessage;
				if (bm == null)
					throw new MessageException(message.GetType(), "Message not a IBytesMessage");

				try
				{
					using (MemoryStream mem = new MemoryStream(bm.Content, false))
					{
						object obj = _serializer.Deserialize(mem);

						if (accept(obj))
						{
							if (_log.IsDebugEnabled)
								_log.DebugFormat("Queue: {0} Received Message Id {1}", _queueName, message.NMSMessageId);

							if (SpecialLoggers.Messages.IsInfoEnabled)
								SpecialLoggers.Messages.InfoFormat("RECV {0} {1} {2}", obj.GetType().FullName, Uri, bm.NMSMessageId);

							return obj;
						}

						if (_log.IsDebugEnabled)
							_log.DebugFormat("Queue: {0} Skipped Message Id {1}", _queueName, message.NMSMessageId);

						return null;
					}
				}
				catch (SerializationException ex)
				{
					try
					{
						_log.Error("Discarded message " + message.NMSMessageId + " due to a serialization error", ex);
					}
					catch (Exception ex2)
					{
						_log.Error("Unable to purge message id " + message.NMSMessageId, ex2);
					}

					throw new MessageException(typeof(object), "An error occurred deserializing a message", ex);
				}
			});
		}

		public void Receive(TimeSpan timeout, Func<object, Func<object, bool>, bool> receiver)
		{
			throw new System.NotImplementedException();
		}

		private V WithinConsumerContext<V>(Func<IMessageConsumer, V> consumerAction)
		{
			return WithinSessionContext<V>(session =>
			{
				IMessageConsumer consumer = null;
				try
				{
					IDestination destination = session.GetQueue(_queueName);

					consumer = session.CreateConsumer(destination);

					V result = consumerAction(consumer);

					return result;
				}
				finally
				{
					if (consumer != null)
					{
						consumer.Close();
						consumer.Dispose();
					}
				}
			});
		}

		private void WithinProducerContext(Action<ISession, IMessageProducer> producerAction)
		{
			WithinSessionContext(session =>
			{
				IMessageProducer producer = null;
				try
				{
					IDestination destination = session.GetQueue(_queueName);

					producer = session.CreateProducer(destination);

					producerAction(session, producer);
				}
				finally
				{
					if (producer != null)
					{
						producer.Close();
					}
				}
			});
		}

		private V WithinSessionContext<V>(Func<ISession, V> sessionAction)
		{
			ISession session = null;
			try
			{
				session = _connection.CreateSession(AcknowledgementMode.Transactional);

				V result = sessionAction(session);

				session.Commit();

				return result;
			}
			catch (Exception ex)
			{
				throw new EndpointException(this, "Error consuming message", ex);
			}
			finally
			{
				if (session != null)
				{
					session.Close();
					session.Dispose();
				}
			}
		}

		private void WithinSessionContext(Action<ISession> sessionAction)
		{
			ISession session = null;
			try
			{
				session = _connection.CreateSession(AcknowledgementMode.Transactional);

				sessionAction(session);

				session.Commit();
			}
			catch (Exception ex)
			{
				throw new EndpointException(this, "Error consuming message", ex);
			}
			finally
			{
				if (session != null)
				{
					session.Close();
					session.Dispose();
				}
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void ConnectionExceptionListener(Exception ex)
		{
			_log.Error("An exception occurred on the endpoint. Uri = " + _uri, ex);
		}

		~NmsEndpoint()
		{
			Dispose(false);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
				if (_connection != null)
				{
					_connection.Stop();
					_connection.ExceptionListener -= ConnectionExceptionListener;
					_connection.Close();
					_connection.Dispose();
					_connection = null;
				}
			}
			_disposed = true;
		}
        public static IEndpoint ConfigureEndpoint(Uri uri, Action<IEndpointConfigurator> configurator)
        {
            if (uri.Scheme.ToLowerInvariant() == Scheme)
            {
                IEndpoint endpoint = NmsEndpointConfigurator.New(x =>
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