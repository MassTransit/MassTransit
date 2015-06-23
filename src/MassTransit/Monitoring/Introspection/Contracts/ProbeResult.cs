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
namespace MassTransit.Monitoring.Introspection.Contracts
{
    using System;
    using System.Collections.Generic;


    /// <summary>
    /// The result of a probe
    /// </summary>
    public interface ProbeResult
    {
        /// <summary>
        /// Unique identifies this result
        /// </summary>
        Guid ResultId { get; }

        /// <summary>
        /// Identifies the initiator of the probe
        /// </summary>
        Guid ProbeId { get; }

        /// <summary>
        /// When the probe was initiated through the system
        /// </summary>
        DateTime StartTimestamp { get; }

        /// <summary>
        /// How long the probe took to execute
        /// </summary>
        TimeSpan Duration { get; }

        /// <summary>
        /// The host from which the result was generated
        /// </summary>
        HostInfo Host { get; }

        /// <summary>
        /// The results returned by the probe
        /// </summary>
        IDictionary<string, object> Results { get; }
    }
}