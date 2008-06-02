namespace WebRequestReply.UI.MonoRail
{
    using Castle.Core.Resource;
    using Castle.Facilities.Startable;
    using Castle.MonoRail.WindsorExtension;
    using Castle.Windsor;
    using Castle.Windsor.Configuration.Interpreters;
    using Controllers;
    using MassTransit.ServiceBus;
    using MassTransit.ServiceBus.MSMQ;
    using MassTransit.ServiceBus.Subscriptions;

    public class WebAppContainer :
        WindsorContainer
    {
        public WebAppContainer()
            : base(new XmlInterpreter(new ConfigResource()))
        {
            RegisterFacilities();
            LoadMassTransit();
            RegisterComponents();
        }

        protected void RegisterFacilities()
        {
            AddFacility("rails", new MonoRailFacility());
            AddFacility("startable", new StartableFacility());
        }

        protected void RegisterComponents()
        {
            AddComponent("demo.controller", typeof(DemoController));
        }

        protected void LoadMassTransit()
        {
            AddComponent("masstransit.bus", typeof (IServiceBus), typeof (ServiceBus));
            AddComponent("masstransit.bus.listen", typeof (IEndpoint), typeof (MsmqEndpoint));

            AddComponent("masstransit.subscription.endpoint", typeof (IEndpoint), typeof (MsmqEndpoint));
            AddComponent("masstransit.subscription.client", typeof (IHostedService), typeof (SubscriptionClient));
            AddComponent("masstransit.cache", typeof (ISubscriptionCache), typeof (LocalSubscriptionCache));
        }
    }
}