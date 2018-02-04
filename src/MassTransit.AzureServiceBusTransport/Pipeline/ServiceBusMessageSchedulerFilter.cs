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
namespace MassTransit.AzureServiceBusTransport.Pipeline
{
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using MassTransit.Scheduling;
    using Scheduling;


    /// <summary>
    /// Adds the service bus message scheduler filter
    /// </summary>
    public class ServiceBusMessageSchedulerFilter :
        IFilter<ConsumeContext>
    {
        void IProbeSite.Probe(ProbeContext context)
        {
            context.CreateFilterScope("serviceBusScheduler");
        }

        [DebuggerNonUserCode]
        Task IFilter<ConsumeContext>.Send(ConsumeContext context, IPipe<ConsumeContext> next)
        {
            context.GetOrAddPayload<MessageSchedulerContext>(() =>
            {
                var scheduler = new MessageScheduler(new ServiceBusScheduleMessageProvider(context));

                return new ConsumeMessageSchedulerContext(scheduler, context.ReceiveContext.InputAddress);
            });

            return next.Send(context);
        }
    }
}