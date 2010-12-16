// Copyright 2007-2008 The Apache Software Foundation.
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
namespace MassTransit.Services.Subscriptions.Server
{
    using System.Collections.Generic;
    using Common.Logging;

    public class InMemorySubscriptionRepository :
        ISubscriptionRepository
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (InMemorySubscriptionRepository));
        readonly IList<Subscription> _subscriptions = new List<Subscription>();

        public void Dispose()
        {
            _subscriptions.Clear();
        }

        public void Save(Subscription subscription)
        {
            _log.Info("Subscription Saved");
            _subscriptions.Add(subscription);
        }

        public void Remove(Subscription subscription)
        {
            _log.Info("Subscription REmoved");
            _subscriptions.Remove(subscription);
        }

        public IEnumerable<Subscription> List()
        {
            return _subscriptions;
        }
    }
}