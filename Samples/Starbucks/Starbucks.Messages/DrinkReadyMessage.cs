namespace Starbucks.Messages
{
	using System;
	using MassTransit;

	[Serializable]
	public class DrinkReadyMessage :
		CorrelatedBy<Guid>
	{
		public string Drink { get; set; }

		public Guid CorrelationId { get; set; }

		public string Name { get; set; }
	}
}