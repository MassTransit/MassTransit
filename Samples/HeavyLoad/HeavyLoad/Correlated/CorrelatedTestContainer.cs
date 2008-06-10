namespace HeavyLoad.Correlated
{
	using MassTransit.WindsorIntegration;

	public class CorrelatedTestContainer :
		DefaultMassTransitContainer
	{
		public CorrelatedTestContainer()
			: base("castle.xml")
		{
		}
	}
}