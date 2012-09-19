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
    using Stact;

    public class BusSubscriptionRepository :
        SubscriptionRepository
    {
        static readonly ILog _log = Logger.Get(typeof(BusSubscriptionRepository));

        readonly Uri _busUri;
        readonly Fiber _fiber;
        readonly SubscriptionStorage _storage;
        bool _disposed;

        public BusSubscriptionRepository(Uri busUri, SubscriptionStorage storage)
        {
            var uri = busUri.AbsoluteUri;
            if (busUri.Query.Length > 0)
                 uri = uri.Replace(busUri.Query, "");

            _busUri = new Uri(uri);
            _storage = storage;
            _fiber = new PoolFiber();

            _fiber.Add(LoadExistingSubscriptions);
        }

        public void Add(Guid peerId, Guid subscriptionId, Uri endpointUri, string messageName, string correlationId)
        {
            var subscription = new PersistentSubscription(_busUri, peerId, subscriptionId, endpointUri, messageName,
                correlationId);

            _fiber.Add(() => Add(subscription));
        }

        public void Remove(Guid peerId, Guid subscriptionId, Uri endpointUri, string messageName, string correlationId)
        {
            var subscription = new PersistentSubscription(_busUri, peerId, subscriptionId, endpointUri, messageName,
                correlationId);

            _fiber.Add(() => Remove(subscription));
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        void LoadExistingSubscriptions()
        {
        }

        void Add(PersistentSubscription subscription)
        {
            try
            {
                if (_log.IsDebugEnabled)
                    _log.DebugFormat("SubscriptionRepository.Add: {0}, {1}", subscription.MessageName,
                        subscription.SubscriptionId);

                _storage.Add(subscription);
            }
            catch (Exception ex)
            {
                _log.Error("Failed to add persistent subscription", ex);
            }
        }

        void Remove(PersistentSubscription subscription)
        {
            try
            {
                if (_log.IsDebugEnabled)
                    _log.DebugFormat("SubscriptionRepository.Remove: {0}, {1}", subscription.MessageName,
                        subscription.SubscriptionId);

                _storage.Remove(subscription);
            }
            catch (Exception ex)
            {
                _log.Error("Failed to remove persistent subscription", ex);
            }
        }

        ~BusSubscriptionRepository()
        {
            Dispose(false);
        }

        void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
            {
                _fiber.Shutdown(TimeSpan.FromSeconds(30));
                _storage.Dispose();
            }

            _disposed = true;
        }
    }
}