// Copyright 2007-2008 The Apache Software Foundation.
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
namespace MassTransit.Configuration
{
	using System;
	using Exceptions;
	using Internal;
	using Subscriptions;

	public class ServiceBusConfigurator :
		ServiceBusConfiguratorDefaults,
		IServiceBusConfigurator
	{
		private static readonly ServiceBusConfiguratorDefaults _defaults = new ServiceBusConfiguratorDefaults();
		private Uri _receiveFromUri;

		private ServiceBusConfigurator()
		{
			_defaults.ApplyTo(this);
		}

		public void ReceiveFrom(string uriString)
		{
			try
			{
				_receiveFromUri = new Uri(uriString);
			}
			catch (UriFormatException ex)
			{
				throw new ConfigurationException("The Uri for the receive endpoint is invalid: " + uriString, ex);
			}
		}

		public void ReceiveFrom(Uri uri)
		{
			_receiveFromUri = uri;
		}

		public void ConfigureService<TServiceConfigurator>(Action<TServiceConfigurator> action) where TServiceConfigurator : IServiceConfigurator
		{
			throw new NotImplementedException();
		}

		public static IServiceBus New(Action<IServiceBusConfigurator> action)
		{
			ServiceBusConfigurator configurator = new ServiceBusConfigurator();

			action(configurator);

			return configurator.Create();
		}


		private IServiceBus Create()
		{
			IEndpointFactory endpointFactory = ObjectBuilder.GetInstance<IEndpointFactory>();

			IEndpoint endpoint = endpointFactory.GetEndpoint(_receiveFromUri);

			ISubscriptionCache subscriptionCache = ObjectBuilder.GetInstance<ISubscriptionCache>() ?? new LocalSubscriptionCache();
			ITypeInfoCache typeInfoCache = ObjectBuilder.GetInstance<ITypeInfoCache>() ?? new TypeInfoCache();

			ServiceBus bus = new ServiceBus(endpoint, ObjectBuilder, subscriptionCache, endpointFactory, typeInfoCache);

			if (ErrorUri != null)
			{
				IEndpoint poisonEndpoint = endpointFactory.GetEndpoint(ErrorUri);
				bus.PoisonEndpoint = poisonEndpoint;
			}

			if (ConcurrentConsumerLimit > 0)
				bus.MaximumConsumerThreads = ConcurrentConsumerLimit;

			if (ConcurrentReceiverLimit > 0)
				bus.ConcurrentReceiveThreads = ConcurrentReceiverLimit;

			bus.ReceiveTimeout = ReceiveTimeout;

			if (AutoSubscribe)
			{
				// get all the types and subscribe them to the bus
			}

			if (AutoStart)
			{
				bus.Start();
			}
			return bus;
		}

		public static void Defaults(Action<IServiceBusConfiguratorDefaults> action)
		{
			action(_defaults);
		}
	}
}