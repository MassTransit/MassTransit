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
namespace MassTransit.Pipeline.Filters
{
    using System;
    using System.Threading.Tasks;
    using Policies;


    /// <summary>
    /// Uses a retry policy to handle exceptions, retrying the operation in according
    /// with the policy
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RetryFilter<T> :
        IFilter<T>
        where T : class, PipeContext
    {
        readonly IRetryPolicy _retryPolicy;

        public RetryFilter(IRetryPolicy retryPolicy)
        {
            _retryPolicy = retryPolicy;
        }

        public IRetryPolicy RetryPolicy
        {
            get { return _retryPolicy; }
        }

        public async Task Send(T context, IPipe<T> next)
        {
            using (IRetryContext retryContext = _retryPolicy.GetRetryContext())
            {
                await Attempt(retryContext, context, next);
            }
        }

        public bool Visit(IPipeVisitor visitor)
        {
            return visitor.Visit(this);
        }

        static async Task Attempt(IRetryContext retryContext, T context, IPipe<T> next)
        {
            TimeSpan delay;
            try
            {
                await next.Send(context);

                return;
            }
            catch (Exception ex)
            {
                if (!retryContext.CanRetry(ex, out delay))
                    throw;
            }

            await Task.Delay(delay);

            await Attempt(retryContext, context, next);
        }
    }
}