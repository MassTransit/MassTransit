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
namespace MassTransit.LegacySupport.SerializationCustomization
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Subscriptions;
    using Subscriptions.Messages;

    public class TypeMappings :
        IEnumerable<TypeMap>
    {
        private readonly List<TypeMap> _typeMaps;

        public TypeMappings()
        {
            _typeMaps = new List<TypeMap>();
            _typeMaps.Add(new TypeMap("MassTransit.ServiceBus, Version=0.2.2133.0, Culture=neutral, PublicKeyToken=null", "MassTransit.ServiceBus.Subscriptions.Subscription", typeof(Subscription)));
            _typeMaps.Add(new TypeMap("MassTransit.ServiceBus, Version=0.2.2133.0, Culture=neutral, PublicKeyToken=null", "MassTransit.ServiceBus.Subscriptions.Subscription[]", typeof(Subscription[])));
            _typeMaps.Add(new TypeMap("mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", "System.Collections.Generic.List`1[[MassTransit.ServiceBus.Subscriptions.Subscription, MassTransit.ServiceBus, Version=0.2.2133.0, Culture=neutral, PublicKeyToken=null]]", typeof(List<Subscription>)));
            _typeMaps.Add(new TypeMap("MassTransit.ServiceBus, Version=0.2.2133.0, Culture=neutral, PublicKeyToken=null", "MassTransit.ServiceBus.Subscriptions.Messages.CacheUpdateResponse", typeof(OldCacheUpdateResponse)));
            _typeMaps.Add(new TypeMap("MassTransit.ServiceBus, Version=0.2.2133.0, Culture=neutral, PublicKeyToken=null", "MassTransit.ServiceBus.Subscriptions.Messages.AddSubscription", typeof(OldAddSubscription)));
            _typeMaps.Add(new TypeMap("MassTransit.ServiceBus, Version=0.2.2133.0, Culture=neutral, PublicKeyToken=null", "MassTransit.ServiceBus.Subscriptions.Messages.CacheUpdateRequest", typeof(OldCacheUpdateRequest)));
            _typeMaps.Add(new TypeMap("MassTransit.ServiceBus, Version=0.2.2133.0, Culture=neutral, PublicKeyToken=null", "MassTransit.ServiceBus.Subscriptions.Messages.CancelSubscriptionUpdates", typeof(OldCancelSubscriptionUpdates)));
            _typeMaps.Add(new TypeMap("MassTransit.ServiceBus, Version=0.2.2133.0, Culture=neutral, PublicKeyToken=null", "MassTransit.ServiceBus.Subscriptions.Messages.RemoveSubscription", typeof(OldRemoveSubscription)));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<TypeMap> GetEnumerator()
        {
            foreach (TypeMap map in _typeMaps)
            {
                yield return map;
            }

            yield break;
        }
    }
}