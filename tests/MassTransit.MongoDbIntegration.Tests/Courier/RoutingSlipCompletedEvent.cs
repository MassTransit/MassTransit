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
namespace MassTransit.MongoDbIntegration.Tests.Courier
{
    using System;
    using System.Collections.Generic;
    using MassTransit.Courier.Contracts;


    public class RoutingSlipCompletedEvent :
        RoutingSlipCompleted
    {
        public RoutingSlipCompletedEvent(Guid trackingNumber, DateTime timestamp, TimeSpan duration)
        {
            Timestamp = timestamp;
            Duration = duration;
            TrackingNumber = trackingNumber;

            Variables = new Dictionary<string, object>
            {
                {"Client", 27},
                {"Reason", "Because I said so"}
            };
        }

        public Guid TrackingNumber { get; private set; }
        public DateTime Timestamp { get; private set; }

        public TimeSpan Duration { get; private set; }

        public IDictionary<string, object> Variables { get; private set; }
    }
}