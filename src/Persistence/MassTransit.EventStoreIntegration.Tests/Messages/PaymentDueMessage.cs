namespace MassTransit.EventStoreIntegration.Tests
{
	using System;

	[Serializable]
	public class PaymentDueMessage 
		: CorrelatedBy<Guid>
	{
		public decimal Amount { get; set; }
		public Guid CorrelationId { get; set; }
	}
}