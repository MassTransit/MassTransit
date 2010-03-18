namespace InternalInventoryService
{
    using Castle.Windsor;
    using log4net;
	using MassTransit.WindsorIntegration;
	using Topshelf;
	using Topshelf.Configuration.Dsl;

    internal static class Program
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (Program));
        static IWindsorContainer _container;

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
						    _container = container;
						});

					c.ConfigureService<InternalInventoryServiceLifeCycle>(s =>
						{
                            s.HowToBuildService(name=>_container.Resolve<InternalInventoryServiceLifeCycle>());
							s.WhenStarted(o => o.Start());
							s.WhenStopped(o => o.Stop());
						});
				});
			Runner.Host(cfg, args);
		}
	}
}