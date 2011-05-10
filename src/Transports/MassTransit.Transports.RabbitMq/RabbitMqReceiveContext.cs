namespace MassTransit.Transports.RabbitMq
{
	using System.IO;
	using RabbitMQ.Client;

	public class RabbitMqReceiveContext :
		IReceiveContext
	{
		readonly IBasicProperties _basicProperties;
		MemoryStream _body;

		public RabbitMqReceiveContext(IBasicProperties basicProperties, byte[] body)
		{
			_basicProperties = basicProperties;
			_body = new MemoryStream(body, false);
		}

		public string MessageId
		{
			get { return _basicProperties.MessageId; }
		}

		public Stream Body
		{
			get { return _body; }
		}

		public void Dispose()
		{
			if (_body != null)
			{
				_body.Dispose();
				_body = null;
			}
		}
	}
}