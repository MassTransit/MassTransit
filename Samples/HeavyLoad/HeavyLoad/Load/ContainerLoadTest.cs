namespace HeavyLoad.Load
{
	using System;
	using System.Threading;
	using Castle.Windsor;
	using MassTransit.ServiceBus;
	using MassTransit.ServiceBus.MSMQ;

	public class ContainerLoadTest : IDisposable
	{
		private const int _repeatCount = 5000;
		private static readonly ManualResetEvent _completeEvent = new ManualResetEvent(false);
		private readonly IWindsorContainer _container;
		private static readonly ManualResetEvent _responseEvent = new ManualResetEvent(false);

		private IServiceBus _bus;
		private static int _counter = 0;
		private static int _responseCounter = 0;

		public ContainerLoadTest()
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
			_bus.AddComponent<RequestConsumer>();
			_bus.AddComponent<ResponseConsumer>();

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