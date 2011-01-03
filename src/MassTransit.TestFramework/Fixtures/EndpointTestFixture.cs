// Copyright 2007-2010 The Apache Software Foundation.
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
	using Configuration;
	using Magnum.Extensions;
	using NUnit.Framework;
	using Rhino.Mocks;
	using Serialization;
	using Transports;

    [TestFixture]
	public class EndpointTestFixture<TEndpointFactory> :
		AbstractTestFixture
		where TEndpointFactory : IEndpointFactory
	{
		[TestFixtureSetUp]
		public void EndpointTestFixtureSetup()
		{
			SetupObjectBuilder();

			SetupMessageSerializer();

			SetupEndpointFactory();

			SetupServiceBusDefaults();
		}

		[TestFixtureTearDown]
		public void EndpointTestFixtureTeardown()
		{
			TeardownBuses();

			EndpointResolver.Dispose();
			EndpointResolver = null;
		}

		protected EndpointTestFixture()
		{
			Buses = new List<IServiceBus>();
		}

		protected virtual void SetupObjectBuilder()
		{
			ObjectBuilder = MockRepository.GenerateMock<IObjectBuilder>();
		}

		protected virtual void SetupMessageSerializer()
		{
			ObjectBuilder.Add(new XmlMessageSerializer());
		}

		protected virtual void SetupEndpointFactory()
		{
			EndpointResolver = EndpointResolverConfigurator.New(x =>
				{
					x.SetObjectBuilder(ObjectBuilder);
					x.RegisterTransport<TEndpointFactory>();
					x.SetDefaultSerializer<XmlMessageSerializer>();

					ConfigureEndpointFactory(x);
				});

			ObjectBuilder.Add(EndpointResolver);
		}

		protected virtual void ConfigureEndpointFactory(IEndpointResolverConfigurator x)
		{
		}

		protected virtual void SetupServiceBusDefaults()
		{
			ServiceBusConfigurator.Defaults(x =>
				{
                    x.SetEndpointFactory(EndpointResolver);
					x.SetObjectBuilder(ObjectBuilder);
					x.SetReceiveTimeout(50.Milliseconds());
					x.SetConcurrentConsumerLimit(Environment.ProcessorCount*2);
				});
		}

		private void TeardownBuses()
		{
			Buses.Reverse().Each(bus => { bus.Dispose(); });
			Buses.Clear();
		}

		protected IList<IServiceBus> Buses { get; private set; }

		protected IEndpointResolver EndpointResolver { get; private set; }

		protected IObjectBuilder ObjectBuilder { get; private set; }

		protected virtual IServiceBus SetupServiceBus(Uri uri, Action<IServiceBusConfigurator> configure)
		{
			IServiceBus bus = ServiceBusConfigurator.New(x =>
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

		protected virtual void ConfigureServiceBus(Uri uri, IServiceBusConfigurator configurator)
		{
		}
	}
}