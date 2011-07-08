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
namespace MassTransit.SystemView.Core.Consumer
{
	using System;
	using Distributor.Messages;
	using Magnum;
	using Services.Subscriptions.Messages;
	using ViewModel;

	public class SubscriptionDataConsumer :
		Consumes<SubscriptionRefresh>.All,
		Consumes<AddSubscription>.All,
		Consumes<RemoveSubscription>.All,
		Consumes<IWorkerAvailable>.All,
		IDisposable
	{
		readonly IServiceBus _bus;
		readonly Guid _clientId = CombGuid.Generate();
		readonly Uri _subscriptionServiceUri;
		readonly UnsubscribeAction _unsubscribe;
		IEndpoint _subscriptionServiceEndpoint;

		public SubscriptionDataConsumer(IServiceBus bus, Uri subscriptionServiceUri)
		{
			_bus = bus;
			_subscriptionServiceUri = subscriptionServiceUri;

			_unsubscribe = _bus.SubscribeInstance(this);

			ConnectToSubscriptionService();
		}

		public void Consume(AddSubscription message)
		{
			LocalSubscriptionCache.Endpoints.Update(message.Subscription);
		}

		public void Consume(IWorkerAvailable message)
		{
			LocalSubscriptionCache.Endpoints.Update(message);
		}

		public void Consume(RemoveSubscription message)
		{
			LocalSubscriptionCache.Endpoints.Remove(message.Subscription.EndpointUri, message.Subscription.MessageName);
		}

		public void Consume(SubscriptionRefresh message)
		{
			LocalSubscriptionCache.Endpoints.Update(message.Subscriptions);
		}

		public void Dispose()
		{
			_unsubscribe();
		}

		public void UpdateWorker(Uri controlUri, string type, int pendingLimit, int inProgressLimit)
		{
			IEndpoint endpoint = _bus.GetEndpoint(controlUri);

			endpoint.Send(new ConfigureWorker
				{
					MessageType = type,
					InProgressLimit = inProgressLimit,
					PendingLimit = pendingLimit
				}, context => context.SetSourceAddress(_bus.Endpoint.Address.Uri));
		}

		public void RemoveSubscription(Guid clientId, string messageName, string correlationId, Uri endpointUri)
		{
			var subscription = new SubscriptionInformation(clientId, 0, messageName, correlationId, endpointUri);

			_subscriptionServiceEndpoint.Send(new RemoveSubscription(subscription));
		}

		void ConnectToSubscriptionService()
		{
			_subscriptionServiceEndpoint = _bus.GetEndpoint(_subscriptionServiceUri);

			_subscriptionServiceEndpoint.Send(new AddSubscriptionClient(_clientId, _bus.Endpoint.Address.Uri,
				_bus.Endpoint.Address.Uri));
		}
	}
}