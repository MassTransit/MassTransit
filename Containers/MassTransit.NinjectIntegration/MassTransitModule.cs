using MassTransit.Internal;

namespace MassTransit.NinjectIntegration
{
    using System;
    using Infrastructure.Subscriptions;
    using Ninject;
    using Ninject.Modules;
    using Services.HealthMonitoring;
    using Services.Subscriptions;
    using Subscriptions;
    using Configuration;

    /// <summary>
    /// This is an extension of the Ninject Module exposing methods to make it easy to get Mass
    /// Transit set up.
    /// </summary>
    public class MassTransitModule :
        Module
    {
        // @cbioley :
        // I moved the ctor implementation in the Load method.
        public override void Load()
        {
            Bind<IObjectBuilder>()
                .To<NinjectObjectBuilder>()

            Bind<IServiceBus>()
                .To<ServiceBus>();
            
            Bind<ITypeInfoCache>()
                .To<TypeInfoCache>()
                .InSingletonScope();
        }

        //this at least once
        public void AddTransport<TTransport>() where TTransport : IEndpoint
        {
            AddTransports(typeof(TTransport));
        }

        public void AddTransports(params Type[] transportTypes)
        {
            var endpointFactory = EndpointFactoryConfigurator.New(x =>
            {
                x.SetObjectBuilder(Kernel.Get<IObjectBuilder>());
                foreach (var transportType in transportTypes)
                {
                    x.RegisterTransport(transportType);
                }
            });
            Bind<IEndpointFactory>()
                .ToConstant(endpointFactory)
                .InSingletonScope()
                .Named("endpointFactory");
        }

        //at least one of these
        public void AddBus(string id, Uri endpointToListenOn)
        {
            IEndpoint ep = Kernel.Get<IEndpointFactory>().GetEndpoint(endpointToListenOn);

            Bind<IServiceBus>()
                .To<ServiceBus>()
                .InSingletonScope()
                .Named(id)
                .WithConstructorArgument("endpointToListenOn", ep)
                .WithPropertyValue("MinimumConsumerThreads", 1)
                .WithPropertyValue("MaximumConsumerThreads", 10);
        }

        //TODO: this is an either/or choice
        public void UseALocalSubscriptionCache()
        {
            Bind<ISubscriptionCache>().To<LocalSubscriptionCache>();
        }

        public void UseADistributedSubscriptionCache(string[] servers)
        {
            Bind<ISubscriptionCache>()
                .To<DistributedSubscriptionCache>()
                .WithConstructorArgument("servers", servers);
        }

        //optional
        public void TurnOnHealthClient(string busId, int heartbeatInterval)
        {
            Bind<HealthClient>()
                .ToSelf()
                .InSingletonScope()
                .Named("health_client")
                .WithConstructorArgument("heartbeatInterval", heartbeatInterval)
                .WithConstructorArgument("bus", Kernel.Get<IServiceBus>(busId))
                .OnActivation(x=>x.Start());
        }

        //optional
        public void TurnOnSubscriptionClient(string busId, Uri subscribedVia)
        {
            IEndpoint ep = Kernel.Get<IEndpointFactory>().GetEndpoint(subscribedVia);

            Bind<SubscriptionClient>()
                .ToSelf()
                .InSingletonScope()
                .Named("subscription_client")
                .WithConstructorArgument("subscriptionServiceEndpoint", ep)
                .WithConstructorArgument("serviceBus", Kernel.Get<IServiceBus>(busId))
                .OnActivation(x => x.Start());
        }
    }
}
