namespace HeavyLoad.Correlated
{
	using System;
	using System.Threading;
	using Castle.Windsor;
	using MassTransit;
	using MassTransit.MSMQ;
	using MassTransit.Threading;

    public class CorrelatedMessageTest : IDisposable
	{
		private readonly int _attempts = 5000;
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
                MsmqUtilities.ValidateAndPurgeQueue(endpoint.QueuePath);
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
		    CheckPoint responsePoint = stopWatch.Mark("Correlated Responses");

		    ManagedThreadPool<CorrelatedController> pool = new ManagedThreadPool<CorrelatedController>(DoWorker, 10, 10);
            try
            {

                for (int index = 0; index < _attempts; index++)
                {
                    CorrelatedController controller = new CorrelatedController(_bus, OnSuccess, OnTimeout);
                    pool.Enqueue(controller);
                }

                point.Complete(_attempts);

                _finishedEvent.WaitOne(TimeSpan.FromSeconds(60), true);

                responsePoint.Complete(_timeouts + _successes);
            }
            finally
            {
                pool.Dispose();
            }

		    _bus.Unsubscribe(service);

			Console.WriteLine("Attempts: {0}, Succeeded: {1}, Timeouts: {2}", _attempts, _successes, _timeouts);

			stopWatch.Stop();
		}

        private void DoWorker(CorrelatedController controller)
        {
            controller.SimulateRequestResponse();
        }

        private void OnTimeout(CorrelatedController obj)
		{
			lock (_finishedEvent)
			{
				Interlocked.Increment(ref _timeouts);

				if (_timeouts + _successes == _attempts)
					_finishedEvent.Set();
			}
		}

        private void OnSuccess(CorrelatedController obj)
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