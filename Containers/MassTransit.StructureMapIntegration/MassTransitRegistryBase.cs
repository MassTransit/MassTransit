// Copyright 2007-2008 The Apache Software Foundation.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.StructureMapIntegration
{
	using System;
	using Configuration;
	using Internal;
	using Saga;
	using Services.Subscriptions;
	using Services.Subscriptions.Configuration;
	using Services.Subscriptions.Server;
	using StructureMap.Attributes;
	using StructureMap.Configuration.DSL;

	/// <summary>
	/// This is an extension of the StrutureMap registry exposing methods to make it easy to get Mass
	/// Transit set up.
	/// </summary>
	public class MassTransitRegistryBase :
		Registry
	{
		/// <summary>
		/// Default constructor with not actual registration
		/// </summary>
		public MassTransitRegistryBase()
		{
		}

		/// <summary>
		/// Creates a registry for a service bus listening to an endpoint
		/// </summary>
		public MassTransitRegistryBase(params Type[] transportTypes)
		{
			RegisterBusDependencies();

			RegisterEndpointFactory(x =>
				{
					foreach (Type type in transportTypes)
					{
						x.RegisterTransport(type);
					}
				});
		}


		/// <summary>
		/// Registers the in-memory subscription service so that all buses created in the same
		/// process share subscriptions
		/// </summary>
		protected void RegisterInMemorySubscriptionService()
		{
			ForRequestedType<IEndpointSubscriptionEvent>()
				.CacheBy(InstanceScope.Singleton)
				.AddInstances(o => o.OfConcreteType<LocalSubscriptionService>());

			ForRequestedType<SubscriptionPublisher>()
				.TheDefault.Is.OfConcreteType<SubscriptionPublisher>();
			ForRequestedType<SubscriptionConsumer>()
				.TheDefault.Is.OfConcreteType<SubscriptionConsumer>();
		}

		protected void RegisterInMemorySubscriptionRepository()
		{
			ForRequestedType<ISubscriptionRepository>()
				.CacheBy(InstanceScope.Singleton)
				.TheDefault.Is.OfConcreteType<InMemorySubscriptionRepository>();
		}

		protected void RegisterInMemorySagaRepository()
		{
			ForRequestedType(typeof (ISagaRepository<>))
				.CacheBy(InstanceScope.Singleton)
				.AddConcreteType(typeof (InMemorySagaRepository<>));
		}

		/// <summary>
		/// Registers the types used by the service bus internally and as part of the container.
		/// These are typically items that are not swapped based on the container implementation
		/// </summary>
		protected void RegisterBusDependencies()
		{
			ForRequestedType<IObjectBuilder>()
				.TheDefaultIsConcreteType<StructureMapObjectBuilder>()
				.CacheBy(InstanceScope.Singleton);

            //we are expecting SM to auto-resolve
            // SubscriptionClient
            // InitiateSagaMessageSink<,>
            // OrchestrateSagaMessageSink<,>)
            // InitiateSagaStateMachineSink<,>)
            // OrchestrateSagaStateMachineSink<,>)
		}

		protected void RegisterEndpointFactory(Action<IEndpointFactoryConfigurator> configAction)
		{
			ForRequestedType<IEndpointFactory>()
				.CacheBy(InstanceScope.Singleton)
				.TheDefault.Is.ConstructedBy(context =>
					{
						return EndpointFactoryConfigurator.New(x =>
							{
								x.SetObjectBuilder(context.GetInstance<IObjectBuilder>());
								configAction(x);
							});
					});
		}

		protected void RegisterServiceBus(string endpointUri, Action<IServiceBusConfigurator> configAction)
		{
			RegisterServiceBus(new Uri(endpointUri), configAction);
		}

		protected void RegisterServiceBus(Uri endpointUri, Action<IServiceBusConfigurator> configAction)
		{
			ForRequestedType<IServiceBus>()
				.CacheBy(InstanceScope.Singleton)
				.TheDefault.Is.ConstructedBy(context =>
					{
						return ServiceBusConfigurator.New(x =>
							{
								x.SetObjectBuilder(context.GetInstance<IObjectBuilder>());
								x.ReceiveFrom(endpointUri);

								configAction(x);
							});
					});
		}

		protected void RegisterControlBus(string endpointUri, Action<IServiceBusConfigurator> configAction)
		{
			RegisterControlBus(new Uri(endpointUri), configAction);
		}

		protected void RegisterControlBus(Uri endpointUri, Action<IServiceBusConfigurator> configAction)
		{
			ForRequestedType<IControlBus>()
				.CacheBy(InstanceScope.Singleton)
				.TheDefault.Is.ConstructedBy(context =>
				{
					return ControlBusConfigurator.New(x =>
					{
						x.SetObjectBuilder(context.GetInstance<IObjectBuilder>());
						x.ReceiveFrom(endpointUri);

						configAction(x);
					});
				});
		}

		protected static void ConfigureSubscriptionClient(string subscriptionServiceEndpointAddress, IServiceBusConfigurator configurator)
		{
			ConfigureSubscriptionClient(new Uri(subscriptionServiceEndpointAddress), configurator);
		}

		protected static void ConfigureSubscriptionClient(Uri subscriptionServiceEndpointAddress, IServiceBusConfigurator configurator)
		{
			configurator.ConfigureService<SubscriptionClientConfigurator>(y =>
				{
					// this is fairly easy inline, but wanted to include the example for completeness
					y.SetSubscriptionServiceEndpoint(subscriptionServiceEndpointAddress);
				});
		}
	}
}