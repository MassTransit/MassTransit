namespace HeavyLoad
{
    using Castle.Facilities.FactorySupport;
    using Castle.Windsor;
    using MassTransit.WindsorIntegration;

    public class CorrelatedTestContainer :
        WindsorContainer
    {

        public CorrelatedTestContainer()
            : base("castle.xml")
        {
            LoadMassTransit();
        }

        protected void LoadMassTransit()
        {

            AddFacility("factory.support", new FactorySupportFacility());
            AddFacility("masstransit", new MassTransitFacility());
        }
    }
}