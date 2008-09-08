namespace MassTransit.Services
{
	using System;
	using MassTransit.ServiceBus.Timeout.Messages;
	using MassTransit.ServiceBus.Util;
	using Messages;
	using ServiceBus;

	public class DeferMessageConsumer :
		Consumes<DeferMessage>.All
	{
		private readonly IServiceBus _bus;
		private readonly IDeferredMessageRepository _deferred;

		public DeferMessageConsumer(IServiceBus bus, IDeferredMessageRepository deferred)
		{
			_deferred = deferred;
			_bus = bus;
		}

		public void Consume(DeferMessage message)
		{
			Guid deferredId = CombGuid.NewCombGuid();

			_deferred.Add(deferredId, message);

			_bus.Publish(new ScheduleTimeout(deferredId, message.DeliverAt));
		}
	}
}