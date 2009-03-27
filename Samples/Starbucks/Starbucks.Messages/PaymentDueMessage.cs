namespace Starbucks.Messages
{
	using System;
	using MassTransit;

	[Serializable]
	public class PaymentDueMessage :
		CorrelatedBy<Guid>
	{
		public decimal Amount { get; set; }

		public Guid CorrelationId { get; set; }
	}
}