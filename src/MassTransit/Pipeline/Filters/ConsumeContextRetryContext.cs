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
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;


    public class ConsumeContextRetryContext :
        RetryContext<ConsumeContext>
    {
        readonly RetryConsumeContext _context;
        readonly RetryContext<ConsumeContext> _retryContext;

        public ConsumeContextRetryContext(RetryContext<ConsumeContext> retryContext, RetryConsumeContext context)
        {
            _retryContext = retryContext;
            _context = context;

            _context.RetryAttempt = retryContext.RetryCount;
        }

        public ConsumeContext Context => _context;

        public Exception Exception => _retryContext.Exception;

        public int RetryCount => _retryContext.RetryCount;

        public int RetryAttempt => _retryContext.RetryAttempt;

        public Type ContextType => _retryContext.ContextType;

        public TimeSpan? Delay => _retryContext.Delay;

        public async Task PreRetry()
        {
            await _retryContext.PreRetry().ConfigureAwait(false);

            await _context.ClearPendingFaults().ConfigureAwait(false);
        }

        public async Task RetryFaulted(Exception exception)
        {
            await _retryContext.RetryFaulted(exception).ConfigureAwait(false);

            await _context.NotifyPendingFaults().ConfigureAwait(false);
        }

        public bool CanRetry(Exception exception, out RetryContext<ConsumeContext> retryContext)
        {
            RetryContext<ConsumeContext> policyRetryContext;
            var canRetry = _retryContext.CanRetry(exception, out policyRetryContext);

            retryContext = new ConsumeContextRetryContext(policyRetryContext, _context);

            return canRetry;
        }

        bool PipeContext.HasPayloadType(Type payloadType)
        {
            return _retryContext.HasPayloadType(payloadType);
        }

        bool PipeContext.TryGetPayload<TPayload>(out TPayload payload)
        {
            return _retryContext.TryGetPayload(out payload);
        }

        TPayload PipeContext.GetOrAddPayload<TPayload>(PayloadFactory<TPayload> payloadFactory)
        {
            return _retryContext.GetOrAddPayload(payloadFactory);
        }

        CancellationToken PipeContext.CancellationToken => _retryContext.CancellationToken;
    }


    public class ConsumeContextRetryContext<TFilter, TContext> :
        RetryContext<TFilter>
        where TFilter : class, ConsumeContext
        where TContext : RetryConsumeContext, TFilter
    {
        readonly TContext _context;
        readonly RetryContext<TFilter> _retryContext;

        public ConsumeContextRetryContext(RetryContext<TFilter> retryContext, TContext context)
        {
            _retryContext = retryContext;
            _context = context;

            _context.RetryAttempt = retryContext.RetryCount;
        }

        public TFilter Context => _context;

        public Exception Exception => _retryContext.Exception;

        public int RetryCount => _retryContext.RetryCount;

        public int RetryAttempt => _retryContext.RetryAttempt;

        public Type ContextType => _retryContext.ContextType;

        public TimeSpan? Delay => _retryContext.Delay;

        public async Task PreRetry()
        {
            await _retryContext.PreRetry().ConfigureAwait(false);

            await _context.ClearPendingFaults().ConfigureAwait(false);
        }

        public async Task RetryFaulted(Exception exception)
        {
            await _retryContext.RetryFaulted(exception).ConfigureAwait(false);

            await _context.NotifyPendingFaults().ConfigureAwait(false);
        }

        public bool CanRetry(Exception exception, out RetryContext<TFilter> retryContext)
        {
            RetryContext<TFilter> policyRetryContext;
            var canRetry = _retryContext.CanRetry(exception, out policyRetryContext);

            retryContext = new ConsumeContextRetryContext<TFilter, TContext>(policyRetryContext, _context);

            return canRetry;
        }

        bool PipeContext.HasPayloadType(Type payloadType)
        {
            return _retryContext.HasPayloadType(payloadType);
        }

        bool PipeContext.TryGetPayload<TPayload>(out TPayload payload)
        {
            return _retryContext.TryGetPayload(out payload);
        }

        TPayload PipeContext.GetOrAddPayload<TPayload>(PayloadFactory<TPayload> payloadFactory)
        {
            return _retryContext.GetOrAddPayload(payloadFactory);
        }

        CancellationToken PipeContext.CancellationToken => _retryContext.CancellationToken;
    }
}