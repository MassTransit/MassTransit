namespace MassTransit.WindsorIntegration
{
    using System;
    using System.Collections;
    using Castle.Core.Configuration;
    using Castle.MicroKernel;
    using Castle.MicroKernel.Facilities;
    using Castle.MicroKernel.Registration;
    using MassTransit.ServiceBus.HealthMonitoring;
    using MassTransit.ServiceBus.Internal;
    using MassTransit.ServiceBus.Subscriptions;
    using ServiceBus;

    public class MassTransitFacility :
        AbstractFacility
    {
        protected override void Init()
        {
            string _listenUri = this.FacilityConfig.Attributes["listenAt"];
            string _subscriptionUri = this.FacilityConfig.Attributes["subscriptionsAt"];
            ConfigurationCollection _transports = this.FacilityConfig.Children["transports"].Children;

            foreach (IConfiguration transport in _transports)
            {
                Type t = Type.GetType(transport.Value, true, true);
                this.Kernel.AddComponent("transport." + t.Name, typeof (IEndpoint), t);
            }

            this.Kernel.AddComponentInstance("kernel", typeof(IKernel), this.Kernel);
            this.Kernel.Register(
                Component.For<ISubscriptionCache>().ImplementedBy<LocalSubscriptionCache>(),
                Component.For<IObjectBuilder>().ImplementedBy<WindsorObjectBuilder>(),
                Component.For<IEndpointResolver>().ImplementedBy<EndpointResolver>().Named("endpoint.factory"),
                Component.For<IEndpoint>()
                    .AddAttributeDescriptor("factoryId", "endpoint.factory")
                    .AddAttributeDescriptor("factoryCreate", "Resolve"),
                AddStartable<HealthClient>()
                );


            //TODO: Hack
            IDictionary args = new Hashtable();
            args.Add("uri", new Uri(_listenUri));

            IServiceBus bus = new ServiceBus(this.Kernel.Resolve<IEndpoint>(args),
                                             this.Kernel.Resolve<IObjectBuilder>(),
                                             this.Kernel.Resolve<ISubscriptionCache>(),
                                             this.Kernel.Resolve<IEndpointResolver>());

            this.Kernel.AddComponentInstance("servicebus", typeof(IServiceBus), bus);


            //TODO: Hack
            IDictionary args2 = new Hashtable();
            args2.Add("uri", new Uri(_subscriptionUri));

            SubscriptionClient sc = new SubscriptionClient(this.Kernel.Resolve<IServiceBus>(),
                this.Kernel.Resolve<ISubscriptionCache>(),
                this.Kernel.Resolve<IEndpoint>(args2));
            this.Kernel.AddComponentInstance("subscription.client", sc);
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