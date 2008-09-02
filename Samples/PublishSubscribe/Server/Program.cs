namespace Server
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

			IInstallationConfiguration cfg = new ServerEnvironment("castle.xml");

			Runner.Run(cfg, args);
		}
	}
}
