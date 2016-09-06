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
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using CircuitBreaker;
    using GreenPipes;


    public class CircuitBreakerFilter<T> :
        IFilter<T>,
        ICircuitBreaker
        where T : class, PipeContext
    {
        readonly CircuitBreakerSettings _settings;
        ICircuitBreakerBehavior _behavior;

        public CircuitBreakerFilter(CircuitBreakerSettings settings)
        {
            _settings = settings;

            _behavior = new ClosedBehavior(this);
        }

        public TimeSpan OpenDuration => _settings.TrackingPeriod;

        void ICircuitBreaker.Open(Exception exception, ICircuitBreakerBehavior behavior, IEnumerator<TimeSpan> timeoutEnumerator)
        {
            if (timeoutEnumerator == null)
                timeoutEnumerator = _settings.ResetTimeout.GetEnumerator();

            Interlocked.CompareExchange(ref _behavior, new OpenBehavior(this, exception, timeoutEnumerator), behavior);
        }

        void ICircuitBreaker.Close(ICircuitBreakerBehavior behavior)
        {
            Interlocked.CompareExchange(ref _behavior, new ClosedBehavior(this), behavior);
        }

        void ICircuitBreaker.ClosePartially(Exception exception, IEnumerator<TimeSpan> timeoutEnumerator, ICircuitBreakerBehavior behavior)
        {
            Interlocked.CompareExchange(ref _behavior, new HalfOpenBehavior(this, exception, timeoutEnumerator), behavior);
        }

        public int TripThreshold => _settings.TripThreshold;

        public int ActiveThreshold => _settings.ActiveThreshold;

        public async Task Send(T context, IPipe<T> next)
        {
            try
            {
                _behavior.PreSend();

                await next.Send(context).ConfigureAwait(false);

                _behavior.PostSend();
            }
            catch (Exception ex)
            {
                _behavior.SendFault(ex);

                throw;
            }
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            ProbeContext scope = context.CreateFilterScope("circuitBreaker");
            scope.Set(new
            {
                ActiveCount = _settings.ActiveThreshold,
                _settings.TripThreshold,
                Duration = _settings.TrackingPeriod,
                ResetTimeout = _settings.ResetTimeout.Take(10).ToArray(),
            });

            _behavior.Probe(scope);
        }
    }
}