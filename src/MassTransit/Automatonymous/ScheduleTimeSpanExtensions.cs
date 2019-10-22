namespace Automatonymous
{
    using System;
    using System.Threading.Tasks;
    using Activities;
    using Binders;
    using MassTransit;


    public static class ScheduleTimeSpanExtensions
    {
        public static EventActivityBinder<TInstance> Schedule<TInstance, TMessage>(this EventActivityBinder<TInstance> source,
            Schedule<TInstance, TMessage> schedule, TMessage message, Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
        {
            DateTime TimeProvider(ConsumeEventContext<TInstance> context)
            {
                return DateTime.UtcNow + schedule.Delay;
            }

            return source.Add(new ScheduleActivity<TInstance, TMessage>(x => message, schedule, TimeProvider, contextCallback));
        }

        public static EventActivityBinder<TInstance> Schedule<TInstance, TMessage>(this EventActivityBinder<TInstance> source,
            Schedule<TInstance, TMessage> schedule, Task<TMessage> message, Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
        {
            DateTime TimeProvider(ConsumeEventContext<TInstance> context)
            {
                return DateTime.UtcNow + schedule.Delay;
            }

            return source.Add(new ScheduleActivity<TInstance, TMessage>(x => message, schedule, TimeProvider, contextCallback));
        }

        public static EventActivityBinder<TInstance> Schedule<TInstance, TMessage>(this EventActivityBinder<TInstance> source,
            Schedule<TInstance, TMessage> schedule, TMessage message, ScheduleDelayProvider<TInstance> delayProvider,
            Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
        {
            DateTime TimeProvider(ConsumeEventContext<TInstance> context)
            {
                return DateTime.UtcNow + delayProvider(context);
            }

            return source.Add(new ScheduleActivity<TInstance, TMessage>(x => message, schedule, TimeProvider, contextCallback));
        }

        public static EventActivityBinder<TInstance> Schedule<TInstance, TMessage>(this EventActivityBinder<TInstance> source,
            Schedule<TInstance, TMessage> schedule, Task<TMessage> message, ScheduleDelayProvider<TInstance> delayProvider,
            Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
        {
            DateTime TimeProvider(ConsumeEventContext<TInstance> context)
            {
                return DateTime.UtcNow + delayProvider(context);
            }

            return source.Add(new ScheduleActivity<TInstance, TMessage>(x => message, schedule, TimeProvider, contextCallback));
        }

        public static EventActivityBinder<TInstance> Schedule<TInstance, TMessage>(this EventActivityBinder<TInstance> source,
            Schedule<TInstance, TMessage> schedule, EventMessageFactory<TInstance, TMessage> messageFactory, Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
        {
            DateTime TimeProvider(ConsumeEventContext<TInstance> context)
            {
                return DateTime.UtcNow + schedule.Delay;
            }

            return source.Add(new ScheduleActivity<TInstance, TMessage>(messageFactory, schedule, TimeProvider, contextCallback));
        }

        public static EventActivityBinder<TInstance> Schedule<TInstance, TMessage>(this EventActivityBinder<TInstance> source,
            Schedule<TInstance, TMessage> schedule, AsyncEventMessageFactory<TInstance, TMessage> messageFactory, Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
        {
            DateTime TimeProvider(ConsumeEventContext<TInstance> context)
            {
                return DateTime.UtcNow + schedule.Delay;
            }

            return source.Add(new ScheduleActivity<TInstance, TMessage>(messageFactory, schedule, TimeProvider, contextCallback));
        }

        public static EventActivityBinder<TInstance> Schedule<TInstance, TMessage>(this EventActivityBinder<TInstance> source,
            Schedule<TInstance, TMessage> schedule, EventMessageFactory<TInstance, TMessage> messageFactory, ScheduleDelayProvider<TInstance> delayProvider,
            Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
        {
            DateTime TimeProvider(ConsumeEventContext<TInstance> context)
            {
                return DateTime.UtcNow + delayProvider(context);
            }

            return source.Add(new ScheduleActivity<TInstance, TMessage>(messageFactory, schedule, TimeProvider, contextCallback));
        }

        public static EventActivityBinder<TInstance> Schedule<TInstance, TMessage>(this EventActivityBinder<TInstance> source,
            Schedule<TInstance, TMessage> schedule, AsyncEventMessageFactory<TInstance, TMessage> messageFactory,
            ScheduleDelayProvider<TInstance> delayProvider, Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
        {
            DateTime TimeProvider(ConsumeEventContext<TInstance> context)
            {
                return DateTime.UtcNow + delayProvider(context);
            }

            return source.Add(new ScheduleActivity<TInstance, TMessage>(messageFactory, schedule, TimeProvider, contextCallback));
        }

        public static EventActivityBinder<TInstance, TData> Schedule<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> source,
            Schedule<TInstance, TMessage> schedule, TMessage message, Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            DateTime TimeProvider(ConsumeEventContext<TInstance, TData> context)
            {
                return DateTime.UtcNow + schedule.Delay;
            }

            return source.Add(new ScheduleActivity<TInstance, TData, TMessage>(x => message, schedule, TimeProvider, contextCallback));
        }

        public static EventActivityBinder<TInstance, TData> Schedule<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> source,
            Schedule<TInstance, TMessage> schedule, Task<TMessage> message, Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            DateTime TimeProvider(ConsumeEventContext<TInstance, TData> context)
            {
                return DateTime.UtcNow + schedule.Delay;
            }

            return source.Add(new ScheduleActivity<TInstance, TData, TMessage>(x => message, schedule, TimeProvider, contextCallback));
        }

        public static EventActivityBinder<TInstance, TData> Schedule<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> source,
            Schedule<TInstance, TMessage> schedule, TMessage message, ScheduleDelayProvider<TInstance, TData> delayProvider,
            Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            DateTime TimeProvider(ConsumeEventContext<TInstance, TData> context)
            {
                return DateTime.UtcNow + delayProvider(context);
            }

            return source.Add(new ScheduleActivity<TInstance, TData, TMessage>(x => message, schedule, TimeProvider, contextCallback));
        }

        public static EventActivityBinder<TInstance, TData> Schedule<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> source,
            Schedule<TInstance, TMessage> schedule, Task<TMessage> message, ScheduleDelayProvider<TInstance, TData> delayProvider,
            Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            DateTime TimeProvider(ConsumeEventContext<TInstance, TData> context)
            {
                return DateTime.UtcNow + delayProvider(context);
            }

            return source.Add(new ScheduleActivity<TInstance, TData, TMessage>(x => message, schedule, TimeProvider, contextCallback));
        }

        public static EventActivityBinder<TInstance, TData> Schedule<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> source,
            Schedule<TInstance, TMessage> schedule, EventMessageFactory<TInstance, TData, TMessage> messageFactory, Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            DateTime TimeProvider(ConsumeEventContext<TInstance, TData> context)
            {
                return DateTime.UtcNow + schedule.Delay;
            }

            return source.Add(new ScheduleActivity<TInstance, TData, TMessage>(messageFactory, schedule, TimeProvider, contextCallback));
        }

        public static EventActivityBinder<TInstance, TData> Schedule<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> source,
            Schedule<TInstance, TMessage> schedule, AsyncEventMessageFactory<TInstance, TData, TMessage> messageFactory, Action<SendContext<TMessage>> contextCallback =
                null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            DateTime TimeProvider(ConsumeEventContext<TInstance, TData> context)
            {
                return DateTime.UtcNow + schedule.Delay;
            }

            return source.Add(new ScheduleActivity<TInstance, TData, TMessage>(messageFactory, schedule, TimeProvider, contextCallback));
        }

        public static EventActivityBinder<TInstance, TData> Schedule<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> source,
            Schedule<TInstance, TMessage> schedule,
            EventMessageFactory<TInstance, TData, TMessage> messageFactory,
            ScheduleDelayProvider<TInstance, TData> delayProvider, Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            DateTime TimeProvider(ConsumeEventContext<TInstance, TData> context)
            {
                return DateTime.UtcNow + delayProvider(context);
            }

            return source.Add(new ScheduleActivity<TInstance, TData, TMessage>(messageFactory, schedule, TimeProvider, contextCallback));
        }

        public static EventActivityBinder<TInstance, TData> Schedule<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> source,
            Schedule<TInstance, TMessage> schedule,
            AsyncEventMessageFactory<TInstance, TData, TMessage> messageFactory,
            ScheduleDelayProvider<TInstance, TData> delayProvider, Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            DateTime TimeProvider(ConsumeEventContext<TInstance, TData> context)
            {
                return DateTime.UtcNow + delayProvider(context);
            }

            return source.Add(new ScheduleActivity<TInstance, TData, TMessage>(messageFactory, schedule, TimeProvider, contextCallback));
        }

        public static ExceptionActivityBinder<TInstance, TException> Schedule<TInstance, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TException> source, Schedule<TInstance, TMessage> schedule, TMessage message,
            Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TException : Exception
            where TMessage : class
        {
            DateTime TimeProvider(ConsumeExceptionEventContext<TInstance, TException> context)
            {
                return DateTime.UtcNow + schedule.Delay;
            }

            return source.Add(new FaultedScheduleActivity<TInstance, TException, TMessage>(x => message, schedule, TimeProvider, contextCallback));
        }

        public static ExceptionActivityBinder<TInstance, TException> Schedule<TInstance, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TException> source, Schedule<TInstance, TMessage> schedule, Task<TMessage> message,
            Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TException : Exception
            where TMessage : class
        {
            DateTime TimeProvider(ConsumeExceptionEventContext<TInstance, TException> context)
            {
                return DateTime.UtcNow + schedule.Delay;
            }

            return source.Add(new FaultedScheduleActivity<TInstance, TException, TMessage>(x => message, schedule, TimeProvider, contextCallback));
        }

        public static ExceptionActivityBinder<TInstance, TException> Schedule<TInstance, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TException> source, Schedule<TInstance, TMessage> schedule, TMessage message,
            ScheduleDelayExceptionProvider<TInstance, TException> delayProvider, Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TException : Exception
            where TMessage : class
        {
            DateTime TimeProvider(ConsumeExceptionEventContext<TInstance, TException> context)
            {
                return DateTime.UtcNow + delayProvider(context);
            }

            return source.Add(new FaultedScheduleActivity<TInstance, TException, TMessage>(x => message, schedule, TimeProvider, contextCallback));
        }

        public static ExceptionActivityBinder<TInstance, TException> Schedule<TInstance, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TException> source, Schedule<TInstance, TMessage> schedule, Task<TMessage> message,
            ScheduleDelayExceptionProvider<TInstance, TException> delayProvider, Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TException : Exception
            where TMessage : class
        {
            DateTime TimeProvider(ConsumeExceptionEventContext<TInstance, TException> context)
            {
                return DateTime.UtcNow + delayProvider(context);
            }

            return source.Add(new FaultedScheduleActivity<TInstance, TException, TMessage>(x => message, schedule, TimeProvider, contextCallback));
        }

        public static ExceptionActivityBinder<TInstance, TException> Schedule<TInstance, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TException> source, Schedule<TInstance, TMessage> schedule,
            EventExceptionMessageFactory<TInstance, TException, TMessage> messageFactory, Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TException : Exception
            where TMessage : class
        {
            DateTime TimeProvider(ConsumeExceptionEventContext<TInstance, TException> context)
            {
                return DateTime.UtcNow + schedule.Delay;
            }

            return source.Add(new FaultedScheduleActivity<TInstance, TException, TMessage>(messageFactory, schedule, TimeProvider, contextCallback));
        }

        public static ExceptionActivityBinder<TInstance, TException> Schedule<TInstance, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TException> source, Schedule<TInstance, TMessage> schedule,
            AsyncEventExceptionMessageFactory<TInstance, TException, TMessage> messageFactory, Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TException : Exception
            where TMessage : class
        {
            DateTime TimeProvider(ConsumeExceptionEventContext<TInstance, TException> context)
            {
                return DateTime.UtcNow + schedule.Delay;
            }

            return source.Add(new FaultedScheduleActivity<TInstance, TException, TMessage>(messageFactory, schedule, TimeProvider, contextCallback));
        }

        public static ExceptionActivityBinder<TInstance, TException> Schedule<TInstance, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TException> source, Schedule<TInstance, TMessage> schedule,
            EventExceptionMessageFactory<TInstance, TException, TMessage> messageFactory,
            ScheduleDelayExceptionProvider<TInstance, TException> delayProvider, Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TException : Exception
            where TMessage : class
        {
            DateTime TimeProvider(ConsumeExceptionEventContext<TInstance, TException> context)
            {
                return DateTime.UtcNow + delayProvider(context);
            }

            return source.Add(new FaultedScheduleActivity<TInstance, TException, TMessage>(messageFactory, schedule, TimeProvider, contextCallback));
        }

        public static ExceptionActivityBinder<TInstance, TException> Schedule<TInstance, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TException> source, Schedule<TInstance, TMessage> schedule,
            AsyncEventExceptionMessageFactory<TInstance, TException, TMessage> messageFactory,
            ScheduleDelayExceptionProvider<TInstance, TException> delayProvider, Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TException : Exception
            where TMessage : class
        {
            DateTime TimeProvider(ConsumeExceptionEventContext<TInstance, TException> context)
            {
                return DateTime.UtcNow + delayProvider(context);
            }

            return source.Add(new FaultedScheduleActivity<TInstance, TException, TMessage>(messageFactory, schedule, TimeProvider, contextCallback));
        }

        public static ExceptionActivityBinder<TInstance, TData, TException> Schedule<TInstance, TData, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TData, TException> source, Schedule<TInstance, TMessage> schedule, TMessage message,
            Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TException : Exception
            where TMessage : class
        {
            DateTime TimeProvider(ConsumeExceptionEventContext<TInstance, TData, TException> context)
            {
                return DateTime.UtcNow + schedule.Delay;
            }

            return source.Add(new FaultedScheduleActivity<TInstance, TData, TException, TMessage>(x => message, schedule, TimeProvider, contextCallback));
        }

        public static ExceptionActivityBinder<TInstance, TData, TException> Schedule<TInstance, TData, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TData, TException> source, Schedule<TInstance, TMessage> schedule, Task<TMessage> message,
            Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TException : Exception
            where TMessage : class
        {
            DateTime TimeProvider(ConsumeExceptionEventContext<TInstance, TData, TException> context)
            {
                return DateTime.UtcNow + schedule.Delay;
            }

            return source.Add(new FaultedScheduleActivity<TInstance, TData, TException, TMessage>(x => message, schedule, TimeProvider, contextCallback));
        }

        public static ExceptionActivityBinder<TInstance, TData, TException> Schedule<TInstance, TData, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TData, TException> source, Schedule<TInstance, TMessage> schedule, TMessage message,
            ScheduleDelayExceptionProvider<TInstance, TData, TException> delayProvider, Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TException : Exception
            where TMessage : class
        {
            DateTime TimeProvider(ConsumeExceptionEventContext<TInstance, TData, TException> context)
            {
                return DateTime.UtcNow + delayProvider(context);
            }

            return source.Add(new FaultedScheduleActivity<TInstance, TData, TException, TMessage>(x => message, schedule, TimeProvider, contextCallback));
        }

        public static ExceptionActivityBinder<TInstance, TData, TException> Schedule<TInstance, TData, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TData, TException> source, Schedule<TInstance, TMessage> schedule, Task<TMessage> message,
            ScheduleDelayExceptionProvider<TInstance, TData, TException> delayProvider, Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TException : Exception
            where TMessage : class
        {
            DateTime TimeProvider(ConsumeExceptionEventContext<TInstance, TData, TException> context)
            {
                return DateTime.UtcNow + delayProvider(context);
            }

            return source.Add(new FaultedScheduleActivity<TInstance, TData, TException, TMessage>(x => message, schedule, TimeProvider, contextCallback));
        }

        public static ExceptionActivityBinder<TInstance, TData, TException> Schedule<TInstance, TData, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TData, TException> source, Schedule<TInstance, TMessage> schedule,
            EventExceptionMessageFactory<TInstance, TData, TException, TMessage> messageFactory, Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TException : Exception
            where TMessage : class
        {
            DateTime TimeProvider(ConsumeExceptionEventContext<TInstance, TData, TException> context)
            {
                return DateTime.UtcNow + schedule.Delay;
            }

            return source.Add(new FaultedScheduleActivity<TInstance, TData, TException, TMessage>(messageFactory, schedule, TimeProvider, contextCallback));
        }

        public static ExceptionActivityBinder<TInstance, TData, TException> Schedule<TInstance, TData, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TData, TException> source, Schedule<TInstance, TMessage> schedule,
            AsyncEventExceptionMessageFactory<TInstance, TData, TException, TMessage> messageFactory, Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TException : Exception
            where TMessage : class
        {
            DateTime TimeProvider(ConsumeExceptionEventContext<TInstance, TData, TException> context)
            {
                return DateTime.UtcNow + schedule.Delay;
            }

            return source.Add(new FaultedScheduleActivity<TInstance, TData, TException, TMessage>(messageFactory, schedule, TimeProvider, contextCallback));
        }

        public static ExceptionActivityBinder<TInstance, TData, TException> Schedule<TInstance, TData, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TData, TException> source, Schedule<TInstance, TMessage> schedule,
            EventExceptionMessageFactory<TInstance, TData, TException, TMessage> messageFactory,
            ScheduleDelayExceptionProvider<TInstance, TData, TException> delayProvider, Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TException : Exception
            where TMessage : class
        {
            DateTime TimeProvider(ConsumeExceptionEventContext<TInstance, TData, TException> context)
            {
                return DateTime.UtcNow + delayProvider(context);
            }

            return source.Add(new FaultedScheduleActivity<TInstance, TData, TException, TMessage>(messageFactory, schedule, TimeProvider, contextCallback));
        }

        public static ExceptionActivityBinder<TInstance, TData, TException> Schedule<TInstance, TData, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TData, TException> source, Schedule<TInstance, TMessage> schedule,
            AsyncEventExceptionMessageFactory<TInstance, TData, TException, TMessage> messageFactory,
            ScheduleDelayExceptionProvider<TInstance, TData, TException> delayProvider, Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TException : Exception
            where TMessage : class
        {
            DateTime TimeProvider(ConsumeExceptionEventContext<TInstance, TData, TException> context)
            {
                return DateTime.UtcNow + delayProvider(context);
            }

            return source.Add(new FaultedScheduleActivity<TInstance, TData, TException, TMessage>(messageFactory, schedule, TimeProvider, contextCallback));
        }

        /// <summary>
        /// Unschedule a message, if the message was scheduled.
        /// </summary>
        /// <typeparam name="TInstance"></typeparam>
        /// <typeparam name="TData"></typeparam>
        /// <param name="source"></param>
        /// <param name="schedule"></param>
        /// <returns></returns>
        public static EventActivityBinder<TInstance, TData> Unschedule<TInstance, TData>(this EventActivityBinder<TInstance, TData> source,
            Schedule<TInstance> schedule)
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
        public static EventActivityBinder<TInstance> Unschedule<TInstance>(this EventActivityBinder<TInstance> source, Schedule<TInstance> schedule)
            where TInstance : class, SagaStateMachineInstance
        {
            return source.Add(new UnscheduleActivity<TInstance>(schedule));
        }

        /// <summary>
        /// Unschedule a message, if the message was scheduled.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="schedule"></param>
        /// <returns></returns>
        public static ExceptionActivityBinder<TInstance, TException> Unschedule<TInstance, TException>(
            this ExceptionActivityBinder<TInstance, TException> source, Schedule<TInstance> schedule)
            where TInstance : class, SagaStateMachineInstance
            where TException : Exception
        {
            return source.Add(new FaultedUnscheduleActivity<TInstance>(schedule));
        }
    }
}
