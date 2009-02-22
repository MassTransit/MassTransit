namespace MassTransit.StructureMapIntegration
{
	using System;
	using Configuration;
	using Infrastructure.Subscriptions;
	using Services.HealthMonitoring;
	using Services.Subscriptions;
	using StructureMap;
	using StructureMap.Configuration.DSL;
	using Subscriptions;

	/// <summary>
	/// This is an extension of the StrutureMap registry exposing methods to make it easy to get Mass
	/// Transit set up.
	/// </summary>
	public class MassTransitRegistry :
        Registry
	{
		public MassTransitRegistry()
		{
			ForRequestedType<IObjectBuilder>().AddConcreteType<StructureMapObjectBuilder>().AsSingletons();
			ForRequestedType<ISubscriptionCache>().AddConcreteType<LocalSubscriptionCache>().AsSingletons();
			ForRequestedType<IServiceBus>().AddConcreteType<ServiceBus>();
		}

		//this at least once
		public void AddTransport<TTransport>() where TTransport : IEndpoint
		{
			AddTransports(typeof (TTransport));
		}

		public void AddTransports(params Type[] transportTypes)
		{
			var endpointFactory = EndpointFactoryConfigurator.New(x =>
				{
					x.SetObjectBuilder(ObjectFactory.GetInstance<IObjectBuilder>());
					foreach (var transportType in transportTypes)
					{
						x.RegisterTransport(transportType);
					}
				});
			ForRequestedType<IEndpointFactory>().AddInstances(o => o.IsThis(endpointFactory).WithName("endpointFactory")).AsSingletons();
		}

		//at least one of these
		public void AddBus(string id, Uri endpointToListenOn)
		{
			IEndpoint ep = ObjectFactory.GetInstance<IEndpointFactory>().GetEndpoint(endpointToListenOn);
			ForRequestedType<IServiceBus>().AddInstances(o => o.OfConcreteType<ServiceBus>().WithName(id)
                                                            .WithCtorArg("endpointToListenOn").EqualTo(ep)
			                                             	.SetProperty(x => x.MinimumConsumerThreads = 1)
			                                             	.SetProperty(x => x.MaximumConsumerThreads = 10));
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
			IEndpoint ep = ObjectFactory.GetInstance<IEndpointFactory>().GetEndpoint(subscribedVia);
			ForConcreteType<SubscriptionClient>().Configure
				.WithName("subscription_client")
				.CtorDependency<IServiceBus>().IsTheDefault().WithName(busId)
				.WithCtorArg("subscriptionServiceEndpoint").EqualTo(ep)
				.OnCreation(o => o.Start());
		}
	}
}