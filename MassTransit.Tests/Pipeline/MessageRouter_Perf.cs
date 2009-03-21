namespace MassTransit.Tests.Pipeline
{
	using System;
	using System.Diagnostics;
	using System.Threading;
	using Magnum.DateTimeExtensions;
	using MassTransit.Pipeline;
	using MassTransit.Pipeline.Sinks;
	using Messages;
	using NUnit.Framework;
	using Threading;

	[TestFixture]
	public class MessageRouter_Perf
	{
		private ManagedThreadPool<int> _threads;
		private static PerformantConsumer<PingMessage> _consumer;

		internal class PerformantConsumer<T> :
			Consumes<T>.All
			where T : class
		{
			private readonly long _limit;
			private long _count = 0;
			private readonly ManualResetEvent _complete = new ManualResetEvent(false);

			public ManualResetEvent Complete
			{
				get { return _complete; }
			}

			public PerformantConsumer(long limit)
			{
				_limit = limit;
			}

			public void Consume(T message)
			{
				long value = Interlocked.Increment(ref _count);
				if (value == _limit)
					_complete.Set();
			}
		}


		[Test, Explicit]
		public void Router_performance_measurement()
		{
			IPipelineSink<PingMessage> router = SetupRouterOnly();

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
			IPipelineSink<object> router = SetupTwoRoutersOnly();

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
		public void Threaded_router_performance_test()
		{
			IPipelineSink<object> router = SetupTwoRoutersOnly();

			const int primeLoopCount = 10;
			SendMessages(router, primeLoopCount);


			const int loopCount = 1500000;
			_threads = new ManagedThreadPool<int>(delegate { SendMessages(router, loopCount / 2); }, 2, 2);

			Stopwatch timer = Stopwatch.StartNew();

			for (int i = 0; i < 2; i++)
			{
				_threads.Enqueue(100);
			}

			_consumer.Complete.WaitOne(20.Seconds(), true);

			timer.Stop();

			Trace.WriteLine("Elapsed Time: " + timer.ElapsedMilliseconds + "ms");
			Trace.WriteLine("Messages Per Second: " + loopCount*1000/timer.ElapsedMilliseconds);
		}

		private static IPipelineSink<PingMessage> SetupRouterOnly()
		{
			_consumer = new PerformantConsumer<PingMessage>(1500000);

			var messageSink = new InstanceMessageSink<PingMessage>(message => _consumer.Consume);

			var router = new MessageRouter<PingMessage>();
			router.Connect(messageSink);
			return router;
		}

		private static IPipelineSink<object> SetupTwoRoutersOnly()
		{
			_consumer = new PerformantConsumer<PingMessage>(1500000);

			var messageSink = new InstanceMessageSink<PingMessage>(message => _consumer.Consume);

			var router = new MessageRouter<PingMessage>();
			router.Connect(messageSink);

			var translater = new MessageTranslator<object, PingMessage>(router);

			var nextRouter = new MessageRouter<object>();
			nextRouter.Connect(translater);

			return nextRouter;
		}

		private static void SendMessages(IPipelineSink<PingMessage> sink, int primeLoopCount)
		{
			for (int i = 0; i < primeLoopCount; i++)
			{
				var message = new PingMessage();
				foreach (var item in sink.Enumerate(message))
				{
					item(message);
				}
			}
		}

		private static void SendMessages(IPipelineSink<object> sink, int primeLoopCount)
		{
			for (int i = 0; i < primeLoopCount; i++)
			{
				var message = new PingMessage();
				foreach (var item in sink.Enumerate(message))
				{
					item(message);
				}
			}
		}
	}
}