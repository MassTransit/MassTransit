namespace MassTransit.WindsorIntegration
{
    using Castle.MicroKernel;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using MassTransit.ServiceBus.HealthMonitoring;
    using MassTransit.ServiceBus.Subscriptions;
    using ServiceBus;

    public class MassTransitFacility : 
        Castle.MicroKernel.Facilities.AbstractFacility
    {
        private IWindsorContainer _container;

        protected override void Init()
        {
            string _listenUri = this.FacilityConfig.Attributes["listenAt"];
            string _subscriptionUri = this.FacilityConfig.Attributes["subscriptionsAt"];


            _container = this.Kernel.Resolve<IWindsorContainer>();

            

            _container.Register(
                Component.For<ISubscriptionCache>().ImplementedBy<LocalSubscriptionCache>(),
                Component.For<IObjectBuilder>().ImplementedBy<WindsorObjectBuilder>(),
                Component.For<IServiceBus>().ImplementedBy<ServiceBus>(),
                //Component.For<IEndpoint>().ImplementedBy<MsmqEndpoint>().Parameters(Parameter.ForKey("endpointToListenOn").Eq(_listenUri))
                Component.For<HealthClient>(),
                Component.For<SubscriptionClient>()
                    .AddAttributeDescriptor("startable", "true")
                    .AddAttributeDescriptor("startMethod", "Start")
                    .AddAttributeDescriptor("stopMethod", "Stop")
                    .Parameters(Parameter.ForKey("subscriptionServiceEndpoint").Eq(_subscriptionUri)) //needs its own endpoint
                );
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}