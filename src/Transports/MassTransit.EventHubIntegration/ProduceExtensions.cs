namespace Automatonymous
{
    using System;
    using System.Threading.Tasks;
    using Binders;
    using MassTransit.EventHubIntegration;
    using MassTransit.EventHubIntegration.Activities;


    public static class ProduceExtensions
    {
        public static EventActivityBinder<TInstance> Produce<TInstance, TMessage>(this EventActivityBinder<TInstance> source,
            EventHubNameProvider<TInstance> nameProvider, TMessage message, Action<EventHubSendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new ProduceActivity<TInstance, TMessage>(nameProvider, x => Task.FromResult(message), contextCallback));
        }

        public static EventActivityBinder<TInstance> Produce<TInstance, TMessage>(this EventActivityBinder<TInstance> source,
            EventHubNameProvider<TInstance> nameProvider, Task<TMessage> message, Action<EventHubSendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new ProduceActivity<TInstance, TMessage>(nameProvider, x => message, contextCallback));
        }

        public static EventActivityBinder<TInstance> Produce<TInstance, TMessage>(this EventActivityBinder<TInstance> source,
            EventHubNameProvider<TInstance> nameProvider, AsyncEventMessageFactory<TInstance, TMessage> messageFactory,
            Action<EventHubSendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new ProduceActivity<TInstance, TMessage>(nameProvider, messageFactory, contextCallback));
        }

        public static EventActivityBinder<TInstance, TData> Produce<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> source,
            EventHubNameProvider<TInstance, TData> nameProvider, TMessage message, Action<EventHubSendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new ProduceActivity<TInstance, TData, TMessage>(nameProvider, x => Task.FromResult(message), contextCallback));
        }

        public static EventActivityBinder<TInstance, TData> Produce<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> source,
            EventHubNameProvider<TInstance, TData> nameProvider, Task<TMessage> message, Action<EventHubSendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new ProduceActivity<TInstance, TData, TMessage>(nameProvider, x => message, contextCallback));
        }

        public static EventActivityBinder<TInstance, TData> Produce<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> source,
            EventHubNameProvider<TInstance, TData> nameProvider, AsyncEventMessageFactory<TInstance, TData, TMessage> messageFactory,
            Action<EventHubSendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new ProduceActivity<TInstance, TData, TMessage>(nameProvider, messageFactory, contextCallback));
        }

        public static ExceptionActivityBinder<TInstance, TException> Produce<TInstance, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TException> source, ExceptionEventHubNameProvider<TInstance, TException> nameProvider, TMessage message,
            Action<EventHubSendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedProduceActivity<TInstance, TException, TMessage>(nameProvider, x => Task.FromResult(message), contextCallback));
        }

        public static ExceptionActivityBinder<TInstance, TException> Produce<TInstance, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TException> source, ExceptionEventHubNameProvider<TInstance, TException> nameProvider,
            Task<TMessage> message, Action<EventHubSendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedProduceActivity<TInstance, TException, TMessage>(nameProvider, x => message, contextCallback));
        }

        public static ExceptionActivityBinder<TInstance, TException> Produce<TInstance, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TException> source, ExceptionEventHubNameProvider<TInstance, TException> nameProvider,
            AsyncEventExceptionMessageFactory<TInstance, TException, TMessage> messageFactory,
            Action<EventHubSendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedProduceActivity<TInstance, TException, TMessage>(nameProvider, messageFactory, contextCallback));
        }

        public static ExceptionActivityBinder<TInstance, TData, TException> Produce<TInstance, TData, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TData, TException> source, ExceptionEventHubNameProvider<TInstance, TData, TException>
                nameProvider, TMessage message,
            Action<EventHubSendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedProduceActivity<TInstance, TData, TException, TMessage>(nameProvider, x => Task.FromResult(message), contextCallback));
        }

        public static ExceptionActivityBinder<TInstance, TData, TException> Produce<TInstance, TData, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TData, TException> source, ExceptionEventHubNameProvider<TInstance, TData, TException> nameProvider,
            Task<TMessage> message, Action<EventHubSendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedProduceActivity<TInstance, TData, TException, TMessage>(nameProvider, x => message, contextCallback));
        }

        public static ExceptionActivityBinder<TInstance, TData, TException> Produce<TInstance, TData, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TData, TException> source, ExceptionEventHubNameProvider<TInstance, TData, TException> nameProvider,
            AsyncEventExceptionMessageFactory<TInstance, TData, TException, TMessage> messageFactory,
            Action<EventHubSendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedProduceActivity<TInstance, TData, TException, TMessage>(nameProvider, messageFactory, contextCallback));
        }
    }
}
