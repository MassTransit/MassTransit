namespace HeavyLoad.Load
{
	using System;
	using System.Threading;
	using MassTransit;
	using StructureMap;

	public class StructureMapConsumerLoadTest :
		IDisposable
	{
		const int _repeatCount = 10000;
		static readonly ManualResetEvent _completeEvent = new ManualResetEvent(false);
		static readonly ManualResetEvent _responseEvent = new ManualResetEvent(false);

		static int _requestCounter;
		static int _responseCounter;
		IServiceBus _bus;
		IContainer _container;

		public StructureMapConsumerLoadTest()
		{
			_container = new Container(x =>
				{
					x.For<RequestConsumer>()
						.Use<RequestConsumer>();
					x.For<ResponseConsumer>()
						.Use<ResponseConsumer>();

					x.For<IServiceBus>()
						.Singleton()
						.Use(context => ServiceBusFactory.New(c =>
							{
								c.ReceiveFrom("loopback://localhost/heavy_load");

								c.Subscribe(s => s.LoadFrom(_container));
							}));
				});

			_bus = _container.GetInstance<IServiceBus>();
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

		public class ResponseConsumer : Consumes<SimpleResponse>.All
		{
			public void Consume(SimpleResponse message)
			{
				Interlocked.Increment(ref _responseCounter);
				if (_responseCounter == _repeatCount)
					_responseEvent.Set();
			}
		}
	}
}