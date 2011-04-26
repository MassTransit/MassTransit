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
	using System.Linq;
	using System.Reflection;
	using BusConfigurators;
	using EndpointConfigurators;
	using Saga;
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
		private Type[] _transports;
		private TransportFactoryConvention _transportFactoryConvention;

		/// <summary>
		/// Default constructor with not actual registration
		/// </summary>
		public MassTransitRegistryBase()
			: this(DefaultTransportScanner)
		{
		}

		/// <summary>
		/// Overload to allow explicit assembly scanning specification.
		/// 
		/// EX: since Assembly.GetExecutingAssembly().Location could return null if the Assembly.GetExecutingAssembly() was dynamically loaded from a resource, this overload allows for explicit assemblies scanning strategies.
		/// </summary>
		/// <param name="configurationAction"></param>
		/// <param name="transportAssemblyScanner"></param>
		public MassTransitRegistryBase(Action<IAssemblyScanner> transportAssemblyScanner)
		{
			RegisterBusDependencies();

			_transportFactoryConvention = new TransportFactoryConvention();
			Scan(scanner =>
				{
					scanner.With(_transportFactoryConvention);
					transportAssemblyScanner(scanner);
				});
		}

		/// <summary>
		/// Creates a registry for a service bus listening to an endpoint
		/// </summary>
		public MassTransitRegistryBase(params Type[] transportTypes)
		{
			RegisterBusDependencies();

			_transports = transportTypes;
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
		}

		/// <summary>
		/// This is not needed really
		/// </summary>
		/// <param name="configAction"></param>
		protected void RegisterEndpointFactory(Action<EndpointFactoryConfigurator> configAction)
		{
			For<IEndpointCache>()
				.Singleton()
				.Use(context =>
					{
						return EndpointCacheFactory.New(x =>
							{
								configAction(x);
							});
					});
		}

		protected void RegisterServiceBus(string endpointUri, Action<ServiceBusConfigurator> configAction)
		{
			RegisterServiceBus(new Uri(endpointUri), configAction);
		}

		protected void RegisterServiceBus(Uri endpointUri, Action<ServiceBusConfigurator> configAction)
		{
			For<IServiceBus>()
				.Singleton()
				.Use(context =>
					{
						return ServiceBusFactory.New(x =>
							{
								if (_transports != null)
									_transports.Each(transport => x.AddTransportFactory(transport));

								if (_transportFactoryConvention != null)
									_transportFactoryConvention.Each(transport => x.AddTransportFactory(transport));

								x.SetObjectBuilder(context.GetInstance<IObjectBuilder>());
								x.ReceiveFrom(endpointUri);

								configAction(x);
							});
					});
		}

		protected void RegisterServiceBus(string endpointUri, Action<ServiceBusConfigurator, IContext> configAction)
		{
			RegisterServiceBus(new Uri(endpointUri), configAction);
		}

		protected void RegisterServiceBus(Uri endpointUri, Action<ServiceBusConfigurator, IContext> configAction)
		{
			For<IServiceBus>()
				.Singleton()
				.Use(context =>
					{
						return ServiceBusFactory.New(x =>
							{
								if (_transports != null)
									_transports.Each(transport => x.AddTransportFactory(transport));

								if (_transportFactoryConvention != null)
									_transportFactoryConvention.Each(transport => x.AddTransportFactory(transport));

								x.SetObjectBuilder(context.GetInstance<IObjectBuilder>());
								x.ReceiveFrom(endpointUri);

								configAction(x, context);
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

		private static void DefaultTransportScanner(IAssemblyScanner scanner)
		{
			string assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

			scanner.AssembliesFromPath(assemblyPath, assembly =>
			                                         assembly.GetName().Name.StartsWith("MassTransit.Transports."));
		}
	}
}