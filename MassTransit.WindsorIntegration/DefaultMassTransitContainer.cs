namespace MassTransit.WindsorIntegration
{
    using Castle.Facilities.FactorySupport;
    using Castle.Facilities.Startable;
    using Castle.Windsor;

    public class DefaultMassTransitContainer : WindsorContainer
    {
        public DefaultMassTransitContainer()
        {
            Initialize();
        }

        public DefaultMassTransitContainer(string xmlFile) : base(xmlFile)
        {
            Initialize();
        }

        public void Initialize()
        {
            this.AddFacility("startable", new StartableFacility());
            this.AddFacility("factory", new FactorySupportFacility());
            this.AddFacility("masstransit", new MassTransitFacility());
        }
    }
}