namespace MassTransit.EventStoreIntegration.Tests
{
	using System;

	[Serializable]
	public class PaymentCompleteMessage 
		: CorrelatedBy<Guid>
	{
		public Guid CorrelationId { get; set; }
	}
}