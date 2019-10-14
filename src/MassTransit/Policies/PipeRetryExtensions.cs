// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using Context;
    using GreenPipes;
    using GreenPipes.Payloads;


    public static class PipeRetryExtensions
    {
        public static async Task Retry(this IRetryPolicy retryPolicy, Func<Task> retryMethod, CancellationToken cancellationToken = default)
        {
            var inlinePipeContext = new InlinePipeContext(cancellationToken);
            using (RetryPolicyContext<InlinePipeContext> policyContext = retryPolicy.CreatePolicyContext(inlinePipeContext))
            {
                try
                {
                    await retryMethod().ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        if (exception is OperationCanceledException canceledException && canceledException.CancellationToken == cancellationToken)
                            throw;

                        cancellationToken.ThrowIfCancellationRequested();
                    }

                    if (!policyContext.CanRetry(exception, out RetryContext<InlinePipeContext> retryContext))
                        throw;

                    await Attempt(inlinePipeContext, retryContext, retryMethod).ConfigureAwait(false);
                }
            }
        }

        public static async Task<T> Retry<T>(this IRetryPolicy retryPolicy, Func<Task<T>> retryMethod, CancellationToken cancellationToken = default)
        {
            var inlinePipeContext = new InlinePipeContext(cancellationToken);
            using (RetryPolicyContext<InlinePipeContext> policyContext = retryPolicy.CreatePolicyContext(inlinePipeContext))
            {
                try
                {
                    return await retryMethod().ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        if (exception is OperationCanceledException canceledException &&
                            canceledException.CancellationToken == cancellationToken)
                            throw;

                        cancellationToken.ThrowIfCancellationRequested();
                    }

                    if (!policyContext.CanRetry(exception, out RetryContext<InlinePipeContext> retryContext))
                        throw;

                    return await Attempt(inlinePipeContext, retryContext, retryMethod).ConfigureAwait(false);
                }
            }
        }

        static async Task Attempt<T>(T context, RetryContext<T> retryContext, Func<Task> retryMethod)
            where T : class, PipeContext
        {
            while (context.CancellationToken.IsCancellationRequested == false)
            {
                if (retryContext.Delay.HasValue)
                    await Task.Delay(retryContext.Delay.Value, context.CancellationToken).ConfigureAwait(false);

                context.CancellationToken.ThrowIfCancellationRequested();

                try
                {
                    await retryMethod().ConfigureAwait(false);

                    return;
                }
                catch (Exception exception)
                {
                    if (context.CancellationToken.IsCancellationRequested)
                    {
                        if (exception is OperationCanceledException canceledException && canceledException.CancellationToken == context.CancellationToken)
                            throw;

                        context.CancellationToken.ThrowIfCancellationRequested();
                    }

                    if (!retryContext.CanRetry(exception, out RetryContext<T> nextRetryContext))
                        throw;

                    retryContext = nextRetryContext;
                }
            }

            context.CancellationToken.ThrowIfCancellationRequested();
        }

        static async Task<TResult> Attempt<T, TResult>(T context, RetryContext<T> retryContext, Func<Task<TResult>> retryMethod)
            where T : class, PipeContext
        {
            while (context.CancellationToken.IsCancellationRequested == false)
            {
                if (retryContext.Delay.HasValue)
                    await Task.Delay(retryContext.Delay.Value, context.CancellationToken).ConfigureAwait(false);

                context.CancellationToken.ThrowIfCancellationRequested();

                try
                {
                    return await retryMethod().ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    if (context.CancellationToken.IsCancellationRequested)
                    {
                        if (exception is OperationCanceledException canceledException && canceledException.CancellationToken == context.CancellationToken)
                            throw;

                        context.CancellationToken.ThrowIfCancellationRequested();
                    }

                    if (!retryContext.CanRetry(exception, out RetryContext<T> nextRetryContext))
                        throw;

                    retryContext = nextRetryContext;
                }
            }

            context.CancellationToken.ThrowIfCancellationRequested();

            throw new OperationCanceledException("Retry was cancelled");
        }

        public static async Task RetryUntilCancelled(this IRetryPolicy retryPolicy, Func<Task> retryMethod, CancellationToken cancellationToken = default)
        {
            await Task.Yield();

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await Retry(retryPolicy, retryMethod, cancellationToken).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                }
                catch (Exception ex)
                {
                    LogContext.Warning?.Log(ex, "Repeating until cancelled: {Cancelled}", cancellationToken.IsCancellationRequested);
                }
            }
        }


        class InlinePipeContext :
            BasePipeContext,
            PipeContext
        {
            public InlinePipeContext(CancellationToken cancellationToken)
                : base(cancellationToken)
            {
            }
        }
    }
}
