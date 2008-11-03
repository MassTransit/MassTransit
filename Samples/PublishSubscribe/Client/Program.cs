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

		    var Container = new DefaultMassTransitContainer("castle.xml");
            Container.AddComponent<PasswordUpdater>();

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