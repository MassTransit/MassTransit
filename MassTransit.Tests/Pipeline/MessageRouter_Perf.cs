namespace MassTransit.Tests.Pipeline
{
	using System.Diagnostics;
	using MassTransit.Pipeline;
	using MassTransit.Pipeline.Sinks;
	using Messages;
	using NUnit.Framework;
	using TestConsumers;

	[TestFixture]
	public class MessageRouter_Perf
	{
		internal class PerformantConsumer<T> :
			Consumes<T>.All
			where T : class
		{
			private long _count = 0;

			public void Consume(T message)
			{
				_count++;
			}
		}


		[Test, Explicit]
		public void Router_performance_measurement()
		{
			IMessageSink<PingMessage> router = SetupRouterOnly();

			const int primeLoopCount = 10;
			SendMessages(router, primeLoopCount);

			Stopwatch timer = Stopwatch.StartNew();

			const int loopCount = 1500000;
			SendMessages(router, loopCount);

			timer.Stop();

			Trace.WriteLine("Elapsed Time: " + timer.ElapsedMilliseconds + "ms");
			Trace.WriteLine("Messages Per Second: " + loopCount*1000/timer.ElapsedMilliseconds);
		}

		[Test, Explicit]
		public void Nested_router_performance_measurement()
		{
			IMessageSink<object> router = SetupTwoRoutersOnly();

			const int primeLoopCount = 10;
			SendMessages(router, primeLoopCount);

			Stopwatch timer = Stopwatch.StartNew();

			const int loopCount = 1500000;
			SendMessages(router, loopCount);

			timer.Stop();

			Trace.WriteLine("Elapsed Time: " + timer.ElapsedMilliseconds + "ms");
			Trace.WriteLine("Messages Per Second: " + loopCount*1000/timer.ElapsedMilliseconds);
		}

		private static IMessageSink<PingMessage> SetupRouterOnly()
		{
			var consumer = new PerformantConsumer<PingMessage>();

			var messageSink = new InstanceMessageSink<PingMessage>(message => consumer);

			var messageSink2 = new InstanceMessageSink<PingMessage>(message => consumer);
			var messageSink3 = new InstanceMessageSink<PingMessage>(message => consumer);

			var router = new MessageRouter<PingMessage>();
			router.Connect(messageSink);
			router.Connect(messageSink2);
			router.Connect(messageSink3);
			return router;
		}

		private static IMessageSink<object> SetupTwoRoutersOnly()
		{
			var consumer = new PerformantConsumer<PingMessage>();

			var messageSink = new InstanceMessageSink<PingMessage>(message => consumer);

			var router = new MessageRouter<PingMessage>();
			router.Connect(messageSink);

			var translater = new MessageTranslator<object, PingMessage>(router);

			var nextRouter = new MessageRouter<object>();
			nextRouter.Connect(translater);

			return nextRouter;
		}

		private static void SendMessages(IMessageSink<PingMessage> sink, int primeLoopCount)
		{
			for (int i = 0; i < primeLoopCount; i++)
			{
				var message = new PingMessage();
				foreach (var item in sink.Enumerate(message))
				{
					item.Consume(message);
				}
			}
		}

		private static void SendMessages(IMessageSink<object> sink, int primeLoopCount)
		{
			for (int i = 0; i < primeLoopCount; i++)
			{
				var message = new PingMessage();
				foreach (var item in sink.Enumerate(message))
				{
					item.Consume(message);
				}
			}
		}
	}
}