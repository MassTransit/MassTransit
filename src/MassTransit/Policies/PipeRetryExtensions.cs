// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;


    public static class PipeRetryExtensions
    {
        public static async Task Retry(this IRetryPolicy retryPolicy, Func<Task> retryMethod, CancellationToken cancellationToken = default(CancellationToken))
        {
            RetryPolicyContext<string> policyContext = retryPolicy.CreatePolicyContext("");

            try
            {
                await retryMethod().ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                RetryContext<string> retryContext;
                if (!policyContext.CanRetry(exception, out retryContext))
                {
                    throw;
                }

                await Attempt(retryContext, retryMethod, cancellationToken).ConfigureAwait(false);
            }
        }

        public static async Task<T> Retry<T>(this IRetryPolicy retryPolicy, Func<Task<T>> retryMethod,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            RetryPolicyContext<string> policyContext = retryPolicy.CreatePolicyContext("");

            try
            {
                return await retryMethod().ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                RetryContext<string> retryContext;
                if (!policyContext.CanRetry(exception, out retryContext))
                {
                    throw;
                }

                return await Attempt(retryContext, retryMethod, cancellationToken).ConfigureAwait(false);
            }
        }

        static async Task Attempt<T>(RetryContext<T> retryContext, Func<Task> retryMethod, CancellationToken cancellationToken)
            where T : class
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                await retryMethod().ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                RetryContext<T> nextRetryContext;
                if (!retryContext.CanRetry(exception, out nextRetryContext))
                {
                    throw;
                }

                if (nextRetryContext.Delay.HasValue)
                    await Task.Delay(nextRetryContext.Delay.Value, cancellationToken).ConfigureAwait(false);

                await Attempt(nextRetryContext, retryMethod, cancellationToken).ConfigureAwait(false);
            }
        }

        static async Task<TResult> Attempt<T, TResult>(RetryContext<T> retryContext, Func<Task<TResult>> retryMethod, CancellationToken cancellationToken)
            where T : class
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                return await retryMethod().ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                RetryContext<T> nextRetryContext;
                if (!retryContext.CanRetry(exception, out nextRetryContext))
                {
                    throw;
                }

                if (nextRetryContext.Delay.HasValue)
                    await Task.Delay(nextRetryContext.Delay.Value, cancellationToken).ConfigureAwait(false);

                return await Attempt(nextRetryContext, retryMethod, cancellationToken).ConfigureAwait(false);
            }
        }

        public static async Task RetryUntilCancelled(this IRetryPolicy retryPolicy, Func<Task> retryMethod,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            await Task.Yield();

            while (!cancellationToken.IsCancellationRequested)
            {
                await Retry(retryPolicy, retryMethod, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}