namespace MassTransit.ServiceBus.Internal
{
	/// <summary>
	/// Implemented by consumers of messages
	/// </summary>
	public interface IEnvelopeConsumer
	{
		/// <summary>
		/// Called when a message is available from the endpoint. If the consumer returns true, the message
		/// will be removed from the endpoint and delivered to the consumer
		/// </summary>
		/// <param name="envelope">The message envelope available</param>
		/// <returns>True is the consumer will handle the message, false if it should be ignored</returns>
		bool IsInterested(IEnvelope envelope);

		/// <summary>
		/// Delivers the message envelope to the consumer
		/// </summary>
		/// <param name="envelope">The message envelope</param>
		void Deliver(IEnvelope envelope);
	}
}