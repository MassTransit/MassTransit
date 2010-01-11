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
    using Subscriptions.Messages;

    public class LegacyMessageMappings :
        IEnumerable<MessageMap>
    {
        private readonly List<MessageMap> _maps2;

        readonly Dictionary<string, Type> _maps;

        public LegacyMessageMappings()
        {
            _maps2 = new List<MessageMap>();
            _maps2.Add(new MessageMap("MassTransit.ServiceBus, Version=0.2.2133.0, Culture=neutral, PublicKeyToken=null", "MassTransit.ServiceBus.Subscriptions.Messages.AddSubscription", typeof(OldAddSubscription)));
            _maps2.Add(new MessageMap("MassTransit.ServiceBus, Version=0.2.2133.0, Culture=neutral, PublicKeyToken=null", "MassTransit.ServiceBus.Subscriptions.Messages.CacheUpdateRequest", typeof(OldCacheUpdateRequest)));
            _maps2.Add(new MessageMap("MassTransit.ServiceBus, Version=0.2.2133.0, Culture=neutral, PublicKeyToken=null", "MassTransit.ServiceBus.Subscriptions.Messages.CacheUpdateResponse", typeof(OldCacheUpdateResponse)));
            _maps2.Add(new MessageMap("MassTransit.ServiceBus, Version=0.2.2133.0, Culture=neutral, PublicKeyToken=null", "MassTransit.ServiceBus.Subscriptions.Messages.CancelSubscriptionUpdates", typeof(OldCancelSubscriptionUpdates)));
            _maps2.Add(new MessageMap("MassTransit.ServiceBus, Version=0.2.2133.0, Culture=neutral, PublicKeyToken=null", "MassTransit.ServiceBus.Subscriptions.Messages.RemoveSubscription", typeof(OldRemoveSubscription)));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<MessageMap> GetEnumerator()
        {
            foreach (MessageMap map in _maps2)
            {
                yield return map;
            }

            yield break;
        }
    }
}