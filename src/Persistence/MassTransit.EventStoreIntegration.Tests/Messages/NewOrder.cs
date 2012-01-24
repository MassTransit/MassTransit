namespace MassTransit.EventStoreIntegration.Tests
{
	using System;

	[Serializable]
	public class NewOrder 
		: CorrelatedBy<Guid>
	{
		public Guid CorrelationId { get; set; }
		public string Name { get; set; }
		public string Item { get; set; }
		public string Size { get; set; }
	}
}