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


    public class FilterRetryExceptionFilter<T> :
        IRetryExceptionFilter
        where T : Exception
    {
        readonly Func<T, bool> _filter;

        public FilterRetryExceptionFilter(Func<T, bool> filter)
        {
            _filter = filter;
        }

        public bool CanRetry(Exception exception)
        {
            var ex = exception as T;
            if (ex != null)
                return _filter(ex);

            return true;
        }
    }
}