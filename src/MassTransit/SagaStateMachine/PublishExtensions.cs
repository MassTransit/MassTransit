namespace MassTransit
{
    using System;
    using System.Threading.Tasks;
    using SagaStateMachine;


    public static class PublishExtensions
    {
        public static EventActivityBinder<TInstance> Publish<TInstance, TMessage>(this EventActivityBinder<TInstance> source,
            TMessage message, Action<PublishContext<TMessage>> callback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new PublishActivity<TInstance, TMessage>(MessageFactory<TMessage>.Create(message, Uplift(callback))));
        }

        public static EventActivityBinder<TInstance> PublishAsync<TInstance, TMessage>(this EventActivityBinder<TInstance> source,
            Task<TMessage> message, Action<PublishContext<TMessage>> callback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new PublishActivity<TInstance, TMessage>(MessageFactory<TMessage>.Create(message, Uplift(callback))));
        }

        public static EventActivityBinder<TInstance> Publish<TInstance, TMessage>(this EventActivityBinder<TInstance> source,
            EventMessageFactory<TInstance, TMessage> messageFactory, Action<PublishContext<TMessage>> callback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new PublishActivity<TInstance, TMessage>(MessageFactory<TMessage>.Create(messageFactory, Uplift(callback))));
        }

        public static EventActivityBinder<TInstance> PublishAsync<TInstance, TMessage>(this EventActivityBinder<TInstance> source,
            AsyncEventMessageFactory<TInstance, TMessage> messageFactory, Action<PublishContext<TMessage>> callback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new PublishActivity<TInstance, TMessage>(MessageFactory<TMessage>.Create(messageFactory, Uplift(callback))));
        }

        public static EventActivityBinder<TInstance> PublishAsync<TInstance, TMessage>(this EventActivityBinder<TInstance> source,
            Func<BehaviorContext<TInstance>, Task<SendTuple<TMessage>>> messageFactory, Action<PublishContext<TMessage>> callback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new PublishActivity<TInstance, TMessage>(MessageFactory<TMessage>.Create(messageFactory, Uplift(callback))));
        }

        public static EventActivityBinder<TInstance, TData> Publish<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> source,
            TMessage message, Action<PublishContext<TMessage>> callback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new PublishActivity<TInstance, TData, TMessage>(MessageFactory<TMessage>.Create(message, Uplift(callback))));
        }

        public static EventActivityBinder<TInstance, TData> PublishAsync<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> source,
            Task<TMessage> message, Action<PublishContext<TMessage>> callback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new PublishActivity<TInstance, TData, TMessage>(MessageFactory<TMessage>.Create(message, Uplift(callback))));
        }

        public static EventActivityBinder<TInstance, TData> Publish<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> source,
            EventMessageFactory<TInstance, TData, TMessage> messageFactory, Action<PublishContext<TMessage>> callback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new PublishActivity<TInstance, TData, TMessage>(MessageFactory<TMessage>.Create(messageFactory, Uplift(callback))));
        }

        public static EventActivityBinder<TInstance, TData> PublishAsync<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> source,
            AsyncEventMessageFactory<TInstance, TData, TMessage> messageFactory, Action<PublishContext<TMessage>> callback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new PublishActivity<TInstance, TData, TMessage>(MessageFactory<TMessage>.Create(messageFactory, Uplift(callback))));
        }

        public static EventActivityBinder<TInstance, TData> PublishAsync<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> source,
            Func<BehaviorContext<TInstance, TData>, Task<SendTuple<TMessage>>> messageFactory,
            Action<PublishContext<TMessage>> callback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new PublishActivity<TInstance, TData, TMessage>(MessageFactory<TMessage>.Create(messageFactory, Uplift(callback))));
        }

        public static ExceptionActivityBinder<TInstance, TException> Publish<TInstance, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TException> source, TMessage message,
            Action<PublishContext<TMessage>> callback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedPublishActivity<TInstance, TException, TMessage>(MessageFactory<TMessage>.Create(message, Uplift(callback))));
        }

        public static ExceptionActivityBinder<TInstance, TException> PublishAsync<TInstance, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TException> source, Task<TMessage> message,
            Action<PublishContext<TMessage>> callback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedPublishActivity<TInstance, TException, TMessage>(MessageFactory<TMessage>.Create(message, Uplift(callback))));
        }

        public static ExceptionActivityBinder<TInstance, TException> Publish<TInstance, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TException> source,
            EventExceptionMessageFactory<TInstance, TException, TMessage> messageFactory,
            Action<PublishContext<TMessage>> callback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedPublishActivity<TInstance, TException, TMessage>(MessageFactory<TMessage>.Create(messageFactory, Uplift(callback))));
        }

        public static ExceptionActivityBinder<TInstance, TException> PublishAsync<TInstance, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TException> source,
            AsyncEventExceptionMessageFactory<TInstance, TException, TMessage> messageFactory,
            Action<PublishContext<TMessage>> callback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedPublishActivity<TInstance, TException, TMessage>(MessageFactory<TMessage>.Create(messageFactory, Uplift(callback))));
        }

        public static ExceptionActivityBinder<TInstance, TException> PublishAsync<TInstance, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TException> source,
            Func<BehaviorExceptionContext<TInstance, TException>, Task<SendTuple<TMessage>>> messageFactory,
            Action<PublishContext<TMessage>> callback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedPublishActivity<TInstance, TException, TMessage>(MessageFactory<TMessage>.Create(messageFactory, Uplift(callback))));
        }

        public static ExceptionActivityBinder<TInstance, TData, TException> Publish<TInstance, TData, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TData, TException> source, TMessage message,
            Action<PublishContext<TMessage>> callback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedPublishActivity<TInstance, TData, TException, TMessage>(MessageFactory<TMessage>.Create(message, Uplift(callback))));
        }

        public static ExceptionActivityBinder<TInstance, TData, TException> PublishAsync<TInstance, TData, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TData, TException> source, Task<TMessage> message,
            Action<PublishContext<TMessage>> callback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedPublishActivity<TInstance, TData, TException, TMessage>(MessageFactory<TMessage>.Create(message, Uplift(callback))));
        }

        public static ExceptionActivityBinder<TInstance, TData, TException> Publish<TInstance, TData, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TData, TException> source,
            EventExceptionMessageFactory<TInstance, TData, TException, TMessage> messageFactory,
            Action<PublishContext<TMessage>> callback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedPublishActivity<TInstance, TData, TException, TMessage>(
                MessageFactory<TMessage>.Create(messageFactory, Uplift(callback))));
        }

        public static ExceptionActivityBinder<TInstance, TData, TException> PublishAsync<TInstance, TData, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TData, TException> source,
            AsyncEventExceptionMessageFactory<TInstance, TData, TException, TMessage> messageFactory,
            Action<PublishContext<TMessage>> callback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedPublishActivity<TInstance, TData, TException, TMessage>(
                MessageFactory<TMessage>.Create(messageFactory, Uplift(callback))));
        }

        public static ExceptionActivityBinder<TInstance, TData, TException> PublishAsync<TInstance, TData, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TData, TException> source,
            Func<BehaviorExceptionContext<TInstance, TData, TException>, Task<SendTuple<TMessage>>> messageFactory,
            Action<PublishContext<TMessage>> callback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedPublishActivity<TInstance, TData, TException, TMessage>(
                MessageFactory<TMessage>.Create(messageFactory, Uplift(callback))));
        }

        static Action<SendContext<T>> Uplift<T>(Action<PublishContext<T>> callback)
            where T : class
        {
            if (callback == null)
                return null;

            return context =>
            {
                var payload = context.GetPayload<PublishContext<T>>();

                callback(payload);
            };
        }
    }
}
