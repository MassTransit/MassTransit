namespace MassTransit.Transports.RabbitMq.Management
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using log4net;
	using RabbitMQ.Client;

	public class RabbitMqEndpointManagement :
		IRabbitMqEndpointManagement
	{
		static readonly ILog _log = LogManager.GetLogger(typeof (RabbitMqEndpointManagement));
		readonly IRabbitMqEndpointAddress _address;
		readonly bool _owned;
		IConnection _connection;
		bool _disposed;

		public RabbitMqEndpointManagement(IRabbitMqEndpointAddress address)
			: this(address, address.ConnectionFactory.CreateConnection())
		{
			_owned = true;
		}

		public RabbitMqEndpointManagement(IRabbitMqEndpointAddress address, IConnection connection)
		{
			_address = address;
			_connection = connection;
		}

		public void BindQueue(string queueName, string exchangeName, string exchangeType, string routingKey)
		{
			using (IModel model = _connection.CreateModel())
			{
				string queue = model.QueueDeclare(queueName, true, false, false, null);
				model.ExchangeDeclare(exchangeName, exchangeType, true);

				model.QueueBind(queue, exchangeName, routingKey);

				model.Close(200, "ok");
			}
		}

		public void UnbindQueue(string queueName, string exchangeName, string routingKey)
		{
			using (IModel model = _connection.CreateModel())
			{
				model.QueueUnbind(queueName, exchangeName, routingKey, null);

				model.Close(200, "ok");
			}
		}

		public void BindExchange(string destination, string source, string exchangeType, string routingKey)
		{
			using (IModel model = _connection.CreateModel())
			{
				model.ExchangeDeclare(destination, exchangeType, true, false, null);
				model.ExchangeDeclare(source, exchangeType, true, false, null);

				model.ExchangeBind(destination, source, routingKey);

				model.Close(200, "ok");
			}
		}

		public void UnbindExchange(string destination, string source, string routingKey)
		{
			try
			{
				using (IModel model = _connection.CreateModel())
				{
					model.ExchangeUnbind(destination, source, routingKey, null);

					model.Close(200, "ok");
				}
			}
			catch (Exception ex)
			{
				_log.Error("Failed to unbind the exchange", ex);
			}
		}

		public IEnumerable<Type> BindExchangesForPublisher(Type messageType)
		{
			var messageName = new MessageName(messageType);

			using (IModel model = _connection.CreateModel())
			{
				model.ExchangeDeclare(messageName.ToString(), ExchangeType.Fanout, true, false, null);

				yield return messageType;

				foreach (var type in messageType.GetMessageTypes().Skip(1))
				{
					var interfaceName = new MessageName(type);

					model.ExchangeDeclare(interfaceName.ToString(), ExchangeType.Fanout, true, false, null);
					model.ExchangeBind(interfaceName.ToString(), messageName.ToString(), "");

					yield return type;
				}

				model.Close(200, "ok");
			}
		}

		public void BindExchangesForSubscriber(Type messageType)
		{
			var messageName = new MessageName(messageType);

			BindExchange(_address.Name, messageName.ToString(), ExchangeType.Fanout, "");
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
				if (_connection != null)
				{
					if (_owned)
					{
						if (_connection.IsOpen)
							_connection.Close(200, "normal");
						_connection.Dispose();
					}

					_connection = null;
				}
			}

			_disposed = true;
		}

		~RabbitMqEndpointManagement()
		{
			Dispose(false);
		}
	}
}