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
	using System.Collections.Generic;
	using MassTransit.Subscriptions.Messages;
	using MassTransit.Services.Subscriptions.Messages;
	using Stact;
	using log4net;

	public class SubscriptionMessageConsumer :
		Consumes<AddSubscriptionClient>.Context,
		Consumes<RemoveSubscriptionClient>.Context,
		Consumes<SubscriptionRefresh>.Context,
		Consumes<Services.Subscriptions.Messages.AddSubscription>.Context,
		Consumes<Services.Subscriptions.Messages.RemoveSubscription>.Context
	{
		static readonly ILog _log = LogManager.GetLogger(typeof (SubscriptionMessageConsumer));
		readonly HashSet<Uri> _ignoredSourceAddresses;
		readonly string _network;
		readonly UntypedChannel _output;

		public SubscriptionMessageConsumer(UntypedChannel output, string network, params Uri[] ignoredSourceAddresses)
		{
			_output = output;
			_network = network;
			_ignoredSourceAddresses = new HashSet<Uri>(ignoredSourceAddresses);
		}

		public void Consume(IConsumeContext<Services.Subscriptions.Messages.AddSubscription> context)
		{
			if (DiscardMessage(context))
				return;

			_output.Send(new AddSubscriptionMessage
				{
					PeerId = context.Message.Subscription.ClientId,
					EndpointUri = context.Message.Subscription.EndpointUri,
					MessageName = context.Message.Subscription.MessageName,
					MessageNumber = context.Message.Subscription.SequenceNumber,
					SubscriptionId = context.Message.Subscription.SubscriptionId,
				});
		}

		public void Consume(IConsumeContext<AddSubscriptionClient> context)
		{
			if (DiscardMessage(context))
				return;

			_output.Send(new AddSubscriptionPeerMessage
				{
					PeerId = context.Message.CorrelationId,
					DataUri = context.Message.DataUri,
					ControlUri = context.Message.ControlUri,
				});
		}

		public void Consume(IConsumeContext<Services.Subscriptions.Messages.RemoveSubscription> context)
		{
			if (DiscardMessage(context))
				return;

			_output.Send(new RemoveSubscriptionMessage
				{
					PeerId = context.Message.Subscription.ClientId,
					EndpointUri = context.Message.Subscription.EndpointUri,
					MessageName = context.Message.Subscription.MessageName,
					MessageNumber = context.Message.Subscription.SequenceNumber,
					SubscriptionId = context.Message.Subscription.SubscriptionId,
				});
		}

		public void Consume(IConsumeContext<RemoveSubscriptionClient> context)
		{
			if (DiscardMessage(context))
				return;

			_output.Send(new RemoveSubscriptionPeerMessage
				{
					PeerId = context.Message.CorrelationId,
					DataUri = context.Message.DataUri,
					ControlUri = context.Message.ControlUri,
				});
		}

		public void Consume(IConsumeContext<SubscriptionRefresh> context)
		{
			if (DiscardMessage(context))
				return;

			foreach (SubscriptionInformation subscription in context.Message.Subscriptions)
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

		bool DiscardMessage<T>(IConsumeContext<T> context)
			where T : class
		{
			if (_ignoredSourceAddresses.Contains(context.SourceAddress))
			{
				_log.Debug("Ignoring subscription because its source address equals the busses address");
				return true;
			}

			if (!string.Equals(context.Network, _network))
			{
				_log.DebugFormat("Ignoring subscription because the network '{0}' != ours '{1}1", context.Network, _network);
				return true;
			}

			return false;
		}
	}
}