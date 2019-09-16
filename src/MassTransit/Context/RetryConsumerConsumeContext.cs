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
namespace MassTransit.Context
{
    using System;
    using GreenPipes;
    using Metadata;
    using Util;


    public class RetryConsumerConsumeContext<TConsumer> :
        RetryConsumeContext,
        ConsumerConsumeContext<TConsumer>
        where TConsumer : class
    {
        readonly ConsumerConsumeContext<TConsumer> _context;

        public RetryConsumerConsumeContext(ConsumerConsumeContext<TConsumer> context, IRetryPolicy retryPolicy, RetryContext retryContext)
            : base(context, retryPolicy, retryContext)
        {
            _context = context;
        }

        public TConsumer Consumer => _context.Consumer;

        public override TContext CreateNext<TContext>(RetryContext retryContext)
        {
            return new RetryConsumerConsumeContext<TConsumer>(_context, RetryPolicy, retryContext) as TContext
                ?? throw new ArgumentException($"The context type is not valid: {TypeMetadataCache<TContext>.ShortName}");
        }
    }
}