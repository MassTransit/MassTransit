namespace Automatonymous
{
    using System;
    using System.Threading.Tasks;
    using Activities;
    using Binders;
    using MassTransit;


    public static class PublishExtensions
    {
        public static EventActivityBinder<TInstance> Publish<TInstance, TMessage>(this EventActivityBinder<TInstance> source,
            TMessage message, Action<PublishContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new PublishActivity<TInstance, TMessage>(x => message, contextCallback));
        }

        public static EventActivityBinder<TInstance> PublishAsync<TInstance, TMessage>(this EventActivityBinder<TInstance> source,
            Task<TMessage> message, Action<PublishContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new PublishActivity<TInstance, TMessage>(x => message, contextCallback));
        }

        public static EventActivityBinder<TInstance> Publish<TInstance, TMessage>(this EventActivityBinder<TInstance> source,
            EventMessageFactory<TInstance, TMessage> messageFactory, Action<PublishContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new PublishActivity<TInstance, TMessage>(messageFactory, contextCallback));
        }

        public static EventActivityBinder<TInstance> PublishAsync<TInstance, TMessage>(this EventActivityBinder<TInstance> source,
            AsyncEventMessageFactory<TInstance, TMessage> messageFactory, Action<PublishContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new PublishActivity<TInstance, TMessage>(messageFactory, contextCallback));
        }

        public static EventActivityBinder<TInstance, TData> Publish<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> source,
            TMessage message, Action<PublishContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new PublishActivity<TInstance, TData, TMessage>(x => message, contextCallback));
        }

        public static EventActivityBinder<TInstance, TData> PublishAsync<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> source,
            Task<TMessage> message, Action<PublishContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new PublishActivity<TInstance, TData, TMessage>(x => message, contextCallback));
        }

        public static EventActivityBinder<TInstance, TData> Publish<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> source,
            EventMessageFactory<TInstance, TData, TMessage> messageFactory, Action<PublishContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new PublishActivity<TInstance, TData, TMessage>(messageFactory, contextCallback));
        }

        public static EventActivityBinder<TInstance, TData> PublishAsync<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> source,
            AsyncEventMessageFactory<TInstance, TData, TMessage> messageFactory, Action<PublishContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new PublishActivity<TInstance, TData, TMessage>(messageFactory, contextCallback));
        }

        public static ExceptionActivityBinder<TInstance, TException> Publish<TInstance, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TException> source, TMessage message,
            Action<PublishContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new PublishActivity<TInstance, TMessage>(x => message, contextCallback));
        }

        public static ExceptionActivityBinder<TInstance, TException> PublishAsync<TInstance, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TException> source, Task<TMessage> message,
            Action<PublishContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new PublishActivity<TInstance, TMessage>(x => message, contextCallback));
        }

        public static ExceptionActivityBinder<TInstance, TException> Publish<TInstance, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TException> source,
            EventExceptionMessageFactory<TInstance, TException, TMessage> messageFactory,
            Action<PublishContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedPublishActivity<TInstance, TException, TMessage>(messageFactory, contextCallback));
        }

        public static ExceptionActivityBinder<TInstance, TException> PublishAsync<TInstance, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TException> source,
            AsyncEventExceptionMessageFactory<TInstance, TException, TMessage> messageFactory,
            Action<PublishContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedPublishActivity<TInstance, TException, TMessage>(messageFactory, contextCallback));
        }

        public static ExceptionActivityBinder<TInstance, TData, TException> Publish<TInstance, TData, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TData, TException> source, TMessage message,
            Action<PublishContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new PublishActivity<TInstance, TData, TMessage>(x => message, contextCallback));
        }

        public static ExceptionActivityBinder<TInstance, TData, TException> PublishAsync<TInstance, TData, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TData, TException> source, Task<TMessage> message,
            Action<PublishContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new PublishActivity<TInstance, TData, TMessage>(x => message, contextCallback));
        }

        public static ExceptionActivityBinder<TInstance, TData, TException> Publish<TInstance, TData, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TData, TException> source,
            EventExceptionMessageFactory<TInstance, TData, TException, TMessage> messageFactory,
            Action<PublishContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedPublishActivity<TInstance, TData, TException, TMessage>(messageFactory, contextCallback));
        }

        public static ExceptionActivityBinder<TInstance, TData, TException> PublishAsync<TInstance, TData, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TData, TException> source,
            AsyncEventExceptionMessageFactory<TInstance, TData, TException, TMessage> messageFactory,
            Action<PublishContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedPublishActivity<TInstance, TData, TException, TMessage>(messageFactory, contextCallback));
        }
    }
}
