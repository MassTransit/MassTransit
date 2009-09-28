namespace Inventory.Messages
{
	using System;
	using MassTransit;

	[Serializable]
	public class PartInventoryLevelStatus :
		CorrelatedBy<string>
	{
		public PartInventoryLevelStatus(string partNumber, int onHand, int onOrder)
		{
			PartNumber = partNumber;
			OnHand = onHand;
			OnOrder = onOrder;
		}

		protected PartInventoryLevelStatus()
		{
		}

		public int OnOrder { get; set; }

		public int OnHand { get; set; }

		public string PartNumber { get; set; }

		public string CorrelationId
		{
			get { return PartNumber; }
		}
	}
}