using System;

namespace MassTransit.ServiceBus
{
	/// <summary>
	/// A public interface to the envelope containing message(s)
	/// </summary>
	public interface IEnvelope
	{
		/// <summary>
		/// The messages contained in the envelope
		/// </summary>
		IMessage[] Messages { get; set; }

		/// <summary>
		/// The return endpoint for the message(s) in the envelope
		/// </summary>
		IEndpoint ReturnTo { get; }

		string Id { get; set; }

		string CorrelationId { get; set; }

		bool Recoverable { get; set; }

		TimeSpan TimeToBeReceived { get; set; }

		DateTime SentTime { get; set; }

		DateTime ArrivedTime { get; set; }

		string Label { get; set; }
	}
}