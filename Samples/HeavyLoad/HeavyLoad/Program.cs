namespace HeavyLoad
{
	using System;
	using BatchLoad;
	using Correlated;
	using Load;
	using log4net;

	internal class Program
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (Program));

		private static void Main(string[] args)
		{
			_log.Info("HeavyLoad - MassTransit Load Generator");

			Console.WriteLine("HeavyLoad - MassTransit Load Generator");

			RunLocalMsmqLoadTest();

			RunCorrelatedMessageTest();

			RunBatchLoadTest();

			//RunLocalActiveMqLoadTest();

			Console.WriteLine("End of line.");
			Console.ReadLine();
		}

		private static void RunBatchLoadTest()
		{
			StopWatch stopWatch = new StopWatch();

			using (BatchLoadTest test = new BatchLoadTest())
			{
				test.Run(stopWatch);
			}

			Console.WriteLine("Batch Load Test: ");
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
	}
}