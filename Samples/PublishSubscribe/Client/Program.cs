namespace Client
{
	using log4net;
	using MassTransit.Host;
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
            

		    var credentials = Credentials.LocalSystem;
		    var settings = WinServiceSettings.Custom(
		        "SampleClientService",
		        "SampleClientService",
		        "SampleClientService",
		        KnownServiceNames.Msmq);
		    var lifecycle = new ClientLifeCycle(ServiceLocator.Current);

		    
			Runner.Run(credentials, settings, lifecycle, args);
		}
	}
}