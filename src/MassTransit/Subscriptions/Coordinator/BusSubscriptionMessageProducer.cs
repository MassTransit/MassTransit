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
	using System.Threading;
	using Messages;
	using Stact;
	using log4net;

	public class BusSubscriptionMessageProducer :
		BusSubscriptionEventObserver
	{
		static readonly ILog _log = LogManager.GetLogger(typeof (BusSubscriptionMessageProducer));

		readonly UntypedChannel _output;
		readonly Guid _peerId;
		long _lastMessageNumber;

		public BusSubscriptionMessageProducer(Guid peerId, UntypedChannel output)
		{
			_peerId = peerId;
			_output = output;
		}

		public void OnSubscriptionAdded(SubscriptionAdded message)
		{
			long messageNumber = Interlocked.Increment(ref _lastMessageNumber);

			var add = new AddPeerSubscriptionMessage
				{
					PeerId = _peerId,
					MessageNumber = messageNumber,
					SubscriptionId = message.SubscriptionId,
					EndpointUri = message.EndpointUri,
					MessageName = message.MessageName,
				};

			if (_log.IsDebugEnabled)
				_log.DebugFormat("AddSubscription: {0}, {1}", add.MessageName, add.SubscriptionId);

			_output.Send(add);
		}

		public void OnSubscriptionRemoved(SubscriptionRemoved message)
		{
			long messageNumber = Interlocked.Increment(ref _lastMessageNumber);

			var remove = new RemovePeerSubscriptionMessage
				{
					PeerId = _peerId,
					MessageNumber = messageNumber,
					SubscriptionId = message.SubscriptionId,
					EndpointUri = message.EndpointUri,
					MessageName = message.MessageName,
				};

			if (_log.IsDebugEnabled)
				_log.DebugFormat("RemoveSubscription: {0}, {1}", remove.MessageName, remove.SubscriptionId);

			_output.Send(remove);
		}
	}
}