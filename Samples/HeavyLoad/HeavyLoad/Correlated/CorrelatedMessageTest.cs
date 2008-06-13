namespace HeavyLoad.Correlated
{
	using System;
	using System.Threading;
	using Castle.Windsor;
	using MassTransit.ServiceBus;
	using MassTransit.ServiceBus.MSMQ;

	public class CorrelatedMessageTest : IDisposable
	{
		private readonly int _attempts = 1500;
		private readonly IWindsorContainer _container;
		private readonly ManualResetEvent _finishedEvent = new ManualResetEvent(false);
		private IServiceBus _bus;
		private int _successes;
		private int _timeouts;

		public CorrelatedMessageTest()
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
			stopWatch.Start();

			SimpleRequestService service = new SimpleRequestService(_bus);

			_bus.Subscribe(service);

			CheckPoint point = stopWatch.Mark("Correlated Requests");

			for (int index = 0; index < _attempts; index++)
			{
				CorrelatedController controller = new CorrelatedController(_bus);

				controller.OnSuccess += controller_OnSuccess;
				controller.OnTimeout += controller_OnTimeout;
				controller.SimulateRequestResponse();

				controller.OnSuccess -= controller_OnSuccess;
				controller.OnTimeout -= controller_OnTimeout;
			}

			point.Complete(_attempts);

			_finishedEvent.WaitOne(TimeSpan.FromSeconds(60), true);

			_bus.Unsubscribe(service);

			Console.WriteLine("Attempts: {0}, Succeeded: {1}, Timeouts: {2}", _attempts, _successes, _timeouts);

			stopWatch.Stop();
		}

		private void controller_OnTimeout(CorrelatedController obj)
		{
			lock (_finishedEvent)
			{
				Interlocked.Increment(ref _timeouts);

				if (_timeouts + _successes == _attempts)
					_finishedEvent.Set();
			}
		}

		private void controller_OnSuccess(CorrelatedController obj)
		{
			lock (_finishedEvent)
			{
				Interlocked.Increment(ref _successes);

				if (_timeouts + _successes == _attempts)
					_finishedEvent.Set();
			}
		}
	}
}