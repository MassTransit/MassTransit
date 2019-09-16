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
    using System.Diagnostics;
    using System.Threading.Tasks;
    using GreenPipes;
    using Metadata;
    using Util;


    /// <summary>
    /// Consumes a message via a message handler and reports the message as consumed or faulted
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public class ObserverMessageFilter<TMessage> :
        IFilter<ConsumeContext<TMessage>>
        where TMessage : class
    {
        readonly string _observerType;
        readonly IObserver<ConsumeContext<TMessage>> _observer;

        public ObserverMessageFilter(IObserver<ConsumeContext<TMessage>> observer)
        {
            if (observer == null)
                throw new ArgumentNullException(nameof(observer));

            _observer = observer;
            _observerType = TypeMetadataCache.GetShortName(observer.GetType());
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("observer");
            scope.Add("observerType", _observerType);

        }

        [DebuggerNonUserCode]
        async Task IFilter<ConsumeContext<TMessage>>.Send(ConsumeContext<TMessage> context, IPipe<ConsumeContext<TMessage>> next)
        {
            var timer = Stopwatch.StartNew();
            try
            {
                await Task.Yield();

                _observer.OnNext(context);

                await context.NotifyConsumed(timer.Elapsed, _observerType).ConfigureAwait(false);

                await next.Send(context).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await context.NotifyFaulted(timer.Elapsed, _observerType, ex).ConfigureAwait(false);

                _observer.OnError(ex);

                throw;
            }
        }
    }
}