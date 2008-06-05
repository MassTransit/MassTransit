namespace MassTransit.Dashboard
{
    using Castle.Facilities.Startable;
    using Castle.MicroKernel.Registration;
    using Castle.MonoRail.WindsorExtension;
    using Castle.Windsor;
    using Controllers;
    using MassTransit.ServiceBus.MSMQ;
    using MassTransit.ServiceBus.Subscriptions;
    using ServiceBus;

    public class WebAppContainer :
        WindsorContainer
    {
        public WebAppContainer()
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
            AddComponent("home.controller", typeof(SubscriptionsController));
        }

        protected void LoadMassTransit()
        {
            this.Register(
                Component.For<IEndpoint>()
                    .ImplementedBy<MsmqEndpoint>()
                    .Named("masstransit.bus.listen")
                    .Parameters(Parameter.ForKey("uriString").Eq("msmq://localhost/mt_dashboard")),
                Component.For<IServiceBus>()
                    .ImplementedBy<ServiceBus>()
                    .Named("masstransit.bus")
                    .Parameters(Parameter.ForKey("endpointToListenOn").Eq("${masstransit.bus.listen}")),
                Component.For<IEndpoint>()
                    .ImplementedBy<MsmqEndpoint>()
                    .Named("masstransit.subscription.endpoint")
                    .Parameters(Parameter.ForKey("uriString").Eq("msmq://localhost/mt_pubsub")),
                Component.For<IHostedService>()
                    .ImplementedBy<SubscriptionClient>()
                    .Named("masstransit.subscription.client")
                    .AddAttributeDescriptor("startable", "true")
                    .AddAttributeDescriptor("startMethod", "Start")
                    .AddAttributeDescriptor("stopMethod", "Stop")
                    .Parameters(Parameter.ForKey("subscriptionServiceEndpoint").Eq("${masstransit.subscription.endpoint}")),
                Component.For<ISubscriptionCache>()
                    .ImplementedBy<LocalSubscriptionCache>()
                    .Named("masstransit.cache")
                );
        }
    }
}