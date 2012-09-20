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
namespace MassTransit.NHibernateIntegration.Subscriptions
{
    using MassTransit.Subscriptions.Coordinator;
    using NHibernate.Mapping.ByCode.Conformist;

    public class PersistentSubscriptionMap :
        ClassMapping<PersistentSubscription>
    {
        public PersistentSubscriptionMap()
        {
            DynamicInsert(false);
            DynamicUpdate(false);
            Lazy(false);

            ComposedId(x =>
                {
                    x.Property(p => p.BusUri, m => m.Length(256));
                    x.Property(p => p.PeerId);
                    x.Property(p => p.SubscriptionId);
                });

            Property(x => x.PeerUri, m => m.Length(256));
            Property(x => x.MessageName, m => m.Length(256));
            Property(x => x.CorrelationId);
            Property(x => x.EndpointUri, m => m.Length(256));
            Property(x => x.Created);
            Property(x => x.Updated);
        }
    }
}