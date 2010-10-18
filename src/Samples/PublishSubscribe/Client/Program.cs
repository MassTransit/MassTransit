namespace Client
{
	using log4net;
	using MassTransit;
	using MassTransit.Configuration;
	using MassTransit.Services.Subscriptions.Configuration;
	using MassTransit.Transports.Msmq;
	using MassTransit.WindsorIntegration;
	using Topshelf;
	using Topshelf.Configuration.Dsl;

	internal class Program
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (Program));

		private static void Main(string[] args)
		{
			_log.Info("Client Loading");

            var container = new DefaultMassTransitContainer();
            IEndpointFactory endpointFactory = EndpointFactoryConfigurator.New(e =>
            {
                e.SetObjectBuilder(container.ObjectBuilder);
                e.RegisterTransport<MsmqEndpoint>();
            });

			MsmqEndpointConfigurator.Defaults(def =>
			{
				def.CreateMissingQueues = true;
			});
      
			container.Kernel.AddComponentInstance("endpointFactory", typeof(IEndpointFactory), endpointFactory);
            container.AddComponent<ClientService>(typeof(ClientService).Name);
            container.AddComponent<PasswordUpdater>();
									
			var cfg = RunnerConfigurator.New(c =>
				{
					c.SetServiceName("SampleClientService");
					c.SetDisplayName("SampleClientService");
					c.SetDescription("SampleClientService");

					c.DependencyOnMsmq();
					c.RunAsLocalSystem();

					c.ConfigureService<ClientService>(s =>
						{
                            s.Named(typeof(ClientService).Name);
							s.WhenStarted(o =>
								{
									var bus = ServiceBusConfigurator.New(servicesBus =>
										{
											servicesBus.ReceiveFrom("msmq://localhost/mt_client");
											servicesBus.ConfigureService<SubscriptionClientConfigurator>(client =>
												{
													// need to add the ability to read from configuration settings somehow
													client.SetSubscriptionServiceEndpoint("msmq://localhost/mt_subscriptions");
												});
										});

									o.Start(bus);
								});
							s.WhenStopped(o => o.Stop());

                            s.HowToBuildService(b =>
                            {
                            	return container.ObjectBuilder.GetInstance<ClientService>();
                            });
						});
				});
			Runner.Host(cfg, args);
		}
	}
}