namespace MassTransit.Transports.RabbitMq
{
	public sealed class RabbitMqRoutingKeyProvider
	{
		private static RabbitMqRoutingKeyProvider _instance = new RabbitMqRoutingKeyProvider();

		private RabbitMqRoutingKeyProvider()
		{
			RouteKey = string.Empty;
		}

		public static RabbitMqRoutingKeyProvider Instance
		{
			get { return _instance; }
		}
		
		public string RouteKey { get;  private set; }

		public static void CreateProvider(string routingKey)
		{
			var instance = new RabbitMqRoutingKeyProvider {RouteKey = routingKey};
			_instance = instance;
		}
	}
}