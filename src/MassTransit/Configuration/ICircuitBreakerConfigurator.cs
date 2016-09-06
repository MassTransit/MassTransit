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
namespace MassTransit
{
    using System;
    using GreenPipes;


    /// <summary>
    /// Configure the settings on the circuit breaker
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICircuitBreakerConfigurator<T>
        where T : class, PipeContext
    {
        /// <summary>
        /// The period after which the attempt/failure counts are reset.
        /// </summary>
        TimeSpan TrackingPeriod { set; }

        /// <summary>
        /// The percentage of attempts that must fail before the circuit breaker trips into
        /// an open state.
        /// </summary>
        int TripThreshold { set; }

        /// <summary>
        /// The number of attempts that must occur before the circuit breaker becomes active. Until the
        /// breaker activates, it will not open on failure
        /// </summary>
        int ActiveThreshold { set; }

        /// <summary>
        /// Sets a specific reset interval for the circuit to attempt to close after being tripped.
        /// By default, this is an incrementing scale up to one minute.
        /// </summary>
        /// <param name="interval"></param>
        void ResetInterval(TimeSpan interval);
    }
}