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
namespace MassTransit.ActiveMqTransport
{
    using System;
    using System.Threading.Tasks;
    using Contexts;
    using MassTransit.Scheduling;
    using Scheduling;

    public static class ActiveMqDeferMessageExtensions
    {
        /// <summary>
        /// Defers the message for redelivery using a delayed exchange.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="delay"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static Task Defer<T>(this ConsumeContext<T> context, TimeSpan delay, Action<ConsumeContext, SendContext> callback = null)
            where T : class
        {
            if (!context.TryGetPayload(out IMessageScheduler scheduler))
            {
                if (!context.TryGetPayload(out SessionContext modelContext))
                {
                    throw new ArgumentException("A valid message scheduler was not found, and no ModelContext was available", nameof(context));
                }

                var provider = new DelayedExchangeScheduleMessageProvider(context, modelContext.ConnectionContext.Topology,
                    modelContext.ConnectionContext.HostAddress);

                scheduler = new MessageScheduler(provider);
            }

            MessageRedeliveryContext redeliveryContext = new DelayedExchangeMessageRedeliveryContext<T>(context, scheduler);

            return redeliveryContext.ScheduleRedelivery(delay, callback);
        }
    }
}
