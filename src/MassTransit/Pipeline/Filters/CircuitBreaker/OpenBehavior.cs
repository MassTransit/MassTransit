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
    using System.Diagnostics;
    using System.Threading;
    using Events;
    using GreenPipes;


    /// <summary>
    /// Represents a circuit that is unavailable, with a timer waiting to partially close
    /// the circuit.
    /// </summary>
    class OpenBehavior :
        ICircuitBreakerBehavior
    {
        readonly ICircuitBreaker _breaker;
        readonly Stopwatch _elapsed;
        readonly Exception _exception;
        readonly IEnumerator<TimeSpan> _timeoutEnumerator;
        readonly Timer _timer;

        public OpenBehavior(ICircuitBreaker breaker, Exception exception, IEnumerator<TimeSpan> timeoutEnumerator)
        {
            _breaker = breaker;
            _exception = exception;
            _timeoutEnumerator = timeoutEnumerator;

            _timer = GetTimer(timeoutEnumerator);
            _elapsed = Stopwatch.StartNew();
        }

        void ICircuitBreakerBehavior.PreSend()
        {
            throw _exception;
        }

        void ICircuitBreakerBehavior.PostSend()
        {
        }

        void ICircuitBreakerBehavior.SendFault(Exception exception)
        {
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            TimeSpan timeout = _timeoutEnumerator.Current;
            context.Set(new
            {
                State = "open",
                ExceptionInfo = new FaultExceptionInfo(_exception),
                Timeout = timeout,
                Remaining = timeout - _elapsed.Elapsed
            });
        }

        Timer GetTimer(IEnumerator<TimeSpan> timeoutEnumerator)
        {
            timeoutEnumerator.MoveNext();

            return new Timer(PartiallyCloseCircuit, this, timeoutEnumerator.Current, TimeSpan.FromMilliseconds(-1));
        }

        void PartiallyCloseCircuit(object state)
        {
            _timer.Dispose();
            _breaker.ClosePartially(_exception, _timeoutEnumerator, this);
        }
    }
}