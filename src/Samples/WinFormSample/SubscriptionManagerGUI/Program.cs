namespace SubscriptionManagerGUI
{
	using log4net;
	using MassTransit;
	using MassTransit.Configuration;
	using MassTransit.Saga;
	using MassTransit.Services.HealthMonitoring;
	using MassTransit.Services.Subscriptions.Configuration;
	using MassTransit.Services.Subscriptions.Server;
	using MassTransit.Services.Timeout;
	using MassTransit.Services.Timeout.Server;
	using MassTransit.Transports.Msmq;
	using MassTransit.WindsorIntegration;
	using Microsoft.Practices.ServiceLocation;
	using Topshelf;
	using Topshelf.Configuration;

	internal static class Program
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (Program));

		private static void Main(string[] args)
		{
			_log.Info("SubscriptionManagerGUI Loading...");

			var cfg = RunnerConfigurator.New(x =>
				{
					x.SetServiceName("SubscriptionManagerGUI");
					x.SetDisplayName("Sample GUI Subscription Service");
					x.SetDescription("Coordinates subscriptions between multiple systems");
					x.DependencyOnMsmq();
					x.RunAsLocalSystem();
					x.UseWinFormHost<SubscriptionManagerForm>();

					x.BeforeStartingServices(s =>
						{
							var container = new DefaultMassTransitContainer();

							IEndpointFactory endpointFactory = EndpointFactoryConfigurator.New(e =>
								{
									e.SetObjectBuilder(container.Resolve<IObjectBuilder>());
									e.RegisterTransport<MsmqEndpoint>();
								});
							container.Kernel.AddComponentInstance("endpointFactory", typeof (IEndpointFactory), endpointFactory);

							container.AddComponent<SubscriptionManagerForm>();

							var wob = new WindsorObjectBuilder(container.Kernel);
							ServiceLocator.SetLocatorProvider(() => wob);
						});

					x.ConfigureService<SubscriptionService>(typeof(SubscriptionService).Name, ConfigureSubscriptionService);

					x.ConfigureService<TimeoutService>(typeof(TimeoutService).Name, ConfigureTimeoutService);

					x.ConfigureService<HealthService>(typeof(HealthService).Name, ConfigureHealthService);
				});
			Runner.Host(cfg, args);
		}

		private static void ConfigureSubscriptionService(IServiceConfigurator<SubscriptionService> configurator)
		{
			configurator.CreateServiceLocator(() =>
				{
					var container = new DefaultMassTransitContainer();

					container.AddComponent("sagaRepository", typeof (ISagaRepository<>), typeof (InMemorySagaRepository<>));

					container.AddComponent<ISubscriptionRepository, InMemorySubscriptionRepository>();
					container.AddComponent<SubscriptionService, SubscriptionService>(typeof (SubscriptionService).Name);

					var endpointFactory = EndpointFactoryConfigurator.New(x =>
						{
							// the default
							x.SetObjectBuilder(container.Resolve<IObjectBuilder>());
							x.RegisterTransport<MsmqEndpoint>();
						});

					container.Kernel.AddComponentInstance("endpointFactory", typeof (IEndpointFactory), endpointFactory);

					var bus = ServiceBusConfigurator.New(servicesBus =>
						{
							servicesBus.SetObjectBuilder(container.Resolve<IObjectBuilder>());
							servicesBus.ReceiveFrom("msmq://localhost/mt_subscriptions");
							servicesBus.SetConcurrentConsumerLimit(1);
						});

					container.Kernel.AddComponentInstance("bus", typeof (IServiceBus), bus);

				    return container.ObjectBuilder;
				});

			configurator.WhenStarted(service => service.Start());

			configurator.WhenStopped(service =>
				{
					service.Stop();
					service.Dispose();
				});
		}

		private static void ConfigureTimeoutService(IServiceConfigurator<TimeoutService> configurator)
		{
			configurator.CreateServiceLocator(() =>
				{
					var container = new DefaultMassTransitContainer();

					container.AddComponent("sagaRepository", typeof(ISagaRepository<>), typeof(InMemorySagaRepository<>));
					container.AddComponent<TimeoutService>(typeof(TimeoutService).Name);

					var endpointFactory = EndpointFactoryConfigurator.New(x =>
						{
							// the default
							x.SetObjectBuilder(container.Resolve<IObjectBuilder>());
							x.RegisterTransport<MsmqEndpoint>();
						});

					container.Kernel.AddComponentInstance("endpointFactory", typeof (IEndpointFactory), endpointFactory);

					var bus = ServiceBusConfigurator.New(x =>
						{
							x.SetObjectBuilder(container.Resolve<IObjectBuilder>());
							x.ReceiveFrom("msmq://localhost/mt_timeout");
							x.ConfigureService<SubscriptionClientConfigurator>(client =>
								{
									// need to add the ability to read from configuratino settings somehow
									client.SetSubscriptionServiceEndpoint("msmq://localhost/mt_subscriptions");
								});
						});
					container.Kernel.AddComponentInstance("bus", typeof (IServiceBus), bus);

					return container.Resolve<IObjectBuilder>();
				});

			configurator.WhenStarted(service => { service.Start(); });

			configurator.WhenStopped(service =>
				{
					service.Stop();
					service.Dispose();
				});
		}

		private static void ConfigureHealthService(IServiceConfigurator<HealthService> configurator)
		{
			configurator.CreateServiceLocator(() =>
				{
					var container = new DefaultMassTransitContainer();

					container.AddComponent<HealthService>(typeof (HealthService).Name);
					container.AddComponent("sagaRepository", typeof (ISagaRepository<>), typeof (InMemorySagaRepository<>));

					var endpointFactory = EndpointFactoryConfigurator.New(x =>
						{
							// the default
							x.SetObjectBuilder(container.Resolve<IObjectBuilder>());
							x.RegisterTransport<MsmqEndpoint>();
						});

					container.Kernel.AddComponentInstance("endpointFactory", typeof (IEndpointFactory), endpointFactory);

					var bus = ServiceBusConfigurator.New(x =>
						{
							x.SetObjectBuilder(container.Resolve<IObjectBuilder>());
							x.ReceiveFrom("msmq://localhost/mt_health");
							x.ConfigureService<SubscriptionClientConfigurator>(client =>
								{
									// need to add the ability to read from configuratino settings somehow
									client.SetSubscriptionServiceEndpoint("msmq://localhost/mt_subscriptions");
								});
						});

					container.Kernel.AddComponentInstance("bus", typeof (IServiceBus), bus);

					return container.Resolve<IObjectBuilder>();
				});

			configurator.WhenStarted(service => { service.Start(); });

			configurator.WhenStopped(service =>
				{
					service.Stop();
					service.Dispose();
				});
		}
	}
}