namespace Client
{
	using log4net;
	using MassTransit.WindsorIntegration;
	using Microsoft.Practices.ServiceLocation;
	using Topshelf;
	using Topshelf.Configuration;

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

                                                     c.BeforeStart(a=>
                                                                       {
                                                                           var container = new DefaultMassTransitContainer("castle.xml");
                                                                           container.AddComponent<ClientService>();
                                                                           container.AddComponent<PasswordUpdater>();
                                                                           
                                                                           var wob =new WindsorObjectBuilder(container.Kernel);
                                                                           ServiceLocator.SetLocatorProvider(()=>wob);
                                                                       });

		                                             c.ConfigureService<ClientService>(s=>
		                                                                                     {
		                                                                                         s.WhenStarted(o=>o.Start());
		                                                                                         s.WhenStopped(o=>o.Stop());
		                                                                                     });
		                                         });
			Runner.Host(cfg, args);
		}
	}
}