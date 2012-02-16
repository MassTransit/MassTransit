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
namespace MassTransit.Services.Subscriptions.Messages
{
    using System;

    [Serializable]
    public abstract class SubscriptionChange :
        CorrelatedBy<Guid>
    {
        protected SubscriptionChange()
        {
        }

        protected SubscriptionChange(SubscriptionInformation subscription)
        {
            Subscription = subscription;

            if (Subscription.SubscriptionId == Guid.Empty)
                Subscription.SubscriptionId = NewId.NextGuid();
        }

        public SubscriptionInformation Subscription { get; set; }

        public Guid CorrelationId
        {
            get { return Subscription.SubscriptionId; }
        }

        public bool Equals(SubscriptionChange obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj.Subscription, Subscription);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (SubscriptionChange)) return false;
            return Equals((SubscriptionChange) obj);
        }

        public override int GetHashCode()
        {
            return (Subscription != null ? Subscription.GetHashCode() : 0);
        }
    }
}