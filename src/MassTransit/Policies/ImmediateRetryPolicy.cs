// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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


    public class ImmediateRetryPolicy :
        IRetryPolicy
    {
        readonly IRetryExceptionFilter _filter;
        readonly int _retryLimit;

        public ImmediateRetryPolicy(IRetryExceptionFilter filter, int retryLimit)
        {
            _filter = filter;
            _retryLimit = retryLimit;
        }

        public IRetryContext GetRetryContext()
        {
            return new IntervalRetryContext(this, GetIntervals());
        }

        public bool CanRetry(Exception exception)
        {
            return _filter.CanRetry(exception);
        }

        IEnumerable<TimeSpan> GetIntervals()
        {
            for (int i = 0; i < _retryLimit; i++)
                yield return TimeSpan.Zero;
        }
    }
}