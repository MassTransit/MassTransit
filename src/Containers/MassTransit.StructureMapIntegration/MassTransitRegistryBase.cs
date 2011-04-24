// Copyright 2007-2011 The Apache Software Foundation.
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
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using System.Reflection;
	using Configuration;
	using Configurators;
	using EndpointConfigurators;
	using Internal;
	using Saga;
	using Services.Subscriptions;
	using Services.Subscriptions.Configuration;
	using Services.Subscriptions.Server;
	using StructureMap;
	using StructureMap.Configuration.DSL;
	using StructureMap.Graph;
	using Transports;

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
			: this(x => { })
		{
		}

		/// <summary>
		/// Scan the executing assemblies current directory for assemblies containing MassTransit transports.
		/// </summary>
		/// <param name="configurationAction"></param>
		public MassTransitRegistryBase(Action<EndpointFactoryConfigurator> configurationAction)
			: this(configurationAction, DefaultTransportScanner)
		{
		}

		/// <summary>
		/// Overload to allow explicit assembly scanning specification.
		/// 
		/// EX: since Assembly.GetExecutingAssembly().Location could return null if the Assembly.GetExecutingAssembly() was dynamically loaded from a resource, this overload allows for explicit assemblies scanning strategies.
		/// </summary>
		/// <param name="configurationAction"></param>
		/// <param name="transportAssemblyScanner"></param>
		public MassTransitRegistryBase(Action<EndpointFactoryConfigurator> configurationAction, Action<IAssemblyScanner> transportAssemblyScanner)
		{
			RegisterBusDependencies();

			var transportFactoryConvention = new TransportFactoryConvention();
			Scan(scanner =>
				{
					transportAssemblyScanner(scanner);
					scanner.With(transportFactoryConvention);
				});

			RegisterEndpointFactory(x =>
				{
					foreach (Type transportType in transportFactoryConvention)
					{
						x.AddTransportFactory(transportType);
					}

					configurationAction(x);
				});
		}

		/// <summary>
		/// Creates a registry for a service bus listening to an endpoint
		/// </summary>
		public MassTransitRegistryBase(params Type[] transportTypes)
		{
			RegisterBusDependencies();

			RegisterEndpointFactory(x =>
				{
					x.AddTransportFactory<LoopbackTransportFactory>();
					x.AddTransportFactory<MulticastUdpTransportFactory>();

					foreach (Type type in transportTypes)
					{
						x.AddTransportFactory(type);
					}
				});
		}


		/// <summary>
		/// Registers the in-memory subscription service so that all buses created in the same
		/// process share subscriptions
		/// </summary>
		protected void RegisterInMemorySubscriptionService()
		{
			For<IEndpointSubscriptionEvent>()
				.Singleton()
				.Use<LocalSubscriptionService>();

			For<SubscriptionPublisher>()
				.Use<SubscriptionPublisher>();

			For<SubscriptionConsumer>()
				.Use<SubscriptionConsumer>();
		}

		protected void RegisterInMemorySagaRepository()
		{
			For(typeof (ISagaRepository<>))
				.Singleton()
				.Use(typeof (InMemorySagaRepository<>));
		}

		/// <summary>
		/// Registers the types used by the service bus internally and as part of the container.
		/// These are typically items that are not swapped based on the container implementation
		/// </summary>
		protected void RegisterBusDependencies()
		{
			For<IObjectBuilder>()
				.Singleton()
				.Use<StructureMapObjectBuilder>();

			//we are expecting SM to auto-resolve
			// SubscriptionClient
			// InitiateSagaMessageSink<,>
			// OrchestrateSagaMessageSink<,>)
			// InitiateSagaStateMachineSink<,>)
			// OrchestrateSagaStateMachineSink<,>)
		}

		protected void RegisterEndpointFactory(Action<EndpointFactoryConfigurator> configAction)
		{
			For<IEndpointCache>()
				.Singleton()
				.Use(context =>
					{
						return EndpointResolverConfiguratorImpl.New(x =>
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
			For<IServiceBus>()
				.Singleton()
				.Use(context =>
					{
						return Configuration.ServiceBusConfigurator.New(x =>
							{
								x.SetEndpointFactory(context.GetInstance<IEndpointCache>());
								x.SetObjectBuilder(context.GetInstance<IObjectBuilder>());
								x.ReceiveFrom(endpointUri);

								configAction(x);
							});
					});
		}

		protected void RegisterServiceBus(string endpointUri, Action<IServiceBusConfigurator, IContext> configAction)
		{
			RegisterServiceBus(new Uri(endpointUri), configAction);
		}

		protected void RegisterServiceBus(Uri endpointUri, Action<IServiceBusConfigurator, IContext> configAction)
		{
			For<IServiceBus>()
				.Singleton()
				.Use(context =>
					{
						return Configuration.ServiceBusConfigurator.New(x =>
							{
								x.SetEndpointFactory(context.GetInstance<IEndpointCache>());
								x.SetObjectBuilder(context.GetInstance<IObjectBuilder>());
								x.ReceiveFrom(endpointUri);

								configAction(x, context);
							});
					});
		}

		protected void RegisterControlBus(string endpointUri, Action<IServiceBusConfigurator> configAction)
		{
			RegisterControlBus(new Uri(endpointUri), configAction);
		}

		protected void RegisterControlBus(Uri endpointUri, Action<IServiceBusConfigurator> configAction)
		{
			For<IControlBus>()
				.Singleton()
				.Use(context =>
					{
						return Configuration.ControlBusConfigurator.New(x =>
							{
								x.SetEndpointFactory(context.GetInstance<IEndpointCache>());
								x.SetObjectBuilder(context.GetInstance<IObjectBuilder>());
								x.ReceiveFrom(endpointUri);
								x.SetConcurrentConsumerLimit(1);

								configAction(x);
							});
					});
		}

		private class TransportFactoryConvention :
			IRegistrationConvention,
			IEnumerable<Type>
		{
			private readonly IList<Type> _transportTypes;

			public TransportFactoryConvention()
			{
				_transportTypes = new List<Type>();
			}

			public IEnumerator<Type> GetEnumerator()
			{
				return _transportTypes.GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			public void Process(Type type, Registry registry)
			{
				if (typeof (ITransportFactory).IsAssignableFrom(type))
				{
					_transportTypes.Add(type);
				}
			}
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

		private static void DefaultTransportScanner(IAssemblyScanner scanner)
		{
			string assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

			scanner.AssembliesFromPath(assemblyPath, assembly =>
			                                         assembly.GetName().Name.StartsWith("MassTransit.Transports."));
		}
	}
}