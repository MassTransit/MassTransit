namespace MassTransit.StructureMapIntegration
{
    using System;
    using MassTransit.Internal;
    using MassTransit.Subscriptions;
    
    using Services.HealthMonitoring;
    using StructureMap;
    using StructureMap.Configuration.DSL;

    /// <summary>
    /// This is an extension of the StrutureMap registry exposing methods to make it easy to get Mass
    /// Transit set up.
    /// </summary>
    public class MassTransitRegistry : Registry
    {
        public MassTransitRegistry()
        {
            ForRequestedType<IObjectBuilder>().AddConcreteType<StructureMapObjectBuilder>().AsSingletons();
            ForRequestedType<ISubscriptionCache>().AddConcreteType<LocalSubscriptionCache>().AsSingletons();
            ForRequestedType<IEndpointResolver>().AddConcreteType<EndpointResolver>().AsSingletons();
            ForRequestedType<IServiceBus>().AddConcreteType<ServiceBus>();
        }

        //this at least once
        public void AddTransport<T>() where T : IEndpoint
        {
            EndpointResolver.AddTransport(typeof (T));
        }

        //at least one of these
        public void AddBus(string id, Uri endpointToListenOn)
        {
            IEndpoint ep = ObjectFactory.GetInstance<IEndpointResolver>().Resolve(endpointToListenOn);
            ForRequestedType<IServiceBus>().AddInstances(o => o.OfConcreteType<ServiceBus>().WithName(id)
                                                                  .WithCtorArg("endpointListenOn").EqualTo(ep)
                                                                  .SetProperty(x => x.MinThreadCount = 1)
                                                                  .SetProperty(x => x.MaxThreadCount = 10));
        }


        //TODO: this is an either/or choice
        public void UseALocalSubscriptionCache()
        {
            ForRequestedType<ISubscriptionCache>().AddConcreteType<LocalSubscriptionCache>();
        }
        public void UseADistributedSubscriptionCache(string[] servers)
        {
            ForRequestedType<ISubscriptionCache>().AddConcreteType<DistributedSubscriptionCache>();
            ForConcreteType<DistributedSubscriptionCache>().Configure.WithCtorArg("servers").EqualTo(servers);
        }


        //optional
        public void TurnOnHealthClient(string busId, int heartbeatInterval)
        {
            ForConcreteType<HealthClient>().Configure
                .WithName("health_client")
                .CtorDependency<IServiceBus>().IsTheDefault().WithName(busId)
                .WithCtorArg("heartbeatInterval").EqualTo(heartbeatInterval)
                .OnCreation(o => o.Start());
        }
        //optional
        public void TurnOnSubscriptionClient(string busId, Uri subscribedVia)
        {
            IEndpoint ep = ObjectFactory.GetInstance<IEndpointResolver>().Resolve(subscribedVia);
            ForConcreteType<SubscriptionClient>().Configure
                .WithName("subscription_client")
                .CtorDependency<IServiceBus>().IsTheDefault().WithName(busId)
                .WithCtorArg("subscriptionServiceEndpoint").EqualTo(ep)
                .OnCreation(o => o.Start());
        }
    }
}
