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
namespace MassTransit.Subscriptions.Actors
{
	using System;
	using Magnum;
	using Pipeline;
	using Stact;

	public class SubscriptionEventChannelPublisher :
		ISubscriptionEvent
	{
		UntypedChannel _output;

		public SubscriptionEventChannelPublisher(UntypedChannel output)
		{
			_output = output;
		}

		public UnsubscribeAction SubscribedTo<TMessage>()
			where TMessage : class
		{
			return Subscribe<TMessage>(null);
		}

		public UnsubscribeAction SubscribedTo<TMessage, TKey>(TKey correlationId)
			where TMessage : class, CorrelatedBy<TKey>
		{
			return Subscribe<TMessage>(string.Format("{0}", correlationId));
		}

		UnsubscribeAction Subscribe<TMessage>(string correlationId)
			where TMessage : class
		{
			Guid subscriptionId = CombGuid.Generate();

			string messageName = typeof (TMessage).ToMessageName();

			var subscribeTo = new SubscribeTo
				{
					SubscriptionId = subscriptionId,
					MessageName = messageName,
					CorrelationId = correlationId,
				};

			_output.Send(subscribeTo);

			return () => Unsubscribe(subscriptionId, messageName, correlationId);
		}

		bool Unsubscribe(Guid subscriptionId, string messageName, string correlationId)
		{
			var unsubscribeFrom = new UnsubscribeFrom
				{
					SubscriptionId = subscriptionId,
					MessageName = messageName,
					CorrelationId = correlationId,
				};

			_output.Send(unsubscribeFrom);

			return true;
		}
	}
}