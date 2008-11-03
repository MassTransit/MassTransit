namespace Client
{
	using log4net;
	using MassTransit.Host;
	using MassTransit.Host.Configurations;
	using MassTransit.WindsorIntegration;
	using Microsoft.Practices.ServiceLocation;

    internal class Program
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (Program));

		private static void Main(string[] args)
		{
			_log.Info("Server Loading");

		    var container = new DefaultMassTransitContainer("castle.xml");
            container.AddComponent<PasswordUpdater>();
            
            var wob = new WindsorObjectBuilder(container.Kernel);
            ServiceLocator.SetLocatorProvider(() => wob);

		    IInstallationConfiguration cfg = new WinServiceConfiguration(
		        Credentials.LocalSystem,
		        WinServiceSettings.Custom(
                    "SampleClientService",
                    "SampleClientService",
                    "SampleClientService",
		            KnownServiceNames.Msmq),
		        new ClientLifeCycle(ServiceLocator.Current));

			Runner.Run(cfg, args);
		}
	}
}