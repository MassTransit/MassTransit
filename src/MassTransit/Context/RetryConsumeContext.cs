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
namespace MassTransit.Context
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using GreenPipes.Payloads;
    using Util;


    public class RetryConsumeContext :
        ConsumeContextProxy,
        ConsumeRetryContext
    {
        readonly ConsumeContext _context;
        readonly IList<PendingFault> _pendingFaults;

        public RetryConsumeContext(ConsumeContext context)
            : base(context, new PayloadCacheScope(context))
        {
            _context = context;
            _pendingFaults = new List<PendingFault>();
        }

        public int RetryAttempt { get; set; }

        public override bool TryGetMessage<T>(out ConsumeContext<T> consumeContext)
        {
            if (base.TryGetMessage(out ConsumeContext<T> messageContext))
            {
                consumeContext = new MessageConsumeContext<T>(this, messageContext.Message);
                return true;
            }

            consumeContext = null;
            return false;
        }

        public override Task NotifyFaulted<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception)
        {
            _pendingFaults.Add(new PendingFault<T>(context, duration, consumerType, exception));

            return TaskUtil.Completed;
        }

        public RetryConsumeContext CreateNext()
        {
            return new RetryConsumeContext(_context);
        }

        public virtual TContext CreateNext<TContext>()
            where TContext : RetryConsumeContext
        {
            throw new InvalidOperationException("This is only supported by a derived type");
        }

        public Task NotifyPendingFaults()
        {
            return Task.WhenAll(_pendingFaults.Select(x => x.Notify(_context)));
        }


        interface PendingFault
        {
            Task Notify(ConsumeContext context);
        }


        class PendingFault<T> :
            PendingFault
            where T : class
        {
            readonly string _consumerType;
            readonly ConsumeContext<T> _context;
            readonly TimeSpan _elapsed;
            readonly Exception _exception;

            public PendingFault(ConsumeContext<T> context, TimeSpan elapsed, string consumerType, Exception exception)
            {
                _context = context;
                _elapsed = elapsed;
                _consumerType = consumerType;
                _exception = exception;
            }

            public Task Notify(ConsumeContext context)
            {
                return context.NotifyFaulted(_context, _elapsed, _consumerType, _exception);
            }
        }
    }


    public class RetryConsumeContext<T> :
        RetryConsumeContext,
        ConsumeContext<T>
        where T : class
    {
        readonly ConsumeContext<T> _context;

        public RetryConsumeContext(ConsumeContext<T> context)
            : base(context)
        {
            _context = context;
        }

        T ConsumeContext<T>.Message => _context.Message;

        public Task NotifyConsumed(TimeSpan duration, string consumerType)
        {
            return NotifyConsumed(_context, duration, consumerType);
        }

        public Task NotifyFaulted(TimeSpan duration, string consumerType, Exception exception)
        {
            return NotifyFaulted(_context, duration, consumerType, exception);
        }

        public override TContext CreateNext<TContext>()
        {
            return new RetryConsumeContext<T>(_context) as TContext
                ?? throw new ArgumentException($"The context type is not valid: {TypeMetadataCache<T>.ShortName}");
        }
    }
}