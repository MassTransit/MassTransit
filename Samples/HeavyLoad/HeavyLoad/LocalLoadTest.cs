namespace HeavyLoad
{
	using System;
	using System.Collections;
	using System.Threading;
	using Castle.Facilities.FactorySupport;
	using Castle.Windsor;
	using MassTransit.ServiceBus;
	using MassTransit.WindsorIntegration;

    public abstract class LocalLoadTest : IDisposable
	{
	    private IWindsorContainer _container;
		private const int _repeatCount = 5000;
		private readonly ManualResetEvent _completeEvent = new ManualResetEvent(false);
		private readonly ManualResetEvent _responseEvent = new ManualResetEvent(false);

		private IServiceBus _bus;
		private int _counter = 0;
		private IEndpoint _localEndpoint;
		private int _responseCounter = 0;

		public LocalLoadTest(Uri listenAt)
		{
		    _container = new WindsorContainer("castle.xml");
		    _container.AddFacility("factory.support", new FactorySupportFacility());
		    _container.AddFacility("masstransit",new MassTransitFacility());

		    IDictionary args = new Hashtable();
            args.Add("endpointToListenOn", listenAt);

		    _localEndpoint = _container.Resolve<IEndpoint>(args);
		    _bus = _container.Resolve<IServiceBus>();
		}

		public IServiceBus Bus
		{
			get { return _bus; }
		}

		public IEndpoint LocalEndpoint
		{
			get { return _localEndpoint; }
		}

		public void Dispose()
		{
			if (_bus != null)
			{
				_bus.Dispose();
				_bus = null;
			}

			if (_localEndpoint != null)
			{
				_localEndpoint.Dispose();
				_localEndpoint = null;
			}
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