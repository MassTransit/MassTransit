namespace MassTransit.WindsorIntegration
{
	using Castle.Facilities.FactorySupport;
	using Castle.Facilities.Startable;
	using Castle.Windsor;
	using Castle.Windsor.Configuration;

	public class DefaultMassTransitContainer :
		WindsorContainer
	{
		public DefaultMassTransitContainer()
		{
			Initialize();
		}

		public DefaultMassTransitContainer(string xmlFile) 
			: base(xmlFile)
		{
			Initialize();
		}

		public DefaultMassTransitContainer(IConfigurationInterpreter configurationInterpreter)
			: base(configurationInterpreter)
		{
			Initialize();
		}

		public void Initialize()
		{
			AddFacility("startable", new StartableFacility());
			AddFacility("factory", new FactorySupportFacility());
			AddFacility("masstransit", new MassTransitFacility());
		}
	}
}