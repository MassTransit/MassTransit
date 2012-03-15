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
    using Logging;
    using Magnum.Caching;
    using Messages;

    /// <summary>
    /// Connects received subscriptions to a local bus.
    /// The connector is responsible for picking between the data and the control bus.
    /// </summary>
    public class BusSubscriptionConnector :
        SubscriptionObserver
    {
        static readonly ILog _log = Logger.Get(typeof(BusSubscriptionConnector));
        readonly Cache<Guid, UnsubscribeAction> _connectionCache;
        readonly EndpointSubscriptionConnectorCache _controlBusSubscriptionCache;
        readonly EndpointSubscriptionConnectorCache _dataBusSubscriptionCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="BusSubscriptionConnector"/> class.
        /// </summary>
        /// <param name="bus">The bus.</param>
        public BusSubscriptionConnector(IServiceBus bus)
        {
            _dataBusSubscriptionCache = new EndpointSubscriptionConnectorCache(bus);
            _controlBusSubscriptionCache = new EndpointSubscriptionConnectorCache(bus.ControlBus);

            _connectionCache = new ConcurrentCache<Guid, UnsubscribeAction>();
        }

        /// <summary>
        /// Adds a remote subscription to the route path or a local data or control bus 
        /// </summary>
        /// <param name="message"></param>
        public void OnSubscriptionAdded(SubscriptionAdded message)
        {
            // determine whether the message should be send over the control bus
            bool isControlMessage = message.EndpointUri.IsControlAddress();

            // connect the message to the correct cache
            if (!isControlMessage)
                _connectionCache[message.SubscriptionId] = _dataBusSubscriptionCache.Connect(message.MessageName,
                    message.EndpointUri, message.CorrelationId);
            else
                _connectionCache[message.SubscriptionId] = _controlBusSubscriptionCache.Connect(message.MessageName,
                    message.EndpointUri, message.CorrelationId);

            _log.Debug(() => string.Format("Added: {0} => {1}, {2}", message.MessageName, message.EndpointUri,
                message.SubscriptionId));
        }

        /// <summary>
        /// Removes a remote subscription from the route path or a local data or control bus 
        /// </summary>
        /// <param name="message"></param>
        public void OnSubscriptionRemoved(SubscriptionRemoved message)
        {
            _connectionCache.WithValue(message.SubscriptionId, unsubscribe =>
                {
                    unsubscribe();
                    _connectionCache.Remove(message.SubscriptionId);

                    _log.Debug(() => string.Format("Removed: {0} => {1}, {2}", message.MessageName, message.EndpointUri,
                        message.SubscriptionId));
                });
        }

        public void OnComplete()
        {
        }
    }
}