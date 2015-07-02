// Copyright 2007-2013 Chris Patterson
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
namespace MassTransit.Steward
{
    using System;


    /// <summary>
    /// A reference to a command that was sent for execution
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface DispatchHandle
    {
        /// <summary>
        /// Uniquely identifies the dispatch
        /// </summary>
        Guid DispatchId { get; }

        /// <summary>
        /// The time the dispatched was created
        /// </summary>
        DateTime CreateTime { get; }

        /// <summary>
        /// The destination address of the service endpoint
        /// </summary>
        Uri Destination { get; }
    }
}