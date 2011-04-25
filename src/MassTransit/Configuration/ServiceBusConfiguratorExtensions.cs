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
	using Exceptions;
	using Serialization;
	using Transports;

	public static class ServiceBusConfiguratorExtensions
	{
		/// <summary>
		/// Specify the endpoint from which messages should be read
		/// </summary>
		/// <param name="configurator"></param>
		/// <param name="uriString">The uri of the endpoint</param>
		public static void ReceiveFrom(this ServiceBusConfigurator configurator, string uriString)
		{
			configurator.ReceiveFrom(uriString.ToUri("The receive endpoint URI is invalid"));
		}

		/// <summary>
		/// Returns a configurator for the specified endpoint URI
		/// </summary>
		/// <param name="configurator"></param>
		/// <param name="uriString"></param>
		/// <returns></returns>
		public static EndpointConfigurator ConfigureEndpoint(this ServiceBusConfigurator configurator, string uriString)
		{
			return ((EndpointFactoryConfigurator) configurator).ConfigureEndpoint(uriString);
		}

		/// <summary>
		/// Returns a configurator for the specified endpoint URI
		/// </summary>
		/// <param name="configurator"></param>
		/// <param name="uri"></param>
		/// <returns></returns>
		public static EndpointConfigurator ConfigureEndpoint(this ServiceBusConfigurator configurator, Uri uri)
		{
			return ((EndpointFactoryConfigurator) configurator).ConfigureEndpoint(uri);
		}

		/// <summary>
		/// Configures the endpoint for the specified endpoint URI
		/// </summary>
		/// <param name="configurator"></param>
		/// <param name="uriString"></param>
		/// <param name="configureCallback"></param>
		/// <returns></returns>
		public static void ConfigureEndpoint(this ServiceBusConfigurator configurator, string uriString,
		                                     Action<EndpointConfigurator> configureCallback)
		{
			((EndpointFactoryConfigurator) configurator).ConfigureEndpoint(uriString, configureCallback);
		}

		/// <summary>
		/// Configures the endpoint for the specified URI
		/// </summary>
		/// <param name="configurator"></param>
		/// <param name="uri"></param>
		/// <param name="configureCallback"></param>
		public static void ConfigureEndpoint(this ServiceBusConfigurator configurator, Uri uri,
		                                     Action<EndpointConfigurator> configureCallback)
		{
			((EndpointFactoryConfigurator) configurator).ConfigureEndpoint(uri, configureCallback);
		}

		public static ServiceBusConfigurator AddTransportFactory(this ServiceBusConfigurator configurator, ITransportFactory transportFactory)
		{
			((EndpointFactoryConfigurator) configurator).AddTransportFactory(transportFactory);

			return configurator;
		}

		public static ServiceBusConfigurator AddTransportFactory<TTransportFactory>(this ServiceBusConfigurator configurator)
			where TTransportFactory : ITransportFactory, new()
		{
			((EndpointFactoryConfigurator) configurator).AddTransportFactory<TTransportFactory>();

			return configurator;
		}

		public static ServiceBusConfigurator AddTransportFactory(this ServiceBusConfigurator configurator, Type transportFactoryType)
		{
			((EndpointFactoryConfigurator) configurator).AddTransportFactory(transportFactoryType);

			return configurator;
		}


		public static ServiceBusConfigurator AddTransportFactory(this ServiceBusConfigurator configurator,
		                                                         Func<ITransportFactory> transportFactoryFactory)
		{
			((EndpointFactoryConfigurator) configurator).AddTransportFactory(transportFactoryFactory);

			return configurator;
		}

		public static ServiceBusConfigurator SetDefaultSerializer(this ServiceBusConfigurator configurator, Func<IMessageSerializer> serializerFactory)
		{
			((EndpointFactoryConfigurator) configurator).SetDefaultSerializer(serializerFactory);

			return configurator;
		}

		/// <summary>
		/// Sets the default message serializer for endpoints
		/// </summary>
		/// <typeparam name="TSerializer"></typeparam>
		/// <param name="configurator"></param>
		/// <returns></returns>
		public static ServiceBusConfigurator SetDefaultSerializer<TSerializer>(this ServiceBusConfigurator configurator)
			where TSerializer : IMessageSerializer, new()
		{
			((EndpointFactoryConfigurator) configurator).SetDefaultSerializer<TSerializer>();

			return configurator;
		}

		/// <summary>
		/// Sets the default message serializer for endpoints
		/// </summary>
		/// <param name="configurator"></param>
		/// <param name="serializerType"></param>
		/// <returns></returns>
		public static ServiceBusConfigurator SetDefaultSerializer(this ServiceBusConfigurator configurator, Type serializerType)
		{
			((EndpointFactoryConfigurator) configurator).SetDefaultSerializer(serializerType);

			return configurator;
		}

		/// <summary>
		/// Sets the default message serializer for endpoints
		/// </summary>
		/// <param name="configurator"></param>
		/// <param name="serializer"></param>
		/// <returns></returns>
		public static ServiceBusConfigurator SetDefaultSerializer(this ServiceBusConfigurator configurator, IMessageSerializer serializer)
		{
			((EndpointFactoryConfigurator) configurator).SetDefaultSerializer(serializer);

			return configurator;
		}

		internal static Uri ToUri(this string uriString)
		{
			try
			{
				return new Uri(uriString);
			}
			catch (UriFormatException ex)
			{
				throw new ConfigurationException("The URI specified is invalid: " + uriString, ex);
			}
		}

		internal static Uri ToUri(this string uriString, string message)
		{
			try
			{
				return new Uri(uriString);
			}
			catch (UriFormatException ex)
			{
				throw new ConfigurationException(string.Format("{0}: {1}", message, uriString), ex);
			}
		}
	}
}