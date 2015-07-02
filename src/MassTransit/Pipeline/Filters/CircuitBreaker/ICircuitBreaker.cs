// Copyright 2012-2013 Chris Patterson
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
// except in compliance with the License. You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software distributed under the
// License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF
// ANY KIND, either express or implied. See the License for the specific language governing
// permissions and limitations under the License.
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
        int OpenThreshold { get; }

        /// <summary>
        /// The number of successful executions before closing the circuit breaker
        /// </summary>
        int CloseThreshold { get; }

        /// <summary>
        /// Open the circuit breaker, preventing any further access to the resource until
        /// the timer expires
        /// </summary>
        /// <param name="exception">The exception to return when the circuit breaker is accessed</param>
        /// <param name="behavior"></param>
        /// <param name="timeoutEnumerator">A previously created enumerator for a timeout period</param>
        void Open(Exception exception, ICircuitBreakerBehavior behavior, IEnumerator<int> timeoutEnumerator = null);

        /// <summary>
        /// Partially open the circuit breaker, allowing the eventual return to a closed
        /// state
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="timeoutEnumerator"></param>
        /// <param name="behavior"></param>
        void ClosePartially(Exception exception, IEnumerator<int> timeoutEnumerator, ICircuitBreakerBehavior behavior);

        /// <summary>
        /// Close the circuit breaker, allowing normal execution
        /// </summary>
        /// <param name="behavior"></param>
        void Close(ICircuitBreakerBehavior behavior);
    }
}