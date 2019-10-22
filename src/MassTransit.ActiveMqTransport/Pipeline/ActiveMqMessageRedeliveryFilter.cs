// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.ActiveMqTransport.Pipeline
{
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Contexts;
    using GreenPipes;
    using MassTransit.Scheduling;
    using Scheduling;


    /// <summary>
    /// Uses a delayed exchange in ActiveMQ to delay a message retry
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public class ActiveMqMessageRedeliveryFilter<TMessage> :
        IFilter<ConsumeContext<TMessage>>
        where TMessage : class
    {
        void IProbeSite.Probe(ProbeContext context)
        {
        }

        [DebuggerNonUserCode]
        Task IFilter<ConsumeContext<TMessage>>.Send(ConsumeContext<TMessage> context, IPipe<ConsumeContext<TMessage>> next)
        {
            context.GetOrAddPayload<MessageRedeliveryContext>(() =>
            {
                var provider = new ActiveMqScheduleMessageProvider(context);

                return new ActiveMqMessageRedeliveryContext<TMessage>(context, new MessageScheduler(provider));
            });

            return next.Send(context);
        }
    }
}
