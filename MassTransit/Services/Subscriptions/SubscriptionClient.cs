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
namespace MassTransit.Services.Subscriptions
{
	using Client;
	using Exceptions;
	using Internal;
	using Messages;

	public class SubscriptionClient :
		IBusService
	{
		private readonly LocalEndpointHandler _localEndpoints;
		private readonly IServiceBus _serviceBus;
		private readonly IEndpoint _subscriptionServiceEndpoint;
		private UnsubscribeAction _localUnsubscribe;
		private UnsubscribeAction _remoteUnsubscribe;


		public SubscriptionClient(IServiceBus serviceBus, IEndpoint subscriptionServiceEndpoint, LocalEndpointHandler localEndpoints)
		{
			_serviceBus = serviceBus;
			_localEndpoints = localEndpoints;
			_subscriptionServiceEndpoint = subscriptionServiceEndpoint;
		}


		public void Dispose()
		{
			//the bus owns the client so it shouldn't be disposed
			//the bus owns the cache so it shouldn't be disposed
			_subscriptionServiceEndpoint.Dispose();
		}

		public void Start()
		{
			VerifyClientAndServiceNotOnSameEndpoint(_serviceBus, _subscriptionServiceEndpoint);

			_localEndpoints.AddLocalEndpoint(_serviceBus.Endpoint);

			_remoteUnsubscribe = _serviceBus.Subscribe<RemoteSubscriptionCoordinator>();
			_localUnsubscribe = _serviceBus.Subscribe<LocalSubscriptionCoordinator>();

			//remote coordinator?
			_subscriptionServiceEndpoint.Send(new CacheUpdateRequest(_serviceBus.Endpoint.Uri));
		}

		public void Stop()
		{
			//remote coordinator?
			_subscriptionServiceEndpoint.Send(new CancelSubscriptionUpdates(_serviceBus.Endpoint.Uri));

			_localUnsubscribe();
			_remoteUnsubscribe();
		}


		private static void VerifyClientAndServiceNotOnSameEndpoint(IServiceBus bus, IEndpoint endpoint)
		{
			if (bus.Endpoint.Uri.Equals(endpoint.Uri))
			{
				string message = string.Format("Both the service bus and subscription client are listening on the same endpoint {0}", endpoint.Uri);
				throw new EndpointException(endpoint, message);
			}
		}
	}
}