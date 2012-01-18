namespace MassTransit.EventStoreIntegration.Tests
{
	using System;

	[Serializable]
	public class SubmitPayment :
		CorrelatedBy<Guid>
	{
		public PaymentType PaymentType { get; set; }
		public decimal Amount { get; set; }
		public Guid CorrelationId { get; set; }
	}
}