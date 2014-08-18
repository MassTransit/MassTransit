namespace MassTransit.Transports.RabbitMq
{
	using System.Configuration;

	public class RabbitMqRoutingKeyProvider
	{
		public static string RouteKey
		{
			get
			{
				var routingKey = ConfigurationManager.AppSettings["RabbitMqRoutingKey"];
				return string.IsNullOrEmpty(routingKey) ? string.Empty : routingKey;
			}
		}
	}
}
