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
    using System.Threading;
    using System.Threading.Tasks;


    /// <summary>
    /// Executes until the success count is met. If a fault occurs before the success 
    /// count is reached, the circuit reopens.
    /// </summary>
    class HalfOpenBehavior :
        ICircuitBreakerBehavior
    {
        readonly ICircuitBreaker _breaker;
        readonly Exception _exception;
        readonly IEnumerator<TimeSpan> _timeoutEnumerator;
        int _attemptCount;

        public HalfOpenBehavior(ICircuitBreaker breaker, Exception exception, IEnumerator<TimeSpan> timeoutEnumerator)
        {
            _breaker = breaker;
            _exception = exception;
            _timeoutEnumerator = timeoutEnumerator;
        }

        bool IsActive => _attemptCount > _breaker.ActiveThreshold;

        void ICircuitBreakerBehavior.PreSend()
        {
            Interlocked.Increment(ref _attemptCount);
        }

        void ICircuitBreakerBehavior.PostSend()
        {
            if (IsActive)
            {
                _breaker.Close(this);
                _timeoutEnumerator.Dispose();
            }
        }

        void ICircuitBreakerBehavior.SendFault(Exception exception)
        {
            _breaker.Open(_exception, this, _timeoutEnumerator);
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            context.Set(new
            {
                State = "halfOpen",
                AttempCount = _attemptCount,
            });
        }
    }
}