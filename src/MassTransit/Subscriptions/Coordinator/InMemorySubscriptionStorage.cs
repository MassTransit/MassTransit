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
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Logging;
    using Magnum.Extensions;
    using Stact;

    public class InMemorySubscriptionStorage :
        SubscriptionStorage
    {
        readonly Fiber _fiber;
        readonly ILog _log = Logger.Get<InMemorySubscriptionStorage>();
        readonly HashSet<PersistentSubscription> _subscriptions;

        bool _disposed;

        public InMemorySubscriptionStorage()
        {
            _subscriptions = new HashSet<PersistentSubscription>();
            _fiber = new PoolFiber();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public void Add(PersistentSubscription subscription)
        {
            _fiber.Add(() =>
                {
                    _subscriptions.Add(subscription);

                    _log.DebugFormat("SubscriptionStorage.Add: {0} [{1}]", subscription, _subscriptions.Count);
                });
        }

        public void Remove(PersistentSubscription subscription)
        {
            _fiber.Add(() =>
                {
                    _subscriptions.Remove(subscription);

                    _log.DebugFormat("SubscriptionStorage.Remove: {0} [{1}]", subscription, _subscriptions.Count);
                });
        }

        public IEnumerable<PersistentSubscription> Load(Uri peerUri)
        {
            var subscriptions = new Future<IList<PersistentSubscription>>();
            _fiber.Add(() =>
                {
                    subscriptions.Complete(_subscriptions.ToList());
                });

            var completed = subscriptions.WaitUntilCompleted(60.Seconds());
            if (completed)
                return subscriptions.Value;

            _log.Error("Failed to load subscriptions in the time required");
            return Enumerable.Empty<PersistentSubscription>();
        }

        void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
            {
                _fiber.Shutdown();
            }

            _disposed = true;
        }
    }
}