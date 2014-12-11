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
namespace MassTransit.Courier.Contracts
{
    using System;
    using System.Collections.Generic;


    /// <summary>
    /// A RoutingSlip is the transport-level interface that is used to carry the details
    /// of a message routing slip over the network.
    /// </summary>
    public interface RoutingSlip
    {
        /// <summary>
        /// The unique tracking number for this routing slip, used to correlate events
        /// and activities
        /// </summary>
        Guid TrackingNumber { get; }

        /// <summary>
        /// The time when the routing slip was created
        /// </summary>
        DateTime CreateTimestamp { get; }

        /// <summary>
        /// The list of activities that are remaining
        /// </summary>
        IList<Activity> Itinerary { get; }

        /// <summary>
        /// The logs of activities that have already been executed
        /// </summary>
        IList<ActivityLog> ActivityLogs { get; }

        /// <summary>
        /// The logs of activities that can be compensated
        /// </summary>
        IList<CompensateLog> CompensateLogs { get; }

        /// <summary>
        /// Variables that are carried with the routing slip for use by any activity
        /// </summary>
        IDictionary<string, object> Variables { get; }

        /// <summary>
        /// A list of exceptions that have occurred during routing slip execution
        /// </summary>
        IList<ActivityException> ActivityExceptions { get; }

        /// <summary>
        /// Subscriptions to routing slip events
        /// </summary>
        IList<Subscription> Subscriptions { get; } 
    }
}