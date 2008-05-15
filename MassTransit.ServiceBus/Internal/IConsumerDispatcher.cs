namespace MassTransit.ServiceBus.Internal
{
	public interface IConsumerDispatcher<T> where T : class
	{
		bool Dispatch(T message);
		void Subscribe(Consumes<T>.Any consumer);
		void Unsubscribe(Consumes<T>.Any consumer);
	}
}