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


    /// <summary>
    /// Specifies the specific contents of routing slip events to be included for a subscription
    /// </summary>
    [Flags]
    public enum RoutingSlipEventContents
    {
        /// <summary>
        /// Include all event contents
        /// </summary>
        All = 0,

        /// <summary>
        /// Do not include any contents with the routing slip events
        /// </summary>
        None = 0x100,

        /// <summary>
        /// The routing slip variables after the activity was executed or compensated
        /// </summary>
        Variables = 0x0001,

        /// <summary>
        /// The arguments provided to the activity
        /// </summary>
        Arguments = 0x0002,

        /// <summary>
        /// The data logged by an activity when completed or compensated
        /// </summary>
        Data = 0x0004,

        /// <summary>
        /// If specified, encrypted content is excluded from the event
        /// </summary>
        SkipEncrypted = 0x0008,
    }
}