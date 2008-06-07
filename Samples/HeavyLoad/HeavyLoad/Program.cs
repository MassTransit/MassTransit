namespace HeavyLoad
{
	using System;
	using System.IO;
	using log4net;
	using log4net.Appender;
	using log4net.Layout;
	using log4net.Repository.Hierarchy;

	internal class Program
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof(Program));

		private static void Main(string[] args)
		{
			//log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo("log4net.config"));

			_log.Info("HeavyLoad - MassTransit Load Generator");


			Console.WriteLine("HeavyLoad - MassTransit Load Generator");


		//	RunLocalActiveMqLoadTest();

			RunLocalMsmqLoadTest();

			Console.WriteLine("Waiting for next test...");

			RunCorrelatedMessageTest();

			Console.WriteLine("End of line.");
			//Console.ReadLine();
		}

		private static void RunCorrelatedMessageTest()
		{
			StopWatch stopWatch = new StopWatch();

			using (CorrelatedMessageTest test = new CorrelatedMessageTest())
			{
				test.Run(stopWatch);
			}

			Console.WriteLine("Correlated Message Test: ");
			Console.WriteLine(stopWatch.ToString());
		}

		private static void RunLocalMsmqLoadTest()
		{
			StopWatch stopWatch = new StopWatch();

			using (LocalMsmqLoadTest test = new LocalMsmqLoadTest())
			{
				test.Run(stopWatch);
			}

			Console.WriteLine("Local MSMQ Load Test: ");
			Console.WriteLine(stopWatch.ToString());
		}

		private static void RunLocalActiveMqLoadTest()
		{
			StopWatch stopWatch = new StopWatch();

			using (LocalActiveMqLoadTest test = new LocalActiveMqLoadTest())
			{
				test.Run(stopWatch);
			}

			Console.WriteLine("Local ActiveMQ Load Test: ");
			Console.WriteLine(stopWatch.ToString());
		}

		private static void AddConsoleAppender()
		{
			ConsoleAppender appender = new ConsoleAppender();

			ILayout layout = new PatternLayout("%-5p %d{yyyy-MM-dd hh:mm:ss} %logger - %m%n");
			
			appender.Layout = layout;
			appender.ActivateOptions();

			((Hierarchy)LogManager.GetRepository()).Root.AddAppender(appender);
		}

	}
}