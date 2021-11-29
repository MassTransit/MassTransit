namespace MassTransit
{
    using System;
    using System.Threading.Tasks;
    using SagaStateMachine;


    public static class RespondExtensions
    {
        public static EventActivityBinder<TInstance, TData> Respond<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> source,
            TMessage message, Action<SendContext<TMessage>> callback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new RespondActivity<TInstance, TData, TMessage>(MessageFactory<TMessage>.Create(message, callback)));
        }

        public static EventActivityBinder<TInstance, TData> RespondAsync<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> source,
            Task<TMessage> message, Action<SendContext<TMessage>> callback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new RespondActivity<TInstance, TData, TMessage>(MessageFactory<TMessage>.Create(message, callback)));
        }

        public static EventActivityBinder<TInstance, TData> Respond<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> source,
            EventMessageFactory<TInstance, TData, TMessage> messageFactory,
            Action<SendContext<TMessage>> callback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new RespondActivity<TInstance, TData, TMessage>(MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static EventActivityBinder<TInstance, TData> RespondAsync<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> source,
            AsyncEventMessageFactory<TInstance, TData, TMessage> messageFactory,
            Action<SendContext<TMessage>> callback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new RespondActivity<TInstance, TData, TMessage>(MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static EventActivityBinder<TInstance, TData> RespondAsync<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> source,
            Func<BehaviorContext<TInstance, TData>, Task<SendTuple<TMessage>>> messageFactory,
            Action<SendContext<TMessage>> callback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new RespondActivity<TInstance, TData, TMessage>(MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static ExceptionActivityBinder<TInstance, TException> Respond<TInstance, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TException> source, TMessage message,
            Action<SendContext<TMessage>> callback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedRespondActivity<TInstance, TException, TMessage>(MessageFactory<TMessage>.Create(message, callback)));
        }

        public static ExceptionActivityBinder<TInstance, TException> RespondAsync<TInstance, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TException> source, Task<TMessage> message,
            Action<SendContext<TMessage>> callback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedRespondActivity<TInstance, TException, TMessage>(MessageFactory<TMessage>.Create(message, callback)));
        }

        public static ExceptionActivityBinder<TInstance, TException> Respond<TInstance, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TException> source,
            EventExceptionMessageFactory<TInstance, TException, TMessage> messageFactory,
            Action<SendContext<TMessage>> callback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedRespondActivity<TInstance, TException, TMessage>(MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static ExceptionActivityBinder<TInstance, TException> RespondAsync<TInstance, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TException> source,
            AsyncEventExceptionMessageFactory<TInstance, TException, TMessage> messageFactory,
            Action<SendContext<TMessage>> callback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedRespondActivity<TInstance, TException, TMessage>(MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static ExceptionActivityBinder<TInstance, TData, TException> Respond<TInstance, TData, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TData, TException> source, TMessage message,
            Action<SendContext<TMessage>> callback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedRespondActivity<TInstance, TData, TException, TMessage>(MessageFactory<TMessage>.Create(message, callback)));
        }

        public static ExceptionActivityBinder<TInstance, TData, TException> RespondAsync<TInstance, TData, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TData, TException> source, Task<TMessage> message,
            Action<SendContext<TMessage>> callback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedRespondActivity<TInstance, TData, TException, TMessage>(MessageFactory<TMessage>.Create(message, callback)));
        }

        public static ExceptionActivityBinder<TInstance, TData, TException> Respond<TInstance, TData, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TData, TException> source,
            EventExceptionMessageFactory<TInstance, TData, TException, TMessage> messageFactory,
            Action<SendContext<TMessage>> callback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedRespondActivity<TInstance, TData, TException, TMessage>(
                MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static ExceptionActivityBinder<TInstance, TData, TException> RespondAsync<TInstance, TData, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TData, TException> source,
            AsyncEventExceptionMessageFactory<TInstance, TData, TException, TMessage> messageFactory,
            Action<SendContext<TMessage>> callback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedRespondActivity<TInstance, TData, TException, TMessage>(
                MessageFactory<TMessage>.Create(messageFactory, callback)));
        }
    }
}
