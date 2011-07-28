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
	using Messages;
	using Stact;
	using log4net;

	public class EndpointSubscriptionMessageProducer :
		Actor
	{
		static readonly ILog _log = LogManager.GetLogger(typeof (EndpointSubscriptionMessageProducer));
		
		readonly UntypedChannel _output;
		readonly Guid _clientId;
		readonly Uri _endpointUri;
		long _lastMessageNumber;

		public EndpointSubscriptionMessageProducer(Guid clientId, Uri endpointUri, UntypedChannel output)
		{
			_clientId = clientId;
			_output = output;
			_endpointUri = endpointUri;
		}

		public void Handle(Message<SubscriptionAdded> message)
		{
			var messageNumber = ++_lastMessageNumber;

			var add = new AddSubscriptionMessage
				{
					PeerId = _clientId,
					MessageNumber = messageNumber,
					EndpointUri = _endpointUri,
					SubscriptionId = message.Body.SubscriptionId,
					MessageName = message.Body.MessageName,
				};

			if (_log.IsDebugEnabled)
				_log.DebugFormat("AddSubscription: {0}, {1}", add.MessageName, add.SubscriptionId);

			_output.Send<AddSubscription>(add);
		}

		public void Handle(Message<SubscriptionRemoved> message)
		{
			var messageNumber = ++_lastMessageNumber;

			var remove = new RemoveSubscriptionMessage
			{
				PeerId = _clientId,
				MessageNumber = messageNumber,
				EndpointUri = _endpointUri,
				SubscriptionId = message.Body.SubscriptionId,
				MessageName = message.Body.MessageName,
			};

			if (_log.IsDebugEnabled)
				_log.DebugFormat("RemoveSubscription: {0}, {1}", remove.MessageName, remove.SubscriptionId);

			_output.Send<RemoveSubscription>(remove);
		}
	}
}