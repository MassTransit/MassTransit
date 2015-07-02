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
namespace MassTransit.Pipeline.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using CircuitBreaker;


    public class CircuitBreakerFilter<T> :
        IFilter<T>,
        ICircuitBreaker
        where T : class, PipeContext
    {
        readonly int _closeThreshold;
        readonly object _lock = new object();
        readonly int _openThreshold;
        ICircuitBreakerBehavior _behavior;

        public CircuitBreakerFilter(int openThreshold, int closeThreshold)
        {
            _openThreshold = openThreshold;
            _closeThreshold = closeThreshold;

            _behavior = new ClosedCircuitBreakerBehavior(this);
        }

        static IEnumerable<int> Timeouts
        {
            get
            {
                yield return 100;
                yield return 1000;
                yield return 2000;
                yield return 5000;
                yield return 10000;
                while (true)
                    yield return 30000;
            }
        }

        void ICircuitBreaker.Open(Exception exception, ICircuitBreakerBehavior behavior, IEnumerator<int> timeoutEnumerator = null)
        {
            if (timeoutEnumerator == null)
                timeoutEnumerator = Timeouts.GetEnumerator();

            Interlocked.CompareExchange(ref _behavior, new OpenCircuitBreakerBehavior(this, exception, timeoutEnumerator), behavior);
        }

        void ICircuitBreaker.Close(ICircuitBreakerBehavior behavior)
        {
            Interlocked.CompareExchange(ref _behavior, new ClosedCircuitBreakerBehavior(this), behavior);
        }

        void ICircuitBreaker.ClosePartially(Exception exception, IEnumerator<int> timeoutEnumerator, ICircuitBreakerBehavior behavior)
        {
            Interlocked.CompareExchange(ref _behavior, new HalfClosedCircuitBreakerBehavior(this, exception, timeoutEnumerator), behavior);
        }

        public int OpenThreshold
        {
            get { return _openThreshold; }
        }

        public int CloseThreshold
        {
            get { return _closeThreshold; }
        }

        public async Task Send(T context, IPipe<T> next)
        {
            try
            {
                _behavior.PreSend();

                await next.Send(context);

                _behavior.PostSend();
            }
            catch (Exception ex)
            {
                _behavior.SendFault(ex);

                throw;
            }
        }

        Task IProbeSite.Probe(ProbeContext context)
        {
            ProbeContext scope = context.CreateFilterScope("circuitBreaker");

            return _behavior.Probe(scope);
        }
    }
}