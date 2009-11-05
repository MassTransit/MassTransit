namespace OrderWeb
{
	using MassTransit;
	using MassTransit.StructureMapIntegration;
	using MassTransit.Transports;
	using MassTransit.Transports.Msmq;
	using StructureMap;

	public class OrderWebRegistry :
		MassTransitRegistryBase
	{
		public OrderWebRegistry()
		{
			MsmqEndpointConfigurator.Defaults(x =>
				{
					x.CreateMissingQueues = true;
					x.CreateTransactionalQueues = true;
					x.PurgeOnStartup = true;
				});

			RegisterEndpointFactory(x =>
				{
					x.RegisterTransport<MsmqEndpoint>();
					x.RegisterTransport<LoopbackEndpoint>();
				});

			RegisterControlBus("msmq://localhost/mts_orderweb_control", x => { });

			RegisterServiceBus("msmq://localhost/mts_orderweb", x =>
				{
					x.UseControlBus(ObjectFactory.GetInstance<IControlBus>());
					x.SetConcurrentConsumerLimit(20);

					ConfigureSubscriptionClient("msmq://localhost/mt_subscriptions", x);
			});

		}
	}
}