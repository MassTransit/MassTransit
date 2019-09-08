namespace Automatonymous
{
    using System;
    using System.Threading.Tasks;
    using Activities;
    using Binders;
    using MassTransit;


    public static class SendByConventionExtensions
    {
        public static EventActivityBinder<TInstance> Send<TInstance, TMessage>(this EventActivityBinder<TInstance> source,
            TMessage message, Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new SendActivity<TInstance, TMessage>(x => message, contextCallback));
        }

        public static EventActivityBinder<TInstance> SendAsync<TInstance, TMessage>(this EventActivityBinder<TInstance> source,
            Task<TMessage> message, Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new SendActivity<TInstance, TMessage>(x => message, contextCallback));
        }

        public static EventActivityBinder<TInstance> Send<TInstance, TMessage>(this EventActivityBinder<TInstance> source,
            EventMessageFactory<TInstance, TMessage> messageFactory, Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new SendActivity<TInstance, TMessage>(messageFactory, contextCallback));
        }

        public static EventActivityBinder<TInstance> SendAsync<TInstance, TMessage>(this EventActivityBinder<TInstance> source,
            AsyncEventMessageFactory<TInstance, TMessage> messageFactory, Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new SendActivity<TInstance, TMessage>(messageFactory, contextCallback));
        }

        public static EventActivityBinder<TInstance, TData> Send<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> source,
            TMessage message, Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new SendActivity<TInstance, TData, TMessage>(x => message, contextCallback));
        }

        public static EventActivityBinder<TInstance, TData> SendAsync<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> source,
            Task<TMessage> message, Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new SendActivity<TInstance, TData, TMessage>(x => message, contextCallback));
        }

        public static EventActivityBinder<TInstance, TData> Send<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> source,
            EventMessageFactory<TInstance, TData, TMessage> messageFactory, Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new SendActivity<TInstance, TData, TMessage>(messageFactory, contextCallback));
        }

        public static EventActivityBinder<TInstance, TData> SendAsync<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> source,
            AsyncEventMessageFactory<TInstance, TData, TMessage> messageFactory, Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new SendActivity<TInstance, TData, TMessage>(messageFactory, contextCallback));
        }

        public static ExceptionActivityBinder<TInstance, TException> Send<TInstance, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TException> source, TMessage message,
            Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new SendActivity<TInstance, TMessage>(x => message, contextCallback));
        }

        public static ExceptionActivityBinder<TInstance, TException> SendAsync<TInstance, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TException> source, Task<TMessage> message,
            Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new SendActivity<TInstance, TMessage>(x => message, contextCallback));
        }

        public static ExceptionActivityBinder<TInstance, TException> Send<TInstance, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TException> source,
            EventExceptionMessageFactory<TInstance, TException, TMessage> messageFactory, Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedSendActivity<TInstance, TException, TMessage>(messageFactory, contextCallback));
        }

        public static ExceptionActivityBinder<TInstance, TException> SendAsync<TInstance, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TException> source,
            AsyncEventExceptionMessageFactory<TInstance, TException, TMessage> messageFactory, Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedSendActivity<TInstance, TException, TMessage>(messageFactory, contextCallback));
        }

        public static ExceptionActivityBinder<TInstance, TData, TException> Send<TInstance, TData, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TData, TException> source, TMessage message,
            Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new SendActivity<TInstance, TData, TMessage>(x => message, contextCallback));
        }

        public static ExceptionActivityBinder<TInstance, TData, TException> SendAsync<TInstance, TData, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TData, TException> source, Task<TMessage> message,
            Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new SendActivity<TInstance, TData, TMessage>(x => message, contextCallback));
        }

        public static ExceptionActivityBinder<TInstance, TData, TException> Send<TInstance, TData, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TData, TException> source,
            EventExceptionMessageFactory<TInstance, TData, TException, TMessage> messageFactory, Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedSendActivity<TInstance, TData, TException, TMessage>(messageFactory, contextCallback));
        }

        public static ExceptionActivityBinder<TInstance, TData, TException> SendAsync<TInstance, TData, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TData, TException> source,
            AsyncEventExceptionMessageFactory<TInstance, TData, TException, TMessage> messageFactory, Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedSendActivity<TInstance, TData, TException, TMessage>(messageFactory, contextCallback));
        }
    }
}
