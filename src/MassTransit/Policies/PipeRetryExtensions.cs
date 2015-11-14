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
    using System.Threading;
    using System.Threading.Tasks;


    public static class PipeRetryExtensions
    {
        public static async Task Retry(this IRetryPolicy retryPolicy, Func<Task> retryMethod,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            using (IRetryContext retryContext = retryPolicy.GetRetryContext())
            {
                await Attempt(retryContext, retryMethod, cancellationToken).ConfigureAwait(false);
            }
        }

        static async Task Attempt(IRetryContext retryContext, Func<Task> retryMethod,
            CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                throw new TaskCanceledException();

            TimeSpan delay;
            try
            {
                await retryMethod().ConfigureAwait(false);

                return;
            }
            catch (Exception ex)
            {
                if (!retryContext.CanRetry(ex, out delay))
                    throw;
            }

            await Task.Delay(delay, cancellationToken).ConfigureAwait(false);

            await Attempt(retryContext, retryMethod, cancellationToken).ConfigureAwait(false);
        }

        public static async Task<T> Retry<T>(this IRetryPolicy retryPolicy, Func<Task<T>> retryMethod, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            using (IRetryContext retryContext = retryPolicy.GetRetryContext())
            {
                return await Attempt(retryContext, retryMethod, cancellationToken).ConfigureAwait(false);
            }
        }

        static async Task<T> Attempt<T>(IRetryContext retryContext, Func<Task<T>> retryMethod, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                throw new TaskCanceledException();

            TimeSpan delay;
            try
            {
                return await retryMethod().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (!retryContext.CanRetry(ex, out delay))
                    throw;
            }

            await Task.Delay(delay, cancellationToken).ConfigureAwait(false);

            return await Attempt(retryContext, retryMethod, cancellationToken).ConfigureAwait(false);
        }
    }
}