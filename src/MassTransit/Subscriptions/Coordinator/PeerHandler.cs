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
    using Messages;
    using Stact;
    using Util;
    using log4net;

    public class PeerHandler :
        Actor
    {
        static readonly ILog _log = LogManager.GetLogger(typeof (PeerHandler));

        readonly EndpointSubscriptionCache _endpointSubscriptionCache;
        readonly SubscriptionObserver _observer;
        Guid _peerId;
        Uri _peerUri;

        public PeerHandler(Fiber fiber, Scheduler scheduler, Inbox inbox, SubscriptionObserver observer)
        {
            _observer = observer;
            _endpointSubscriptionCache = new EndpointSubscriptionCache(fiber, scheduler, observer);

            inbox.Receive<InitializePeerHandler>(init =>
                {
                    _peerId = init.PeerId;
                    _peerUri = init.PeerUri;
                });
        }

        [UsedImplicitly]
        public void Handle(Message<AddPeer> message)
        {
            try
            {
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
                _endpointSubscriptionCache.Send(message.Body);
            }
            catch (Exception ex)
            {
                _log.Error("PeerException(" + _peerUri + "): RemovePeerSubscription", ex);
            }
        }
    }
}