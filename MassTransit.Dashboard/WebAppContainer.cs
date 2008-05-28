namespace MassTransit.Dashboard
{
    using Castle.Core.Resource;
    using Castle.Facilities.Startable;
    using Castle.MonoRail.WindsorExtension;
    using Castle.Windsor;
    using Castle.Windsor.Configuration.Interpreters;
    using Controllers;
    using MassTransit.ServiceBus.MSMQ;
    using MassTransit.ServiceBus.Subscriptions;
    using MassTransit.ServiceBus.Subscriptions.Messages;
    using ServiceBus;

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
            AddFacility("rails", new RailsFacility());
            AddFacility("startable", new StartableFacility());
        }

        protected void RegisterComponents()
        {
            AddComponent("home.controller", typeof(HomeController));
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