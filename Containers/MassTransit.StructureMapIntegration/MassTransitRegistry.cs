namespace MassTransit.StructureMapIntegration
{
    using MassTransit.ServiceBus.Internal;
    using MassTransit.ServiceBus.Subscriptions;
    using ServiceBus;
    using ServiceBus.Services.HealthMonitoring;
    using StructureMap.Configuration.DSL;

    public class MassTransitRegistry : Registry
    {
        protected override void configure()
        {
            ForRequestedType<IObjectBuilder>().AddConcreteType<StructureMapObjectBuilder>().AsSingletons();
            ForRequestedType<ISubscriptionCache>().AddConcreteType<LocalSubscriptionCache>().AsSingletons();
            ForRequestedType<IEndpointResolver>().AddConcreteType<EndpointResolver>().AsSingletons();

            CreateProfile("distributed")
                .For<ISubscriptionCache>().UseConcreteType<DistributedSubscriptionCache>();

            CreateProfile("local")
                .For<ISubscriptionCache>().UseConcreteType<LocalSubscriptionCache>();


            ForRequestedType<IServiceBus>().AddInstances(o =>
                                                             {
                                                                 o.OfConcreteType<ServiceBus>().WithName("data-bus")
                                                                     .WithCtorArg("endpointListenOn").EqualToAppSetting("listenOn")
                                                                     .SetProperty(x=>x.MinThreadCount = 1)
                                                                     .SetProperty(x=>x.MaxThreadCount = 10);

                                                                 o.OfConcreteType<ServiceBus>().WithName("control-bus")
                                                                     .WithCtorArg("endpointToListenOn").EqualToAppSetting("controlledOn")
                                                                     .SetProperty(x=>x.MinThreadCount = 1)
                                                                     .SetProperty(x=>x.MaxThreadCount = 10);
                                                             });

            ForConcreteType<HealthClient>().Configure.WithName("health_client")
                .CtorDependency<IServiceBus>().IsTheDefault().WithName("data-bus")
                .WithCtorArg("heartbeatInterval").EqualTo(3)
                .OnCreation(o=>o.Start());

            ForConcreteType<SubscriptionClient>().Configure
                .CtorDependency<IServiceBus>().IsTheDefault().WithName("data-bus")
                .WithCtorArg("subscriptionServiceEndpoint").EqualToAppSetting("subscribedTo")
                .OnCreation(o => o.Start());
        }
    }
}
