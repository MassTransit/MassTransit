namespace HeavyLoad
{
	using Castle.MicroKernel.Registration;
	using Castle.Windsor;
	using MassTransit.ServiceBus;
	using MassTransit.ServiceBus.Internal;
	using MassTransit.ServiceBus.MSMQ;
	using MassTransit.ServiceBus.Subscriptions;
	using MassTransit.WindsorIntegration;

	public class TestAppContainer :
		WindsorContainer
	{
		public TestAppContainer()
		{
			LoadMassTransit();
		}

		protected void LoadMassTransit()
		{
			Register(
				Component.For<IEndpoint>()
					.ImplementedBy<MsmqEndpoint>()
					.Named("masstransit.bus.listen")
					.Parameters(Parameter.ForKey("uriString").Eq("msmq://localhost/test_servicebus")),
				Component.For<ISubscriptionCache>()
					.ImplementedBy<LocalSubscriptionCache>()
					.Named("masstransit.cache"),
				Component.For<IObjectBuilder>()
					.ImplementedBy<WindsorObjectBuilder>()
					.Named("masstransit.objectbuilder"),
				Component.For<IEndpointResolver>()
					.ImplementedBy<EndpointResolver>()
					.Named("masstransit.endpointresolver"));

			Register(
				Component.For<IServiceBus>()
					.ImplementedBy<ServiceBus>()
					.Named("masstransit.bus")
					.Parameters(
						Parameter.ForKey("endpointToListenOn").Eq("${masstransit.bus.listen}"),
						Parameter.ForKey("subscriptionCache").Eq("${masstransit.cache}"),
						Parameter.ForKey("objectBuilder").Eq("${masstransit.objectbuilder}"),
						Parameter.ForKey("endpointResolver").Eq("${masstransit.endpointresolver}"))
				);
		}
	}
}