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
namespace MassTransit.Pipeline.Filters
{
    using System;
    using System.Threading;
    using Context;
    using GreenPipes;


    public class ConsumeContextRetryPolicy :
        IRetryPolicy
    {
        readonly CancellationToken _cancellationToken;
        readonly IRetryPolicy _retryPolicy;

        public ConsumeContextRetryPolicy(IRetryPolicy retryPolicy, CancellationToken cancellationToken)
        {
            _retryPolicy = retryPolicy;
            _cancellationToken = cancellationToken;
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("retry-consumeContext");

            _retryPolicy.Probe(scope);
        }

        RetryPolicyContext<T> IRetryPolicy.CreatePolicyContext<T>(T context)
        {
            if (context is ConsumeContext consumeContext)
            {
                RetryPolicyContext<ConsumeContext> retryPolicyContext = _retryPolicy.CreatePolicyContext(consumeContext);

                var retryConsumeContext = new RetryConsumeContext(consumeContext, _retryPolicy, null);

                return new ConsumeContextRetryPolicyContext(retryPolicyContext, retryConsumeContext, _cancellationToken) as RetryPolicyContext<T>;
            }

            throw new ArgumentException("The argument must be a ConsumeContext", nameof(context));
        }

        public bool IsHandled(Exception exception)
        {
            return _retryPolicy.IsHandled(exception);
        }
    }


    public class ConsumeContextRetryPolicy<TFilter, TContext> :
        IRetryPolicy
        where TFilter : class, ConsumeContext
        where TContext : class, TFilter, ConsumeRetryContext
    {
        readonly CancellationToken _cancellationToken;
        readonly Func<TFilter, IRetryPolicy, RetryContext, TContext> _contextFactory;
        readonly IRetryPolicy _retryPolicy;

        public ConsumeContextRetryPolicy(IRetryPolicy retryPolicy, CancellationToken cancellationToken, Func<TFilter, IRetryPolicy, RetryContext, TContext> contextFactory)
        {
            _retryPolicy = retryPolicy;
            _cancellationToken = cancellationToken;
            _contextFactory = contextFactory;
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("retry-consumeContext");

            _retryPolicy.Probe(scope);
        }

        RetryPolicyContext<T> IRetryPolicy.CreatePolicyContext<T>(T context)
        {
            var filterContext = context as TFilter;
            if (filterContext == null)
                throw new ArgumentException($"The argument must be a {typeof(TFilter).Name}", nameof(context));

            RetryPolicyContext<TFilter> retryPolicyContext = _retryPolicy.CreatePolicyContext(filterContext);

            var retryConsumeContext = _contextFactory(filterContext, _retryPolicy, null);

            return new ConsumeContextRetryPolicyContext<TFilter, TContext>(retryPolicyContext, retryConsumeContext, _cancellationToken) as RetryPolicyContext<T>;
        }

        public bool IsHandled(Exception exception)
        {
            return _retryPolicy.IsHandled(exception);
        }
    }
}
