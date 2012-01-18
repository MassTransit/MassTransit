namespace MassTransit.EventStoreIntegration
{
	using Saga;

	/// <summary>
	/// Interface specifying event sourced sagas.
	/// </summary>
	public interface ISagaEventSourced : ISaga
	{
		ISagaDeltaManager DeltaManager { get; }
	}
}