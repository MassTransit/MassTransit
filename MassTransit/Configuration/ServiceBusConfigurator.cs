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
		IServiceBusConfigurator
	{
		private static readonly ServiceBusConfiguratorDefaults _defaults = new ServiceBusConfiguratorDefaults();
		private bool _autoStart = true;
		private bool _autoSubscribe;
		private Uri _errorUri;
		private Uri _listenToUri;
		private IObjectBuilder _objectBuilder;
		private int _threadLimit;
		private TimeSpan _receiveTimeout;

		private ServiceBusConfigurator()
		{
			_objectBuilder = _defaults.ObjectBuilder;
			_receiveTimeout = _defaults.ReceiveTimeout;
		}

		public void SetObjectBuilder(IObjectBuilder builder)
		{
			_objectBuilder = builder;
		}

		public void ReceiveFrom(string uriString)
		{
			try
			{
				_listenToUri = new Uri(uriString);
			}
			catch (UriFormatException ex)
			{
				throw new ConfigurationException("The Uri for the receive endpoint is invalid: " + uriString, ex);
			}
		}

		public void ReceiveFrom(Uri uri)
		{
			_listenToUri = uri;
		}

		public void SendErrorsTo(string uriString)
		{
			try
			{
				_errorUri = new Uri(uriString);
			}
			catch (UriFormatException ex)
			{
				throw new ConfigurationException("The Uri for the error endpoint is invalid: " + uriString, ex);
			}
		}

		public void SendErrorsTo(Uri uri)
		{
			_errorUri = uri;
		}

		public void SetThreadLimit(int threadLimit)
		{
			_threadLimit = threadLimit;
		}

		public void EnableAutoSubscribe()
		{
			_autoSubscribe = true;
		}

		public void DisableAutoStart()
		{
			_autoStart = false;
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
			IEndpointFactory endpointFactory = _objectBuilder.GetInstance<IEndpointFactory>();

			IEndpoint endpoint = endpointFactory.GetEndpoint(_listenToUri);

			ISubscriptionCache subscriptionCache = _objectBuilder.GetInstance<ISubscriptionCache>() ?? new LocalSubscriptionCache();
			ITypeInfoCache typeInfoCache = _objectBuilder.GetInstance<ITypeInfoCache>() ?? new TypeInfoCache();

			ServiceBus bus = new ServiceBus(endpoint, _objectBuilder, subscriptionCache, endpointFactory, typeInfoCache);

			if (_errorUri != null)
			{
				IEndpoint poisonEndpoint = endpointFactory.GetEndpoint(_errorUri);
				bus.PoisonEndpoint = poisonEndpoint;
			}

			if (_threadLimit > 0)
				bus.MaxThreadCount = _threadLimit;

			bus.ReceiveTimeout = _receiveTimeout;

			if (_autoStart)
			{
				bus.Start();
			}

			if (_autoSubscribe)
			{
				// get all the types and subscribe them to the bus
			}

			return bus;
		}

		public static void Defaults(Action<IServiceBusConfiguratorDefaults> action)
		{
			action(_defaults);
		}
	}
}