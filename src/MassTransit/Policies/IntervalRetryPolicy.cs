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
    using System.Linq;
    using System.Threading.Tasks;


    public class IntervalRetryPolicy :
        IRetryPolicy
    {
        readonly IPolicyExceptionFilter _filter;
        readonly TimeSpan[] _intervals;

        public IntervalRetryPolicy(IPolicyExceptionFilter filter, params TimeSpan[] intervals)
        {
            if (intervals == null)
                throw new ArgumentNullException("intervals");
            if (intervals.Length == 0)
                throw new ArgumentOutOfRangeException("intervals", "At least one interval must be specified");

            _filter = filter;
            _intervals = intervals;
        }

        public IntervalRetryPolicy(IPolicyExceptionFilter filter, params int[] intervals)
        {
            if (intervals == null)
                throw new ArgumentNullException("intervals");
            if (intervals.Length == 0)
                throw new ArgumentOutOfRangeException("intervals", "At least one interval must be specified");

            _filter = filter;
            _intervals = intervals.Select(x => TimeSpan.FromMilliseconds(x)).ToArray();
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            context.Set(new
            {
                Policy = "Interval",
                Limit = _intervals.Length,
                Intervals = _intervals,
            });

            _filter.Probe(context);
        }

        public IRetryContext GetRetryContext()
        {
            return new IntervalRetryContext(this, _intervals);
        }

        public bool CanRetry(Exception exception)
        {
            return _filter.Match(exception);
        }

        public override string ToString()
        {
            return string.Format("Interval (limit {0}, intervals {1})", _intervals.Length,
                string.Join(";", _intervals.Take(5).Select(x => x.ToString())));
        }
    }
}