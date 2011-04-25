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
	using EndpointConfigurators;
	using Magnum.Reflection;
	using Serialization;
	using Transports;

	public static class EndpointFactoryConfiguratorExtensions
	{
		/// <summary>
		/// Returns a configurator for the specified endpoint URI
		/// </summary>
		/// <param name="configurator"></param>
		/// <param name="uriString"></param>
		/// <returns></returns>
		public static EndpointConfigurator ConfigureEndpoint(this EndpointFactoryConfigurator configurator, string uriString)
		{
			return configurator.ConfigureEndpoint(uriString.ToUri("The configure endpoint URI is invalid"));
		}

		/// <summary>
		/// Returns a configurator for the specified endpoint URI
		/// </summary>
		/// <param name="configurator"></param>
		/// <param name="uri"></param>
		/// <returns></returns>
		public static EndpointConfigurator ConfigureEndpoint(this EndpointFactoryConfigurator configurator, Uri uri)
		{
			var endpointConfigurator = new EndpointConfiguratorImpl(uri, configurator.Defaults);

			configurator.AddEndpointFactoryConfigurator(endpointConfigurator);

			return endpointConfigurator;
		}

		/// <summary>
		/// Configures the endpoint for the specified endpoint URI
		/// </summary>
		/// <param name="configurator"></param>
		/// <param name="uriString"></param>
		/// <param name="configureCallback"></param>
		/// <returns></returns>
		public static void ConfigureEndpoint(this EndpointFactoryConfigurator configurator, string uriString,
		                                     Action<EndpointConfigurator> configureCallback)
		{
			EndpointConfigurator endpointConfigurator = configurator.ConfigureEndpoint(uriString.ToUri("The configure endpoint URI is invalid"));

			configureCallback(endpointConfigurator);
		}

		/// <summary>
		/// Configures the endpoint for the specified URI
		/// </summary>
		/// <param name="configurator"></param>
		/// <param name="uri"></param>
		/// <param name="configureCallback"></param>
		public static void ConfigureEndpoint(this EndpointFactoryConfigurator configurator, Uri uri,
		                                     Action<EndpointConfigurator> configureCallback)
		{
			EndpointConfigurator endpointConfigurator = configurator.ConfigureEndpoint(uri);

			configureCallback(endpointConfigurator);
		}


		public static EndpointFactoryConfigurator AddTransportFactory(this EndpointFactoryConfigurator configurator, ITransportFactory transportFactory)
		{
			return AddTransportFactory(configurator, () => transportFactory);
		}

		public static EndpointFactoryConfigurator AddTransportFactory<TTransportFactory>(this EndpointFactoryConfigurator configurator)
			where TTransportFactory : ITransportFactory, new()
		{
			return AddTransportFactory(configurator, () => new TTransportFactory());
		}

		public static EndpointFactoryConfigurator AddTransportFactory(this EndpointFactoryConfigurator configurator, Type transportFactoryType)
		{
			return AddTransportFactory(configurator, () => (ITransportFactory) FastActivator.Create(transportFactoryType));
		}

		public static EndpointFactoryConfigurator AddTransportFactory(this EndpointFactoryConfigurator configurator,
		                                                              Func<ITransportFactory> transportFactoryFactory)
		{
			var transportFactoryConfigurator = new TransportFactoryEndpointFactoryConfigurator(transportFactoryFactory);

			configurator.AddEndpointFactoryConfigurator(transportFactoryConfigurator);

			return configurator;
		}

		public static EndpointFactoryConfigurator SetDefaultSerializer(this EndpointFactoryConfigurator configurator, Func<IMessageSerializer> serializerFactory)
		{
			var serializerConfigurator = new DefaultSerializerEndpointFactoryConfigurator(serializerFactory);

			configurator.AddEndpointFactoryConfigurator(serializerConfigurator);

			return configurator;
		}

		/// <summary>
		/// Sets the default message serializer for endpoints
		/// </summary>
		/// <typeparam name="TSerializer"></typeparam>
		/// <param name="configurator"></param>
		/// <returns></returns>
		public static EndpointFactoryConfigurator SetDefaultSerializer<TSerializer>(this EndpointFactoryConfigurator configurator)
			where TSerializer : IMessageSerializer, new()
		{
			return SetDefaultSerializer(configurator, () => new TSerializer());
		}

		/// <summary>
		/// Sets the default message serializer for endpoints
		/// </summary>
		/// <param name="configurator"></param>
		/// <param name="serializerType"></param>
		/// <returns></returns>
		public static EndpointFactoryConfigurator SetDefaultSerializer(this EndpointFactoryConfigurator configurator, Type serializerType)
		{
			return SetDefaultSerializer(configurator, () => (IMessageSerializer) FastActivator.Create(serializerType));
		}

		/// <summary>
		/// Sets the default message serializer for endpoints
		/// </summary>
		/// <param name="configurator"></param>
		/// <param name="serializer"></param>
		/// <returns></returns>
		public static EndpointFactoryConfigurator SetDefaultSerializer(this EndpointFactoryConfigurator configurator, IMessageSerializer serializer)
		{
			return SetDefaultSerializer(configurator, () => serializer);
		}

		public static EndpointFactoryConfigurator SetDefaultTransactionTimeout(this EndpointFactoryConfigurator configurator, TimeSpan timeout)
		{
			var builderConfigurator = new DelegateEndpointFactoryBuilderConfigurator(x => x.SetDefaultTransactionTimeout(timeout));

			configurator.AddEndpointFactoryConfigurator(builderConfigurator);

			return configurator;
		}

		public static EndpointFactoryConfigurator SetCreateMissingQueues(this EndpointFactoryConfigurator configurator, bool value)
		{
			var builderConfigurator = new DelegateEndpointFactoryBuilderConfigurator(x => x.SetCreateMissingQueues(value));

			configurator.AddEndpointFactoryConfigurator(builderConfigurator);

			return configurator;
		}

		public static EndpointFactoryConfigurator SetCreateTransactionalQueues(this EndpointFactoryConfigurator configurator, bool value)
		{
			var builderConfigurator = new DelegateEndpointFactoryBuilderConfigurator(x => x.SetCreateTransactionalQueues(value));

			configurator.AddEndpointFactoryConfigurator(builderConfigurator);

			return configurator;
		}

		public static EndpointFactoryConfigurator SetPurgeOnStartup(this EndpointFactoryConfigurator configurator, bool value)
		{
			var builderConfigurator = new DelegateEndpointFactoryBuilderConfigurator(x => x.SetPurgeOnStartup(value));

			configurator.AddEndpointFactoryConfigurator(builderConfigurator);

			return configurator;
		}
	}
}