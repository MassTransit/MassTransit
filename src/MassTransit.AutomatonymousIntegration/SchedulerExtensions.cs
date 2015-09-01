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
namespace Automatonymous
{
    using System;
    using Activities;
    using Binders;
    using MassTransit;


    public static class SchedulerExtensions
    {
        public static EventActivityBinder<TInstance> Schedule<TInstance, TMessage>(this EventActivityBinder<TInstance> source,
            Schedule<TInstance, TMessage> schedule, TMessage message)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new ScheduleActivity<TInstance, TMessage>(x => message, schedule));
        }

        public static EventActivityBinder<TInstance> Schedule<TInstance, TMessage>(this EventActivityBinder<TInstance> source,
            Schedule<TInstance, TMessage> schedule, TMessage message, Action<SendContext> contextCallback)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new ScheduleActivity<TInstance, TMessage>(x => message, schedule, contextCallback));
        }

        public static EventActivityBinder<TInstance> Schedule<TInstance, TMessage>(this EventActivityBinder<TInstance> source,
            Schedule<TInstance, TMessage> schedule, EventMessageFactory<TInstance, TMessage> messageFactory)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new ScheduleActivity<TInstance, TMessage>(messageFactory, schedule));
        }

        public static EventActivityBinder<TInstance> Schedule<TInstance, TMessage>(this EventActivityBinder<TInstance> source,
            Schedule<TInstance, TMessage> schedule, EventMessageFactory<TInstance, TMessage> messageFactory, Action<SendContext> contextCallback)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new ScheduleActivity<TInstance, TMessage>(messageFactory, schedule, contextCallback));
        }

        public static EventActivityBinder<TInstance, TData> Schedule<TInstance, TData, TMessage>(
            this EventActivityBinder<TInstance, TData> source, Schedule<TInstance, TMessage> schedule, TMessage message)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new ScheduleActivity<TInstance, TData, TMessage>(x => message, schedule));
        }

        public static EventActivityBinder<TInstance, TData> Schedule<TInstance, TData, TMessage>(
            this EventActivityBinder<TInstance, TData> source, Schedule<TInstance, TMessage> schedule, TMessage message,
            Action<SendContext> contextCallback)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new ScheduleActivity<TInstance, TData, TMessage>(x => message, schedule, contextCallback));
        }

        public static EventActivityBinder<TInstance, TData> Schedule<TInstance, TData, TMessage>(
            this EventActivityBinder<TInstance, TData> source, Schedule<TInstance, TMessage> schedule,
            EventMessageFactory<TInstance, TData, TMessage> messageFactory)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new ScheduleActivity<TInstance, TData, TMessage>(messageFactory, schedule));
        }

        public static EventActivityBinder<TInstance, TData> Schedule<TInstance, TData, TMessage>(
            this EventActivityBinder<TInstance, TData> source, Schedule<TInstance, TMessage> schedule,
            EventMessageFactory<TInstance, TData, TMessage> messageFactory,
            Action<SendContext> contextCallback)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new ScheduleActivity<TInstance, TData, TMessage>(messageFactory, schedule, contextCallback));
        }

        public static EventActivityBinder<TInstance, TData> Unschedule<TInstance, TData>(
            this EventActivityBinder<TInstance, TData> source, Schedule<TInstance> schedule)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
        {
            return source.Add(new UnscheduleActivity<TInstance>(schedule));
        }

        public static EventActivityBinder<TInstance> Unschedule<TInstance>(
            this EventActivityBinder<TInstance> source, Schedule<TInstance> schedule)
            where TInstance : class, SagaStateMachineInstance
        {
            return source.Add(new UnscheduleActivity<TInstance>(schedule));
        }
    }
}