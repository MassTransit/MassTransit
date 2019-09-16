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
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using Logging;
    using Metadata;
    using Util;


    /// <summary>
    /// Consumes a message via a message handler and reports the message as consumed or faulted
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public class HandlerMessageFilter<TMessage> :
        IFilter<ConsumeContext<TMessage>>
        where TMessage : class
    {
        readonly MessageHandler<TMessage> _handler;
        long _completed;
        long _faulted;

        // TODO this needs a pipe like instance and consumer, to handle things like retry, etc.
        public HandlerMessageFilter(MessageHandler<TMessage> handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            _handler = handler;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            ProbeContext scope = context.CreateFilterScope("handler");
            scope.Add("completed", _completed);
            scope.Add("faulted", _faulted);
        }

        [DebuggerNonUserCode]
        async Task IFilter<ConsumeContext<TMessage>>.Send(ConsumeContext<TMessage> context, IPipe<ConsumeContext<TMessage>> next)
        {
            var activity = LogContext.IfEnabled(OperationName.Consumer.Handle)?.StartActivity(new {MessageType = TypeMetadataCache<TMessage>.ShortName});

            Stopwatch timer = Stopwatch.StartNew();
            try
            {
                await Task.Yield();

                await _handler(context).ConfigureAwait(false);

                await context.NotifyConsumed(timer.Elapsed, TypeMetadataCache<MessageHandler<TMessage>>.ShortName).ConfigureAwait(false);

                Interlocked.Increment(ref _completed);

                await next.Send(context).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await context.NotifyFaulted(timer.Elapsed, TypeMetadataCache<MessageHandler<TMessage>>.ShortName, ex).ConfigureAwait(false);

                Interlocked.Increment(ref _faulted);
                throw;
            }
            finally
            {
                activity?.Stop();
            }
        }
    }
}
