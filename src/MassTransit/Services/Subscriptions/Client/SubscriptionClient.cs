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
namespace MassTransit.Services.Subscriptions.Client
{
	using System;
	using System.Threading;
	using Exceptions;
	using MassTransit.Subscriptions.Coordinator;
	using MassTransit.Subscriptions.Messages;
	using Messages;
	using log4net;

	public class SubscriptionClient :
		SubscriptionObserver,
		Consumes<SubscriptionRefresh>.Context
	{
		static readonly ILog _log = LogManager.GetLogger(typeof (SubscriptionClient));
		readonly IServiceBus _bus;
		readonly SubscriptionRouter _router;
		readonly string _network;
		readonly SubscriptionServiceMessageProducer _producer;
		readonly ManualResetEvent _ready = new ManualResetEvent(false);
		readonly TimeSpan _startTimeout;
		readonly Uri _subscriptionServiceUri;
		UnsubscribeAction _unsubscribeAction;

		public SubscriptionClient(IServiceBus bus, SubscriptionRouter router, Uri subscriptionServiceUri,
		                          TimeSpan startTimeout)
		{
			_bus = bus;
			_router = router;
			_subscriptionServiceUri = subscriptionServiceUri;
			_startTimeout = startTimeout;
			_network = router.Network;

			if (_log.IsDebugEnabled)
				_log.DebugFormat("Starting SubscriptionClient using {0}", subscriptionServiceUri);

			VerifyClientAndServiceNotOnSameEndpoint(bus);

			_ready.Reset();

			var consumerInstance = new SubscriptionMessageConsumer(_router, _network);

			_unsubscribeAction = _bus.ControlBus.SubscribeInstance(consumerInstance);
			_unsubscribeAction += _bus.ControlBus.SubscribeHandler<SubscriptionRefresh>(x => _ready.Set());

			_producer = new SubscriptionServiceMessageProducer(router, _bus.GetEndpoint(subscriptionServiceUri));

			WaitForSubscriptionServiceResponse();
		}

		public void OnSubscriptionAdded(SubscriptionAdded message)
		{
			_producer.OnSubscriptionAdded(message);
		}

		public void OnSubscriptionRemoved(SubscriptionRemoved message)
		{
			_producer.OnSubscriptionRemoved(message);
		}

		public void OnComplete()
		{
			if (_unsubscribeAction != null)
			{
				_unsubscribeAction();
				_unsubscribeAction = null;
			}

			_producer.OnComplete();
		}

		public void Consume(IConsumeContext<SubscriptionRefresh> context)
		{
			if (_subscriptionServiceUri.Equals(context.SourceAddress))
			{
				_ready.Set();
			}
		}

		void WaitForSubscriptionServiceResponse()
		{
			if (_log.IsDebugEnabled)
				_log.Debug("Waiting for response from the subscription service");

			using (_ready)
			{
				bool received = _ready.WaitOne(_startTimeout);
				if (!received)
				{
					throw new InvalidOperationException("Timeout waiting for subscription service to respond");
				}
			}
		}

		void VerifyClientAndServiceNotOnSameEndpoint(IServiceBus bus)
		{
			Uri controlUri = bus.ControlBus.Endpoint.Address.Uri;

			if (!controlUri.Equals(_subscriptionServiceUri))
				return;

			string message = "The service bus and subscription service cannot use the same endpoint: " +
			                 controlUri;

			throw new EndpointException(controlUri, message);
		}
	}
}