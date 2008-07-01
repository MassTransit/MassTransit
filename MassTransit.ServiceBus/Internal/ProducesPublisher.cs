namespace MassTransit.ServiceBus.Internal
{
	public class ProducesPublisher<TMessage> :
		Consumes<TMessage>.All
		where TMessage : class
	{
		private readonly IServiceBus _bus;

		public ProducesPublisher(IServiceBus bus)
		{
			_bus = bus;
		}

		public void Consume(TMessage message)
		{
			_bus.Publish(message);
		}
	}
}