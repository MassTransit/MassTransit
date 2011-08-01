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
	using Magnum;
	using Messages;
	using Services.Subscriptions.Messages;
	using log4net;

	public class SubscriptionLoopback :
		BusSubscriptionEventObserver
	{
		static readonly ILog _log = LogManager.GetLogger(typeof (SubscriptionLoopback));
		
		readonly BusSubscriptionCoordinator _coordinator;
		readonly Guid _peerId;
		readonly List<Action<BusSubscriptionCoordinator>> _waiting;
		readonly BusSubscriptionConnector _connector;
		long _messageNumber;
		BusSubscriptionCoordinator _targetCoordinator;
		readonly HashSet<string> _ignoredMessageTypes;

		public SubscriptionLoopback(IServiceBus bus, BusSubscriptionCoordinator coordinator)
		{
			_coordinator = coordinator;
			_peerId = CombGuid.Generate();

			_waiting = new List<Action<BusSubscriptionCoordinator>>();
			_connector = new BusSubscriptionConnector(bus);

			_ignoredMessageTypes = IgnoredMessageTypes();

			WithTarget(x =>
				{
					if (_log.IsDebugEnabled)
						_log.DebugFormat("Send AddPeer: {0}, {1}", _peerId, bus.ControlBus.Endpoint.Address.Uri);

					x.Send(new AddPeerMessage
						{
							PeerId = _peerId,
							PeerUri = bus.ControlBus.Endpoint.Address.Uri,
							Timestamp = DateTime.UtcNow.Ticks,
						});
				});
		}

		public BusSubscriptionCoordinator Coordinator
		{
			get { return _coordinator; }
		}

		public void OnSubscriptionAdded(SubscriptionAdded message)
		{
			_connector.OnSubscriptionAdded(message);

			if (_ignoredMessageTypes.Contains(message.MessageName))
				return;

			WithTarget(x =>
				{
					if (_log.IsDebugEnabled)
						_log.DebugFormat("Send AddPeerSubscription: {0}, {1}", _peerId, message.MessageName);

					x.Send(new AddPeerSubscriptionMessage
						{
							PeerId = _peerId,
							MessageNumber = ++_messageNumber,
							EndpointUri = message.EndpointUri,
							MessageName = message.MessageName,
							SubscriptionId = message.SubscriptionId,
						});
				});
		}

		public void OnSubscriptionRemoved(SubscriptionRemoved message)
		{
			_connector.OnSubscriptionRemoved(message);

			if (_ignoredMessageTypes.Contains(message.MessageName))
				return;

			WithTarget(x => x.Send(new RemovePeerSubscriptionMessage
				{
					PeerId = _peerId,
					MessageNumber = ++_messageNumber,
					EndpointUri = message.EndpointUri,
					MessageName = message.MessageName,
					SubscriptionId = message.SubscriptionId,
				}));
		}

		public void OnComplete()
		{
		}

		public void SetTargetCoordinator(BusSubscriptionCoordinator targetCoordinator)
		{
			lock (this)
			{
				_targetCoordinator = targetCoordinator;
				_waiting.ForEach(x => x(_targetCoordinator));
				_waiting.Clear();
			}
		}

		void WithTarget(Action<BusSubscriptionCoordinator> callback)
		{
			lock (this)
			{
				if (_targetCoordinator == null)
				{
					_waiting.Add(callback);
					return;
				}

				callback(_targetCoordinator);
			}
		}

		HashSet<string> IgnoredMessageTypes()
		{
			var ignoredMessageTypes = new HashSet<string>
				{
					typeof (AddSubscription).ToMessageName(),
					typeof (RemoveSubscription).ToMessageName(),
					typeof (AddSubscriptionClient).ToMessageName(),
					typeof (RemoveSubscriptionClient).ToMessageName(),
					typeof (SubscriptionRefresh).ToMessageName()
				};

			return ignoredMessageTypes;
		}

	}
}