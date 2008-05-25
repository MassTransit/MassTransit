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

			stopWatch.Start();

			CheckPoint publishCheckpoint = stopWatch.Mark("Publishing 1000 messages");
			CheckPoint receiveCheckpoint = stopWatch.Mark("Receiving 1000 messages");

			for (int index = 0; index < _repeatCount; index++)
			{
				_bus.Publish(new GeneralMessage());
			}

			publishCheckpoint.Complete(_repeatCount);

			bool completed = _completeEvent.WaitOne(TimeSpan.FromSeconds(30), true);

			receiveCheckpoint.Complete(_counter);

			stopWatch.Stop();
		}

		private void Handle(IMessageContext<GeneralMessage> obj)
		{
			_counter++;
			if (_counter == _repeatCount)
				_completeEvent.Set();
		}
	}
}