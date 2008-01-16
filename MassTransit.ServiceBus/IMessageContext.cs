namespace MassTransit.ServiceBus
{
	public interface IMessageContext<T>
	{
		/// <summary>
		/// The envelope containing the message
		/// </summary>
		IEnvelope Envelope { get; }

		/// <summary>
		/// The actual message being delivered
		/// </summary>
		T Message { get; }

		/// <summary>
		/// The service bus on which the message was received
		/// </summary>
		IServiceBus Bus { get; }

		/// <summary>
		/// Builds an envelope with the correlation id set to the id of the incoming envelope
		/// </summary>
		/// <param name="messages">The messages to include with the reply</param>
		void Reply(params IMessage[] messages);

		/// <summary>
		/// Moves the specified messages to the back of the list of available 
		/// messages so they can be handled later. Could screw up message order.
		/// </summary>
		void HandleMessagesLater(params IMessage[] messages);

		/// <summary>
		/// Marks the whole context as poison
		/// </summary>
		void MarkPoison();

		/// <summary>
		/// Marks a specific message as poison
		/// </summary>
		void MarkPoison(IMessage msg);
	}
}