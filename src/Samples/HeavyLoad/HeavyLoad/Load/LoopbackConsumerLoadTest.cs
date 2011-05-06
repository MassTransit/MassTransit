namespace HeavyLoad.Load
{
	using System;
	using System.Collections.Generic;
	using System.Threading;
	using MassTransit;

	public class LoopbackConsumerLoadTest :
		IConsumerFactory<LoopbackConsumerLoadTest.RequestConsumer>,
		IDisposable
	{
		const int _repeatCount = 10000;
		static readonly ManualResetEvent _completeEvent = new ManualResetEvent(false);
		static readonly ManualResetEvent _responseEvent = new ManualResetEvent(false);

		static int _requestCounter;
		static int _responseCounter;
		IServiceBus _bus;

		public LoopbackConsumerLoadTest()
		{
			_bus = ServiceBusFactory.New(x =>
				{
					x.ReceiveFrom("loopback://localhost/heavy_load");

					x.Subscribe(s =>
						{
							s.Consumer<RequestConsumer>(this);
							s.Consumer<ResponseConsumer>();
						});
				});
		}

		public void Dispose()
		{
			_bus.Dispose();
		}

		public void Run(StopWatch stopWatch)
		{
			stopWatch.Start();

			CheckPoint publishCheckpoint = stopWatch.Mark("Publishing " + _repeatCount + " messages");
			CheckPoint receiveCheckpoint = stopWatch.Mark("Receiving " + _repeatCount + " messages");

			for (int index = 0; index < _repeatCount; index++)
			{
				_bus.Publish(new GeneralMessage());
			}

			publishCheckpoint.Complete(_repeatCount);

			_completeEvent.WaitOne(TimeSpan.FromSeconds(60), true);

			_responseEvent.WaitOne(TimeSpan.FromSeconds(60), true);

			receiveCheckpoint.Complete(_requestCounter + _responseCounter);

			stopWatch.Stop();
		}

		public class RequestConsumer :
			Consumes<GeneralMessage>.All
		{
			readonly IServiceBus _bus;

			public RequestConsumer(IServiceBus bus)
			{
				_bus = bus;
			}

			public void Consume(GeneralMessage message)
			{
				_bus.Publish(new SimpleResponse());

				Interlocked.Increment(ref _requestCounter);
				if (_requestCounter == _repeatCount)
					_completeEvent.Set();
			}
		}

		internal class ResponseConsumer : Consumes<SimpleResponse>.All
		{
			public void Consume(SimpleResponse message)
			{
				Interlocked.Increment(ref _responseCounter);
				if (_responseCounter == _repeatCount)
					_responseEvent.Set();
			}
		}

		public IEnumerable<Action<TMessage>> GetConsumer<TMessage>(Func<RequestConsumer, Action<TMessage>> callback)
		{
			var consumer = new RequestConsumer(_bus);

			Action<TMessage> result = callback(consumer);
			if (result == null)
				yield break;

			yield return result;
		}
	}
}