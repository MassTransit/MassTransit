namespace Starbucks.Barista
{
	using System;
	using System.Diagnostics;
	using System.IO;
	using Castle.Windsor;
	using log4net.Config;
	using Magnum;
	using Magnum.StateMachine;
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
			XmlConfigurator.Configure(new FileInfo("barista.log4net.xml"));

			var cfg = RunnerConfigurator.New(c =>
				{
					c.SetServiceName("StarbucksBarista");
					c.SetDisplayName("Starbucks Barista");
					c.SetDescription("a Mass Transit sample service for making orders of coffee.");

					c.DependencyOnMsmq();
					c.RunAsFromInteractive();

					c.BeforeStart(a => { });

					c.ConfigureService<BaristaService>(s =>
						{
							s.CreateServiceLocator(() =>
								{
									IWindsorContainer container = new DefaultMassTransitContainer("Starbucks.Barista.Castle.xml");
									container.AddComponent("sagaRepository", typeof(ISagaRepository<>), typeof(InMemorySagaRepository<>));

									container.AddComponent<DrinkPreparationSaga>();
									container.AddComponent<BaristaService>();

									Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));

									StateMachineInspector.Trace(new DrinkPreparationSaga(CombGuid.Generate()));

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