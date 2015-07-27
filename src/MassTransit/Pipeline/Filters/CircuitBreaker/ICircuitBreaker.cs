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


    /// <summary>
    /// Provides access to a circuit breaker from a state object
    /// </summary>
    interface ICircuitBreaker
    {
        /// <summary>
        /// The number of failures before opening the circuit breaker
        /// </summary>
        int TripThreshold { get; }

        /// <summary>
        /// The minimum number of attempts before the breaker can possibly trip
        /// </summary>
        int ActiveThreshold { get; }

        /// <summary>
        /// Window duration before attempt/success/failure counts are reset
        /// </summary>
        TimeSpan OpenDuration { get; }

        /// <summary>
        /// Open the circuit breaker, preventing any further access to the resource until
        /// the timer expires
        /// </summary>
        /// <param name="exception">The exception to return when the circuit breaker is accessed</param>
        /// <param name="behavior"></param>
        /// <param name="timeoutEnumerator">A previously created enumerator for a timeout period</param>
        void Open(Exception exception, ICircuitBreakerBehavior behavior, IEnumerator<TimeSpan> timeoutEnumerator = null);

        /// <summary>
        /// Partially open the circuit breaker, allowing the eventual return to a closed
        /// state
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="timeoutEnumerator"></param>
        /// <param name="behavior"></param>
        void ClosePartially(Exception exception, IEnumerator<TimeSpan> timeoutEnumerator, ICircuitBreakerBehavior behavior);

        /// <summary>
        /// Close the circuit breaker, allowing normal execution
        /// </summary>
        /// <param name="behavior"></param>
        void Close(ICircuitBreakerBehavior behavior);
    }
}