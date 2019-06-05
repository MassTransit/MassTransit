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
namespace MassTransit.AzureServiceBusTransport.Scheduling
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using MassTransit.Scheduling;


    /// <summary>
    /// Sets the message enqueue time when sending the message, and invokes
    /// any developer-specified pipes.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ServiceBusScheduleMessagePipe<T> :
        ScheduleMessageContextPipe<T>
        where T : class
    {
        readonly DateTime _scheduledTime;

        public ServiceBusScheduleMessagePipe(DateTime scheduledTime)
        {
            _scheduledTime = scheduledTime;
        }

        public ServiceBusScheduleMessagePipe(DateTime scheduledTime, IPipe<SendContext<T>> pipe)
            : base(pipe)
        {
            _scheduledTime = scheduledTime;
        }

        public override Task Send(SendContext<T> context)
        {
            context.SetScheduledEnqueueTime(_scheduledTime);

            return base.Send(context);
        }
    }


    /// <summary>
    /// Sets the message enqueue time when sending the message, and invokes
    /// any developer-specified pipes.
    /// </summary>
    public class ServiceBusScheduleMessagePipe :
        ScheduleMessageContextPipe
    {
        readonly DateTime _scheduledTime;

        public ServiceBusScheduleMessagePipe(DateTime scheduledTime)
        {
            _scheduledTime = scheduledTime;
        }

        public ServiceBusScheduleMessagePipe(DateTime scheduledTime, IPipe<SendContext> pipe)
            : base(pipe)
        {
            _scheduledTime = scheduledTime;
        }

        public override Task Send(SendContext context)
        {
            context.SetScheduledEnqueueTime(_scheduledTime);

            return base.Send(context);
        }
    }
}
