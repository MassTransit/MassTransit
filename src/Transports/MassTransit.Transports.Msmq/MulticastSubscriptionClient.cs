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
namespace MassTransit.Transports.Msmq
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Services.Subscriptions.Messages;
	using Subscriptions.Coordinator;
	using Subscriptions.Messages;
	using log4net;

	public class MulticastSubscriptionClient :
		SubscriptionObserver,
		Consumes<AddSubscriptionClient>.Context
	{
		static readonly ILog _log = LogManager.GetLogger(typeof (MulticastSubscriptionClient));
		readonly string _network;
		readonly Guid _peerId;
		readonly Uri _peerUri;
		readonly BusSubscriptionMessageProducer _producer;
		readonly SubscriptionRouter _router;
		IServiceBus _subscriptionBus;
		UnsubscribeAction _unsubscribeAction;

		public MulticastSubscriptionClient(IServiceBus subscriptionBus, SubscriptionRouter router)
		{
			_subscriptionBus = subscriptionBus;
			_router = router;
			_network = router.Network;
			_peerId = router.PeerId;
			_peerUri = router.PeerUri;

			if (_log.IsDebugEnabled)
				_log.DebugFormat("Starting MulticastSubscriptionService using {0}", subscriptionBus.Endpoint.Address);

			var consumerInstance = new SubscriptionMessageConsumer(_router, _network);

			_unsubscribeAction = _subscriptionBus.SubscribeInstance(consumerInstance);

			_producer = new BusSubscriptionMessageProducer(router, subscriptionBus.Endpoint);
		}

		public void Consume(IConsumeContext<AddSubscriptionClient> context)
		{
			// made a new friend, let's introduce ourselves, but may need to send
			// an addclient first, which would loop forever, so how do we only send
			// to people we just met? Observe a PeerAdded event? ;) OnPeerAdded/OnPeerRemoved

			List<SubscriptionInformation> subscriptions = _router.LocalSubscriptions
				.Select(x => new SubscriptionInformation(_peerId, x.SubscriptionId, x.MessageName, x.CorrelationId, x.EndpointUri))
				.ToList();

			var response = new SubscriptionRefresh(subscriptions);

			IEndpoint clientEndpoint = _subscriptionBus.GetEndpoint(context.SourceAddress);

			clientEndpoint.Send(response, x => x.SetSourceAddress(_peerUri));
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

			if (_subscriptionBus != null)
			{
				_subscriptionBus.Dispose();
				_subscriptionBus = null;
			}
		}
	}
}