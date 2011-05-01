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
namespace MassTransit.NinjectIntegration
{
	using System;
	using BusConfigurators;
	using EndpointConfigurators;
	using Ninject;
	using Ninject.Modules;
	using Saga;

	/// <summary>
	/// This is an extension of the Ninject Module exposing methods to make it easy to get Mass
	/// Transit set up.
	/// </summary>
	public class MassTransitModuleBase :
		NinjectModule
	{
		private readonly Action<EndpointFactoryConfigurator> _configurationAction;
		private readonly Type[] _transportTypes;

		public MassTransitModuleBase()
			: this((Action<EndpointFactoryConfigurator>) null, (Type[]) null)
		{
		}

		public MassTransitModuleBase(Action<EndpointFactoryConfigurator> configurationAction)
			: this(configurationAction, null)
		{
		}

		/// <summary>
		/// Creates a registry for a service bus listening to an endpoint
		/// </summary>
		public MassTransitModuleBase(params Type[] transportTypes)
			: this((Action<EndpointFactoryConfigurator>) null, transportTypes)
		{
		}

		/// <summary>
		/// Creates a new instance of MassTransitModuleBase.
		/// </summary>
		/// <remarks>
		/// With NInject, its not possible to actually build anything yet,
		/// because the underlying binding mechanism does not exist at
		/// construction time. We will save the values and do the setup later
		/// when Load() is called.
		/// </remarks>
		/// <param name="configurationAction">
		/// The endpoint factory configuration action to use when creating an
		/// endpoint factory.
		/// </param>
		/// <param name="transportTypes">
		/// The transport types to configure.
		/// </param>
		public MassTransitModuleBase(Action<EndpointFactoryConfigurator> configurationAction,
		                             params Type[] transportTypes)
		{
			if (configurationAction == null)
			{
				_configurationAction = DefaultEndpointResolverConfigurator;
			}
			else
			{
				_configurationAction = configurationAction;
			}

			_transportTypes = transportTypes;
		}

		public override void Load()
		{
			RegisterBusDependencies();
		}

		protected void DefaultEndpointResolverConfigurator(EndpointFactoryConfigurator endpointFactoryConfigurator)
		{
			foreach (Type type in _transportTypes)
			{
				endpointFactoryConfigurator.AddTransportFactory(type);
			}
		}

		protected void RegisterInMemorySagaRepository()
		{
			Bind(typeof (ISagaRepository<>))
				.To(typeof (InMemorySagaRepository<>))
				.InSingletonScope();
		}

		/// <summary>
		/// Registers the types used by the service bus internally and as part of the container.
		/// These are typically items that are not swapped based on the container implementation
		/// </summary>
		protected void RegisterBusDependencies()
		{

			//we are expecting NINJECT to auto-resolve
			// SubscriptionClient
			// InitiateSagaMessageSink<,>
			// OrchestrateSagaMessageSink<,>)
			// InitiateSagaStateMachineSink<,>)
			// OrchestrateSagaStateMachineSink<,>)
		}

		protected void RegisterServiceBus(string endpointUri, Action<ServiceBusConfigurator> configAction)
		{
			RegisterServiceBus(new Uri(endpointUri), configAction);
		}

		protected void RegisterServiceBus(Uri endpointUri, Action<ServiceBusConfigurator> configAction)
		{
			Bind<IServiceBus>()
				.ToMethod(context =>
					{
						return ServiceBusFactory.New(x =>
							{
								if (_configurationAction != null)
									_configurationAction(x);

								x.ReceiveFrom(endpointUri);

								x.UseControlBus();

								configAction(x);
							});
					})
				.InSingletonScope();
		}

		protected void RegisterControlBus(string endpointUri, Action<ServiceBusConfigurator> configAction)
		{
			RegisterControlBus(new Uri(endpointUri), configAction);
		}

		protected void RegisterControlBus(Uri endpointUri, Action<ServiceBusConfigurator> configAction)
		{
			Bind<IControlBus>()
				.ToMethod(context =>
					{return (IControlBus)context.Kernel.Get<IServiceBus>().ControlBus;
					}).InSingletonScope();
		}
	}
}
