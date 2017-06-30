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
            return source.Add(new ScheduleActivity<TInstance, TMessage>(x => message, schedule, x => schedule.Delay));
        }

        public static EventActivityBinder<TInstance> Schedule<TInstance, TMessage>(this EventActivityBinder<TInstance> source,
            Schedule<TInstance, TMessage> schedule, TMessage message, ScheduleDelayProvider<TInstance> delayProvider)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new ScheduleActivity<TInstance, TMessage>(x => message, schedule, delayProvider));
        }

        public static EventActivityBinder<TInstance> Schedule<TInstance, TMessage>(this EventActivityBinder<TInstance> source,
            Schedule<TInstance, TMessage> schedule, TMessage message, Action<SendContext> contextCallback)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new ScheduleActivity<TInstance, TMessage>(x => message, schedule, contextCallback, x => schedule.Delay));
        }

        public static EventActivityBinder<TInstance> Schedule<TInstance, TMessage>(this EventActivityBinder<TInstance> source,
            Schedule<TInstance, TMessage> schedule, TMessage message, Action<SendContext> contextCallback, ScheduleDelayProvider<TInstance> delayProvider)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new ScheduleActivity<TInstance, TMessage>(x => message, schedule, contextCallback, delayProvider));
        }

        public static EventActivityBinder<TInstance> Schedule<TInstance, TMessage>(this EventActivityBinder<TInstance> source,
            Schedule<TInstance, TMessage> schedule, EventMessageFactory<TInstance, TMessage> messageFactory)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new ScheduleActivity<TInstance, TMessage>(messageFactory, schedule, x => schedule.Delay));
        }

        public static EventActivityBinder<TInstance> Schedule<TInstance, TMessage>(this EventActivityBinder<TInstance> source,
            Schedule<TInstance, TMessage> schedule, EventMessageFactory<TInstance, TMessage> messageFactory, ScheduleDelayProvider<TInstance> delayProvider)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new ScheduleActivity<TInstance, TMessage>(messageFactory, schedule, delayProvider));
        }

        public static EventActivityBinder<TInstance> Schedule<TInstance, TMessage>(this EventActivityBinder<TInstance> source,
            Schedule<TInstance, TMessage> schedule, EventMessageFactory<TInstance, TMessage> messageFactory, Action<SendContext> contextCallback)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new ScheduleActivity<TInstance, TMessage>(messageFactory, schedule, contextCallback, x => schedule.Delay));
        }

        public static EventActivityBinder<TInstance> Schedule<TInstance, TMessage>(this EventActivityBinder<TInstance> source,
            Schedule<TInstance, TMessage> schedule, EventMessageFactory<TInstance, TMessage> messageFactory, Action<SendContext> contextCallback,
            ScheduleDelayProvider<TInstance> delayProvider)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new ScheduleActivity<TInstance, TMessage>(messageFactory, schedule, contextCallback, delayProvider));
        }

        public static EventActivityBinder<TInstance, TData> Schedule<TInstance, TData, TMessage>(
            this EventActivityBinder<TInstance, TData> source, Schedule<TInstance, TMessage> schedule, TMessage message)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new ScheduleActivity<TInstance, TData, TMessage>(x => message, schedule, x => schedule.Delay));
        }

        public static EventActivityBinder<TInstance, TData> Schedule<TInstance, TData, TMessage>(
            this EventActivityBinder<TInstance, TData> source, Schedule<TInstance, TMessage> schedule, TMessage message,
            ScheduleDelayProvider<TInstance, TData> delayProvider)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new ScheduleActivity<TInstance, TData, TMessage>(x => message, schedule, delayProvider));
        }

        public static EventActivityBinder<TInstance, TData> Schedule<TInstance, TData, TMessage>(
            this EventActivityBinder<TInstance, TData> source, Schedule<TInstance, TMessage> schedule, TMessage message,
            Action<SendContext> contextCallback)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new ScheduleActivity<TInstance, TData, TMessage>(x => message, schedule, x => schedule.Delay));
        }

        public static EventActivityBinder<TInstance, TData> Schedule<TInstance, TData, TMessage>(
            this EventActivityBinder<TInstance, TData> source, Schedule<TInstance, TMessage> schedule, TMessage message,
            Action<SendContext> contextCallback, ScheduleDelayProvider<TInstance, TData> delayProvider)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new ScheduleActivity<TInstance, TData, TMessage>(x => message, schedule, contextCallback, delayProvider));
        }

        public static EventActivityBinder<TInstance, TData> Schedule<TInstance, TData, TMessage>(
            this EventActivityBinder<TInstance, TData> source, Schedule<TInstance, TMessage> schedule,
            EventMessageFactory<TInstance, TData, TMessage> messageFactory)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new ScheduleActivity<TInstance, TData, TMessage>(messageFactory, schedule, x => schedule.Delay));
        }

        public static EventActivityBinder<TInstance, TData> Schedule<TInstance, TData, TMessage>(
            this EventActivityBinder<TInstance, TData> source, Schedule<TInstance, TMessage> schedule,
            EventMessageFactory<TInstance, TData, TMessage> messageFactory, ScheduleDelayProvider<TInstance, TData> delayProvider)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new ScheduleActivity<TInstance, TData, TMessage>(messageFactory, schedule, delayProvider));
        }

        public static EventActivityBinder<TInstance, TData> Schedule<TInstance, TData, TMessage>(
            this EventActivityBinder<TInstance, TData> source, Schedule<TInstance, TMessage> schedule,
            EventMessageFactory<TInstance, TData, TMessage> messageFactory,
            Action<SendContext> contextCallback)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new ScheduleActivity<TInstance, TData, TMessage>(messageFactory, schedule, contextCallback, x => schedule.Delay));
        }

        public static EventActivityBinder<TInstance, TData> Schedule<TInstance, TData, TMessage>(
            this EventActivityBinder<TInstance, TData> source, Schedule<TInstance, TMessage> schedule,
            EventMessageFactory<TInstance, TData, TMessage> messageFactory,
            Action<SendContext> contextCallback, ScheduleDelayProvider<TInstance, TData> delayProvider)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new ScheduleActivity<TInstance, TData, TMessage>(messageFactory, schedule, contextCallback, delayProvider));
        }

        public static ExceptionActivityBinder<TInstance, TData, TException> Schedule<TInstance, TData, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TData, TException> source, Schedule<TInstance, TMessage> schedule, TMessage message)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TException : Exception
            where TMessage : class
        {
            return source.Add(new FaultedScheduleActivity<TInstance, TData, TException, TMessage>(x => message, schedule, x => schedule.Delay));
        }

        public static ExceptionActivityBinder<TInstance, TData, TException> Schedule<TInstance, TData, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TData, TException> source, Schedule<TInstance, TMessage> schedule, TMessage message,
            ScheduleDelayProvider<TInstance, TData, TException> delayProvider)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TException : Exception
            where TMessage : class
        {
            return source.Add(new FaultedScheduleActivity<TInstance, TData, TException, TMessage>(x => message, schedule, delayProvider));
        }

        public static ExceptionActivityBinder<TInstance, TData, TException> Schedule<TInstance, TData, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TData, TException> source, Schedule<TInstance, TMessage> schedule, TMessage message,
            Action<SendContext> contextCallback)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TException : Exception
            where TMessage : class
        {
            return source.Add(new FaultedScheduleActivity<TInstance, TData, TException, TMessage>(x => message, schedule, contextCallback, x => schedule.Delay));
        }

        public static ExceptionActivityBinder<TInstance, TData, TException> Schedule<TInstance, TData, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TData, TException> source, Schedule<TInstance, TMessage> schedule, TMessage message,
            Action<SendContext> contextCallback, ScheduleDelayProvider<TInstance, TData, TException> delayProvider)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TException : Exception
            where TMessage : class
        {
            return source.Add(new FaultedScheduleActivity<TInstance, TData, TException, TMessage>(x => message, schedule, contextCallback, delayProvider));
        }

        public static ExceptionActivityBinder<TInstance, TData, TException> Schedule<TInstance, TData, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TData, TException> source, Schedule<TInstance, TMessage> schedule,
            EventExceptionMessageFactory<TInstance, TData, TException, TMessage> messageFactory)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TException : Exception
            where TMessage : class
        {
            return source.Add(new FaultedScheduleActivity<TInstance, TData, TException, TMessage>(messageFactory, schedule, x => schedule.Delay));
        }

        public static ExceptionActivityBinder<TInstance, TData, TException> Schedule<TInstance, TData, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TData, TException> source, Schedule<TInstance, TMessage> schedule,
            EventExceptionMessageFactory<TInstance, TData, TException, TMessage> messageFactory,
            Action<SendContext> contextCallback, ScheduleDelayProvider<TInstance, TData, TException> delayProvider)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TException : Exception
            where TMessage : class
        {
            return source.Add(new FaultedScheduleActivity<TInstance, TData, TException, TMessage>(messageFactory, schedule, contextCallback, delayProvider));
        }

        /// <summary>
        /// Unschedule a message, if the message was scheduled.
        /// </summary>
        /// <typeparam name="TInstance"></typeparam>
        /// <typeparam name="TData"></typeparam>
        /// <param name="source"></param>
        /// <param name="schedule"></param>
        /// <returns></returns>
        public static EventActivityBinder<TInstance, TData> Unschedule<TInstance, TData>(
            this EventActivityBinder<TInstance, TData> source, Schedule<TInstance> schedule)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
        {
            return source.Add(new UnscheduleActivity<TInstance>(schedule));
        }

        /// <summary>
        /// Unschedule a message, if the message was scheduled.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="schedule"></param>
        /// <returns></returns>
        public static ExceptionActivityBinder<TInstance, TData, TException> Unschedule<TInstance, TData, TException>(
            this ExceptionActivityBinder<TInstance, TData, TException> source, Schedule<TInstance> schedule)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TException : Exception
        {
            return source.Add(new FaultedUnscheduleActivity<TInstance>(schedule));
        }

        /// <summary>
        /// Unschedule a message, if the message was scheduled.
        /// </summary>
        /// <typeparam name="TInstance"></typeparam>
        /// <param name="source"></param>
        /// <param name="schedule"></param>
        /// <returns></returns>
        public static EventActivityBinder<TInstance> Unschedule<TInstance>(
            this EventActivityBinder<TInstance> source, Schedule<TInstance> schedule)
            where TInstance : class, SagaStateMachineInstance
        {
            return source.Add(new UnscheduleActivity<TInstance>(schedule));
        }
    }
}