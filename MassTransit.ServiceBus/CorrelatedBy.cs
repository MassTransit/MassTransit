namespace MassTransit.ServiceBus
{
	public interface CorrelatedBy<TKey>
	{
		TKey CorrelationId { get; }
	}
}