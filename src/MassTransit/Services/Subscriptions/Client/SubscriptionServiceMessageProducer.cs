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
namespace MassTransit.Services.Subscriptions.Client
{
    using System;
    using System.Threading;
    using Logging;
    using MassTransit.Subscriptions.Coordinator;
    using MassTransit.Subscriptions.Messages;
    using Messages;

    /// <summary>
    /// The message producer is used to send subscription updates to the central subscription coordinator.
    /// </summary>
    internal class SubscriptionServiceMessageProducer 
    {
        static readonly ILog _log = Logger.Get(typeof(SubscriptionServiceMessageProducer));
        readonly IEndpoint _endpoint;
        readonly string _network;

        readonly Guid _peerId;
        readonly Uri _peerUri;
        long _lastMessageNumber;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionServiceMessageProducer"/> class.
        /// </summary>
        /// <param name="router">The router.</param>
        /// <param name="endpoint">The endpoint.</param>
        public SubscriptionServiceMessageProducer(SubscriptionRouter router, IEndpoint endpoint)
        {
            _peerId = router.PeerId;
            _peerUri = router.PeerUri;
            _network = router.Network;
            _endpoint = endpoint;

            SendAddPeerMessage();
        }

        /// <summary>
        /// Called when a subscription was added.
        /// </summary>
        /// <param name="message"></param>
        public void OnSubscriptionAdded(SubscriptionAdded message)
        {
            long messageNumber = Interlocked.Increment(ref _lastMessageNumber);

            var subscription = new SubscriptionInformation(_peerId, messageNumber, message.MessageName, message.CorrelationId, message.EndpointUri)
                {SubscriptionId = message.SubscriptionId};
            
            var add = new AddSubscription(subscription);

            if (_log.IsDebugEnabled)
                _log.DebugFormat("AddSubscription: {0}, {1}", subscription.MessageName, subscription.SubscriptionId);

            _endpoint.Send(add, SetSendContext);
        }

        /// <summary>
        /// Called when a subscription was removed.
        /// </summary>
        /// <param name="message">The message.</param>
        public void OnSubscriptionRemoved(SubscriptionRemoved message)
        {
            long messageNumber = Interlocked.Increment(ref _lastMessageNumber);

            var subscription = new SubscriptionInformation(_peerId, messageNumber, message.MessageName, message.CorrelationId, message.EndpointUri)
                {SubscriptionId = message.SubscriptionId};

            var remove = new RemoveSubscription(subscription);

            if (_log.IsDebugEnabled)
                _log.DebugFormat("RemoveSubscription: {0}, {1}", subscription.MessageName, subscription.SubscriptionId);

            _endpoint.Send(remove, SetSendContext);
        }

        /// <summary>
        /// Called when the observation is complete and we should go away
        /// </summary>
        public void OnComplete()
        {
            _endpoint.Send(new RemoveSubscriptionClient(_peerId, _peerUri, _peerUri), SetSendContext);
        }

        void SendAddPeerMessage()
        {
            _endpoint.Send(new AddSubscriptionClient(_peerId, _peerUri, _peerUri), SetSendContext);
        }

        void SetSendContext<T>(ISendContext<T> context)
            where T : class
        {
            context.SetNetwork(_network);
            context.SetSourceAddress(_peerUri);
        }
    }
}