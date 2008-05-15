namespace MassTransit.ServiceBus.Internal
{
	/// <summary>
	/// A correlated message dispatcher sends a message to any attached consumers
	/// with a matching correlation identifier
	/// </summary>
	public interface IMessageDispatcher
	{
		/// <summary>
		/// Dispatches a message
		/// </summary>
		/// <param name="message">The message to dispatch</param>
		/// <returns>True if the message was dispatched to at least one consumer</returns>
		bool Dispatch(object message);

		/// <summary>
		/// Connects any consumers for the component to the message dispatcher
		/// </summary>
		/// <typeparam name="T">The component type</typeparam>
		/// <param name="component">The component</param>
		void Subscribe<T>(T component) where T : class;

		/// <summary>
		/// Disconnects any consumers for the component from the message dispatcher
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="component"></param>
		void Unsubscribe<T>(T component) where T : class;
	}
}