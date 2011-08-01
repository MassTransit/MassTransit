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
	using Magnum.Extensions;
	using Messages;
	using Stact;
	using Util;
	using log4net;

	public class PeerCache :
		Actor
	{
		static readonly ILog _log = LogManager.GetLogger(typeof (PeerCache));
		readonly ActorFactory<PeerHandler> _peerHandlerFactory;
		readonly IDictionary<Guid, Uri> _peerIds;
		readonly IDictionary<Uri, ActorInstance> _peers;
		readonly EndpointSubscriptionCache _endpointSubscriptionCache;

		public PeerCache(BusSubscriptionEventObserver observer, Guid clientId, Uri controlUri)
		{
			_peers = new Dictionary<Uri, ActorInstance>();
			_peerIds = new Dictionary<Guid, Uri>();
			_endpointSubscriptionCache = new EndpointSubscriptionCache(observer);

			_peerHandlerFactory = ActorFactory.Create((f, s, i) => new PeerHandler(i, observer));

			// create a peer for our local client
			WithPeer(clientId, controlUri, x => { }, true);
		}

		[UsedImplicitly]
		public void Handle(Message<Stop> message)
		{
			_peers.Values.Each(x => x.ExitOnDispose(30.Seconds()).Dispose());
		}

		public void Handle(Message<AddPeer> message)
		{
			WithPeer(message.Body.PeerId, message.Body.PeerUri, x =>
				{
					if(_log.IsDebugEnabled)
						_log.DebugFormat("AddPeer: {0}, {1}", message.Body.PeerId, message.Body.PeerUri);

					x.Send(message);
				}, true);
		}

		public void Handle(Message<RemovePeer> message)
		{
			_endpointSubscriptionCache.Send(message.Body);
			
			WithPeer(message.Body.PeerId, message.Body.PeerUri, x => x.Send(message), false);
		}

		public void Handle(Message<AddPeerSubscription> message)
		{
			WithPeer(message.Body.PeerId, x =>
				{
					if (_log.IsDebugEnabled)
						_log.DebugFormat("AddPeerSubscription: {0}, {1} - {2}", message.Body.MessageName, message.Body.SubscriptionId,
							message.Body.PeerId);

					_endpointSubscriptionCache.Send(message.Body);

					x.Send(message);
				});
		}

		public void Handle(Message<RemovePeerSubscription> message)
		{
			WithPeer(message.Body.PeerId, x =>
				{
					if (_log.IsDebugEnabled)
						_log.DebugFormat("RemovePeerSubscription: {0}, {1} - {2}", message.Body.MessageName, message.Body.SubscriptionId,
							message.Body.PeerId);

					_endpointSubscriptionCache.Send(message.Body);

					x.Send(message);
				});
		}

		void WithPeer(Guid peerId, Action<ActorInstance> callback)
		{
			Uri peerUri;
			if (_peerIds.TryGetValue(peerId, out peerUri))
			{
				WithPeer(peerId, peerUri, callback, false);
			}
		}

		void WithPeer(Guid peerId, Uri controlUri, Action<ActorInstance> callback, bool createIfMissing)
		{
			ActorInstance peer;
			if (!_peers.TryGetValue(controlUri, out peer) && createIfMissing)
			{
				peer = _peerHandlerFactory.GetActor();
				peer.Send(new InitializePeerHandler(peerId, controlUri));
				_peers.Add(controlUri, peer);
				_peerIds[peerId] = controlUri;
			}

			if (peer != null)
				callback(peer);
		}
	}
}