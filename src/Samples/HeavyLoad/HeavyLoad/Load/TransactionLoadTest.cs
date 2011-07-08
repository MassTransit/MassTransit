namespace HeavyLoad.Load
{
	using System;
	using System.Threading;
	using MassTransit;

	public class TransactionLoadTest :
		IDisposable
	{
		const int _repeatCount = 5000;
		static readonly ManualResetEvent _completeEvent = new ManualResetEvent(false);
		static readonly ManualResetEvent _responseEvent = new ManualResetEvent(false);

		IServiceBus _bus;
		int _requestCounter;
		int _responseCounter;

		public TransactionLoadTest()
		{
			_bus = ServiceBusFactory.New(x =>
				{
					x.ReceiveFrom("msmq://localhost/heavy_load_tx");
					x.SetPurgeOnStartup(true);

					x.UseMsmq();

					x.Subscribe(s =>
						{
							s.Handler<GeneralMessage>(Handle);
							s.Handler<SimpleResponse>(Handler);
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

		void Handler(SimpleResponse obj)
		{
			Interlocked.Increment(ref _responseCounter);
			if (_responseCounter == _repeatCount)
				_responseEvent.Set();
		}

		void Handle(GeneralMessage obj)
		{
			_bus.Publish(new SimpleResponse());

			Interlocked.Increment(ref _requestCounter);
			if (_requestCounter == _repeatCount)
				_completeEvent.Set();
		}
	}
}