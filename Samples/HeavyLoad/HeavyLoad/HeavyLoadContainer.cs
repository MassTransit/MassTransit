namespace HeavyLoad
{
	using MassTransit.WindsorIntegration;

	public class HeavyLoadContainer : 
		DefaultMassTransitContainer
	{
		public HeavyLoadContainer()
			: base("castle.xml")
		{
		}
	}
}