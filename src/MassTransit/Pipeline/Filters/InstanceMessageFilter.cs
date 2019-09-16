// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using GreenPipes;
    using Metadata;
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

        public InstanceMessageFilter(TConsumer instance, IPipe<ConsumerConsumeContext<TConsumer, TMessage>> instancePipe)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));
            if (instancePipe == null)
                throw new ArgumentNullException(nameof(instancePipe));

            _instance = instance;
            _instancePipe = instancePipe;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            ProbeContext scope = context.CreateFilterScope("instance");
            scope.Add("type", TypeMetadataCache<TConsumer>.ShortName);

            _instancePipe.Probe(scope);
        }

        [DebuggerNonUserCode]
        async Task IFilter<ConsumeContext<TMessage>>.Send(ConsumeContext<TMessage> context, IPipe<ConsumeContext<TMessage>> next)
        {
            Stopwatch timer = Stopwatch.StartNew();
            try
            {
                await Task.Yield();

                await _instancePipe.Send(context.PushConsumer(_instance)).ConfigureAwait(false);

                await context.NotifyConsumed(timer.Elapsed, TypeMetadataCache<TConsumer>.ShortName).ConfigureAwait(false);

                await next.Send(context).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await context.NotifyFaulted(timer.Elapsed, TypeMetadataCache<TConsumer>.ShortName, ex).ConfigureAwait(false);

                throw;
            }
        }
    }
}