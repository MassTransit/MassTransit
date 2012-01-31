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

using MassTransit.Util;

namespace MassTransit.Subscriptions.Coordinator
{
	using Messages;

	/// <summary>
	/// Implemented by observers of subscriptions - subscriptions are locally published inside of the bus,
	/// so if you're implementing a transport; implementing this interface will allow you to bind whatever
	/// topics/queues/exchanges/etc to your inbound queue 
	/// (e.g. if doing equivalent of RMQ:publish w/ fanout, MSMQ: multicast, ZMQ: PGM). 
	/// This interface is also used internally to route subscriptions and manage their lifestyles.
	/// </summary>
	public interface SubscriptionObserver
	{
		/// <summary>
		/// Called when a subscription is registered in the service bus.
		/// </summary>
		/// <param name="message">The subscription added message.</param>
		void OnSubscriptionAdded([NotNull] SubscriptionAdded message);

		/// <summary>
		/// Called when a subscription is unregistered in the service bus.
		/// </summary>
		/// <param name="message">The subscription removed message.</param>
		void OnSubscriptionRemoved([NotNull] SubscriptionRemoved message);

		/// <summary>
		/// Called when the observation is complete and we should go away
		/// </summary>
		void OnComplete();
	}
}