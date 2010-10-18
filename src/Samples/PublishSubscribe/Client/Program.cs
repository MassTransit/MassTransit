namespace Client
{
	using log4net;
	using MassTransit;
	using MassTransit.Configuration;
	using MassTransit.Services.Subscriptions.Configuration;
	using MassTransit.Transports.Msmq;
	using MassTransit.WindsorIntegration;
	using Microsoft.Practices.ServiceLocation;
	using Topshelf;
	using Topshelf.Configuration.Dsl;

	internal class Program
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (Program));

		private static void Main(string[] args)
		{
			_log.Info("Server Loading");

			var cfg = RunnerConfigurator.New(c =>
				{
					c.SetServiceName("SampleClientService");
					c.SetDisplayName("SampleClientService");
					c.SetDescription("SampleClientService");
					c.DependencyOnMsmq();
					c.RunAsLocalSystem();


					c.ConfigureService<ClientService>(s =>
						{
							s.WhenStarted(o =>
								{

								    var container = new DefaultMassTransitContainer("castle.xml");
								    container.AddComponent<PasswordUpdater>();
								    var wob = new WindsorObjectBuilder(container.Kernel);

								    var endpointFactory = EndpointFactoryConfigurator.New(e =>
								    {
								        e.SetObjectBuilder(wob);
								        e.RegisterTransport<MsmqEndpoint>();
								    });

									var bus = ServiceBusConfigurator.New(servicesBus =>
										{
                                            servicesBus.SetObjectBuilder(wob);
											servicesBus.ReceiveFrom("msmq://localhost/mt_client");
											servicesBus.ConfigureService<SubscriptionClientConfigurator>(client =>
												{
													// need to add the ability to read from configuratino settings somehow
													client.SetSubscriptionServiceEndpoint("msmq://localhost/mt_subscriptions");
												});
										});

									o.Start(bus);
								});
							s.WhenStopped(o => o.Stop());

							s.HowToBuildService(name => new ClientService());
						});
				});
			Runner.Host(cfg, args);
		}
	}
}