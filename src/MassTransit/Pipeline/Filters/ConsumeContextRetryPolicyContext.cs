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
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using Transports;


    public class ConsumeContextRetryPolicyContext :
        RetryPolicyContext<ConsumeContext>
    {
        readonly RetryConsumeContext _context;
        readonly RetryPolicyContext<ConsumeContext> _policyContext;
        CancellationToken _cancellationToken;
        CancellationTokenRegistration _registration;

        public ConsumeContextRetryPolicyContext(RetryPolicyContext<ConsumeContext> policyContext, RetryConsumeContext context,
            CancellationToken cancellationToken)
        {
            _policyContext = policyContext;
            _context = context;
            _cancellationToken = cancellationToken;
        }

        public void Cancel()
        {
            _policyContext.Cancel();
        }

        public ConsumeContext Context => _context;

        public bool CanRetry(Exception exception, out RetryContext<ConsumeContext> retryContext)
        {
            var canRetry = _policyContext.CanRetry(exception, out RetryContext<ConsumeContext> policyRetryContext);
            if (canRetry)
            {
                _context.LogRetry(exception);
                _registration = _cancellationToken.Register(Cancel);
            }

            retryContext = new ConsumeContextRetryContext(policyRetryContext, canRetry ? _context.CreateNext(policyRetryContext) : _context);

            return canRetry;
        }

        public Task RetryFaulted(Exception exception)
        {
            return Task.WhenAll(_context.NotifyPendingFaults(), _policyContext.RetryFaulted(exception));
        }

        public void Dispose()
        {
            _registration.Dispose();
            _policyContext.Dispose();
        }
    }


    public class ConsumeContextRetryPolicyContext<TFilter, TContext> :
        RetryPolicyContext<TFilter>
        where TFilter : class, ConsumeContext
        where TContext : class, TFilter, ConsumeRetryContext
    {
        readonly TContext _context;
        CancellationToken _cancellationToken;
        readonly RetryPolicyContext<TFilter> _policyContext;
        CancellationTokenRegistration _registration;

        public ConsumeContextRetryPolicyContext(RetryPolicyContext<TFilter> policyContext, TContext context, CancellationToken cancellationToken)
        {
            _policyContext = policyContext;
            _context = context;
            _cancellationToken = cancellationToken;
        }

        public void Cancel()
        {
            _policyContext.Cancel();
        }

        public TFilter Context => _context;

        public bool CanRetry(Exception exception, out RetryContext<TFilter> retryContext)
        {
            var canRetry = _policyContext.CanRetry(exception, out RetryContext<TFilter> policyRetryContext);
            if (canRetry)
            {
                _context.LogRetry(exception);
                _registration = _cancellationToken.Register(Cancel);
            }

            retryContext = new ConsumeContextRetryContext<TFilter, TContext>(policyRetryContext,
                canRetry ? _context.CreateNext<TContext>(policyRetryContext) : _context);

            return canRetry;
        }

        public Task RetryFaulted(Exception exception)
        {
            return Task.WhenAll(_context.NotifyPendingFaults(), _policyContext.RetryFaulted(exception));
        }

        public void Dispose()
        {
            _registration.Dispose();
            _policyContext.Dispose();
        }
    }
}
