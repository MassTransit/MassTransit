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
namespace MassTransit.Policies
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Monitoring.Introspection;


    public class IncrementalRetryPolicy :
        IRetryPolicy
    {
        readonly IRetryExceptionFilter _filter;
        readonly TimeSpan _initialInterval;
        readonly TimeSpan _intervalIncrement;
        readonly int _retryLimit;

        public IncrementalRetryPolicy(IRetryExceptionFilter filter, int retryLimit, TimeSpan initialInterval,
            TimeSpan intervalIncrement)
        {
            if (initialInterval < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException("initialInterval",
                    "The initialInterval must be non-negative or -1, and it must be less than or equal to TimeSpan.MaxValue.");
            }

            if (intervalIncrement < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException("intervalIncrement",
                    "The intervalIncrement must be non-negative or -1, and it must be less than or equal to TimeSpan.MaxValue.");
            }

            _filter = filter;
            _retryLimit = retryLimit;
            _initialInterval = initialInterval;
            _intervalIncrement = intervalIncrement;
        }

        Task IProbeSite.Probe(ProbeContext context)
        {
            context.Set(new
            {
                Policy = "Incremental",
                Limit = _retryLimit,
                Initial = _initialInterval,
                Increment = _intervalIncrement,
            });

            return _filter.Probe(context);
        }

        public IRetryContext GetRetryContext()
        {
            return new IntervalRetryContext(this, GetIntervals(_retryLimit, _initialInterval, _intervalIncrement));
        }

        public bool CanRetry(Exception exception)
        {
            return _filter.CanRetry(exception);
        }

        IEnumerable<TimeSpan> GetIntervals(int retryLimit, TimeSpan initialInterval, TimeSpan intervalIncrement)
        {
            TimeSpan interval = initialInterval;
            for (int i = 0; i < retryLimit; i++)
            {
                yield return interval;

                interval += intervalIncrement;
            }
        }
    }
}