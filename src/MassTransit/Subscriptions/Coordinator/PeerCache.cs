// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using Logging;
    using Magnum.Caching;
    using Magnum.Extensions;
    using Messages;
    using Stact;
    using Util;

    public class PeerCache :
        Actor
    {
        static readonly ILog _log = Logger.Get(typeof(PeerCache));
        readonly ActorFactory<PeerHandler> _peerHandlerFactory;
        readonly Cache<Guid, Uri> _peerIds;
        readonly Uri _peerUri;
        readonly Cache<Uri, ActorRef> _peers;

        public PeerCache(SubscriptionObserver observer, Guid clientId, Uri controlUri, SubscriptionRepository repository)
        {
            _peers = new DictionaryCache<Uri, ActorRef>();
            _peerUri = controlUri;
            _peerIds = new DictionaryCache<Guid, Uri>();

            _peerHandlerFactory = ActorFactory.Create((f, s, i) => new PeerHandler(i, observer, repository));

            // create a peer for our local client
            WithPeer(clientId, controlUri, x => { }, true);
        }

        [UsedImplicitly]
        public void Handle(Message<StopSubscriptionRouterService> message)
        {
            try
            {
                _peers.Each(x => x.SendRequestWaitForResponse<Exit>(new ExitImpl(), 30.Seconds()));
            }
            catch (Exception ex)
            {
                _log.Error("Message<StopSubscriptionRouterService> Exception", ex);
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
                        if (_log.IsDebugEnabled)
                            _log.DebugFormat("RemovePeer: {0}, {1}", message.Body.PeerId, message.Body.PeerUri);

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
                        if (_log.IsDebugEnabled)
                            _log.DebugFormat("AddPeerSubscription: {0}, {1} - {2}", message.Body.MessageName,
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
                        if (_log.IsDebugEnabled)
                            _log.DebugFormat("RemovePeerSubscription: {0}, {1} - {2}", message.Body.MessageName,
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

        void WithPeer(Guid peerId, Action<ActorRef> callback)
        {
            _peerIds.WithValue(peerId, peerUri => { WithPeer(peerId, peerUri, callback, false); });
        }

        void WithPeer(Guid peerId, Uri controlUri, Action<ActorRef> callback, bool createIfMissing)
        {
            bool found = _peers.Has(controlUri);
            if (!found)
            {
                if (!createIfMissing)
                {
                    return;
                }

                ActorRef peer = _peerHandlerFactory.GetActor();
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

        class ExitImpl : Exit
        {
        }
    }
}