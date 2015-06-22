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
    using Monitoring.Introspection;
    using Util;


    /// <summary>
    /// Consumes a message via a message handler and reports the message as consumed or faulted
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public class ObserverMessageFilter<TMessage> :
        IFilter<ConsumeContext<TMessage>>
        where TMessage : class
    {
        readonly IObserver<ConsumeContext<TMessage>> _observer;

        public ObserverMessageFilter(IObserver<ConsumeContext<TMessage>> observer)
        {
            if (observer == null)
                throw new ArgumentNullException("observer");

            _observer = observer;
        }

        async Task IProbeSite.Probe(ProbeContext context)
        {
        }

        [DebuggerNonUserCode]
        async Task IFilter<ConsumeContext<TMessage>>.Send(ConsumeContext<TMessage> context, IPipe<ConsumeContext<TMessage>> next)
        {
            Stopwatch timer = Stopwatch.StartNew();
            try
            {
                _observer.OnNext(context);

                context.NotifyConsumed(timer.Elapsed, TypeMetadataCache.GetShortName(_observer.GetType()));

                await next.Send(context);
            }
            catch (Exception ex)
            {
                context.NotifyFaulted(timer.Elapsed, TypeMetadataCache.GetShortName(_observer.GetType()), ex);
                
                _observer.OnError(ex);

                throw;
            }
        }

        bool IFilter<ConsumeContext<TMessage>>.Visit(IPipelineVisitor visitor)
        {
            return visitor.Visit(this);
        }
    }
}