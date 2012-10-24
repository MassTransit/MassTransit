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
    using Messages;
    using Stact;
    using Util;

    public class PeerHandler :
        Actor
    {
        static readonly ILog _log = Logger.Get(typeof(PeerHandler));

        readonly SubscriptionObserver _observer;
        EndpointSubscriptionCache _endpointSubscriptionCache;
        Guid _peerId;
        Uri _peerUri;

        public PeerHandler(Inbox inbox, SubscriptionObserver observer,
            SubscriptionRepository repository)
        {
            _observer = observer;

            inbox.Receive<InitializePeerHandler>(init =>
                {
                    _peerId = init.PeerId;
                    _peerUri = init.PeerUri;

                    _endpointSubscriptionCache = new EndpointSubscriptionCache(observer, _peerUri,
                        repository);
                });
        }

        [UsedImplicitly]
        public void Handle(Message<AddPeer> message)
        {
            try
            {
                if (_endpointSubscriptionCache != null)
                    _endpointSubscriptionCache.Send(message.Body);
            }
            catch (Exception ex)
            {
                _log.Error("PeerException(" + _peerUri + "): AddPeer", ex);
            }
        }

        [UsedImplicitly]
        public void Handle(Message<RemovePeer> message)
        {
            try
            {
                if (_endpointSubscriptionCache != null)
                    _endpointSubscriptionCache.Send(message.Body);
            }
            catch (Exception ex)
            {
                _log.Error("PeerException(" + _peerUri + "): RemovePeer", ex);
            }
        }

        [UsedImplicitly]
        public void Handle(Message<AddPeerSubscription> message)
        {
            try
            {
                if (_endpointSubscriptionCache != null)
                    _endpointSubscriptionCache.Send(message.Body);
            }
            catch (Exception ex)
            {
                _log.Error("PeerException(" + _peerUri + "): AddPeerSubscription", ex);
            }
        }

        [UsedImplicitly]
        public void Handle(Message<RemovePeerSubscription> message)
        {
            try
            {
                if (_endpointSubscriptionCache != null)
                    _endpointSubscriptionCache.Send(message.Body);
            }
            catch (Exception ex)
            {
                _log.Error("PeerException(" + _peerUri + "): RemovePeerSubscription", ex);
            }
        }
    }
}