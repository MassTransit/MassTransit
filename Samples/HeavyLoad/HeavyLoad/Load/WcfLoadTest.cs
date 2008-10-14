namespace HeavyLoad.Load
{
	using System;
	using System.Threading;
	using Castle.Windsor;
	using MassTransit.ServiceBus;
	using MassTransit.WindsorIntegration;

	public class WcfLoadTest : IDisposable
	{
		private const int _repeatCount = 3000;
		private static readonly ManualResetEvent _completeEvent = new ManualResetEvent(false);
		private readonly IWindsorContainer _container;
		private static readonly ManualResetEvent _responseEvent = new ManualResetEvent(false);


		private readonly IServiceBus _localBus;
		private readonly IServiceBus _remoteBus;
		private static int _counter;
		private static int _responseCounter;

		public WcfLoadTest()
		{
			_container = new DefaultMassTransitContainer("wcf.castle.xml");

			_localBus = _container.Resolve<IServiceBus>("local");
			_remoteBus = _container.Resolve<IServiceBus>("remote");
		}

		public void Dispose()
		{
			_localBus.Dispose();
			_remoteBus.Dispose();
			_container.Dispose();
		}

		public void Run(StopWatch stopWatch)
		{
		    _container.AddComponent<RequestConsumer>();
		    _container.AddComponent<ResponseConsumer>();

			_remoteBus.AddComponent<RequestConsumer>();
			_localBus.AddComponent<ResponseConsumer>();

			stopWatch.Start();

			CheckPoint publishCheckpoint = stopWatch.Mark("Publishing " + _repeatCount + " messages");
			CheckPoint receiveCheckpoint = stopWatch.Mark("Receiving " + _repeatCount + " messages");

			for (int index = 0; index < _repeatCount; index++)
			{
				_localBus.Publish(new GeneralMessage());
			}

			publishCheckpoint.Complete(_repeatCount);

			_completeEvent.WaitOne(TimeSpan.FromSeconds(60), true);

			_responseEvent.WaitOne(TimeSpan.FromSeconds(60), true);

			receiveCheckpoint.Complete(_counter + _responseCounter);

			stopWatch.Stop();
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

		internal class RequestConsumer : 
			Consumes<GeneralMessage>.All
		{
		    private IServiceBus _bus = ServiceBus.Null;

		    public void Consume(GeneralMessage message)
			{
				Interlocked.Increment(ref _counter);
				if (_counter == _repeatCount)
					_completeEvent.Set();

				_bus.Publish(new SimpleResponse());
			}

		    public IServiceBus Bus
		    {
                set { _bus = value; }
		    }
		}
	}
}
