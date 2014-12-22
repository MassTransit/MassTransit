// Copyright 2007-2014 Chris Patterson
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


    class RoutingSlipFaultedMessage :
        RoutingSlipFaulted
    {
        public RoutingSlipFaultedMessage(Guid trackingNumber, DateTime timestamp, TimeSpan duration,
            IEnumerable<ActivityException> activityExceptions, IDictionary<string, object> variables)
        {
            TrackingNumber = trackingNumber;
            Timestamp = timestamp;
            Duration = duration;

            Variables = variables;
            ActivityExceptions = activityExceptions.ToArray();
        }

        public RoutingSlipFaultedMessage(Guid trackingNumber, DateTime timestamp, TimeSpan duration, ActivityException activityException)
        {
            Timestamp = timestamp;
            Duration = duration;

            TrackingNumber = trackingNumber;
            ActivityExceptions = new[] {activityException};
        }

        public Guid TrackingNumber { get; private set; }
        public DateTime Timestamp { get; private set; }
        public TimeSpan Duration { get; private set; }
        public ActivityException[] ActivityExceptions { get; private set; }
        public IDictionary<string, object> Variables { get; private set; }
    }
}