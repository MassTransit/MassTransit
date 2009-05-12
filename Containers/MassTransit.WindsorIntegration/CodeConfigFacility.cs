namespace MassTransit.WindsorIntegration
{
    using System;
    using Castle.MicroKernel.Facilities;
    using Castle.MicroKernel.Registration;
    using Configuration;
    using Infrastructure.Subscriptions;
    using Services.HealthMonitoring;
    using Services.Subscriptions.Client;

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
			//Component.For<IEndpoint>().Instance(ep).Named("lep"),
		    Kernel.Register(
                Component.For<IServiceBus>().ImplementedBy<ServiceBus>().Parameters(
                    Parameter.ForKey("endpointListenOn").Eq("${lep}"), //how to handle this?
                    Parameter.ForKey("MinimumConsumerThreads").Eq("1"),
                    Parameter.ForKey("MaximumConsumerThreads").Eq("10"))
                    .Named(id)
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
                               .Parameters(Parameter.ForKey("bus").Eq("${" + busId + "}"))
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
                .Parameters(Parameter.ForKey("serviceBus").Eq("${" + busId + "}"))
                //how to mark startable
		        );				
		}
    }
}