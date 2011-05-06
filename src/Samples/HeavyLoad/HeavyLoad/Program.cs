namespace HeavyLoad
{
	using System;
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

			RunLoopbackHandlerLoadTest();

			RunLoopbackConsumerLoadTest();

			RunStructureMapLoadTest();

			RunLocalMsmqLoadTest();
			
			RunTransactionLoadTest();

			RunCorrelatedMessageTest();

//		    RunLocalRabbitMqLoadTest();



			//RunLocalActiveMqLoadTest();

			Console.WriteLine("End of line.");
		}

		private static void RunLocalActiveMqLoadTest()
		{
			StopWatch stopWatch = new StopWatch();

			using (var test = new ActiveMQLoadTest())
			{
				test.Run(stopWatch);
			}

			Console.WriteLine("ActiveMQ Load Test: ");
            Console.WriteLine(stopWatch.ToString());
            Console.WriteLine();
		}

        private static void RunLocalRabbitMqLoadTest()
        {
            var stopWatch = new StopWatch();

            using (var test = new RabbitMQLoadTest())
            {
                test.Run(stopWatch);
            }

            Console.WriteLine("RabbitMQ Load Test: ");
            Console.WriteLine(stopWatch.ToString());
            Console.WriteLine();
        }

		private static void RunLocalMsmqLoadTest()
		{
            Console.WriteLine("Starting Local MSMQ Load Test");
			StopWatch stopWatch = new StopWatch();

            using (var test = new LocalLoadTest())
			{
				test.Run(stopWatch);
			}

			Console.WriteLine("Local MSMQ Load Test: ");
            Console.WriteLine(stopWatch.ToString());
            Console.WriteLine();
		}

		private static void RunStructureMapLoadTest()
        {
            Console.WriteLine("Starting StructureMap Load Test");
			StopWatch stopWatch = new StopWatch();

			using (var test = new StructureMapConsumerLoadTest())
			{
				test.Run(stopWatch);
			}

			Console.WriteLine("StructureMap Load Test: ");
            Console.WriteLine(stopWatch.ToString());
            Console.WriteLine();
		}

		private static void RunTransactionLoadTest()
        {
            Console.WriteLine("Starting Local MSMQ Transactional Load Test");
			StopWatch stopWatch = new StopWatch();

            using (var test = new TransactionLoadTest())
			{
				test.Run(stopWatch);
			}

			Console.WriteLine("Transaction Load Test: ");
            Console.WriteLine(stopWatch.ToString());
            Console.WriteLine();
		}

		private static void RunLoopbackHandlerLoadTest()
        {
            Console.WriteLine("Starting Local Loopback Handler Load Test");
			StopWatch stopWatch = new StopWatch();

			using (var test = new LoopbackHandlerLoadTest())
			{
				test.Run(stopWatch);
			}

			Console.WriteLine("Loopback Load Handler Test: ");
            Console.WriteLine(stopWatch.ToString());
            Console.WriteLine();
		}

		private static void RunLoopbackConsumerLoadTest()
        {
            Console.WriteLine("Starting Local Loopback Consumer Load Test");
			StopWatch stopWatch = new StopWatch();

			using (LoopbackConsumerLoadTest test = new LoopbackConsumerLoadTest())
			{
				test.Run(stopWatch);
			}

			Console.WriteLine("Loopback Load Consumer Test: ");
            Console.WriteLine(stopWatch.ToString());
            Console.WriteLine();
		}


	    private static void RunCorrelatedMessageTest()
        {
            Console.WriteLine("Starting Local MSMQ Correlated Load Test");
			StopWatch stopWatch = new StopWatch();

			using (var test = new CorrelatedMessageTest())
			{
				test.Run(stopWatch);
			}

			Console.WriteLine("Correlated Message Test: ");
            Console.WriteLine(stopWatch.ToString());
            Console.WriteLine();
		}
	}
}