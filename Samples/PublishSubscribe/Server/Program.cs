namespace Server
{
	using log4net;
	using MassTransit.Host;

	internal class Program
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (Program));

		private static void Main(string[] args)
		{
			_log.Info("Server Loading");

			HostedEnvironment env = new ServerEnvironment("castle.xml");

			Runner.Run(env, args);
		}
	}
}
