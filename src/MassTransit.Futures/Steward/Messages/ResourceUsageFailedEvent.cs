// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Steward.Messages
{
    using System;
    using Contracts;


    class ResourceUsageFailedEvent :
        ResourceUsageFailed
    {
        public ResourceUsageFailedEvent(Guid dispatchId, Uri resource, DateTime timestamp, TimeSpan duration, int reasonCode, string reasonText)
        {
            ReasonCode = reasonCode;
            Duration = duration;
            Timestamp = timestamp;
            Resource = resource;
            ReasonText = reasonText;
            DispatchId = dispatchId;

            EventId = NewId.NextGuid();
        }

        public Guid EventId { get; private set; }
        public Guid DispatchId { get; private set; }
        public DateTime Timestamp { get; private set; }
        public TimeSpan Duration { get; private set; }
        public Uri Resource { get; private set; }
        public int ReasonCode { get; private set; }
        public string ReasonText { get; private set; }
    }
}