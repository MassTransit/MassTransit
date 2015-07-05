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
namespace MassTransit.Pipeline.Filters.CircuitBreaker
{
    using System;
    using System.Collections.Generic;


    public interface CircuitBreakerSettings
    {
        /// <summary>
        /// The window duration to keep track of errors before they fall off the breaker state
        /// </summary>
        TimeSpan Duration { get; }

        /// <summary>
        /// The time to wait after the breaker has opened before attempting to close it
        /// </summary>
        IEnumerable<TimeSpan> ResetTimeout { get; }

        /// <summary>
        /// A percentage of how many failures versus successful calls before the breaker
        /// is opened. Should be 0-100, but seriously like 5-10.
        /// </summary>
        int TripThreshold { get; }

        /// <summary>
        /// The active count of attempts before the circuit breaker can be tripped
        /// </summary>
        int ActiveCount { get; }
    }
}