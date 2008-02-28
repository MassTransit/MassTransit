using MassTransit.ServiceBus;

namespace MassTransit.Patterns.Batching
{
	public interface IBatchMessage : IMessage
	{
		/// <summary>
		/// The number of messages in the batch
		/// </summary>
		int BatchLength { get; }

		/// <summary>
		/// Identifies the batch containing this message
		/// </summary>
		object BatchId { get; }
	}
}