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
    using Magnum.Caching;
    using Magnum.Extensions;
    using Messages;
    using Stact;
    using Util;
    using log4net;

    public class PeerCache :
        Actor
    {
        static readonly ILog _log = LogManager.GetLogger(typeof (PeerCache));
        readonly Fiber _fiber;
        readonly ActorFactory<PeerHandler> _peerHandlerFactory;
        readonly Cache<Guid, Uri> _peerIds;
        readonly Uri _peerUri;
        readonly Cache<Uri, ActorInstance> _peers;
        readonly Scheduler _scheduler;

        public PeerCache(Fiber fiber, Scheduler scheduler, SubscriptionObserver observer, Guid clientId,
                         Uri controlUri)
        {
            _peers = new DictionaryCache<Uri, ActorInstance>();
            _peerUri = controlUri;
            _fiber = fiber;
            _scheduler = scheduler;
            _peerIds = new DictionaryCache<Guid, Uri>();

            _peerHandlerFactory = ActorFactory.Create((f, s, i) => new PeerHandler(f, s, i, observer));

            // create a peer for our local client
            WithPeer(clientId, controlUri, x => { }, true);
        }

        [UsedImplicitly]
        public void Handle(Message<Stop> message)
        {
            try
            {
                _peers.Each(x => x.ExitOnDispose(30.Seconds()).Dispose());
            }
            catch (Exception ex)
            {
                _log.Error("Message<Stop> Exception", ex);
            }
        }

        [UsedImplicitly]
        public void Handle(Message<AddPeer> message)
        {
            try
            {
                WithPeer(message.Body.PeerId, message.Body.PeerUri, x =>
                    {
                        if (_log.IsInfoEnabled)
                            _log.InfoFormat("AddPeer: {0}, {1}", message.Body.PeerId, message.Body.PeerUri);

                        x.Send(message);
                    }, true);
            }
            catch (Exception ex)
            {
                _log.Error("Message<AddPeer> Exception", ex);
            }
        }

        [UsedImplicitly]
        public void Handle(Message<RemovePeer> message)
        {
            try
            {
                WithPeer(message.Body.PeerId, message.Body.PeerUri, x =>
                    {
                        if (_log.IsInfoEnabled)
                            _log.InfoFormat("RemovePeer: {0}, {1}", message.Body.PeerId, message.Body.PeerUri);

                        x.Send(message);
                    }, false);
            }
            catch (Exception ex)
            {
                _log.Error("Message<RemovePeer> Exception", ex);
            }
        }

        [UsedImplicitly]
        public void Handle(Message<AddPeerSubscription> message)
        {
            try
            {
                WithPeer(message.Body.PeerId, x =>
                    {
                        if (_log.IsInfoEnabled)
                            _log.InfoFormat("AddPeerSubscription: {0}, {1} - {2}", message.Body.MessageName,
                                message.Body.SubscriptionId,
                                message.Body.PeerId);

                        x.Send(message);
                    });
            }
            catch (Exception ex)
            {
                _log.Error("Message<AddPeerSubscription> Exception", ex);
            }
        }

        [UsedImplicitly]
        public void Handle(Message<RemovePeerSubscription> message)
        {
            try
            {
                WithPeer(message.Body.PeerId, x =>
                    {
                        if (_log.IsInfoEnabled)
                            _log.InfoFormat("RemovePeerSubscription: {0}, {1} - {2}", message.Body.MessageName,
                                message.Body.SubscriptionId,
                                message.Body.PeerId);

                        x.Send(message);
                    });
            }
            catch (Exception ex)
            {
                _log.Error("Message<RemovePeerSubscription> Exception", ex);
            }
        }

        void WithPeer(Guid peerId, Action<ActorInstance> callback)
        {
            _peerIds.WithValue(peerId, peerUri => { WithPeer(peerId, peerUri, callback, false); });
        }

        void WithPeer(Guid peerId, Uri controlUri, Action<ActorInstance> callback, bool createIfMissing)
        {
            bool found = _peers.Has(controlUri);
            if (!found)
            {
                if (!createIfMissing)
                {
                    return;
                }

                ActorInstance peer = _peerHandlerFactory.GetActor();
                peer.Send(new InitializePeerHandler(peerId, controlUri));
                _peers.Add(controlUri, peer);
                _peerIds[peerId] = controlUri;
            }
            else
            {
                if (!_peerIds.Has(peerId))
                    _peerIds[peerId] = controlUri;
            }

            callback(_peers[controlUri]);
        }
    }
}