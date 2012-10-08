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
    using Exceptions;
    using Logging;
    using MassTransit.Subscriptions.Coordinator;
    using MassTransit.Subscriptions.Messages;
    using Messages;

    /// <summary>
    /// The subscription client is responsible for exchanging subscription information between a local bus and the central subscription coordinator.
    /// </summary>
    public class SubscriptionClient :
        SubscriptionObserver
    {
        static readonly ILog _log = Logger.Get(typeof (SubscriptionClient));
        readonly IServiceBus _bus;
        readonly SubscriptionRouter _router;
        readonly string _network;
        readonly SubscriptionServiceMessageProducer _producer;
        readonly ManualResetEvent _ready = new ManualResetEvent(false);
        readonly TimeSpan _startTimeout;
        readonly Uri _subscriptionServiceUri;
        UnsubscribeAction _unsubscribeAction;
        readonly IEndpoint _subscriptionEndpoint;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionClient"/> class.
        /// </summary>
        /// <param name="bus">The bus.</param>
        /// <param name="router">The router.</param>
        /// <param name="subscriptionServiceUri">The subscription service URI.</param>
        /// <param name="startTimeout">The start timeout.</param>
        public SubscriptionClient(IServiceBus bus, SubscriptionRouter router, Uri subscriptionServiceUri, TimeSpan startTimeout)
        {
            _bus = bus;
            _router = router;
            _subscriptionServiceUri = subscriptionServiceUri;
            _startTimeout = startTimeout;
            _network = router.Network;

            if (_log.IsDebugEnabled)
                _log.DebugFormat("Starting SubscriptionClient using {0}", subscriptionServiceUri);

            VerifyClientAndServiceNotOnSameEndpoint(bus);

            _ready.Reset();

            var consumerInstance = new SubscriptionMessageConsumer(_router, _network);

            _unsubscribeAction = _bus.ControlBus.SubscribeInstance(consumerInstance);
            _unsubscribeAction += _bus.ControlBus.SubscribeContextHandler<SubscriptionRefresh>(Consume);

            _subscriptionEndpoint = _bus.GetEndpoint(subscriptionServiceUri);
            _producer = new SubscriptionServiceMessageProducer(router, _subscriptionEndpoint);

            WaitForSubscriptionServiceResponse();
        }

        /// <summary>
        /// Called when a subscription was added.
        /// </summary>
        /// <param name="message"></param>
        public void OnSubscriptionAdded(SubscriptionAdded message)
        {
            _producer.OnSubscriptionAdded(message);
        }

        /// <summary>
        /// Called when a subscription was removed.
        /// </summary>
        /// <param name="message">The message.</param>
        public void OnSubscriptionRemoved(SubscriptionRemoved message)
        {
            _producer.OnSubscriptionRemoved(message);
        }

        /// <summary>
        /// Called when the observation is complete and we should go away
        /// </summary>
        public void OnComplete()
        {
            if (_unsubscribeAction != null)
            {
                _unsubscribeAction();
                _unsubscribeAction = null;
            }

            _producer.OnComplete();

            using(_ready)
            {
                _ready.Close();
            }
        }

        void Consume(IConsumeContext<SubscriptionRefresh> context)
        {
            var subscriptionUri = _subscriptionEndpoint.Address.Uri.AbsoluteUri;
            if(_subscriptionEndpoint.Address.Uri.Query.Length > 0)
                subscriptionUri = subscriptionUri.Replace(_subscriptionEndpoint.Address.Uri.Query, "");

            var sourceUri = context.SourceAddress.AbsoluteUri;
            if (context.SourceAddress.Query.Length > 0)
                sourceUri = sourceUri.Replace(context.SourceAddress.Query, "");

            if (subscriptionUri.Equals(sourceUri))
            {
                if (_log.IsDebugEnabled)
                    _log.DebugFormat("SubscriptionRefresh received, setting ready event");

                _ready.Set();
            }
            else
            {
                if (_log.IsErrorEnabled)
                    _log.ErrorFormat("SubscriptionRefresh source address invalid: {0} (expected {1})",
                        sourceUri,
                        subscriptionUri);
                
            }
        }

        void WaitForSubscriptionServiceResponse()
        {
            if (_log.IsDebugEnabled)
                _log.Debug("Waiting for response from the subscription service");

            bool received = _ready.WaitOne(_startTimeout);
            if (!received)
            {
                throw new InvalidOperationException("Timeout waiting for subscription service to respond");
            }
        }

        void VerifyClientAndServiceNotOnSameEndpoint(IServiceBus bus)
        {
            Uri controlUri = bus.ControlBus.Endpoint.Address.Uri;

            if (!controlUri.Equals(_subscriptionServiceUri))
                return;

            string message = "The service bus and subscription service cannot use the same endpoint: " +
                             controlUri;

            throw new EndpointException(controlUri, message);
        }
    }
}