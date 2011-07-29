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
namespace MassTransit.Subscriptions.Coordinator
{
	using System;
	using Services.Subscriptions.Messages;
	using Stact;
	using Util;

	public class SubscriptionClientSubscriptionWriter :
		Actor
	{
		readonly IEndpoint _endpoint;
		readonly string _network;
		readonly Uri _sourceAddress;

		public SubscriptionClientSubscriptionWriter(IEndpoint endpoint, Uri sourceAddress, string network)
		{
			_endpoint = endpoint;
			_sourceAddress = sourceAddress;
			_network = network;
		}

		[UsedImplicitly]
		public void Handle(Message<Messages.AddPeerSubscription> message)
		{
			var subscriptionInformation = new SubscriptionInformation(message.Body.PeerId,
				message.Body.MessageNumber, message.Body.MessageName, null, message.Body.EndpointUri);

			subscriptionInformation.SubscriptionId = message.Body.SubscriptionId;

			Send(new AddSubscription(subscriptionInformation));
		}

		[UsedImplicitly]
		public void Handle(Message<Messages.RemovePeerSubscription> message)
		{
			var subscriptionInformation = new SubscriptionInformation(message.Body.PeerId,
				message.Body.MessageNumber, message.Body.MessageName, null, message.Body.EndpointUri);

			subscriptionInformation.SubscriptionId = message.Body.SubscriptionId;

			Send(new RemoveSubscription(subscriptionInformation));
		}

		void Send<T>(T message)
			where T : class
		{
			_endpoint.Send(message, context =>
				{
					context.SetSourceAddress(_sourceAddress);
					context.SetNetwork(_network);
				});
		}
	}
}