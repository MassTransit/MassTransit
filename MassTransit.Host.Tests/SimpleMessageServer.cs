namespace MassTransit.Host.Tests
{
	using System;
	using ServiceBus;

	public class SimpleMessageServer :
		IHostedService
	{
		private readonly IServiceBus _serviceBus;
		private int _hitCount;

		public SimpleMessageServer(IServiceBus serviceBus)
		{
			_serviceBus = serviceBus;
		}

		#region IMessageService Members

		public void Start()
		{
			_serviceBus.Subscribe<MySimpleMessage>(HandleMySimpleMessage);
		}

		public void Stop()
		{
			_serviceBus.Unsubscribe<MySimpleMessage>(HandleMySimpleMessage);
		}

		public void Dispose()
		{
			_serviceBus.Dispose();
		}

		#endregion

		private void HandleMySimpleMessage(IMessageContext<MySimpleMessage> ctx)
		{
			_hitCount++;
		}
	}

	[Serializable]
	public class MySimpleMessage :
		IMessage
	{
	}
}