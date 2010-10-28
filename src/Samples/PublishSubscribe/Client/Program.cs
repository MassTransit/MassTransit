namespace Client
{
    using System.IO;
    using Castle.MicroKernel.Registration;
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
            XmlConfigurator.ConfigureAndWatch(new FileInfo("client.log4net.xml"));
            _log.Info("Client Loading");


            var cfg = RunnerConfigurator.New(c =>
            {
                c.SetServiceName("SampleClientService");
                c.SetDisplayName("SampleClientService");
                c.SetDescription("SampleClientService");

                c.DependencyOnMsmq();
                c.RunAsLocalSystem();


                c.ConfigureService<ClientService>(s =>
                {
                    string serviceName = typeof (ClientService).Name;

                    s.Named(serviceName);
                    s.WhenStarted(o =>
                    {
                        var container = new DefaultMassTransitContainer();
                        var endpointFactory = EndpointFactoryConfigurator.New(e =>
                        {
                            e.SetObjectBuilder(container.ObjectBuilder);
                            e.RegisterTransport<MsmqEndpoint>();
                        });

                        MsmqEndpointConfigurator.Defaults(def =>
                        {
                            def.CreateMissingQueues = true;
                        });

                        container.Register(Component.For<IEndpointFactory>().Named("endpointFactory").Instance(endpointFactory));
                        container.Register(Component.For<ClientService>().Named(serviceName));
                        container.Register(Component.For<PasswordUpdater>());

                        var wob = new WindsorObjectBuilder(container.Kernel);

                        var bus = ServiceBusConfigurator.New(servicesBus =>
                        {
                            servicesBus.SetObjectBuilder(wob);
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

                    s.HowToBuildService(name => new ClientService());
                });
            });
            Runner.Host(cfg, args);
        }
	}
}