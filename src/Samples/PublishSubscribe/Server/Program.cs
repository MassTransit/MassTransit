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
	using Microsoft.Practices.ServiceLocation;
	using Topshelf;
	using Topshelf.Configuration.Dsl;

	internal class Program
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (Program));

		private static void Main(string[] args)
		{
			XmlConfigurator.ConfigureAndWatch(new FileInfo("server.log4net.xml"));
			_log.Info("Server Loading");

            var container = new DefaultMassTransitContainer();
            IEndpointFactory endpointFactory = EndpointFactoryConfigurator.New(x =>
                {
                    x.SetObjectBuilder(container.ObjectBuilder);
                    x.RegisterTransport<MsmqEndpoint>();
                });
            container.Kernel.AddComponentInstance("endpointFactory", typeof(IEndpointFactory), endpointFactory);
            container.AddComponent<PasswordUpdateService>(typeof(PasswordUpdateService).Name);

            var cfg = RunnerConfigurator.New(c =>
                {
                    c.SetServiceName("SampleServer");
                    c.SetDisplayName("SampleServer");
                    c.SetDescription("SampleServer");

                    c.DependencyOnMsmq();

                    c.RunAsLocalSystem();

                    c.ConfigureService<PasswordUpdateService>(s =>
                        {
                            s.Named(typeof(PasswordUpdateService).Name);
                            s.WhenStarted(o =>
                                {
                                    IServiceBus bus = ServiceBusConfigurator.New(x =>
                                        {
                                            x.ReceiveFrom("msmq://localhost/mt_server");
                                            x.ConfigureService<SubscriptionClientConfigurator>(b => { b.SetSubscriptionServiceEndpoint("msmq://localhost/mt_subscriptions"); });
                                        });
                                    o.Start(bus);
                                });
                            s.WhenStopped(o => o.Stop());
                            s.HowToBuildService(a => container.GetService<PasswordUpdateService>());
                        });
                });
			Runner.Host(cfg, args);
		}
	}
}