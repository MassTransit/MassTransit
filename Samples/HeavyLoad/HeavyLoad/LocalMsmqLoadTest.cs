namespace HeavyLoad
{
	using System;
	using System.Threading;
	using MassTransit.ServiceBus;
	using MassTransit.ServiceBus.MSMQ;

	public class LocalMsmqLoadTest : IDisposable
	{
		private const int _repeatCount = 1000;
		private readonly ManualResetEvent _completeEvent = new ManualResetEvent(false);
		private readonly string _queueUri = "msmq://localhost/test_servicebus";
		private IServiceBus _bus;
		private int _counter = 0;
		private MsmqEndpoint _localEndpoint;
		private readonly ManualResetEvent _responseEvent = new ManualResetEvent(false);
		private int _responseCounter = 0;


		public LocalMsmqLoadTest()
		{
			_localEndpoint = new MsmqEndpoint(_queueUri);

			MsmqHelper.ValidateAndPurgeQueue(_localEndpoint.QueuePath);

			_bus = new ServiceBus(_localEndpoint);
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

			Semaphore countdown = new Semaphore(0, 100);

			for (int index = 0; index < _repeatCount/1000; index++)
			{
				ThreadPool.QueueUserWorkItem(
					delegate
						{
							for (int indexer = 0; indexer < 1000; indexer++)
							{
								_bus.Publish(new GeneralMessage());
							}
							countdown.Release();
						});
			}

			for (int index = 0; index < _repeatCount/1000; index++)
			{
				countdown.WaitOne(TimeSpan.FromSeconds(30), true);
			}

			publishCheckpoint.Complete(_repeatCount);

			bool completed = _completeEvent.WaitOne(TimeSpan.FromSeconds(60), true);

			bool responseCompleted = _responseEvent.WaitOne(TimeSpan.FromSeconds(60), true);

			receiveCheckpoint.Complete(_counter + _responseCounter);

			stopWatch.Stop();
		}

		private void Handler(IMessageContext<SimpleResponse> obj)
		{
			_responseCounter++;
			if (_responseCounter == _repeatCount)
				_responseEvent.Set();
		
		}

		private void Handle(IMessageContext<GeneralMessage> obj)
		{
			_bus.Publish(new SimpleResponse());

			_counter++;
			if (_counter == _repeatCount)
				_completeEvent.Set();
		}
	}
}