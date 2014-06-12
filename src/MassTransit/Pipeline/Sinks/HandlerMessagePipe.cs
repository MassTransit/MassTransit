// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Pipeline.Sinks
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using SubscriptionConnectors;
    using Util;


    public class HandlerMessagePipe<TMessage> :
        IConsumeContextPipe<TMessage>
        where TMessage : class
    {
        readonly MessageHandler<TMessage> _handler;
        readonly IMessageRetryPolicy _retryPolicy;

        public HandlerMessagePipe(MessageHandler<TMessage> handler, IMessageRetryPolicy retryPolicy)
        {
            _handler = handler;
            _retryPolicy = retryPolicy;
        }

        public async Task Send(ConsumeContext<TMessage> context)
        {
            Stopwatch timer = Stopwatch.StartNew();
            try
            {
                await _retryPolicy.Retry(context, x => _handler(context));

                context.NotifyConsumed(timer.Elapsed, TypeMetadataCache<MessageHandler<TMessage>>.ShortName);
            }
            catch (Exception ex)
            {
                context.NotifyFaulted(TypeMetadataCache<MessageHandler<TMessage>>.ShortName, ex);
                throw;
            }
        }

        public bool Inspect(IConsumeContextPipeInspector inspector)
        {
            return inspector.Inspect(this);
        }
    }
}