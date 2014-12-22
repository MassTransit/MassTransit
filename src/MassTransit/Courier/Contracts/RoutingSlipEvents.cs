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


    [Flags]
    public enum RoutingSlipEvents
    {
        /// <summary>
        /// By default, all routing slip events are included for a subscription
        /// </summary>
        All = 0,

        /// <summary>
        /// Send the RoutingSlipCompleted event
        /// </summary>
        Completed = 0x0001,

        /// <summary>
        /// Send the RoutingSlipFaulted event
        /// </summary>
        Faulted = 0x0002,

        /// <summary>
        /// Send the RoutingSlipCompensationFaulted event
        /// </summary>
        CompensationFailed = 0x0004,

        /// <summary>
        /// Send the RoutingSlipActivityCompleted event
        /// </summary>
        ActivityCompleted = 0x0010,

        /// <summary>
        /// Send the RoutingSlipActivityFaulted event
        /// </summary>
        ActivityFaulted = 0x0020,

        /// <summary>
        /// Send the RoutingSlipActivityCompensated event
        /// </summary>
        ActivityCompensated = 0x0040,

        /// <summary>
        /// Send the RoutingSlipCompensationFailed event
        /// </summary>
        ActivityCompensationFailed = 0x0080,
    }
}