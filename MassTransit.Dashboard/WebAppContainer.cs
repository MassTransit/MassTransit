namespace MassTransit.Dashboard
{
    using Castle.Facilities.Startable;
    using Castle.MonoRail.WindsorExtension;
    using Castle.Windsor;
    using Castle.Windsor.Configuration.Interpreters;
    using Controllers;
    using WindsorIntegration;

    public class WebAppContainer :
        WindsorContainer
    {
        public WebAppContainer() : base(new XmlInterpreter())
        {
            RegisterFacilities();
            RegisterComponents();
        }

        protected void RegisterFacilities()
        {
            AddFacility("rails", new MonoRailFacility());
            AddFacility("startable", new StartableFacility());
            AddFacility("masstransit", new MassTransitFacility());
        }

        protected void RegisterComponents()
        {
            AddComponent("controller.subscription", typeof(SubscriptionsController));
            AddComponent("controller.health", typeof(HealthController));
            AddComponent("controller.metadata", typeof(MetadataController));
        }
    }
}