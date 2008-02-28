using System;
using System.Runtime.Serialization;
using System.Threading;
using Apache.NMS;
using MassTransit.ServiceBus.Exceptions;
using MassTransit.ServiceBus.Internal;
using IMessageConsumer=Apache.NMS.IMessageConsumer;

namespace MassTransit.ServiceBus.NMS
{
	public class NmsMessageReceiver :
		IMessageReceiver
	{
		private IEnvelopeConsumer _consumer;
		private readonly IEndpoint _endpoint;
		private IConnectionFactory _factory;
		private readonly string _queueName;
		private IConnection _connection;
		private ISession _session;
		private IDestination _destination;
		private IMessageConsumer _messageConsumer;

		public NmsMessageReceiver(IEndpoint endpoint)
		{
			_endpoint = endpoint;

			UriBuilder queueUri = new UriBuilder("tcp", endpoint.Uri.Host, endpoint.Uri.Port);

			_queueName = endpoint.Uri.AbsolutePath.Substring(1);

			_factory = new Apache.NMS.ActiveMQ.ConnectionFactory(queueUri.Uri);
		}

		public void Subscribe(IEnvelopeConsumer consumer)
		{
			lock (this)
			{
				if (_consumer == null)
				{
					_consumer = consumer;

					Start();
				}
				else if (_consumer != consumer)
				{
					throw new EndpointException(_endpoint, "Only one consumer can be registered for a message receiver");
				}
			}
		}

		public void Start()
		{
			_connection = _factory.CreateConnection();
			_session = _connection.CreateSession();
			_destination = _session.GetQueue(_queueName);
			_messageConsumer = _session.CreateConsumer(_destination);

			_messageConsumer.Listener += new MessageListener(MessageListener);
		}

		void MessageListener(Apache.NMS.IMessage message)
		{
			try
			{
//				if (_log.IsDebugEnabled)
//					_log.DebugFormat("Queue: {0} Received Message Id {1}", _queue.Path, msg.Id);

				if (_consumer != null)
				{
					try
					{
						IEnvelope e = NmsEnvelopeMapper.MapFrom(message);

						if (_consumer.IsHandled(e))
						{
								//if (_messageLog.IsInfoEnabled)
						//			_messageLog.InfoFormat("Received message {0} from {1}", e.Messages[0].GetType(), e.ReturnEndpoint.Uri);

								ThreadPool.QueueUserWorkItem(ProcessMessage, e);
						}
					}
					catch (SerializationException ex)
					{
						//_log.Error("Discarding unknown message " + message.NMSMessageId, ex);
						throw;
					}
				}
			}
			catch (Exception ex)
			{
				throw;
			}
		}
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
				//if (_log.IsErrorEnabled)
//_log.Error(string.Format("An exception occured delivering envelope {0}", e.Id), ex);

			}
		}

		public void Stop()
		{
			if (_messageConsumer != null)
			{
				_messageConsumer.Dispose();
				_messageConsumer = null;
			}

			_destination = null;

			if (_session != null)
			{
				_session.Dispose();
				_session = null;
			}
			if (_connection != null)
			{
				_connection.Dispose();
				_connection = null;
			}
		}

		public void Dispose()
		{
			Stop();
		}
	}
}