namespace MassTransit.EventStoreIntegration.Tests
{
	using System;

	[Serializable]
	public class GuardsGetThisDudeOutOfTheStore
	{
		public string Reason { get; set; }
	}
}