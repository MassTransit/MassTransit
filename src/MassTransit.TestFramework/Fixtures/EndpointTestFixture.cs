// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.TestFramework.Fixtures
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using BusConfigurators;
	using Configurators;
	using EndpointConfigurators;
	using Exceptions;
	using Magnum.Extensions;
	using MassTransit.Transports;
	using NUnit.Framework;
	using Saga;
	using Services.Subscriptions;

	/// <summary>
	/// Test fixture that tests a single endpoint, given
	/// a transport factory. The transport factory needs to
	/// have a default public c'tor. The endpoint is one-to-one
	/// with the bus in this test fixture.
	/// </summary>
	/// <typeparam name="TTransportFactory">Type of transport factory to create the endpoint with</typeparam>
	[TestFixture]
	public class EndpointTestFixture<TTransportFactory> :
		AbstractTestFixture
		where TTransportFactory : class, ITransportFactory, new()
	{
		[TestFixtureSetUp]
		public void Setup()
		{
			if (EndpointFactoryConfigurator != null)
			{
				ConfigurationResult result = ConfigurationResultImpl.CompileResults(EndpointFactoryConfigurator.Validate());

				try
				{
					EndpointFactory = EndpointFactoryConfigurator.CreateEndpointFactory();
					EndpointFactoryConfigurator = null;

					_endpointCache = new EndpointCache(EndpointFactory);
					EndpointCache = new EndpointCacheProxy(_endpointCache);
				}
				catch (Exception ex)
				{
					throw new ConfigurationException(result, "An exception was thrown during endpoint cache creation", ex);
				}
			}

			ServiceBusFactory.ConfigureDefaultSettings(x =>
				{
					x.SetEndpointCache(EndpointCache);
					x.SetConcurrentConsumerLimit(4);
					x.SetReceiveTimeout(150.Milliseconds());
					x.EnableAutoStart();
				});
		}

		[TestFixtureTearDown]
		public void FixtureTeardown()
		{
			TeardownBuses();

			if (EndpointCache != null)
			{
				_endpointCache.Dispose();
				_endpointCache = null;
				EndpointCache = null;
			}

			ServiceBusFactory.ConfigureDefaultSettings(x => { x.SetEndpointCache(null); });
		}

		protected EndpointTestFixture()
		{
			Buses = new List<IServiceBus>();

			var defaultSettings = new EndpointFactoryDefaultSettings();

			EndpointFactoryConfigurator = new EndpointFactoryConfiguratorImpl(defaultSettings);
			EndpointFactoryConfigurator.AddTransportFactory<TTransportFactory>();
			EndpointFactoryConfigurator.SetPurgeOnStartup(true);
		}

		protected void AddTransport<T>()
			where T : class, ITransportFactory, new()
		{
			EndpointFactoryConfigurator.AddTransportFactory<T>();
		}

		protected IEndpointFactory EndpointFactory { get; private set; }

		protected void ConfigureEndpointFactory(Action<EndpointFactoryConfigurator> configure)
		{
			if (EndpointFactoryConfigurator == null)
				throw new ConfigurationException("The endpoint factory configurator has already been executed.");

			configure(EndpointFactoryConfigurator);
		}

		protected void ConnectSubscriptionService(ServiceBusConfigurator configurator,
		                                          ISubscriptionService subscriptionService)
		{
			configurator.AddService(BusServiceLayer.Session, () => new SubscriptionPublisher(subscriptionService));
			configurator.AddService(BusServiceLayer.Session, () => new SubscriptionConsumer(subscriptionService));
		}

		protected static InMemorySagaRepository<TSaga> SetupSagaRepository<TSaga>()
			where TSaga : class, ISaga
		{
			var sagaRepository = new InMemorySagaRepository<TSaga>();

			return sagaRepository;
		}

		/// <summary>
		/// Set this property to have custom [<see cref="TestFixtureSetUpAttribute"/>] logic.
		/// </summary>
		protected EndpointFactoryConfigurator EndpointFactoryConfigurator;
		EndpointCache _endpointCache;

		void TeardownBuses()
		{
			Buses.Reverse().Each(bus => { bus.Dispose(); });
			Buses.Clear();
		}

		/// <summary>
		/// Gets the list of buses that are created in this test fixture. Call 
		/// <see cref="SetupServiceBus(System.Uri,System.Action{MassTransit.BusConfigurators.ServiceBusConfigurator})"/>
		/// to create more of them
		/// </summary>
		protected IList<IServiceBus> Buses { get; private set; }

		/// <summary>
		/// Gets the endpoint cache implementation used in this test. Is set up
		/// during the [TestFixtureSetUp] phase  aka. [Given] phase.
		/// </summary>
		protected IEndpointCache EndpointCache { get; private set; }

		/// <summary>
		/// Call this method to set up a new service bus and add it to the <see cref="Buses"/> list.
		/// </summary>
		/// <param name="uri">The bus endpoint uri (its consumption point)</param>
		/// <param name="configure">The configuration action, that allows you to configure the new
		/// bus as you please.</param>
		/// <returns>The new service bus that was configured.</returns>
		protected virtual IServiceBus SetupServiceBus(Uri uri, Action<ServiceBusConfigurator> configure)
		{
			IServiceBus bus = ServiceBusFactory.New(x =>
				{
					x.ReceiveFrom(uri);

					configure(x);
				});

			Buses.Add(bus);

			return bus;
		}

		/// <summary>
		/// See <see cref="SetupServiceBus(System.Uri,System.Action{MassTransit.BusConfigurators.ServiceBusConfigurator})"/>.
		/// </summary>
		/// <param name="uri">The uri to set the service bus up at.</param>
		/// <returns>The service bus instance.</returns>
		protected virtual IServiceBus SetupServiceBus(Uri uri)
		{
			return SetupServiceBus(uri, x =>
				{
					ConfigureServiceBus(uri, x);
				});
		}

		/// <summary>
		/// This method does nothing at all. Override (you don't need to call into the base)
		/// to provide the default action to configure the buses newly created, with.
		/// </summary>
		/// <param name="uri">The uri that is passed from the configuration lambda of the bus.</param>
		/// <param name="configurator">The service bus configurator.</param>
		protected virtual void ConfigureServiceBus(Uri uri, ServiceBusConfigurator configurator)
		{
		}
	}
}