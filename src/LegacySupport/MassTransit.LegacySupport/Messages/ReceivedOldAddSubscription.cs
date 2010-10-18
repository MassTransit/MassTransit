// Copyright 2007-2010 The Apache Software Foundation.
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
namespace MassTransit.Services.LegacyProxy.Messages
{
    using System;

    [Serializable]
    public class ReceivedOldAddSubscription
    {
        public string CorrelationId { get; set; }
        public Uri EndpointUri { get; set; }
        public string MessageName { get; set; }

        public bool Equals(ReceivedOldAddSubscription other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.CorrelationId, CorrelationId) && Equals(other.EndpointUri, EndpointUri) && Equals(other.MessageName, MessageName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (ReceivedOldAddSubscription)) return false;
            return Equals((ReceivedOldAddSubscription) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (CorrelationId != null ? CorrelationId.GetHashCode() : 0);
                result = (result*397) ^ (EndpointUri != null ? EndpointUri.GetHashCode() : 0);
                result = (result*397) ^ (MessageName != null ? MessageName.GetHashCode() : 0);
                return result;
            }
        }
    }
}