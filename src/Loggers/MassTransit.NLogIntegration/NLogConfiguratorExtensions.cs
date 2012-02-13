using MassTransit.BusConfigurators;
using MassTransit.Logging;

namespace MassTransit.NLogIntegration
{
	public static class NLogConfiguratorExtensions
	{
		public static void UseNLog(this ServiceBusConfigurator configrator)
		{
			Logger.UseLogger(new NLogLogger());
		}
	}
}
