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
    using Stact;

    public class BusSubscriptionRepository :
        SubscriptionRepository
    {
        static readonly ILog _log = Logger.Get(typeof(BusSubscriptionRepository));

        readonly Fiber _fiber;
        readonly SubscriptionStorage _storage;

        public BusSubscriptionRepository(SubscriptionStorage storage)
        {
            _storage = storage;
            _fiber = new PoolFiber();

            _fiber.Add(LoadExistingSubscriptions);
        }

        public void Add(Guid peerId, Guid subscriptionId, Uri endpointUri, string messageName, string correlationId)
        {
            var subscription = new PersistentSubscription(peerId, subscriptionId, endpointUri, messageName,
                correlationId);

            _fiber.Add(() => Add(subscription));
        }

        public void Remove(Guid peerId, Guid subscriptionId, Uri endpointUri, string messageName, string correlationId)
        {
            var subscription = new PersistentSubscription(peerId, subscriptionId, endpointUri, messageName,
                correlationId);

            _fiber.Add(() => Remove(subscription));
        }

        void LoadExistingSubscriptions()
        {
        }

        void Add(PersistentSubscription subscription)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("SubscriptionRepository.Add: {0}, {1}", subscription.MessageName,
                    subscription.SubscriptionId);

            _storage.Add(subscription);
        }

        void Remove(PersistentSubscription subscription)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("SubscriptionRepository.Remove: {0}, {1}", subscription.MessageName,
                    subscription.SubscriptionId);

            _storage.Remove(subscription);
        }

        bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~BusSubscriptionRepository()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
            {
                _storage.Dispose();
            }

            _disposed = true;
        }
    }
}