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
namespace MassTransit.Pipeline.Filters
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Monitoring.Introspection;
    using Util;


    /// <summary>
    /// Consumes a message via an existing class instance
    /// </summary>
    /// <typeparam name="TConsumer"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    public class InstanceMessageFilter<TConsumer, TMessage> :
        IFilter<ConsumeContext<TMessage>>
        where TConsumer : class
        where TMessage : class
    {
        readonly TConsumer _instance;
        readonly IPipe<ConsumerConsumeContext<TConsumer, TMessage>> _instancePipe;

        public InstanceMessageFilter(TConsumer instance, IFilter<ConsumerConsumeContext<TConsumer, TMessage>> instanceFilter)
        {
            if (instance == null)
                throw new ArgumentNullException("instance");
            if (instanceFilter == null)
                throw new ArgumentNullException("instanceFilter");

            _instance = instance;
            _instancePipe = Pipe.New<ConsumerConsumeContext<TConsumer, TMessage>>(x => x.Filter(instanceFilter));
        }

        async Task IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("instance");
            scope.Add("type", TypeMetadataCache<TConsumer>.ShortName);

            await _instancePipe.Probe(scope);
        }

        [DebuggerNonUserCode]
        async Task IFilter<ConsumeContext<TMessage>>.Send(ConsumeContext<TMessage> context, IPipe<ConsumeContext<TMessage>> next)
        {
            Stopwatch timer = Stopwatch.StartNew();
            try
            {
                await _instancePipe.Send(context.PushConsumer(_instance));

                context.NotifyConsumed(timer.Elapsed, TypeMetadataCache<TConsumer>.ShortName);

                await next.Send(context);
            }
            catch (Exception ex)
            {
                context.NotifyFaulted(timer.Elapsed, TypeMetadataCache<TConsumer>.ShortName, ex);
                throw;
            }
        }
    }
}