// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Courier.InternalMessages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Contracts;


    class RoutingSlipRevisedMessage :
        RoutingSlipRevised
    {
        public RoutingSlipRevisedMessage(HostInfo host, Guid trackingNumber, string activityName, Guid executionId, DateTime timestamp, TimeSpan duration,
            IDictionary<string, object> variables,
            IEnumerable<Activity> itinerary, IEnumerable<Activity> discardedItinerary)
        {
            Host = host;
            ActivityName = activityName;
            TrackingNumber = trackingNumber;
            Timestamp = timestamp;
            Duration = duration;
            ExecutionId = executionId;
            Variables = variables;
            Itinerary = itinerary.ToArray();
            DiscardedItinerary = discardedItinerary.ToArray();
        }

        public Guid TrackingNumber { get; private set; }
        public DateTime Timestamp { get; private set; }
        public TimeSpan Duration { get; private set; }

        public string ActivityName { get; private set; }
        public Guid ExecutionId { get; private set; }
        public HostInfo Host { get; private set; }

        public IDictionary<string, object> Variables { get; private set; }

        public Activity[] Itinerary { get; private set; }

        public Activity[] DiscardedItinerary { get; private set; }
    }
}