namespace HeavyLoad
{
	using System;
	using BatchLoad;
	using Correlated;
	using Load;
	using log4net;
	using MassTransit.Transports.Msmq;

    internal class Program
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (Program));

		private static void Main(string[] args)
		{
			_log.Info("HeavyLoad - MassTransit Load Generator");

            MsmqEndpointManagement.Manage(new MsmqEndpointAddress(new Uri("msmq://localhost/mt_client")), q =>
            {
                q.Create(false);
                q.Purge();
            });

            Console.WriteLine("HeavyLoad - MassTransit Load Generator");
            Console.WriteLine();

//			RunLoopbackLoadTest();

//			RunBatchLoadTest();

//			RunTransactionLoadTest();

//			RunLocalMsmqLoadTest();

			RunContainerLoadTest();

			//RunWcfLoadTest();

//			RunCorrelatedMessageTest();

			//RunLocalActiveMqLoadTest();

			Console.WriteLine("End of line.");
		}

		private static void RunLocalActiveMqLoadTest()
		{
			StopWatch stopWatch = new StopWatch();

			using (ActiveMQLoadTest test = new ActiveMQLoadTest())
			{
				test.Run(stopWatch);
			}

			Console.WriteLine("ActiveMQ Load Test: ");
            Console.WriteLine(stopWatch.ToString());
            Console.WriteLine();
		}

		private static void RunBatchLoadTest()
		{
            Console.WriteLine("Starting Local MSMQ Batch Load Test");
			StopWatch stopWatch = new StopWatch();

			using (BatchLoadTest test = new BatchLoadTest())
			{
				test.Run(stopWatch);
			}

			Console.WriteLine("Batch Load Test: ");
            Console.WriteLine(stopWatch.ToString());
            Console.WriteLine();
		}

		private static void RunLocalMsmqLoadTest()
		{
            Console.WriteLine("Starting Local MSMQ Load Test");
			StopWatch stopWatch = new StopWatch();

            using (LocalLoadTest test = new LocalLoadTest())
			{
				test.Run(stopWatch);
			}

			Console.WriteLine("Local MSMQ Load Test: ");
            Console.WriteLine(stopWatch.ToString());
            Console.WriteLine();
		}

		private static void RunContainerLoadTest()
        {
            Console.WriteLine("Starting Local MSMQ Container Load Test");
			StopWatch stopWatch = new StopWatch();

			using (ContainerLoadTest test = new ContainerLoadTest())
			{
				test.Run(stopWatch);
			}

			Console.WriteLine("Container Load Test: ");
            Console.WriteLine(stopWatch.ToString());
            Console.WriteLine();
		}

		private static void RunTransactionLoadTest()
        {
            Console.WriteLine("Starting Local MSMQ Transactional Load Test");
			StopWatch stopWatch = new StopWatch();

            using (TransactionLoadTest test = new TransactionLoadTest())
			{
				test.Run(stopWatch);
			}

			Console.WriteLine("Transaction Load Test: ");
            Console.WriteLine(stopWatch.ToString());
            Console.WriteLine();
		}

		private static void RunLoopbackLoadTest()
        {
            Console.WriteLine("Starting Local Loopback Load Test");
			StopWatch stopWatch = new StopWatch();

			using (LoopbackLoadTest test = new LoopbackLoadTest())
			{
				test.Run(stopWatch);
			}

			Console.WriteLine("Loopback Load Test: ");
            Console.WriteLine(stopWatch.ToString());
            Console.WriteLine();
		}

		private static void RunWcfLoadTest()
        {
            Console.WriteLine("Starting Local WCF Load Test");
			StopWatch stopWatch = new StopWatch();

			using (WcfLoadTest test = new WcfLoadTest())
			{
				test.Run(stopWatch);
			}

			Console.WriteLine("WCF Load Test: ");
            Console.WriteLine(stopWatch.ToString());
            Console.WriteLine();
		}

	    private static void RunCorrelatedMessageTest()
        {
            Console.WriteLine("Starting Local MSMQ Correlated Load Test");
			StopWatch stopWatch = new StopWatch();

			using (CorrelatedMessageTest test = new CorrelatedMessageTest())
			{
				test.Run(stopWatch);
			}

			Console.WriteLine("Correlated Message Test: ");
            Console.WriteLine(stopWatch.ToString());
            Console.WriteLine();
		}
	}
}