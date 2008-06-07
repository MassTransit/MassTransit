namespace MassTransit.ServiceBus.Internal
{
	public interface IMessageTypeSubscription
	{
		void Subscribe<T>(MessageTypeDispatcher dispatcher, T component) where T : class;
		void Unsubscribe<T>(MessageTypeDispatcher dispatcher, T component) where T : class;

		void AddComponent(MessageTypeDispatcher dispatcher);
		void RemoveComponent(MessageTypeDispatcher dispatcher);
	}
}