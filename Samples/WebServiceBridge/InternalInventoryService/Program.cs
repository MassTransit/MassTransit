namespace InternalInventoryService
{
	using log4net;
	using MassTransit.WindsorIntegration;
	using Microsoft.Practices.ServiceLocation;
	using Topshelf;
	using Topshelf.Configuration;

	internal static class Program
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (Program));

		private static void Main(string[] args)
		{
			_log.Info("InternalInventoryService Loading...");

			var cfg = RunnerConfigurator.New(c =>
				{
					c.SetServiceName("InternalInventoryService");
					c.SetDisplayName("Internal Inventory Service");
					c.SetDescription("Handles inventory for internal systems");

					c.RunAsLocalSystem();
					c.DependencyOnMsmq();

					c.BeforeStartingServices(a =>
						{
							var container = new DefaultMassTransitContainer("InternalInventoryService.Castle.xml");
							container.AddComponent<InventoryLevelService>();
							container.AddComponent<InternalInventoryServiceLifeCycle>(typeof (InternalInventoryServiceLifeCycle).Name);
							var wob = new WindsorObjectBuilder(container.Kernel);
							ServiceLocator.SetLocatorProvider(() => wob);
						});

					c.ConfigureService<InternalInventoryServiceLifeCycle>(typeof(InternalInventoryServiceLifeCycle).Name, s =>
						{
							s.WhenStarted(o => o.Start());
							s.WhenStopped(o => o.Stop());
						});
				});
			Runner.Host(cfg, args);
		}
	}
}