namespace HeavyLoad.BatchLoad
{
	using MassTransit.WindsorIntegration;

	public class BatchLoadTestContainer :
		DefaultMassTransitContainer
	{
		public BatchLoadTestContainer()
			: base("castle.xml")
		{
		}
	}
}