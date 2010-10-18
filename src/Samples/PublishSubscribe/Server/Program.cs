namespace Server
{
	using System.IO;
	using log4net;
	using log4net.Config;
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
			XmlConfigurator.ConfigureAndWatch(new FileInfo("server.log4net.xml"));
			_log.Info("Server Loading");

			var cfg = RunnerConfigurator.New(c =>
				{
					c.SetServiceName("SampleService");
					c.SetServiceName("Sample Service");
					c.SetServiceName("Something");
					c.DependencyOnMsmq();

			MsmqEndpointConfigurator.Defaults(def =>
			{
				def.CreateMissingQueues = true;
			});

					c.ConfigureService<PasswordUpdateService>(s =>
						{
							s.WhenStarted(o =>
								{
								    var container = new DefaultMassTransitContainer("server.castle.xml");
								    var wob = new WindsorObjectBuilder(container.Kernel);

								    var endpointFactory = EndpointFactoryConfigurator.New(e =>
								    {
								        e.SetObjectBuilder(wob);
								        e.RegisterTransport<MsmqEndpoint>();
								    });

									var bus = ServiceBusConfigurator.New(x =>
										{
                                            x.SetObjectBuilder(wob);
											x.ReceiveFrom("msmq://localhost/mt_server");
											x.ConfigureService<SubscriptionClientConfigurator>(b => { b.SetSubscriptionServiceEndpoint("msmq://localhost/mt_subscriptions"); });
										});
									o.Start(bus);
								});
							s.WhenStopped(o => o.Stop());

                            s.HowToBuildService(name => new PasswordUpdateService());
						});
				});
			Runner.Host(cfg, args);
		}
	}
}