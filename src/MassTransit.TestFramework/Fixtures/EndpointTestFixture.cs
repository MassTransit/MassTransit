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
	using EndpointConfigurators;
	using Magnum.Extensions;
	using MassTransit.Transports;
	using NUnit.Framework;
	using Rhino.Mocks;

	[TestFixture]
	public class EndpointTestFixture<TTransportFactory> :
		AbstractTestFixture
		where TTransportFactory : ITransportFactory, new()
	{
		EndpointFactoryConfigurator _endpointFactoryConfigurator;

		[TestFixtureSetUp]
		public void EndpointTestFixtureSetup()
		{
			SetupObjectBuilder();

			SetupEndpointFactory();

			SetupServiceBusDefaults();
		}

		[TestFixtureTearDown]
		public void EndpointTestFixtureTeardown()
		{
			TeardownBuses();

			EndpointCache.Dispose();
			EndpointCache = null;
		}

		protected EndpointTestFixture()
		{
			Buses = new List<IServiceBus>();
		}

		protected virtual void SetupObjectBuilder()
		{
			ObjectBuilder = MockRepository.GenerateMock<IObjectBuilder>();
		}

		protected virtual void SetupEndpointFactory()
		{
			var defaultSettings = new EndpointFactoryDefaultSettings();

			_endpointFactoryConfigurator = new EndpointFactoryConfiguratorImpl(defaultSettings);
			_endpointFactoryConfigurator.AddTransportFactory<TTransportFactory>();

			_endpointFactoryConfigurator.Validate();

			ObjectBuilder = MockRepository.GenerateMock<IObjectBuilder>();

			ConfigureEndpointFactory(_endpointFactoryConfigurator);

			IEndpointFactory endpointFactory = _endpointFactoryConfigurator.CreateEndpointFactory();
			_endpointFactoryConfigurator = null;

			EndpointCache = new EndpointCache(endpointFactory);

			ServiceBusFactory.ConfigureDefaultSettings(x =>
				{
					x.SetEndpointCache(EndpointCache);
					x.SetConcurrentConsumerLimit(4);
					x.SetReceiveTimeout(50.Milliseconds());
					x.SetObjectBuilder(ObjectBuilder);
					x.EnableAutoStart();
				});

			ObjectBuilder.Add(EndpointCache);
		}

		protected virtual void ConfigureEndpointFactory(EndpointFactoryConfigurator x)
		{
		}

		protected virtual void SetupServiceBusDefaults()
		{
			ServiceBusFactory.ConfigureDefaultSettings(x =>
				{
					x.SetEndpointCache(EndpointCache);
					x.SetObjectBuilder(ObjectBuilder);
					x.SetReceiveTimeout(50.Milliseconds());
					x.SetConcurrentConsumerLimit(Environment.ProcessorCount*2);
				});
		}

		void TeardownBuses()
		{
			Buses.Reverse().Each(bus => { bus.Dispose(); });
			Buses.Clear();
		}

		protected IList<IServiceBus> Buses { get; private set; }

		protected IEndpointCache EndpointCache { get; private set; }

		protected IObjectBuilder ObjectBuilder { get; private set; }

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

		protected virtual IServiceBus SetupServiceBus(Uri uri)
		{
			return SetupServiceBus(uri, x => ConfigureServiceBus(uri, x));
		}

		protected virtual void ConfigureServiceBus(Uri uri, ServiceBusConfigurator configurator)
		{
		}
	}
}