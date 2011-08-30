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
    public class SubscriptionKey
    {
        public string CorrelationId;
        public string MessageName;

        public bool Equals(SubscriptionKey other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.MessageName, MessageName) && Equals(other.CorrelationId, CorrelationId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (SubscriptionKey)) return false;
            return Equals((SubscriptionKey) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((MessageName != null ? MessageName.GetHashCode() : 0)*397) ^
                       (CorrelationId != null ? CorrelationId.GetHashCode() : 0);
            }
        }
    }
}