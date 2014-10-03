namespace MassTransit.Transports.RabbitMq
{
	public class RabbitMqExchangeTypeProvider
	{
		public static string ExchangeType
		{
			get
			{
				return string.IsNullOrEmpty(RabbitMqRoutingKeyProvider.RouteKey)
					? RabbitMQ.Client.ExchangeType.Fanout
					: RabbitMQ.Client.ExchangeType.Topic;
			}
		}
	}
}