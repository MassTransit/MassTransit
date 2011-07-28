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
	using Messages;
	using Services.Subscriptions.Messages;
	using Stact;

	public class SubscriptionMessageConsumer :
		Consumes<AddSubscriptionClient>.All,
		Consumes<RemoveSubscriptionClient>.All,
		Consumes<SubscriptionRefresh>.All,
		Consumes<Services.Subscriptions.Messages.AddSubscription>.All,
		Consumes<Services.Subscriptions.Messages.RemoveSubscription>.All
	{
		readonly UntypedChannel _output;

		public SubscriptionMessageConsumer(UntypedChannel output)
		{
			_output = output;
		}

		public void Consume(Services.Subscriptions.Messages.AddSubscription message)
		{
			_output.Send(new AddSubscriptionMessage
				{
					PeerId = message.Subscription.ClientId,
					EndpointUri = message.Subscription.EndpointUri,
					MessageName = message.Subscription.MessageName,
					MessageNumber = message.Subscription.SequenceNumber,
					SubscriptionId = message.Subscription.SubscriptionId,
				});
		}

		public void Consume(AddSubscriptionClient message)
		{
			_output.Send(new AddSubscriptionPeerMessage
				{
					PeerId = message.CorrelationId,
					DataUri = message.DataUri,
					ControlUri = message.ControlUri,
				});
		}

		public void Consume(Services.Subscriptions.Messages.RemoveSubscription message)
		{
			_output.Send(new RemoveSubscriptionMessage
				{
					PeerId = message.Subscription.ClientId,
					EndpointUri = message.Subscription.EndpointUri,
					MessageName = message.Subscription.MessageName,
					MessageNumber = message.Subscription.SequenceNumber,
					SubscriptionId = message.Subscription.SubscriptionId,
				});
		}

		public void Consume(RemoveSubscriptionClient message)
		{
			_output.Send(new RemoveSubscriptionPeerMessage
				{
					PeerId = message.CorrelationId,
					DataUri = message.DataUri,
					ControlUri = message.ControlUri,
				});
		}

		public void Consume(SubscriptionRefresh message)
		{
			foreach (SubscriptionInformation subscription in message.Subscriptions)
			{
				// TODO do we trust subscriptions that are third-party (sent to us from systems that are not the
				// system containing the actual subscription)

				_output.Send(new AddSubscriptionMessage
					{
						PeerId = subscription.ClientId,
						EndpointUri = subscription.EndpointUri,
						MessageName = subscription.MessageName,
						MessageNumber = subscription.SequenceNumber,
						SubscriptionId = subscription.SubscriptionId,
					});
			}
		}
	}
}