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
    using MassTransit.Serialization;


    /// <summary>
    /// A routing slip subscription defines a specific endpoint where routing
    /// slip events should be sent (not published). If specified, events are not published.
    /// </summary>
    public interface Subscription
    {
        /// <summary>
        /// The address where events should be sent
        /// </summary>
        Uri Address { get; }

        /// <summary>
        /// The events that are subscribed
        /// </summary>
        RoutingSlipEvents Events { get; }

        /// <summary>
        /// The event contents to include when published
        /// </summary>
        RoutingSlipEventContents Include { get; }

        /// <summary>
        /// If specified, events are only used in this subscription if the activity name matches
        /// </summary>
        string ActivityName { get; }

        /// <summary>
        /// The message sent as part of the subscription
        /// </summary>
        MessageEnvelope Message { get; }
    }
}