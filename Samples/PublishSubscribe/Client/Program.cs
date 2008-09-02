namespace Client
{
	using log4net;
	using MassTransit.Host;
	using MassTransit.Host.Configurations;

    internal class Program
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (Program));

		private static void Main(string[] args)
		{
			_log.Info("Server Loading");

			IInstallationConfiguration cfg = new ClientEnvironment("castle.xml");

			Runner.Run(cfg, args);
		}
	}
}