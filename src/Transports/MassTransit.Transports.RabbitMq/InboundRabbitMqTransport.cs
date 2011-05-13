namespace MassTransit.Transports.RabbitMq
{
	using System;
	using System.IO;
	using Context;
	using log4net;
	using Magnum.Extensions;
	using Management;
	using RabbitMQ.Client;
	using RabbitMQ.Client.Events;
	using Util;

	public class InboundRabbitMqTransport :
		IInboundTransport
	{
		static readonly ILog _log = LogManager.GetLogger(typeof (InboundRabbitMqTransport));

		readonly IRabbitMqEndpointAddress _address;
		readonly IConnection _connection;
		IModel _channel;
		QueueingBasicConsumer _consumer;
		string _consumerTag;
		bool _declared;
		bool _disposed;

		public InboundRabbitMqTransport(IRabbitMqEndpointAddress address, IConnection connection)
		{
			_address = address;
			_connection = connection;
		}

		public IEndpointAddress Address
		{
			get { return _address; }
		}

		public void Receive(Func<IReceiveContext, Action<IReceiveContext>> callback, TimeSpan timeout)
		{
			BeginReceiver();

			try
			{
				object obj;
				if (!_consumer.Queue.Dequeue((int) timeout.TotalMilliseconds, out obj))
					return;

				var result = obj as BasicDeliverEventArgs;
				if (result == null)
					return;

				using(var body = new MemoryStream(result.Body, false))
				{
					var context = new ConsumeContext(body);
					using (ContextStorage.CreateContextScope(context))
					{
						Action<IReceiveContext> receive = callback(context);
						if (receive == null)
						{
							if (_log.IsDebugEnabled)
								_log.DebugFormat("SKIP:{0}:{1}", Address, result.BasicProperties.MessageId);

							if (SpecialLoggers.Messages.IsInfoEnabled)
								SpecialLoggers.Messages.InfoFormat("SKIP:{0}:{1}", Address, result.BasicProperties.MessageId);
						}
						else
						{
							receive(context);
						}
					}
				}

				_channel.BasicAck(result.DeliveryTag, false);
			}
			catch (EndOfStreamException)
			{
				return;
			}
			catch (Exception ex)
			{
				_log.Error("Failed to consume message from endpoint", ex);

				throw;
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		void BeginReceiver()
		{
			if (_channel != null)
				return;

			DeclareBindings();

			_channel = _connection.CreateModel();
			_consumer = new QueueingBasicConsumer(_channel);
			_consumerTag = Guid.NewGuid().ToString();
			_channel.BasicConsume(_address.Name, false, _consumerTag, _consumer);
		}

		void EndReceiver()
		{
			if (_consumerTag.IsNotEmpty())
			{
				_channel.BasicCancel(_consumerTag);
				_consumerTag = null;
			}

			_consumer = null;

			if (_channel != null)
			{
				if (_channel.IsOpen)
					_channel.Close(200, "end");
				_channel.Dispose();
				_channel = null;
			}
		}

		~InboundRabbitMqTransport()
		{
			Dispose(false);
		}

		void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
				EndReceiver();
			}

			_disposed = true;
		}

		void DeclareBindings()
		{
			if (_declared)
				return;

			using (var management = new RabbitMqEndpointManagement(_address, _connection))
			{
				management.BindQueue(_address.Name, _address.Name, ExchangeType.Fanout, "");
			}

			_declared = true;
		}
	}
}