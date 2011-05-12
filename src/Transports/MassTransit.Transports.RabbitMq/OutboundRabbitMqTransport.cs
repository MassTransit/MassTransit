namespace MassTransit.Transports.RabbitMq
{
	using System;
	using System.IO;
	using Context;
	using Magnum;
	using RabbitMQ.Client;
	using Util;

	public class OutboundRabbitMqTransport :
		IOutboundTransport
	{
		readonly IRabbitMqEndpointAddress _address;
		readonly IConnection _connection;
		IModel _channel;
		bool _declared;

		public OutboundRabbitMqTransport(IRabbitMqEndpointAddress address, IConnection connection)
		{
			_address = address;
			_connection = connection;
		}

		public IEndpointAddress Address
		{
			get { return _address; }
		}

		public void Send(ISendContext context)
		{
			DeclareBindings();

			IBasicProperties properties = _channel.CreateBasicProperties();

			properties.SetPersistent(true);
			if(context.ExpirationTime.HasValue)
			{
				var value = context.ExpirationTime.Value;
				properties.Expiration = (value.Kind == DateTimeKind.Utc ? value - SystemUtil.UtcNow : value - SystemUtil.Now).ToString();
			}

			using(var body = new MemoryStream())
			{
				context.SerializeTo(body);

				_channel.BasicPublish(_address.Name, "", properties, body.ToArray());

				if (SpecialLoggers.Messages.IsInfoEnabled)
					SpecialLoggers.Messages.InfoFormat("SEND:{0}:{1}", Address, context.MessageId);
			}
		}

		public void Dispose()
		{
			if (_channel != null)
			{
				if (_channel.IsOpen)
					_channel.Close(200, "dispose");
				_channel.Dispose();
				_channel = null;
			}
		}

		void DeclareBindings()
		{
			if (_declared)
				return;

			_channel = _connection.CreateModel();

			_channel.ExchangeDeclare(_address.Name, ExchangeType.Fanout, true);

			_declared = true;
		}
	}
}