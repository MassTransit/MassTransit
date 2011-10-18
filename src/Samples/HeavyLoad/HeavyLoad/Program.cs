namespace HeavyLoad
{
    using System;
    using Correlated;
    using Load;
    using log4net;
    using MassTransit.Transports.Msmq;

    internal class Program
    {
        static readonly ILog _log = LogManager.GetLogger(typeof (Program));

        static void Main(string[] args)
        {
            _log.Info("HeavyLoad - MassTransit Load Generator");

            MsmqEndpointManagement.Manage(new MsmqEndpointAddress(new Uri("msmq://localhost/mt_client")), q =>
                {
                    q.Create(false);
                    q.Purge();
                });

            Console.WriteLine("HeavyLoad - MassTransit Load Generator");
            Console.WriteLine();

            //RunLoopbackHandlerLoadTest();

            //RunLoopbackConsumerLoadTest();

            //RunStructureMapLoadTest();

            //RunLocalMsmqLoadTest();

            //RunTransactionLoadTest();

            //RunCorrelatedMessageTest();

            //RunLocalRequestResponseMsmqLoadTest();

            //RunRabbitMqLoadTest();

            //RunLocalActiveMqLoadTest();

            RunLongRunningMemoryTest();

            Console.WriteLine("End of line.");
        }

        static void RunLocalActiveMqLoadTest()
        {
            var stopWatch = new StopWatch();

            using (var test = new ActiveMQLoadTest())
            {
                test.Run(stopWatch);
            }

            Console.WriteLine("ActiveMQ Load Test: ");
            Console.WriteLine(stopWatch.ToString());
            Console.WriteLine();
        }

        static void RunRabbitMqLoadTest()
        {
            try
            {
                var stopWatch = new StopWatch();

                using (var test = new RabbitMqHandlerLoadTest())
                {
                    test.Run(stopWatch);
                }

                Console.WriteLine("RabbitMQ Load Test: ");
                Console.WriteLine(stopWatch.ToString());
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to run RabbitMQ Load Test: " + ex);
                Console.WriteLine("If RabbitMQ is not installed, this is normal");
            }
        }

        static void RunLocalMsmqLoadTest()
        {
            Console.WriteLine("Starting Local MSMQ Load Test");
            var stopWatch = new StopWatch();

            using (var test = new LocalLoadTest())
            {
                test.Run(stopWatch);
            }

            Console.WriteLine("Local MSMQ Load Test: ");
            Console.WriteLine(stopWatch.ToString());
            Console.WriteLine();
        }

        static void RunLocalRequestResponseMsmqLoadTest()
        {
            Console.WriteLine("Starting MSMQ Local Request Response Load Test");
            var stopWatch = new StopWatch();

            using (var test = new LocalMsmqRequestResponseLoadTest())
            {
                test.Run(stopWatch);
            }

            Console.WriteLine("Local MSMQ Request Response Load Test: ");
            Console.WriteLine(stopWatch.ToString());
            Console.WriteLine();
        }

        static void RunLongRunningMemoryTest()
        {
            var stopWatch = new StopWatch();

            using (var test = new LongRunningMemoryTest())
            {
                test.Run(stopWatch);
                stopWatch.Stop();
            }

            Console.WriteLine("Long Running Memory Test: {0}", stopWatch);
        }

        static void RunStructureMapLoadTest()
        {
            Console.WriteLine("Starting StructureMap Load Test");
            var stopWatch = new StopWatch();

            using (var test = new StructureMapConsumerLoadTest())
            {
                test.Run(stopWatch);
            }

            Console.WriteLine("StructureMap Load Test: ");
            Console.WriteLine(stopWatch.ToString());
            Console.WriteLine();
        }

        static void RunTransactionLoadTest()
        {
            Console.WriteLine("Starting Local MSMQ Transactional Load Test");
            var stopWatch = new StopWatch();

            using (var test = new TransactionLoadTest())
            {
                test.Run(stopWatch);
            }

            Console.WriteLine("Transaction Load Test: ");
            Console.WriteLine(stopWatch.ToString());
            Console.WriteLine();
        }

        static void RunLoopbackHandlerLoadTest()
        {
            Console.WriteLine("Starting Local Loopback Handler Load Test");
            var stopWatch = new StopWatch();

            using (var test = new LoopbackHandlerLoadTest())
            {
                test.Run(stopWatch);
            }

            Console.WriteLine("Loopback Load Handler Test: ");
            Console.WriteLine(stopWatch.ToString());
            Console.WriteLine();
        }

        static void RunLoopbackConsumerLoadTest()
        {
            Console.WriteLine("Starting Local Loopback Consumer Load Test");
            var stopWatch = new StopWatch();

            using (var test = new LoopbackConsumerLoadTest())
            {
                test.Run(stopWatch);
            }

            Console.WriteLine("Loopback Load Consumer Test: ");
            Console.WriteLine(stopWatch.ToString());
            Console.WriteLine();
        }


        static void RunCorrelatedMessageTest()
        {
            Console.WriteLine("Starting Local MSMQ Correlated Load Test");
            var stopWatch = new StopWatch();

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