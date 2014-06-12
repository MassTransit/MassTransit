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
namespace MassTransit.Pipeline.Sinks
{
    using System;
    using System.Linq;


    public class IntervalMessageRetryPolicy :
        IMessageRetryPolicy
    {
        readonly IRetryExceptionFilter _filter;
        readonly TimeSpan[] _intervals;

        public IntervalMessageRetryPolicy(IRetryExceptionFilter filter, params TimeSpan[] intervals)
        {
            if (intervals == null)
                throw new ArgumentNullException("intervals");
            if (intervals.Length == 0)
                throw new ArgumentOutOfRangeException("intervals", "At least one interval must be specified");

            _filter = filter;
            _intervals = intervals;
        }

        public IntervalMessageRetryPolicy(IRetryExceptionFilter filter, params int[] intervals)
        {
            if (intervals == null)
                throw new ArgumentNullException("intervals");
            if (intervals.Length == 0)
                throw new ArgumentOutOfRangeException("intervals", "At least one interval must be specified");

            _filter = filter;
            _intervals = intervals.Select(x => TimeSpan.FromMilliseconds(x)).ToArray();
        }

        public IMessageRetryContext GetRetryContext<T>(ConsumeContext<T> context)
            where T : class
        {
            return new IntervalMessageRetryContext(this, _intervals);
        }

        public bool CanRetry(Exception exception)
        {
            return _filter.CanRetry(exception);
        }
    }
}