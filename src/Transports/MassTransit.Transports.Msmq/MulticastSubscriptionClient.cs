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
	using Subscriptions.Coordinator;
	using Subscriptions.Messages;
	using log4net;

	public class MulticastSubscriptionClient :
		BusSubscriptionEventObserver
	{
		static readonly ILog _log = LogManager.GetLogger(typeof (MulticastSubscriptionClient));
		readonly BusSubscriptionCoordinator _coordinator;
		readonly string _network;
		BusSubscriptionMessageProducer _producer;
		IServiceBus _subscriptionBus;
		UnsubscribeAction _unsubscribeAction;

		public MulticastSubscriptionClient(IServiceBus subscriptionBus, BusSubscriptionCoordinator coordinator)
		{
			_subscriptionBus = subscriptionBus;
			_coordinator = coordinator;
			_network = coordinator.Network;

			if (_log.IsDebugEnabled)
				_log.DebugFormat("Starting MulticastSubscriptionService using {0}", subscriptionBus.Endpoint.Address);

			var consumerInstance = new SubscriptionMessageConsumer(_coordinator, _network);

			_unsubscribeAction = _subscriptionBus.SubscribeInstance(consumerInstance);

			_producer = new BusSubscriptionMessageProducer(coordinator, subscriptionBus.Endpoint);
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