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
namespace MassTransit.Courier
{
    using System;
    using Contracts;
    using MassTransit.Serialization;


    public interface IRoutingSlipSendEndpointTarget
    {
        /// <summary>
        /// Adds a custom subscription message to the routing slip which is sent at the specified events
        /// </summary>
        /// <param name="address">The destination address where the events are sent</param>
        /// <param name="events">The events to include in the subscription</param>
        /// <param name="contents">The contents of the routing slip event</param>
        /// <param name="activityName"></param>
        /// <param name="message">The custom message to be sent</param>
        void AddSubscription(Uri address, RoutingSlipEvents events, RoutingSlipEventContents contents, string activityName, MessageEnvelope message);
    }
}