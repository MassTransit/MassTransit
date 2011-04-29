namespace HeavyLoad
{
    using Castle.Windsor;
	using MassTransit.WindsorIntegration;

	public class HeavyLoadContainer : 
		WindsorContainer
	{
		public HeavyLoadContainer()
			: base("castle.xml")
		{
		    Install(new MassTransitInstaller());
		}
	}
}