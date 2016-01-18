// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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


    /// <summary>
    /// Represents a closed, normally operating circuit breaker state
    /// </summary>
    class ClosedBehavior :
        ICircuitBreakerBehavior
    {
        readonly ICircuitBreaker _breaker;
        readonly Timer _timer;
        int _attemptCount;
        int _failureCount;
        int _successCount;

        public ClosedBehavior(ICircuitBreaker breaker)
        {
            _breaker = breaker;
            _timer = new Timer(Reset, null, breaker.OpenDuration, breaker.OpenDuration);
        }

        bool IsActive => _attemptCount > _breaker.ActiveThreshold;

        void ICircuitBreakerBehavior.PreSend()
        {
            Interlocked.Increment(ref _attemptCount);
        }

        void ICircuitBreakerBehavior.PostSend()
        {
            Interlocked.Increment(ref _successCount);
        }

        void ICircuitBreakerBehavior.SendFault(Exception exception)
        {
            int failureCount = Interlocked.Increment(ref _failureCount);

            if (IsActive && TripThresholdExceeded(failureCount))
            {
                _timer.Dispose();
                _breaker.Open(exception, this);
            }
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            context.Set(new
            {
                State = "closed",
                AttemptCount = _attemptCount,
                SuccessCount = _successCount,
                FailureCount = _failureCount
            });
        }

        bool TripThresholdExceeded(int failureCount)
        {
            return failureCount * 100L / _attemptCount >= _breaker.TripThreshold;
        }

        void Reset(object state)
        {
            lock (_breaker)
            {
                _attemptCount = 0;
                _successCount = 0;
                _failureCount = 0;
            }
        }
    }
}