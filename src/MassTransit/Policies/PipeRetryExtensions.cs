// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using GreenPipes.Payloads;


    public static class PipeRetryExtensions
    {
        public static async Task Retry(this IRetryPolicy retryPolicy, Func<Task> retryMethod, CancellationToken cancellationToken = default(CancellationToken))
        {
            RetryPolicyContext<InlinePipeContext> policyContext = retryPolicy.CreatePolicyContext(new InlinePipeContext(cancellationToken));

            try
            {
                await retryMethod().ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    var canceledException = exception as OperationCanceledException;
                    if (canceledException != null && canceledException.CancellationToken == cancellationToken)
                        throw;

                    cancellationToken.ThrowIfCancellationRequested();
                }

                RetryContext<InlinePipeContext> retryContext;
                if (!policyContext.CanRetry(exception, out retryContext))
                {
                    throw;
                }

                await Attempt(retryContext, retryMethod).ConfigureAwait(false);
            }
        }

        public static async Task<T> Retry<T>(this IRetryPolicy retryPolicy, Func<Task<T>> retryMethod,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            RetryPolicyContext<InlinePipeContext> policyContext = retryPolicy.CreatePolicyContext(new InlinePipeContext(cancellationToken));

            try
            {
                return await retryMethod().ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    var canceledException = exception as OperationCanceledException;
                    if (canceledException != null && canceledException.CancellationToken == cancellationToken)
                        throw;

                    cancellationToken.ThrowIfCancellationRequested();
                }

                RetryContext<InlinePipeContext> retryContext;
                if (!policyContext.CanRetry(exception, out retryContext))
                {
                    throw;
                }

                return await Attempt(retryContext, retryMethod).ConfigureAwait(false);
            }
        }

        static async Task Attempt<T>(RetryContext<T> retryContext, Func<Task> retryMethod)
            where T : class
        {
            while (retryContext.CancellationToken.IsCancellationRequested == false)
            {
                if (retryContext.Delay.HasValue)
                    await Task.Delay(retryContext.Delay.Value, retryContext.CancellationToken).ConfigureAwait(false);

                retryContext.CancellationToken.ThrowIfCancellationRequested();

                try
                {
                    await retryMethod().ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    if (retryContext.CancellationToken.IsCancellationRequested)
                    {
                        var canceledException = exception as OperationCanceledException;
                        if (canceledException != null && canceledException.CancellationToken == retryContext.CancellationToken)
                            throw;

                        retryContext.CancellationToken.ThrowIfCancellationRequested();
                    }

                    RetryContext<T> nextRetryContext;
                    if (!retryContext.CanRetry(exception, out nextRetryContext))
                    {
                        throw;
                    }

                    retryContext = nextRetryContext;
                }
            }

            retryContext.CancellationToken.ThrowIfCancellationRequested();
        }

        static async Task<TResult> Attempt<T, TResult>(RetryContext<T> retryContext, Func<Task<TResult>> retryMethod)
            where T : class
        {
            while (retryContext.CancellationToken.IsCancellationRequested == false)
            {
                if (retryContext.Delay.HasValue)
                    await Task.Delay(retryContext.Delay.Value, retryContext.CancellationToken).ConfigureAwait(false);

                retryContext.CancellationToken.ThrowIfCancellationRequested();

                try
                {
                    return await retryMethod().ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    if (retryContext.CancellationToken.IsCancellationRequested)
                    {
                        var canceledException = exception as OperationCanceledException;
                        if (canceledException != null && canceledException.CancellationToken == retryContext.CancellationToken)
                            throw;

                        retryContext.CancellationToken.ThrowIfCancellationRequested();
                    }

                    RetryContext<T> nextRetryContext;
                    if (!retryContext.CanRetry(exception, out nextRetryContext))
                    {
                        throw;
                    }

                    retryContext = nextRetryContext;
                }
            }

            retryContext.CancellationToken.ThrowIfCancellationRequested();

            throw new OperationCanceledException("Retry was cancelled");
        }

        public static async Task RetryUntilCancelled(this IRetryPolicy retryPolicy, Func<Task> retryMethod,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            await Task.Yield();

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await Retry(retryPolicy, retryMethod, cancellationToken).ConfigureAwait(false);
                }
                catch (TaskCanceledException)
                {
                }
                catch (OperationCanceledException)
                {
                }
            }
        }


        class InlinePipeContext :
            BasePipeContext,
            PipeContext
        {
            public InlinePipeContext(CancellationToken cancellationToken)
                : base(new PayloadCache(), cancellationToken)
            {
            }
        }
    }
}