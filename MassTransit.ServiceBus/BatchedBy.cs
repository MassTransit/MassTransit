namespace MassTransit.ServiceBus
{
	public interface BatchedBy<TBatchId>
	{
		TBatchId BatchId { get; }
		int BatchLength { get; }
	}
}