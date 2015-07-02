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
    using System.Threading;
    using System.Threading.Tasks;


    /// <summary>
    /// Represents a closed, normally operating circuit breaker state
    /// </summary>
    class ClosedCircuitBreakerBehavior :
        ICircuitBreakerBehavior
    {
        readonly ICircuitBreaker _breaker;
        int _failureCount;

        public ClosedCircuitBreakerBehavior(ICircuitBreaker breaker)
        {
            _breaker = breaker;
        }

        void ICircuitBreakerBehavior.SendFault(Exception exception)
        {
            if (Interlocked.Increment(ref _failureCount) >= _breaker.OpenThreshold)
                _breaker.Open(exception, this);
        }

        void ICircuitBreakerBehavior.PreSend()
        {
        }

        void ICircuitBreakerBehavior.PostSend()
        {
        }

        async Task IProbeSite.Probe(ProbeContext context)
        {
            context.Set(new
            {
                State = "closed",
                FailureCount = _failureCount,
            });
        }
    }
}