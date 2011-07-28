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
		readonly IDictionary<Guid, Uri> _clients;
		readonly ActorFactory<PeerSubscriptionActor> _factory;
		readonly IDictionary<Uri, ActorInstance> _peers;

		public PeerCache(UntypedChannel output)
		{
			_peers = new Dictionary<Uri, ActorInstance>();
			_clients = new Dictionary<Guid, Uri>();

			_factory = ActorFactory.Create((f, s, i) => new PeerSubscriptionActor(i, output));
		}

		[UsedImplicitly]
		public void Handle(Message<Stop> message)
		{
			_peers.Values.Each(x => x.ExitOnDispose(30.Seconds()).Dispose());
		}

		public void Handle(Message<AddSubscription> message)
		{
			Uri clientUri;
			if (_clients.TryGetValue(message.Body.PeerId, out clientUri))
			{
				ActorInstance peer;
				if (!_peers.TryGetValue(clientUri, out peer))
				{
					peer = _factory.GetActor();
					peer.Send(new InitializeSubscriptionPeer(message.Body.PeerId, clientUri));
					_peers.Add(clientUri, peer);
				}

				peer.Send(message);

				if (_log.IsDebugEnabled)
					_log.DebugFormat("AddSubscription: {0}, {1} - {2}", message.Body.MessageName, message.Body.SubscriptionId,
						message.Body.PeerId);
			}
		}
	}
}