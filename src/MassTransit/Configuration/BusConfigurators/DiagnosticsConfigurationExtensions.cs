namespace MassTransit.BusConfigurators
{
	using Diagnostics;

	public static class DiagnosticsConfigurationExtensions
	{

			public static void EnableMessageTracing(this ServiceBusConfigurator configurator)
			{
				var busConfigurator = new PostCreateBusBuilderConfiguratorImpl(bus =>
					{
						var service = new MessageTraceBusService(bus.EventChannel);

						bus.AddService(BusServiceLayer.Network, service);
					});

				configurator.AddBusConfigurator(busConfigurator);

			}
		}
}