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
namespace MassTransit.Tests.TextFixtures
{
	using System;
	using Configurators;
	using EndpointConfigurators;
	using Exceptions;
	using Magnum.Extensions;
	using MassTransit.Saga;
	using MassTransit.Transports;
	using NUnit.Framework;

	[TestFixture]
	public abstract class EndpointTestFixture<TTransportFactory>
		where TTransportFactory : class, ITransportFactory, new()
	{
		[SetUp]
		public void Setup()
		{
			if (_endpointFactoryConfigurator != null)
			{
				ConfigurationResult result = ConfigurationResultImpl.CompileResults(_endpointFactoryConfigurator.Validate());

				try
				{
					EndpointFactory = _endpointFactoryConfigurator.CreateEndpointFactory();
				//	_endpointFactoryConfigurator = null;

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
					x.SetReceiveTimeout(50.Milliseconds());
					x.EnableAutoStart();
				    x.DisablePerformanceCounters();
				});

			EstablishContext();
		}

		[TearDown]
		public void Teardown()
		{
			TeardownContext();

			_endpointCache.Clear();
		}

		[TestFixtureTearDown]
		public void FixtureTeardown()
		{
			if (EndpointCache != null)
			{
				EndpointCache.Dispose();
				EndpointCache = null;
			}

			ServiceBusFactory.ConfigureDefaultSettings(x => { x.SetEndpointCache(null); });
		}

		EndpointFactoryConfiguratorImpl _endpointFactoryConfigurator;
		EndpointCache _endpointCache;

		protected EndpointTestFixture()
		{
			var defaultSettings = new EndpointFactoryDefaultSettings();

			_endpointFactoryConfigurator = new EndpointFactoryConfiguratorImpl(defaultSettings);
			_endpointFactoryConfigurator.AddTransportFactory<TTransportFactory>();
			_endpointFactoryConfigurator.SetPurgeOnStartup(true);
		}

		protected void AddTransport<T>()
			where T : class, ITransportFactory, new()
		{
			_endpointFactoryConfigurator.AddTransportFactory<T>();
		}

		protected IEndpointFactory EndpointFactory { get; private set; }
		protected IEndpointCache EndpointCache { get; set; }

		protected virtual void EstablishContext()
		{
		}

		protected virtual void TeardownContext()
		{
		}

		protected void ConfigureEndpointFactory(Action<EndpointFactoryConfigurator> configure)
		{
			if (_endpointFactoryConfigurator == null)
				throw new ConfigurationException("The endpoint factory configurator has already been executed.");

			configure(_endpointFactoryConfigurator);
		}

		protected static InMemorySagaRepository<TSaga> SetupSagaRepository<TSaga>()
			where TSaga : class, ISaga
		{
			var sagaRepository = new InMemorySagaRepository<TSaga>();

			return sagaRepository;
		}
	}
}