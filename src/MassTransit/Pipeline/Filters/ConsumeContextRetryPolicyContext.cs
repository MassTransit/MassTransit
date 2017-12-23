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
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using Transports;


    public class ConsumeContextRetryPolicyContext :
        RetryPolicyContext<ConsumeContext>
    {
        readonly RetryConsumeContext _context;
        readonly RetryPolicyContext<ConsumeContext> _policyContext;

        public ConsumeContextRetryPolicyContext(RetryPolicyContext<ConsumeContext> policyContext, RetryConsumeContext context)
        {
            _policyContext = policyContext;
            _context = context;
        }

        public ConsumeContext Context => _context;

        public bool CanRetry(Exception exception, out RetryContext<ConsumeContext> retryContext)
        {
            var canRetry = _policyContext.CanRetry(exception, out RetryContext<ConsumeContext> policyRetryContext);
            if (canRetry)
                _context.LogRetry(exception);

            retryContext = new ConsumeContextRetryContext(policyRetryContext, canRetry ? _context.CreateNext() : _context);

            return canRetry;
        }

        public Task RetryFaulted(Exception exception)
        {
            return Task.WhenAll(_context.NotifyPendingFaults(), _policyContext.RetryFaulted(exception));
        }
    }


    public class ConsumeContextRetryPolicyContext<TFilter, TContext> :
        RetryPolicyContext<TFilter>
        where TFilter : class, ConsumeContext
        where TContext : RetryConsumeContext, TFilter
    {
        readonly TContext _context;
        readonly RetryPolicyContext<TFilter> _policyContext;

        public ConsumeContextRetryPolicyContext(RetryPolicyContext<TFilter> policyContext, TContext context)
        {
            _policyContext = policyContext;
            _context = context;
        }

        public TFilter Context => _context;

        public bool CanRetry(Exception exception, out RetryContext<TFilter> retryContext)
        {
            var canRetry = _policyContext.CanRetry(exception, out RetryContext<TFilter> policyRetryContext);

            if (canRetry)
                _context.LogRetry(exception);

            retryContext = new ConsumeContextRetryContext<TFilter, TContext>(policyRetryContext, canRetry ? _context.CreateNext<TContext>() : _context);

            return canRetry;
        }

        public Task RetryFaulted(Exception exception)
        {
            return Task.WhenAll(_context.NotifyPendingFaults(), _policyContext.RetryFaulted(exception));
        }
    }
}