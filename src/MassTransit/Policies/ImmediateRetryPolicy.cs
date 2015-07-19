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


    public class ImmediateRetryPolicy :
        IRetryPolicy
    {
        readonly IPolicyExceptionFilter _filter;
        readonly int _retryLimit;

        public ImmediateRetryPolicy(IPolicyExceptionFilter filter, int retryLimit)
        {
            _filter = filter;
            _retryLimit = retryLimit;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            context.Set(new
            {
                Policy = "Immediate",
                Limit = _retryLimit,
            });

            _filter.Probe(context);
        }

        public IRetryContext GetRetryContext()
        {
            return new IntervalRetryContext(this, GetIntervals());
        }

        public bool CanRetry(Exception exception)
        {
            return _filter.Match(exception);
        }

        IEnumerable<TimeSpan> GetIntervals()
        {
            for (int i = 0; i < _retryLimit; i++)
                yield return TimeSpan.Zero;
        }
    }
}