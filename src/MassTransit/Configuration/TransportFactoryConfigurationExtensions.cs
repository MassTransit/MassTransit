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
namespace MassTransit
{
	using System;
	using BusConfigurators;
	using EndpointConfigurators;
	using Magnum.Reflection;
	using Transports;

	public static class TransportFactoryConfigurationExtensions
	{
		public static T AddTransportFactory<T>(this T configurator, ITransportFactory transportFactory)
			where T : EndpointFactoryConfigurator
		{
			return AddTransportFactory(configurator, () => transportFactory);
		}

		public static EndpointFactoryConfigurator AddTransportFactory<TTransportFactory>(
			this EndpointFactoryConfigurator configurator)
			where TTransportFactory : ITransportFactory, new()
		{
			return AddTransportFactory(configurator, () => new TTransportFactory());
		}

		public static EndpointFactoryConfigurator AddTransportFactory<TTransportFactory>(
			this EndpointFactoryConfigurator configurator, Action<TTransportFactory> configureFactory)
			where TTransportFactory : ITransportFactory, new()
		{
			return AddTransportFactory(configurator, () =>
				{
					var transportFactory = new TTransportFactory();
					configureFactory(transportFactory);

					return transportFactory;
				});
		}

		public static ServiceBusConfigurator AddTransportFactory<TTransportFactory>(
			this ServiceBusConfigurator configurator)
			where TTransportFactory : ITransportFactory, new()
		{
			return AddTransportFactory(configurator, () => new TTransportFactory());
		}

		public static ServiceBusConfigurator AddTransportFactory<TTransportFactory>(
			this ServiceBusConfigurator configurator, Action<TTransportFactory> configureFactory)
			where TTransportFactory : ITransportFactory, new()
		{
			return AddTransportFactory(configurator, () =>
				{
					var transportFactory = new TTransportFactory();
					configureFactory(transportFactory);

					return transportFactory;
				});
		}

		public static T AddTransportFactory<T>(this T configurator, Type transportFactoryType)
			where T : EndpointFactoryConfigurator
		{
			return AddTransportFactory(configurator, () => (ITransportFactory) FastActivator.Create(transportFactoryType));
		}

		public static T AddTransportFactory<T, TTransport>(this T configurator, Func<TTransport> transportFactoryFactory)
			where T : EndpointFactoryConfigurator
			where TTransport : ITransportFactory
		{
			var transportFactoryConfigurator = new TransportFactoryConfigurator<TTransport>(transportFactoryFactory);

			configurator.AddEndpointFactoryConfigurator(transportFactoryConfigurator);

			return configurator;
		}
	}
}