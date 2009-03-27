namespace Starbucks.Cashier
{
	using System;
	using System.IO;
	using Castle.Windsor;
	using log4net.Config;
	using MassTransit.Saga;
	using MassTransit.WindsorIntegration;
	using Microsoft.Practices.ServiceLocation;
	using Topshelf;
	using Topshelf.Configuration;

	internal static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		private static void Main(string[] args)
		{
			XmlConfigurator.Configure(new FileInfo("cashier.log4net.xml"));

			var cfg = RunnerConfigurator.New(c =>
				{
					c.SetServiceName("StarbucksCashier");
					c.SetDisplayName("Starbucks Cashier");
					c.SetDescription("a Mass Transit sample service for handling orders of coffee.");

					c.RunAsLocalSystem();
					c.DependencyOnMsmq();

					c.BeforeStart(a => { });

					c.ConfigureService<CashierService>(s =>
						{
							s.CreateServiceLocator(() =>
								{
									IWindsorContainer container = new DefaultMassTransitContainer("Starbucks.Cashier.Castle.xml");
									container.AddComponent("sagaRepository", typeof(ISagaRepository<>), typeof(InMemorySagaRepository<>));

									container.AddComponent<CashierService>();

									container.AddComponent<CashierSaga>();

									return ServiceLocator.Current;
								});
							s.WhenStarted(o => o.Start());
							s.WhenStopped(o => o.Stop());
						});
				});
			Runner.Host(cfg, args);
		}
	}
}