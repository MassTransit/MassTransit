/// Copyright 2007-2008 The Apache Software Foundation.
/// 
/// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
/// this file except in compliance with the License. You may obtain a copy of the 
/// License at 
/// 
///   http://www.apache.org/licenses/LICENSE-2.0 
/// 
/// Unless required by applicable law or agreed to in writing, software distributed 
/// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
/// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
/// specific language governing permissions and limitations under the License.

namespace MassTransit.ServiceBus.SubscriptionsManager
{
    using System;
    using System.Collections.Generic;
    using MassTransit.ServiceBus.Subscriptions.Messages;
    using Subscriptions;

    public class SubscriptionMapper
    {
        public static StoredSubscription MapFrom(SubscriptionChange message)
        {
            return new StoredSubscription(message.Subscription.Address.ToString(), message.Subscription.MessageName);
        }

        public static Subscription MapFrom(StoredSubscription storedSubscription)
        {
            return new Subscription(new Uri(storedSubscription.Address), storedSubscription.Message);
        }

        public static IList<Subscription> MapFrom(IList<StoredSubscription> subscriptions)
        {
            List<Subscription> result = new List<Subscription>();
            
            foreach (StoredSubscription storedSubscription in subscriptions)
            {
                result.Add(MapFrom(storedSubscription));
            }

            return result;
        }
    }
}