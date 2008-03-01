namespace MassTransit.ServiceBus.Internal
{
	/// <summary>
	/// Generic interface for mapping envelopes to messages
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IEnvelopeMapper<T>
	{
		/// <summary>
		/// Maps an envelope to a message
		/// </summary>
		/// <param name="envelope"></param>
		/// <returns></returns>
		T ToMessage(IEnvelope envelope);

		/// <summary>
		/// Maps a message to an envelope
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		IEnvelope ToEnvelope(T message);
	}
}