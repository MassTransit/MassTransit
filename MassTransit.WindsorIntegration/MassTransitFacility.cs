namespace MassTransit.WindsorIntegration
{
    using System;
    using Castle.Core.Configuration;
    using Castle.MicroKernel.Facilities;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using MassTransit.ServiceBus.HealthMonitoring;
    using MassTransit.ServiceBus.Internal;
    using MassTransit.ServiceBus.Subscriptions;
    using ServiceBus;

    public class MassTransitFacility :
        AbstractFacility
    {
        private IWindsorContainer _container;

        protected override void Init()
        {
            string _listenUri = this.FacilityConfig.Attributes["listenAt"];
            string _subscriptionUri = this.FacilityConfig.Attributes["subscriptionsAt"];
            ConfigurationCollection _transports = this.FacilityConfig.Children["transports"].Children;


            _container = this.Kernel.Resolve<IWindsorContainer>();


            foreach (IConfiguration transport in _transports)
            {
                Type t = Type.GetType(transport.Value, true, true);
                _container.AddComponent("transport." + t.Name, typeof (IEndpoint), t);
            }

            _container.Register(
                Component.For<ISubscriptionCache>().ImplementedBy<LocalSubscriptionCache>(),
                Component.For<IObjectBuilder>().ImplementedBy<WindsorObjectBuilder>(),
                Component.For<IServiceBus>().ImplementedBy<ServiceBus>()
                    .Parameters(Parameter.ForKey("endpointToListenOn").Eq(_listenUri)),
                Component.For<IEndpointResolver>().ImplementedBy<EndpointResolver>(),
                AddStartable<HealthClient>(),
                AddStartable<SubscriptionClient>()
                    .Parameters(Parameter.ForKey("subscriptionServiceEndpoint").Eq(_subscriptionUri))
                );
        }

        private static ComponentRegistration<T> AddStartable<T>()
        {
            return Component.For<T>()
                .AddAttributeDescriptor("startable", "true")
                .AddAttributeDescriptor("startMethod", "Start")
                .AddAttributeDescriptor("stopMethod", "Stop");
        }
    }
}