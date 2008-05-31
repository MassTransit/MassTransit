using System;
using System.Collections.Generic;
using System.Text;

namespace HeavyLoad
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("HeavyLoad - MassTransit Load Generator");


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
	}
}
