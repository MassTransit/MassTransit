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
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using MassTransit.Logging;

    public class InMemoryOutboxFilter :
        IFilter<ConsumeContext>
    {
        static readonly ILog _log = Logger.Get<InMemoryOutboxFilter>();

        public async Task Send(ConsumeContext context, IPipe<ConsumeContext> next)
        {
            InMemoryOutboxMessageSchedulerContext outboxSchedulerContext = null;
            if (context.TryGetPayload(out MessageSchedulerContext schedulerContext))
            {
                outboxSchedulerContext = new InMemoryOutboxMessageSchedulerContext(schedulerContext);
                context.AddOrUpdatePayload(() => outboxSchedulerContext, _ => outboxSchedulerContext);
            }

            var outboxContext = new InMemoryOutboxConsumeContext(context);

            try
            {
                await next.Send(outboxContext).ConfigureAwait(false);

                await outboxContext.ExecutePendingActions().ConfigureAwait(false);

                await outboxContext.ConsumeCompleted.ConfigureAwait(false);
            }
            catch (Exception)
            {
                await outboxContext.DiscardPendingActions().ConfigureAwait(false);

                try
                {
                    if(outboxSchedulerContext != null)
                        await outboxSchedulerContext.CancelAllScheduledMessages().ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    // Failed to unschedule some messages
                    if (_log.IsWarnEnabled)
                        _log.Warn("Some or all of the outbox unschedule failed", e);
                }

                throw;
            }
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("outbox");
            scope.Add("type", "in-memory");
        }
    }


    public class InMemoryOutboxFilter<T> :
        IFilter<ConsumeContext<T>>
        where T : class
    {
        static readonly ILog _log = Logger.Get<InMemoryOutboxFilter<T>>();


        public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
        {
            InMemoryOutboxMessageSchedulerContext outboxSchedulerContext = null;
            if (context.TryGetPayload(out MessageSchedulerContext schedulerContext))
            {
                outboxSchedulerContext = new InMemoryOutboxMessageSchedulerContext(schedulerContext);
                context.AddOrUpdatePayload(() => outboxSchedulerContext, _ => outboxSchedulerContext);
            }

            var outboxContext = new InMemoryOutboxConsumeContext<T>(context);
            try
            {
                await next.Send(outboxContext).ConfigureAwait(false);

                await outboxContext.ExecutePendingActions().ConfigureAwait(false);

                await outboxContext.ConsumeCompleted.ConfigureAwait(false);
            }
            catch (Exception)
            {
                await outboxContext.DiscardPendingActions().ConfigureAwait(false);

                try
                {
                    if (outboxSchedulerContext != null)
                        await outboxSchedulerContext.CancelAllScheduledMessages().ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    // Failed to unschedule some messages
                    if (_log.IsWarnEnabled)
                        _log.Warn("Some or all of the outbox unschedule failed", e);
                }

                throw;
            }
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("outbox");
            scope.Add("type", "in-memory");
        }
    }
}
