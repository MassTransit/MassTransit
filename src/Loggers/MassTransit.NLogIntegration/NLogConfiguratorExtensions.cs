using MassTransit.BusConfigurators;
using MassTransit.Logging;
using MassTransit.Util;

namespace MassTransit.NLogIntegration
{
	/// <summary>
	/// Extensions for configuring NLog with MassTransit
	/// </summary>
	public static class NLogConfiguratorExtensions
	{
		/// <summary>
		/// Specify that you want to use the NLog logging framework with MassTransit.
		/// </summary>
		/// <param name="configrator">Optional service bus configurator</param>
		public static void UseNLog([CanBeNull] this ServiceBusConfigurator configrator)
		{
			Logger.UseLogger(new NLogLogger());
		}
	}
}
