namespace HeavyLoad.Load
{
    using Castle.Facilities.FactorySupport;
    using Castle.Windsor;
    using MassTransit.WindsorIntegration;

    public class LoadTestContainer :
        WindsorContainer
    {

        public LoadTestContainer()
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