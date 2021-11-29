namespace MassTransit
{
    using System;
    using System.Threading.Tasks;
    using SagaStateMachine;


    public static class ScheduleTimeSpanExtensions
    {
        public static EventActivityBinder<TSaga> Schedule<TSaga, TMessage>(this EventActivityBinder<TSaga> source,
            Schedule<TSaga, TMessage> schedule, TMessage message, Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TMessage : class
        {
            DateTime TimeProvider(BehaviorContext<TSaga> context)
            {
                return DateTime.UtcNow + schedule.GetDelay(context);
            }

            return source.Add(new ScheduleActivity<TSaga, TMessage>(schedule, TimeProvider, MessageFactory<TMessage>.Create(message, callback)));
        }

        public static EventActivityBinder<TSaga> Schedule<TSaga, TMessage>(this EventActivityBinder<TSaga> source,
            Schedule<TSaga, TMessage> schedule, Task<TMessage> message, Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TMessage : class
        {
            DateTime TimeProvider(BehaviorContext<TSaga> context)
            {
                return DateTime.UtcNow + schedule.GetDelay(context);
            }

            return source.Add(new ScheduleActivity<TSaga, TMessage>(schedule, TimeProvider, MessageFactory<TMessage>.Create(message, callback)));
        }

        public static EventActivityBinder<TSaga> Schedule<TSaga, TMessage>(this EventActivityBinder<TSaga> source,
            Schedule<TSaga, TMessage> schedule, TMessage message, ScheduleDelayProvider<TSaga> delayProvider,
            Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TMessage : class
        {
            DateTime TimeProvider(BehaviorContext<TSaga> context)
            {
                return DateTime.UtcNow + delayProvider(context);
            }

            return source.Add(new ScheduleActivity<TSaga, TMessage>(schedule, TimeProvider, MessageFactory<TMessage>.Create(message, callback)));
        }

        public static EventActivityBinder<TSaga> Schedule<TSaga, TMessage>(this EventActivityBinder<TSaga> source,
            Schedule<TSaga, TMessage> schedule, Task<TMessage> message, ScheduleDelayProvider<TSaga> delayProvider,
            Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TMessage : class
        {
            DateTime TimeProvider(BehaviorContext<TSaga> context)
            {
                return DateTime.UtcNow + delayProvider(context);
            }

            return source.Add(new ScheduleActivity<TSaga, TMessage>(schedule, TimeProvider, MessageFactory<TMessage>.Create(message, callback)));
        }

        public static EventActivityBinder<TSaga> Schedule<TSaga, TMessage>(this EventActivityBinder<TSaga> source,
            Schedule<TSaga, TMessage> schedule, EventMessageFactory<TSaga, TMessage> messageFactory,
            Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TMessage : class
        {
            DateTime TimeProvider(BehaviorContext<TSaga> context)
            {
                return DateTime.UtcNow + schedule.GetDelay(context);
            }

            return source.Add(new ScheduleActivity<TSaga, TMessage>(schedule, TimeProvider, MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static EventActivityBinder<TSaga> Schedule<TSaga, TMessage>(this EventActivityBinder<TSaga> source,
            Schedule<TSaga, TMessage> schedule, AsyncEventMessageFactory<TSaga, TMessage> messageFactory,
            Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TMessage : class
        {
            DateTime TimeProvider(BehaviorContext<TSaga> context)
            {
                return DateTime.UtcNow + schedule.GetDelay(context);
            }

            return source.Add(new ScheduleActivity<TSaga, TMessage>(schedule, TimeProvider, MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static EventActivityBinder<TSaga> Schedule<TSaga, TMessage>(this EventActivityBinder<TSaga> source,
            Schedule<TSaga, TMessage> schedule, Func<BehaviorContext<TSaga>, Task<SendTuple<TMessage>>> messageFactory,
            Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TMessage : class
        {
            DateTime TimeProvider(BehaviorContext<TSaga> context)
            {
                return DateTime.UtcNow + schedule.GetDelay(context);
            }

            return source.Add(new ScheduleActivity<TSaga, TMessage>(schedule, TimeProvider, MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static EventActivityBinder<TSaga> Schedule<TSaga, TMessage>(this EventActivityBinder<TSaga> source,
            Schedule<TSaga, TMessage> schedule, EventMessageFactory<TSaga, TMessage> messageFactory, ScheduleDelayProvider<TSaga> delayProvider,
            Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TMessage : class
        {
            DateTime TimeProvider(BehaviorContext<TSaga> context)
            {
                return DateTime.UtcNow + delayProvider(context);
            }

            return source.Add(new ScheduleActivity<TSaga, TMessage>(schedule, TimeProvider, MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static EventActivityBinder<TSaga> Schedule<TSaga, TMessage>(this EventActivityBinder<TSaga> source,
            Schedule<TSaga, TMessage> schedule, AsyncEventMessageFactory<TSaga, TMessage> messageFactory,
            ScheduleDelayProvider<TSaga> delayProvider, Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TMessage : class
        {
            DateTime TimeProvider(BehaviorContext<TSaga> context)
            {
                return DateTime.UtcNow + delayProvider(context);
            }

            return source.Add(new ScheduleActivity<TSaga, TMessage>(schedule, TimeProvider, MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static EventActivityBinder<TSaga> Schedule<TSaga, TMessage>(this EventActivityBinder<TSaga> source,
            Schedule<TSaga, TMessage> schedule, Func<BehaviorContext<TSaga>, Task<SendTuple<TMessage>>> messageFactory,
            ScheduleDelayProvider<TSaga> delayProvider, Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TMessage : class
        {
            DateTime TimeProvider(BehaviorContext<TSaga> context)
            {
                return DateTime.UtcNow + delayProvider(context);
            }

            return source.Add(new ScheduleActivity<TSaga, TMessage>(schedule, TimeProvider, MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static EventActivityBinder<TSaga, TData> Schedule<TSaga, TData, TMessage>(this EventActivityBinder<TSaga, TData> source,
            Schedule<TSaga, TMessage> schedule, TMessage message, Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            DateTime TimeProvider(BehaviorContext<TSaga, TData> context)
            {
                return DateTime.UtcNow + schedule.GetDelay(context);
            }

            return source.Add(new ScheduleActivity<TSaga, TData, TMessage>(schedule, TimeProvider, MessageFactory<TMessage>.Create(message, callback)));
        }

        public static EventActivityBinder<TSaga, TData> Schedule<TSaga, TData, TMessage>(this EventActivityBinder<TSaga, TData> source,
            Schedule<TSaga, TMessage> schedule, Task<TMessage> message, Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            DateTime TimeProvider(BehaviorContext<TSaga, TData> context)
            {
                return DateTime.UtcNow + schedule.GetDelay(context);
            }

            return source.Add(new ScheduleActivity<TSaga, TData, TMessage>(schedule, TimeProvider, MessageFactory<TMessage>.Create(message, callback)));
        }

        public static EventActivityBinder<TSaga, TData> Schedule<TSaga, TData, TMessage>(this EventActivityBinder<TSaga, TData> source,
            Schedule<TSaga, TMessage> schedule, TMessage message, ScheduleDelayProvider<TSaga, TData> delayProvider,
            Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            DateTime TimeProvider(BehaviorContext<TSaga, TData> context)
            {
                return DateTime.UtcNow + delayProvider(context);
            }

            return source.Add(new ScheduleActivity<TSaga, TData, TMessage>(schedule, TimeProvider, MessageFactory<TMessage>.Create(message, callback)));
        }

        public static EventActivityBinder<TSaga, TData> Schedule<TSaga, TData, TMessage>(this EventActivityBinder<TSaga, TData> source,
            Schedule<TSaga, TMessage> schedule, Task<TMessage> message, ScheduleDelayProvider<TSaga, TData> delayProvider,
            Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            DateTime TimeProvider(BehaviorContext<TSaga, TData> context)
            {
                return DateTime.UtcNow + delayProvider(context);
            }

            return source.Add(new ScheduleActivity<TSaga, TData, TMessage>(schedule, TimeProvider, MessageFactory<TMessage>.Create(message, callback)));
        }

        public static EventActivityBinder<TSaga, TData> Schedule<TSaga, TData, TMessage>(this EventActivityBinder<TSaga, TData> source,
            Schedule<TSaga, TMessage> schedule, EventMessageFactory<TSaga, TData, TMessage> messageFactory,
            Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            DateTime TimeProvider(BehaviorContext<TSaga, TData> context)
            {
                return DateTime.UtcNow + schedule.GetDelay(context);
            }

            return source.Add(new ScheduleActivity<TSaga, TData, TMessage>(schedule, TimeProvider, MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static EventActivityBinder<TSaga, TData> Schedule<TSaga, TData, TMessage>(this EventActivityBinder<TSaga, TData> source,
            Schedule<TSaga, TMessage> schedule, AsyncEventMessageFactory<TSaga, TData, TMessage> messageFactory,
            Action<SendContext<TMessage>> callback =
                null)
            where TSaga : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            DateTime TimeProvider(BehaviorContext<TSaga, TData> context)
            {
                return DateTime.UtcNow + schedule.GetDelay(context);
            }

            return source.Add(new ScheduleActivity<TSaga, TData, TMessage>(schedule, TimeProvider, MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static EventActivityBinder<TSaga, TData> Schedule<TSaga, TData, TMessage>(this EventActivityBinder<TSaga, TData> source,
            Schedule<TSaga, TMessage> schedule, Func<BehaviorContext<TSaga, TData>, Task<SendTuple<TMessage>>> messageFactory,
            Action<SendContext<TMessage>> callback =
                null)
            where TSaga : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            DateTime TimeProvider(BehaviorContext<TSaga, TData> context)
            {
                return DateTime.UtcNow + schedule.GetDelay(context);
            }

            return source.Add(new ScheduleActivity<TSaga, TData, TMessage>(schedule, TimeProvider, MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static EventActivityBinder<TSaga, TData> Schedule<TSaga, TData, TMessage>(this EventActivityBinder<TSaga, TData> source,
            Schedule<TSaga, TMessage> schedule,
            EventMessageFactory<TSaga, TData, TMessage> messageFactory,
            ScheduleDelayProvider<TSaga, TData> delayProvider, Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            DateTime TimeProvider(BehaviorContext<TSaga, TData> context)
            {
                return DateTime.UtcNow + delayProvider(context);
            }

            return source.Add(new ScheduleActivity<TSaga, TData, TMessage>(schedule, TimeProvider, MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static EventActivityBinder<TSaga, TData> Schedule<TSaga, TData, TMessage>(this EventActivityBinder<TSaga, TData> source,
            Schedule<TSaga, TMessage> schedule,
            AsyncEventMessageFactory<TSaga, TData, TMessage> messageFactory,
            ScheduleDelayProvider<TSaga, TData> delayProvider, Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            DateTime TimeProvider(BehaviorContext<TSaga, TData> context)
            {
                return DateTime.UtcNow + delayProvider(context);
            }

            return source.Add(new ScheduleActivity<TSaga, TData, TMessage>(schedule, TimeProvider, MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static EventActivityBinder<TSaga, TData> Schedule<TSaga, TData, TMessage>(this EventActivityBinder<TSaga, TData> source,
            Schedule<TSaga, TMessage> schedule,
            Func<BehaviorContext<TSaga, TData>, Task<SendTuple<TMessage>>> messageFactory,
            ScheduleDelayProvider<TSaga, TData> delayProvider, Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            DateTime TimeProvider(BehaviorContext<TSaga, TData> context)
            {
                return DateTime.UtcNow + delayProvider(context);
            }

            return source.Add(new ScheduleActivity<TSaga, TData, TMessage>(schedule, TimeProvider, MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static ExceptionActivityBinder<TSaga, TException> Schedule<TSaga, TException, TMessage>(this ExceptionActivityBinder<TSaga, TException> source,
            Schedule<TSaga, TMessage> schedule, TMessage message,
            Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TException : Exception
            where TMessage : class
        {
            DateTime TimeProvider(BehaviorExceptionContext<TSaga, TException> context)
            {
                return DateTime.UtcNow + schedule.GetDelay(context);
            }

            return source.Add(new FaultedScheduleActivity<TSaga, TException, TMessage>(schedule, TimeProvider,
                MessageFactory<TMessage>.Create(message, callback)));
        }

        public static ExceptionActivityBinder<TSaga, TException> Schedule<TSaga, TException, TMessage>(this ExceptionActivityBinder<TSaga, TException> source,
            Schedule<TSaga, TMessage> schedule, Task<TMessage> message,
            Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TException : Exception
            where TMessage : class
        {
            DateTime TimeProvider(BehaviorExceptionContext<TSaga, TException> context)
            {
                return DateTime.UtcNow + schedule.GetDelay(context);
            }

            return source.Add(new FaultedScheduleActivity<TSaga, TException, TMessage>(schedule, TimeProvider,
                MessageFactory<TMessage>.Create(message, callback)));
        }

        public static ExceptionActivityBinder<TSaga, TException> Schedule<TSaga, TException, TMessage>(this ExceptionActivityBinder<TSaga, TException> source,
            Schedule<TSaga, TMessage> schedule, TMessage message,
            ScheduleDelayExceptionProvider<TSaga, TException> delayProvider, Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TException : Exception
            where TMessage : class
        {
            DateTime TimeProvider(BehaviorExceptionContext<TSaga, TException> context)
            {
                return DateTime.UtcNow + delayProvider(context);
            }

            return source.Add(new FaultedScheduleActivity<TSaga, TException, TMessage>(schedule, TimeProvider,
                MessageFactory<TMessage>.Create(message, callback)));
        }

        public static ExceptionActivityBinder<TSaga, TException> Schedule<TSaga, TException, TMessage>(this ExceptionActivityBinder<TSaga, TException> source,
            Schedule<TSaga, TMessage> schedule, Task<TMessage> message,
            ScheduleDelayExceptionProvider<TSaga, TException> delayProvider, Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TException : Exception
            where TMessage : class
        {
            DateTime TimeProvider(BehaviorExceptionContext<TSaga, TException> context)
            {
                return DateTime.UtcNow + delayProvider(context);
            }

            return source.Add(new FaultedScheduleActivity<TSaga, TException, TMessage>(schedule, TimeProvider,
                MessageFactory<TMessage>.Create(message, callback)));
        }

        public static ExceptionActivityBinder<TSaga, TException> Schedule<TSaga, TException, TMessage>(this ExceptionActivityBinder<TSaga, TException> source,
            Schedule<TSaga, TMessage> schedule,
            EventExceptionMessageFactory<TSaga, TException, TMessage> messageFactory, Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TException : Exception
            where TMessage : class
        {
            DateTime TimeProvider(BehaviorExceptionContext<TSaga, TException> context)
            {
                return DateTime.UtcNow + schedule.GetDelay(context);
            }

            return source.Add(new FaultedScheduleActivity<TSaga, TException, TMessage>(schedule, TimeProvider,
                MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static ExceptionActivityBinder<TSaga, TException> Schedule<TSaga, TException, TMessage>(this ExceptionActivityBinder<TSaga, TException> source,
            Schedule<TSaga, TMessage> schedule,
            AsyncEventExceptionMessageFactory<TSaga, TException, TMessage> messageFactory, Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TException : Exception
            where TMessage : class
        {
            DateTime TimeProvider(BehaviorExceptionContext<TSaga, TException> context)
            {
                return DateTime.UtcNow + schedule.GetDelay(context);
            }

            return source.Add(new FaultedScheduleActivity<TSaga, TException, TMessage>(schedule, TimeProvider,
                MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static ExceptionActivityBinder<TSaga, TException> Schedule<TSaga, TException, TMessage>(this ExceptionActivityBinder<TSaga, TException> source,
            Schedule<TSaga, TMessage> schedule,
            Func<BehaviorExceptionContext<TSaga, TException>, Task<SendTuple<TMessage>>> messageFactory, Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TException : Exception
            where TMessage : class
        {
            DateTime TimeProvider(BehaviorExceptionContext<TSaga, TException> context)
            {
                return DateTime.UtcNow + schedule.GetDelay(context);
            }

            return source.Add(new FaultedScheduleActivity<TSaga, TException, TMessage>(schedule, TimeProvider,
                MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static ExceptionActivityBinder<TSaga, TException> Schedule<TSaga, TException, TMessage>(this ExceptionActivityBinder<TSaga, TException> source,
            Schedule<TSaga, TMessage> schedule,
            EventExceptionMessageFactory<TSaga, TException, TMessage> messageFactory,
            ScheduleDelayExceptionProvider<TSaga, TException> delayProvider, Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TException : Exception
            where TMessage : class
        {
            DateTime TimeProvider(BehaviorExceptionContext<TSaga, TException> context)
            {
                return DateTime.UtcNow + delayProvider(context);
            }

            return source.Add(new FaultedScheduleActivity<TSaga, TException, TMessage>(schedule, TimeProvider,
                MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static ExceptionActivityBinder<TSaga, TException> Schedule<TSaga, TException, TMessage>(this ExceptionActivityBinder<TSaga, TException> source,
            Schedule<TSaga, TMessage> schedule,
            AsyncEventExceptionMessageFactory<TSaga, TException, TMessage> messageFactory,
            ScheduleDelayExceptionProvider<TSaga, TException> delayProvider, Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TException : Exception
            where TMessage : class
        {
            DateTime TimeProvider(BehaviorExceptionContext<TSaga, TException> context)
            {
                return DateTime.UtcNow + delayProvider(context);
            }

            return source.Add(new FaultedScheduleActivity<TSaga, TException, TMessage>(schedule, TimeProvider,
                MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static ExceptionActivityBinder<TSaga, TException> Schedule<TSaga, TException, TMessage>(this ExceptionActivityBinder<TSaga, TException> source,
            Schedule<TSaga, TMessage> schedule,
            Func<BehaviorExceptionContext<TSaga, TException>, Task<SendTuple<TMessage>>> messageFactory,
            ScheduleDelayExceptionProvider<TSaga, TException> delayProvider, Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TException : Exception
            where TMessage : class
        {
            DateTime TimeProvider(BehaviorExceptionContext<TSaga, TException> context)
            {
                return DateTime.UtcNow + delayProvider(context);
            }

            return source.Add(new FaultedScheduleActivity<TSaga, TException, TMessage>(schedule, TimeProvider,
                MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static ExceptionActivityBinder<TSaga, TData, TException> Schedule<TSaga, TData, TException, TMessage>(
            this ExceptionActivityBinder<TSaga, TData, TException> source, Schedule<TSaga, TMessage> schedule, TMessage message,
            Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TData : class
            where TException : Exception
            where TMessage : class
        {
            DateTime TimeProvider(BehaviorExceptionContext<TSaga, TData, TException> context)
            {
                return DateTime.UtcNow + schedule.GetDelay(context);
            }

            return source.Add(new FaultedScheduleActivity<TSaga, TData, TException, TMessage>(schedule, TimeProvider,
                MessageFactory<TMessage>.Create(message, callback)));
        }

        public static ExceptionActivityBinder<TSaga, TData, TException> Schedule<TSaga, TData, TException, TMessage>(
            this ExceptionActivityBinder<TSaga, TData, TException> source, Schedule<TSaga, TMessage> schedule, Task<TMessage> message,
            Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TData : class
            where TException : Exception
            where TMessage : class
        {
            DateTime TimeProvider(BehaviorExceptionContext<TSaga, TData, TException> context)
            {
                return DateTime.UtcNow + schedule.GetDelay(context);
            }

            return source.Add(new FaultedScheduleActivity<TSaga, TData, TException, TMessage>(schedule, TimeProvider,
                MessageFactory<TMessage>.Create(message, callback)));
        }

        public static ExceptionActivityBinder<TSaga, TData, TException> Schedule<TSaga, TData, TException, TMessage>(
            this ExceptionActivityBinder<TSaga, TData, TException> source, Schedule<TSaga, TMessage> schedule, TMessage message,
            ScheduleDelayExceptionProvider<TSaga, TData, TException> delayProvider, Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TData : class
            where TException : Exception
            where TMessage : class
        {
            DateTime TimeProvider(BehaviorExceptionContext<TSaga, TData, TException> context)
            {
                return DateTime.UtcNow + delayProvider(context);
            }

            return source.Add(new FaultedScheduleActivity<TSaga, TData, TException, TMessage>(schedule, TimeProvider,
                MessageFactory<TMessage>.Create(message, callback)));
        }

        public static ExceptionActivityBinder<TSaga, TData, TException> Schedule<TSaga, TData, TException, TMessage>(
            this ExceptionActivityBinder<TSaga, TData, TException> source, Schedule<TSaga, TMessage> schedule, Task<TMessage> message,
            ScheduleDelayExceptionProvider<TSaga, TData, TException> delayProvider, Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TData : class
            where TException : Exception
            where TMessage : class
        {
            DateTime TimeProvider(BehaviorExceptionContext<TSaga, TData, TException> context)
            {
                return DateTime.UtcNow + delayProvider(context);
            }

            return source.Add(new FaultedScheduleActivity<TSaga, TData, TException, TMessage>(schedule, TimeProvider,
                MessageFactory<TMessage>.Create(message, callback)));
        }

        public static ExceptionActivityBinder<TSaga, TData, TException> Schedule<TSaga, TData, TException, TMessage>(
            this ExceptionActivityBinder<TSaga, TData, TException> source, Schedule<TSaga, TMessage> schedule,
            EventExceptionMessageFactory<TSaga, TData, TException, TMessage> messageFactory, Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TData : class
            where TException : Exception
            where TMessage : class
        {
            DateTime TimeProvider(BehaviorExceptionContext<TSaga, TData, TException> context)
            {
                return DateTime.UtcNow + schedule.GetDelay(context);
            }

            return source.Add(new FaultedScheduleActivity<TSaga, TData, TException, TMessage>(schedule, TimeProvider,
                MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static ExceptionActivityBinder<TSaga, TData, TException> Schedule<TSaga, TData, TException, TMessage>(
            this ExceptionActivityBinder<TSaga, TData, TException> source, Schedule<TSaga, TMessage> schedule,
            AsyncEventExceptionMessageFactory<TSaga, TData, TException, TMessage> messageFactory, Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TData : class
            where TException : Exception
            where TMessage : class
        {
            DateTime TimeProvider(BehaviorExceptionContext<TSaga, TData, TException> context)
            {
                return DateTime.UtcNow + schedule.GetDelay(context);
            }

            return source.Add(new FaultedScheduleActivity<TSaga, TData, TException, TMessage>(schedule, TimeProvider,
                MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static ExceptionActivityBinder<TSaga, TData, TException> Schedule<TSaga, TData, TException, TMessage>(
            this ExceptionActivityBinder<TSaga, TData, TException> source, Schedule<TSaga, TMessage> schedule,
            Func<BehaviorExceptionContext<TSaga, TData, TException>, Task<SendTuple<TMessage>>> messageFactory, Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TData : class
            where TException : Exception
            where TMessage : class
        {
            DateTime TimeProvider(BehaviorExceptionContext<TSaga, TData, TException> context)
            {
                return DateTime.UtcNow + schedule.GetDelay(context);
            }

            return source.Add(new FaultedScheduleActivity<TSaga, TData, TException, TMessage>(schedule, TimeProvider,
                MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static ExceptionActivityBinder<TSaga, TData, TException> Schedule<TSaga, TData, TException, TMessage>(
            this ExceptionActivityBinder<TSaga, TData, TException> source, Schedule<TSaga, TMessage> schedule,
            EventExceptionMessageFactory<TSaga, TData, TException, TMessage> messageFactory,
            ScheduleDelayExceptionProvider<TSaga, TData, TException> delayProvider, Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TData : class
            where TException : Exception
            where TMessage : class
        {
            DateTime TimeProvider(BehaviorExceptionContext<TSaga, TData, TException> context)
            {
                return DateTime.UtcNow + delayProvider(context);
            }

            return source.Add(new FaultedScheduleActivity<TSaga, TData, TException, TMessage>(schedule, TimeProvider,
                MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static ExceptionActivityBinder<TSaga, TData, TException> Schedule<TSaga, TData, TException, TMessage>(
            this ExceptionActivityBinder<TSaga, TData, TException> source, Schedule<TSaga, TMessage> schedule,
            AsyncEventExceptionMessageFactory<TSaga, TData, TException, TMessage> messageFactory,
            ScheduleDelayExceptionProvider<TSaga, TData, TException> delayProvider, Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TData : class
            where TException : Exception
            where TMessage : class
        {
            DateTime TimeProvider(BehaviorExceptionContext<TSaga, TData, TException> context)
            {
                return DateTime.UtcNow + delayProvider(context);
            }

            return source.Add(new FaultedScheduleActivity<TSaga, TData, TException, TMessage>(schedule, TimeProvider,
                MessageFactory<TMessage>.Create(messageFactory, callback)));
        }
        public static ExceptionActivityBinder<TSaga, TData, TException> Schedule<TSaga, TData, TException, TMessage>(
            this ExceptionActivityBinder<TSaga, TData, TException> source, Schedule<TSaga, TMessage> schedule,
            Func<BehaviorExceptionContext<TSaga, TData, TException>, Task<SendTuple<TMessage>>> messageFactory,
            ScheduleDelayExceptionProvider<TSaga, TData, TException> delayProvider, Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TData : class
            where TException : Exception
            where TMessage : class
        {
            DateTime TimeProvider(BehaviorExceptionContext<TSaga, TData, TException> context)
            {
                return DateTime.UtcNow + delayProvider(context);
            }

            return source.Add(new FaultedScheduleActivity<TSaga, TData, TException, TMessage>(schedule, TimeProvider,
                MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        /// <summary>
        /// Unschedule a message, if the message was scheduled.
        /// </summary>
        /// <typeparam name="TSaga"></typeparam>
        /// <typeparam name="TData"></typeparam>
        /// <param name="source"></param>
        /// <param name="schedule"></param>
        /// <returns></returns>
        public static EventActivityBinder<TSaga, TData> Unschedule<TSaga, TData>(this EventActivityBinder<TSaga, TData> source,
            Schedule<TSaga> schedule)
            where TSaga : class, SagaStateMachineInstance
            where TData : class
        {
            return source.Add(new UnscheduleActivity<TSaga>(schedule));
        }

        /// <summary>
        /// Unschedule a message, if the message was scheduled.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="schedule"></param>
        /// <returns></returns>
        public static ExceptionActivityBinder<TSaga, TData, TException> Unschedule<TSaga, TData, TException>(
            this ExceptionActivityBinder<TSaga, TData, TException> source, Schedule<TSaga> schedule)
            where TSaga : class, SagaStateMachineInstance
            where TData : class
            where TException : Exception
        {
            return source.Add(new FaultedUnscheduleActivity<TSaga>(schedule));
        }

        /// <summary>
        /// Unschedule a message, if the message was scheduled.
        /// </summary>
        /// <typeparam name="TSaga"></typeparam>
        /// <param name="source"></param>
        /// <param name="schedule"></param>
        /// <returns></returns>
        public static EventActivityBinder<TSaga> Unschedule<TSaga>(this EventActivityBinder<TSaga> source, Schedule<TSaga> schedule)
            where TSaga : class, SagaStateMachineInstance
        {
            return source.Add(new UnscheduleActivity<TSaga>(schedule));
        }

        /// <summary>
        /// Unschedule a message, if the message was scheduled.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="schedule"></param>
        /// <returns></returns>
        public static ExceptionActivityBinder<TSaga, TException> Unschedule<TSaga, TException>(this ExceptionActivityBinder<TSaga, TException> source,
            Schedule<TSaga> schedule)
            where TSaga : class, SagaStateMachineInstance
            where TException : Exception
        {
            return source.Add(new FaultedUnscheduleActivity<TSaga>(schedule));
        }
    }
}
