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
    public class RetryFilter :
        IFilter<ConsumeContext>
    {
        readonly IRetryPolicy _retryPolicy;

        public RetryFilter(IRetryPolicy retryPolicy)
        {
            _retryPolicy = retryPolicy;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            _retryPolicy.Probe(context.CreateFilterScope("retry"));
        }

        [DebuggerNonUserCode]
        Task IFilter<ConsumeContext>.Send(ConsumeContext context, IPipe<ConsumeContext> next)
        {
            var retryContext = new RetryConsumeContext(context);

            return Attempt(retryContext, next);
        }

        [DebuggerNonUserCode]
        async Task Attempt(RetryConsumeContext context, IPipe<ConsumeContext> next)
        {
            context.ClearPendingFaults();

            TimeSpan delay;
            try
            {
                await next.Send(context).ConfigureAwait(false);

                return;
            }
            catch (Exception ex)
            {
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

            await Attempt(context, next).ConfigureAwait(false);
        }
    }


    /// <summary>
    /// Uses a retry policy to handle exceptions, retrying the operation in according
    /// with the policy
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RetryFilter<T> :
        IFilter<ConsumeContext<T>>
        where T : class
    {
        readonly IRetryPolicy _retryPolicy;

        public RetryFilter(IRetryPolicy retryPolicy)
        {
            _retryPolicy = retryPolicy;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            _retryPolicy.Probe(context.CreateFilterScope("retry"));
        }

        Task IFilter<ConsumeContext<T>>.Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
        {
            var retryContext = new RetryConsumeContext<T>(context);

            return Attempt(retryContext, next);
        }

        async Task Attempt(RetryConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
        {
            context.ClearPendingFaults();

            TimeSpan delay;
            try
            {
                await next.Send(context).ConfigureAwait(false);

                return;
            }
            catch (Exception ex)
            {
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

            await Attempt(context, next).ConfigureAwait(false);
        }
    }
}