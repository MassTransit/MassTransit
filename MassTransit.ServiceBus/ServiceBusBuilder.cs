/// Copyright 2007-2008 The Apache Software Foundation.
/// 
/// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
/// this file except in compliance with the License. You may obtain a copy of the 
/// License at 
/// 
///   http://www.apache.org/licenses/LICENSE-2.0 
/// 
/// Unless required by applicable law or agreed to in writing, software distributed 
/// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
/// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
/// specific language governing permissions and limitations under the License.
namespace MassTransit.ServiceBus
{
	using System;
	using Internal;
	using Subscriptions;
	using Util;

	public class ServiceBusBuilder
	{
		internal ISubscriptionCache _cache;
		internal IEndpointResolver _endpointResolver;
		internal IObjectBuilder _objectBuilder;
		internal Uri _uri;

		public ServiceBusBuilder ListeningOn(string uriString)
		{
			Guard.Against.NullOrEmpty(uriString, "An endpoint Uri string cannot be null or empty");

			_uri = new Uri(uriString);

			return this;
		}

		public ServiceBusBuilder ListeningOn(Uri uri)
		{
			Guard.Against.Null(uri, "An endpoint Uri cannot be null");

			_uri = uri;

			return this;
		}

		public static implicit operator ServiceBus(ServiceBusBuilder builder)
		{
			IObjectBuilder objectBuilder = builder.GetObjectBuilder();
			IEndpointResolver endpointResolver = builder.GetResolver();
			ISubscriptionCache cache = builder.GetCache();
			IEndpoint endpoint = endpointResolver.Resolve(builder._uri);
		    ITypeInfoCache typeInfoCache = new TypeInfoCache();
			return new ServiceBus(endpoint, objectBuilder, cache, endpointResolver, typeInfoCache);
		}

		private IEndpointResolver GetResolver()
		{
			if (_endpointResolver == null)
			{
				if (_objectBuilder == null)
					_endpointResolver = new EndpointResolver();
				else
                    _endpointResolver = _objectBuilder.GetInstance<IEndpointResolver>();
			}

			return _endpointResolver;
		}

		private ISubscriptionCache GetCache()
		{
			if (_cache == null)
			{
				if (_objectBuilder == null)
					_cache = new LocalSubscriptionCache();
				else
                    _cache = _objectBuilder.GetInstance<ISubscriptionCache>();
			}

			return _cache;
		}

		private IObjectBuilder GetObjectBuilder()
		{
			return _objectBuilder;
		}

		public ServiceBusBuilder SupportingTransport<T>()
		{
			EndpointResolver.AddTransport(typeof (T));

			return this;
		}

		public ServiceBusBuilder UsingObjectBuilder(IObjectBuilder builder)
		{
			_objectBuilder = builder;

			return this;
		}
	}
}