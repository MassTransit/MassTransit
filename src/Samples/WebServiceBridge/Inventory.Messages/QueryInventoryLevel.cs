namespace Inventory.Messages
{
	using System;
	using Magnum;
	using MassTransit;

	[Serializable]
	public class QueryInventoryLevel :
		CorrelatedBy<Guid>
	{
		public QueryInventoryLevel(string partNumber)
			: this(CombGuid.Generate(), partNumber)
		{
		}

		public QueryInventoryLevel(Guid correlationId, string partNumber)
		{
			CorrelationId = correlationId;
			PartNumber = partNumber;
		}

		protected QueryInventoryLevel()
		{
		}

		public string PartNumber { get; set; }

		public Guid CorrelationId { get; set; }
	}
}