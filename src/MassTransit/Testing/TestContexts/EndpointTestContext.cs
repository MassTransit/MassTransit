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
namespace MassTransit.Testing.TestContexts
{
	using System;
	using BusConfigurators;
	using Magnum.Extensions;
	using Saga;
	using Services.Subscriptions;
	using Transports;

	public class EndpointTestContext :
		IEndpointTestContext
	{
		readonly EndpointCache _endpointCache;
		bool _disposed;

		public EndpointTestContext(IEndpointFactory endpointFactory)
		{
			EndpointFactory = endpointFactory;

			_endpointCache = new EndpointCache(endpointFactory);

			EndpointCache = new EndpointCacheProxy(_endpointCache);

			ServiceBusFactory.ConfigureDefaultSettings(x =>
				{
					x.SetEndpointCache(EndpointCache);
					x.SetConcurrentConsumerLimit(4);
					x.SetConcurrentReceiverLimit(1);
					x.SetReceiveTimeout(50.Milliseconds());
					x.EnableAutoStart();
				});
		}

		public IEndpointCache EndpointCache { get; private set; }
		public IEndpointFactory EndpointFactory { get; private set; }

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}


		protected void ConnectSubscriptionService(ServiceBusConfigurator configurator,
		                                          ISubscriptionService subscriptionService)
		{
			configurator.AddService(() => new SubscriptionPublisher(subscriptionService));
			configurator.AddService(() => new SubscriptionConsumer(subscriptionService));
		}

		class EndpointCacheProxy :
			IEndpointCache
		{
			readonly IEndpointCache _endpointCache;

			public EndpointCacheProxy(IEndpointCache endpointCache)
			{
				_endpointCache = endpointCache;
			}

			public void Dispose()
			{
				// we don't dispose, since we're in testing
			}

			public IEndpoint GetEndpoint(Uri uri)
			{
				return _endpointCache.GetEndpoint(uri);
			}
		}

		void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
				_endpointCache.Clear();

				if (EndpointCache != null)
				{
					EndpointCache.Dispose();
					EndpointCache = null;
				}

				ServiceBusFactory.ConfigureDefaultSettings(x => x.SetEndpointCache(null));
			}

			_disposed = true;
		}

		~EndpointTestContext()
		{
			Dispose(false);
		}

		protected static InMemorySagaRepository<TSaga> SetupSagaRepository<TSaga>()
			where TSaga : class, ISaga
		{
			var sagaRepository = new InMemorySagaRepository<TSaga>();

			return sagaRepository;
		}
	}
}