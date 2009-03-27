namespace Starbucks.Messages
{
	using System;
	using MassTransit;

	[Serializable]
	public class PaymentCompleteMessage :
		CorrelatedBy<Guid>
	{
		public Guid CorrelationId { get; set; }
	}
}