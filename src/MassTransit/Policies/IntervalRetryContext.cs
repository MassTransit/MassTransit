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


    public class IntervalRetryContext :
        IRetryContext
    {
        readonly TimeSpan[] _delays;
        readonly IRetryPolicy _policy;
        int _retryNumber;

        public IntervalRetryContext(IRetryPolicy policy, TimeSpan[] delays)
        {
            _policy = policy;
            _delays = delays;
        }

        public void Dispose()
        {
        }

        public bool CanRetry(Exception exception, out TimeSpan delay)
        {
            bool canRetry = _policy.CanRetry(exception);
            if (canRetry && _retryNumber < _delays.Length)
            {
                delay = _delays[_retryNumber++];
                return true;
            }

            delay = TimeSpan.Zero;
            return false;
        }
    }
}