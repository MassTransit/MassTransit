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
    using Context;
    using GreenPipes;


    public class ConsumeContextRetryPolicy :
        IRetryPolicy
    {
        readonly IRetryPolicy _retryPolicy;

        public ConsumeContextRetryPolicy(IRetryPolicy retryPolicy)
        {
            _retryPolicy = retryPolicy;
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

                var retryConsumeContext = new RetryConsumeContext(consumeContext);

                return new ConsumeContextRetryPolicyContext(retryPolicyContext, retryConsumeContext) as RetryPolicyContext<T>;
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
        where TContext : RetryConsumeContext, TFilter
    {
        readonly Func<TFilter, TContext> _contextFactory;
        readonly IRetryPolicy _retryPolicy;

        public ConsumeContextRetryPolicy(IRetryPolicy retryPolicy, Func<TFilter, TContext> contextFactory)
        {
            _retryPolicy = retryPolicy;
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

            var retryConsumeContext = _contextFactory(filterContext);

            return new ConsumeContextRetryPolicyContext<TFilter, TContext>(retryPolicyContext, retryConsumeContext) as RetryPolicyContext<T>;
        }

        public bool IsHandled(Exception exception)
        {
            return _retryPolicy.IsHandled(exception);
        }
    }
}