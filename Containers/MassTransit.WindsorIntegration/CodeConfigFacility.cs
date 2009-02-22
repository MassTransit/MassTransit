namespace MassTransit.WindsorIntegration
{
    using System;
    using Castle.MicroKernel.Facilities;
    using Castle.MicroKernel.Registration;
    using Configuration;
    using Infrastructure.Subscriptions;
    using Services.HealthMonitoring;
    using Services.Subscriptions;
    using Subscriptions;

    public class CodeConfigFacility :
        AbstractFacility
    {
        protected override void Init()
        {
            //no-op
        }

        public CodeConfigFacility()
		{
            Kernel.Register(
                Component.For<IObjectBuilder>().ImplementedBy<WindsorObjectBuilder>().LifeStyle.Singleton,
                Component.For<ISubscriptionCache>().ImplementedBy<LocalSubscriptionCache>().LifeStyle.Singleton,
                Component.For<IServiceBus>().ImplementedBy<ServiceBus>()
                );
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
					x.SetObjectBuilder(Kernel.Resolve<IObjectBuilder>());
					foreach (var transportType in transportTypes)
					{
						x.RegisterTransport(transportType);
					}
				});
		    Kernel.Register(
                Component.For<IEndpointFactory>().Instance(endpointFactory).Named("endpointFactory").LifeStyle.Singleton
                );
		}

		//at least one of these
		public void AddBus(string id, Uri endpointToListenOn)
		{
			IEndpoint ep = Kernel.Resolve<IEndpointFactory>().GetEndpoint(endpointToListenOn);
		    Kernel.Register(
                Component.For<IServiceBus>().ImplementedBy<ServiceBus>().Parameters(
                    Parameter.ForKey("endpointListeOn").Eq("ep"), //how to handle this?
                    Parameter.ForKey("MinimumConsumerThreads").Eq("1"),
                    Parameter.ForKey("MaximumConsumerThreads").Eq("10"))
                    .Named(id)
		        );
		}


		//TODO: this is an either/or choice
		public void UseALocalSubscriptionCache()
		{
		    Kernel.Register(
                Component.For<ISubscriptionCache>().ImplementedBy<LocalSubscriptionCache>()
		        );
		}

		public void UseADistributedSubscriptionCache(string[] servers)
		{
		    Kernel.Register(
		        Component.For<ISubscriptionCache>().ImplementedBy<DistributedSubscriptionCache>()
		            .DependsOn(new {
                                        Servers = servers
		                           })
		        );
		}


		//optional
		public void TurnOnHealthClient(string busId, int heartbeatInterval)
		{
		    Kernel.Register(
                Component.For<HealthClient>().Named("health_client")
                .DependsOn(new {
                                   HeartbeatInterval=heartbeatInterval
		                       })
                //how to mark specific dependency: bus
                //how to make startable
                );
		}

		//optional
		public void TurnOnSubscriptionClient(string busId, Uri subscribedVia)
		{
			IEndpoint ep = Kernel.Resolve<IEndpointFactory>().GetEndpoint(subscribedVia);
		    Kernel.Register(
                Component.For<SubscriptionClient>().Named("subscription_client")
                .DependsOn(new { subscriptionServiceEndpoint = ep}
                )
                //how to mark a specific bus
                //how to mark startable
		        );				
		}
    }
}