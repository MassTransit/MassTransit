namespace HeavyLoad.Load
{
	using System;
	using System.Threading;
	using Castle.Windsor;
	using MassTransit.ServiceBus;
	using MassTransit.ServiceBus.MSMQ;

	public class LocalLoadTest : IDisposable
	{
		private const int _repeatCount = 5000;
		private readonly ManualResetEvent _completeEvent = new ManualResetEvent(false);
		private readonly IWindsorContainer _container;
		private readonly ManualResetEvent _responseEvent = new ManualResetEvent(false);

		private readonly IServiceBus _bus;
		private int _counter = 0;
		private int _responseCounter = 0;

		public LocalLoadTest()
		{
			_container = new HeavyLoadContainer();

			_bus = _container.Resolve<IServiceBus>();

			MsmqEndpoint endpoint = _bus.Endpoint as MsmqEndpoint;
			if (endpoint != null)
				MsmqHelper.ValidateAndPurgeQueue(endpoint.QueuePath);
		}

		public void Dispose()
		{
			_bus.Dispose();
			_container.Dispose();
		}

		public void Run(StopWatch stopWatch)
		{
			_bus.Subscribe<GeneralMessage>(Handle);
			_bus.Subscribe<SimpleResponse>(Handler);

			stopWatch.Start();

			CheckPoint publishCheckpoint = stopWatch.Mark("Publishing " + _repeatCount + " messages");
			CheckPoint receiveCheckpoint = stopWatch.Mark("Receiving " + _repeatCount + " messages");

			for (int index = 0; index < _repeatCount; index++)
			{
				_bus.Publish(new GeneralMessage());
			}

			publishCheckpoint.Complete(_repeatCount);

			bool completed = _completeEvent.WaitOne(TimeSpan.FromSeconds(60), true);

			bool responseCompleted = _responseEvent.WaitOne(TimeSpan.FromSeconds(60), true);

			receiveCheckpoint.Complete(_counter + _responseCounter);

			stopWatch.Stop();
		}

		private void Handler(IMessageContext<SimpleResponse> obj)
		{
			Interlocked.Increment(ref _responseCounter);
			if (_responseCounter == _repeatCount)
				_responseEvent.Set();
		}

		private void Handle(IMessageContext<GeneralMessage> obj)
		{
			_bus.Publish(new SimpleResponse());

			Interlocked.Increment(ref _counter);
			if (_counter == _repeatCount)
				_completeEvent.Set();
		}
	}
}