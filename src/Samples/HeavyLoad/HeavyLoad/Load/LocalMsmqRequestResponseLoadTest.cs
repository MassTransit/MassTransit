namespace HeavyLoad.Load
{
	using System;
	using System.Threading;
	using Correlated;
	using Magnum.Extensions;
	using MassTransit;

	public class LocalMsmqRequestResponseLoadTest :
		IDisposable
	{
		const int _repeatCount = 1000;
		readonly IServiceBus _bus;
		readonly ManualResetEvent _handlerComplete = new ManualResetEvent(false);
		readonly ManualResetEvent _clientComplete = new ManualResetEvent(false);

		int _requestCounter;
		int _responseCounter;

		public LocalMsmqRequestResponseLoadTest()
		{
			_bus = ServiceBusFactory.New(x =>
				{
					x.ReceiveFrom("msmq://localhost/heavy_load");
					x.SetPurgeOnStartup(true);

					x.UseMsmq();
					x.UseMulticastSubscriptionClient();

					x.Subscribe(s => { s.Handler<SimpleRequestMessage>(Handle); });
				});
		}

		public void Dispose()
		{
			_bus.Dispose();
		}

		public void Run(StopWatch stopWatch)
		{
			stopWatch.Start();

			CheckPoint publishCheckpoint = stopWatch.Mark("Sending " + _repeatCount + " messages");
			CheckPoint receiveCheckpoint = stopWatch.Mark("Request/Response " + _repeatCount + " messages");

			for (int index = 0; index < _repeatCount; index++)
			{
				Guid transactionId = Guid.NewGuid();

			    _bus.PublishRequest(new SimpleRequestMessage(transactionId), x =>
			        {
			            Interlocked.Increment(ref _requestCounter);

			            x.Handle<SimpleResponseMessage>(message =>
			                {
			                    Interlocked.Increment(ref _responseCounter);

			                    if (_responseCounter == _repeatCount)
			                        _clientComplete.Set();
			                });

			            x.HandleTimeout(10.Seconds(), () => { });
			        });
			}

			publishCheckpoint.Complete(_repeatCount);

			_handlerComplete.WaitOne(TimeSpan.FromSeconds(60), true);

			_clientComplete.WaitOne(TimeSpan.FromSeconds(60), true);

			receiveCheckpoint.Complete(_requestCounter + _responseCounter);

			stopWatch.Stop();
		}

		void Handle(SimpleRequestMessage message)
		{
			_bus.MessageContext<SimpleRequestMessage>()
				.Respond(new SimpleResponseMessage(message.CorrelationId));

			if (_requestCounter == _repeatCount)
				_handlerComplete.Set();
		}
	}
}