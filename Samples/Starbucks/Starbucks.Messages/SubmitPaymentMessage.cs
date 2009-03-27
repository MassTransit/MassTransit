namespace Starbucks.Messages
{
	using System;
	using MassTransit;

	[Serializable]
	public class SubmitPaymentMessage :
		CorrelatedBy<Guid>
	{
		public PaymentType PaymentType { get; set; }
		public decimal Amount { get; set; }
		public Guid CorrelationId { get; set; }
	}
}