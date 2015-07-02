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
    using System.Threading.Tasks;
    using Events;


    /// <summary>
    /// Represents a circuit that is unavailable, with a timer waiting to partially close
    /// the circuit.
    /// </summary>
    class OpenCircuitBreakerBehavior :
        ICircuitBreakerBehavior
    {
        readonly ICircuitBreaker _breaker;
        readonly Exception _exception;
        readonly IEnumerator<int> _timeoutEnumerator;
        readonly Timer _timer;
        Stopwatch _elapsed;

        public OpenCircuitBreakerBehavior(ICircuitBreaker breaker, Exception exception, IEnumerator<int> timeoutEnumerator)
        {
            _breaker = breaker;
            _exception = exception;
            _timeoutEnumerator = timeoutEnumerator;

            _timer = GetTimer(timeoutEnumerator);
            _elapsed = Stopwatch.StartNew();
        }

        void ICircuitBreakerBehavior.PreSend()
        {
            throw new CircuitOpenException("The circuit breaker is open", _exception);
        }

        void ICircuitBreakerBehavior.PostSend()
        {
        }

        void ICircuitBreakerBehavior.SendFault(Exception exception)
        {
        }

        Timer GetTimer(IEnumerator<int> timeoutEnumerator)
        {
            timeoutEnumerator.MoveNext();

            return new Timer(PartiallyCloseCircuit, this, timeoutEnumerator.Current, -1);
        }

        void PartiallyCloseCircuit(object state)
        {
            _timer.Dispose();
            _breaker.ClosePartially(_exception, _timeoutEnumerator, this);
        }

        async Task IProbeSite.Probe(ProbeContext context)
        {
            var timeout = _timeoutEnumerator.Current;
            context.Set(new
            {
                State = "open",
                ExceptionInfo = new FaultExceptionInfo(_exception),
                Timeout = timeout,
                Remaining = timeout - _elapsed.ElapsedMilliseconds
            });
        }
    }
}