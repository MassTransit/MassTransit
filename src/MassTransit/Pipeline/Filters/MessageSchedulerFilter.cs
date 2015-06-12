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
    using Context;


    /// <summary>
    /// Adds the scheduler to the consume context, so that it can be used for message redelivery
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public class MessageSchedulerFilter<TMessage> :
        IFilter<ConsumeContext<TMessage>>
        where TMessage : class
    {
        readonly Uri _schedulerAddress;

        public MessageSchedulerFilter(Uri schedulerAddress)
        {
            _schedulerAddress = schedulerAddress;
        }

        [DebuggerNonUserCode]
        Task IFilter<ConsumeContext<TMessage>>.Send(ConsumeContext<TMessage> context, IPipe<ConsumeContext<TMessage>> next)
        {
            MessageSchedulerContext schedulerContext = new ConsumeMessageSchedulerContext(context, _schedulerAddress);

            context.GetOrAddPayload(() => schedulerContext);
            context.GetOrAddPayload<MessageRedeliveryContext>(() => new ConsumeMessageRedeliveryContext<TMessage>(context, schedulerContext));

            return next.Send(context);
        }

        bool IFilter<ConsumeContext<TMessage>>.Visit(IPipelineVisitor visitor)
        {
            return visitor.Visit(this);
        }
    }
}