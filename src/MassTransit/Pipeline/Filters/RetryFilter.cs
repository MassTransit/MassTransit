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
namespace MassTransit.Pipeline.Filters
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Context;


    /// <summary>
    /// Uses a retry policy to handle exceptions, retrying the operation in according
    /// with the policy
    /// </summary>
    public abstract class RetryFilterBase<T, TContext> :
        IFilter<T>
        where T : class, PipeContext
        where TContext : RetryConsumeContext, T
    {
        readonly IRetryPolicy _retryPolicy;

        protected RetryFilterBase(IRetryPolicy retryPolicy)
        {
            _retryPolicy = retryPolicy;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            _retryPolicy.Probe(context.CreateFilterScope("retry"));
        }

        [DebuggerNonUserCode]
        Task IFilter<T>.Send(T context, IPipe<T> next)
        {
            var retryContext = CreateRetryContext(context);

            return Attempt(retryContext, next);
        }

        protected abstract TContext CreateRetryContext(T context);

        [DebuggerNonUserCode]
        async Task Attempt(TContext context, IPipe<T> next)
        {
            while (true)
            {
                var cancellationToken = context.CancellationToken;
                cancellationToken.ThrowIfCancellationRequested();

                context.ClearPendingFaults();

                TimeSpan delay;
                try
                {
                    await next.Send(context).ConfigureAwait(false);
                    cancellationToken.ThrowIfCancellationRequested();

                    return;
                }
                catch (Exception ex)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        if (ex is OperationCanceledException && ((OperationCanceledException)ex).CancellationToken == cancellationToken)
                        {
                            throw;
                        }
                        cancellationToken.ThrowIfCancellationRequested();
                    }

                    if (!_retryPolicy.CanRetry(ex))
                    {
                        context.NotifyPendingFaults();
                        throw;
                    }

                    // by not adding the retry payload until the exception occurs, the deepest retry filter
                    // is the one to set the actual retry context with the deepest configured policy
                    var retryContext = context.GetOrAddPayload(() => _retryPolicy.GetRetryContext());
                    if (!retryContext.CanRetry(ex, out delay))
                    {
                        context.NotifyPendingFaults();
                        throw;
                    }
                }

                await Task.Delay(delay).ConfigureAwait(false);

                context.RetryAttempt++;
            }
        }
    }


    /// <summary>
    /// Uses a retry policy to handle exceptions, retrying the operation in according
    /// with the policy
    /// </summary>
    public class RetryFilter :
        RetryFilterBase<ConsumeContext, RetryConsumeContext>
    {
        public RetryFilter(IRetryPolicy retryPolicy)
            : base(retryPolicy)
        {
        }

        protected override RetryConsumeContext CreateRetryContext(ConsumeContext context)
        {
            return new RetryConsumeContext(context);
        }
    }


    /// <summary>
    /// Uses a retry policy to handle exceptions, retrying the operation in according
    /// with the policy
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RetryFilter<T> :
        RetryFilterBase<ConsumeContext<T>, RetryConsumeContext<T>>
        where T : class
    {
        public RetryFilter(IRetryPolicy retryPolicy)
            : base(retryPolicy)
        {
        }

        protected override RetryConsumeContext<T> CreateRetryContext(ConsumeContext<T> context)
        {
            return new RetryConsumeContext<T>(context);
        }
    }
}