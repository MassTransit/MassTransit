namespace MassTransit
{
    using System;
    using System.Threading.Tasks;
    using SagaStateMachine;


    public static class ScheduleDateTimeExtensions
    {
        public static EventActivityBinder<TInstance> Schedule<TInstance, TMessage>(this EventActivityBinder<TInstance> source,
            Schedule<TInstance, TMessage> schedule, TMessage message, ScheduleTimeProvider<TInstance> timeProvider,
            Action<SendContext<TMessage>> callback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new ScheduleActivity<TInstance, TMessage>(schedule, timeProvider, MessageFactory<TMessage>.Create(message, callback)));
        }

        public static EventActivityBinder<TInstance> Schedule<TInstance, TMessage>(this EventActivityBinder<TInstance> source,
            Schedule<TInstance, TMessage> schedule, Task<TMessage> message, ScheduleTimeProvider<TInstance> timeProvider,
            Action<SendContext<TMessage>> callback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new ScheduleActivity<TInstance, TMessage>(schedule, timeProvider, MessageFactory<TMessage>.Create(message, callback)));
        }

        public static EventActivityBinder<TInstance> Schedule<TInstance, TMessage>(this EventActivityBinder<TInstance> source,
            Schedule<TInstance, TMessage> schedule, EventMessageFactory<TInstance, TMessage> messageFactory, ScheduleTimeProvider<TInstance> timeProvider,
            Action<SendContext<TMessage>> callback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new ScheduleActivity<TInstance, TMessage>(schedule, timeProvider, MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static EventActivityBinder<TInstance> Schedule<TInstance, TMessage>(this EventActivityBinder<TInstance> source,
            Schedule<TInstance, TMessage> schedule, AsyncEventMessageFactory<TInstance, TMessage> messageFactory, ScheduleTimeProvider<TInstance> timeProvider,
            Action<SendContext<TMessage>> callback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new ScheduleActivity<TInstance, TMessage>(schedule, timeProvider, MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static EventActivityBinder<TInstance> Schedule<TInstance, TMessage>(this EventActivityBinder<TInstance> source,
            Schedule<TInstance, TMessage> schedule, Func<BehaviorContext<TInstance>, Task<SendTuple<TMessage>>> messageFactory,
            ScheduleTimeProvider<TInstance> timeProvider, Action<SendContext<TMessage>> callback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new ScheduleActivity<TInstance, TMessage>(schedule, timeProvider, MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static EventActivityBinder<TInstance, TData> Schedule<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> source,
            Schedule<TInstance, TMessage> schedule, TMessage message, ScheduleTimeProvider<TInstance, TData> timeProvider,
            Action<SendContext<TMessage>> callback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new ScheduleActivity<TInstance, TData, TMessage>(schedule, timeProvider, MessageFactory<TMessage>.Create(message, callback)));
        }

        public static EventActivityBinder<TInstance, TData> Schedule<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> source,
            Schedule<TInstance, TMessage> schedule, Task<TMessage> message, ScheduleTimeProvider<TInstance, TData> timeProvider,
            Action<SendContext<TMessage>> callback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new ScheduleActivity<TInstance, TData, TMessage>(schedule, timeProvider, MessageFactory<TMessage>.Create(message, callback)));
        }

        public static EventActivityBinder<TInstance, TData> Schedule<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> source,
            Schedule<TInstance, TMessage> schedule,
            EventMessageFactory<TInstance, TData, TMessage> messageFactory,
            ScheduleTimeProvider<TInstance, TData> timeProvider, Action<SendContext<TMessage>> callback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(
                new ScheduleActivity<TInstance, TData, TMessage>(schedule, timeProvider, MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static EventActivityBinder<TInstance, TData> Schedule<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> source,
            Schedule<TInstance, TMessage> schedule,
            AsyncEventMessageFactory<TInstance, TData, TMessage> messageFactory,
            ScheduleTimeProvider<TInstance, TData> timeProvider, Action<SendContext<TMessage>> callback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(
                new ScheduleActivity<TInstance, TData, TMessage>(schedule, timeProvider, MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static EventActivityBinder<TInstance, TData> Schedule<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> source,
            Schedule<TInstance, TMessage> schedule,
            Func<BehaviorContext<TInstance, TData>, Task<SendTuple<TMessage>>> messageFactory,
            ScheduleTimeProvider<TInstance, TData> timeProvider, Action<SendContext<TMessage>> callback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(
                new ScheduleActivity<TInstance, TData, TMessage>(schedule, timeProvider, MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static ExceptionActivityBinder<TInstance, TException> Schedule<TInstance, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TException> source, Schedule<TInstance, TMessage> schedule, TMessage message,
            ScheduleTimeExceptionProvider<TInstance, TException> timeProvider, Action<SendContext<TMessage>> callback = null)
            where TInstance : class, SagaStateMachineInstance
            where TException : Exception
            where TMessage : class
        {
            return source.Add(new FaultedScheduleActivity<TInstance, TException, TMessage>(schedule, timeProvider,
                MessageFactory<TMessage>.Create(message, callback)));
        }

        public static ExceptionActivityBinder<TInstance, TException> Schedule<TInstance, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TException> source, Schedule<TInstance, TMessage> schedule, Task<TMessage> message,
            ScheduleTimeExceptionProvider<TInstance, TException> timeProvider, Action<SendContext<TMessage>> callback = null)
            where TInstance : class, SagaStateMachineInstance
            where TException : Exception
            where TMessage : class
        {
            return source.Add(new FaultedScheduleActivity<TInstance, TException, TMessage>(schedule, timeProvider,
                MessageFactory<TMessage>.Create(message, callback)));
        }

        public static ExceptionActivityBinder<TInstance, TException> Schedule<TInstance, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TException> source, Schedule<TInstance, TMessage> schedule,
            EventExceptionMessageFactory<TInstance, TException, TMessage> messageFactory,
            ScheduleTimeExceptionProvider<TInstance, TException> timeProvider, Action<SendContext<TMessage>> callback = null)
            where TInstance : class, SagaStateMachineInstance
            where TException : Exception
            where TMessage : class
        {
            return source.Add(new FaultedScheduleActivity<TInstance, TException, TMessage>(schedule, timeProvider,
                MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static ExceptionActivityBinder<TInstance, TException> Schedule<TInstance, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TException> source, Schedule<TInstance, TMessage> schedule,
            AsyncEventExceptionMessageFactory<TInstance, TException, TMessage> messageFactory,
            ScheduleTimeExceptionProvider<TInstance, TException> timeProvider, Action<SendContext<TMessage>> callback = null)
            where TInstance : class, SagaStateMachineInstance
            where TException : Exception
            where TMessage : class
        {
            return source.Add(new FaultedScheduleActivity<TInstance, TException, TMessage>(schedule, timeProvider,
                MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static ExceptionActivityBinder<TInstance, TException> Schedule<TInstance, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TException> source, Schedule<TInstance, TMessage> schedule,
            Func<BehaviorExceptionContext<TInstance, TException>, Task<SendTuple<TMessage>>> messageFactory,
            ScheduleTimeExceptionProvider<TInstance, TException> timeProvider, Action<SendContext<TMessage>> callback = null)
            where TInstance : class, SagaStateMachineInstance
            where TException : Exception
            where TMessage : class
        {
            return source.Add(new FaultedScheduleActivity<TInstance, TException, TMessage>(schedule, timeProvider,
                MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static ExceptionActivityBinder<TInstance, TData, TException> Schedule<TInstance, TData, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TData, TException> source, Schedule<TInstance, TMessage> schedule, TMessage message,
            ScheduleTimeExceptionProvider<TInstance, TData, TException> timeProvider, Action<SendContext<TMessage>> callback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TException : Exception
            where TMessage : class
        {
            return source.Add(new FaultedScheduleActivity<TInstance, TData, TException, TMessage>(schedule, timeProvider,
                MessageFactory<TMessage>.Create(message, callback)));
        }

        public static ExceptionActivityBinder<TInstance, TData, TException> Schedule<TInstance, TData, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TData, TException> source, Schedule<TInstance, TMessage> schedule, Task<TMessage> message,
            ScheduleTimeExceptionProvider<TInstance, TData, TException> timeProvider, Action<SendContext<TMessage>> callback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TException : Exception
            where TMessage : class
        {
            return source.Add(new FaultedScheduleActivity<TInstance, TData, TException, TMessage>(schedule, timeProvider,
                MessageFactory<TMessage>.Create(message, callback)));
        }

        public static ExceptionActivityBinder<TInstance, TData, TException> Schedule<TInstance, TData, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TData, TException> source, Schedule<TInstance, TMessage> schedule,
            EventExceptionMessageFactory<TInstance, TData, TException, TMessage> messageFactory,
            ScheduleTimeExceptionProvider<TInstance, TData, TException> timeProvider, Action<SendContext<TMessage>> callback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TException : Exception
            where TMessage : class
        {
            return source.Add(new FaultedScheduleActivity<TInstance, TData, TException, TMessage>(schedule, timeProvider,
                MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static ExceptionActivityBinder<TInstance, TData, TException> Schedule<TInstance, TData, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TData, TException> source, Schedule<TInstance, TMessage> schedule,
            AsyncEventExceptionMessageFactory<TInstance, TData, TException, TMessage> messageFactory,
            ScheduleTimeExceptionProvider<TInstance, TData, TException> timeProvider, Action<SendContext<TMessage>> callback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TException : Exception
            where TMessage : class
        {
            return source.Add(new FaultedScheduleActivity<TInstance, TData, TException, TMessage>(schedule, timeProvider,
                MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static ExceptionActivityBinder<TInstance, TData, TException> Schedule<TInstance, TData, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TData, TException> source, Schedule<TInstance, TMessage> schedule,
            Func<BehaviorExceptionContext<TInstance, TData, TException>, Task<SendTuple<TMessage>>> messageFactory,
            ScheduleTimeExceptionProvider<TInstance, TData, TException> timeProvider, Action<SendContext<TMessage>> callback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TException : Exception
            where TMessage : class
        {
            return source.Add(new FaultedScheduleActivity<TInstance, TData, TException, TMessage>(schedule, timeProvider,
                MessageFactory<TMessage>.Create(messageFactory, callback)));
        }
    }
}
