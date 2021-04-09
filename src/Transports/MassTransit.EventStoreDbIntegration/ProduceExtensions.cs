using System;
using System.Threading.Tasks;
using Automatonymous.Binders;
using MassTransit.EventStoreDbIntegration;
using MassTransit.EventStoreDbIntegration.Activities;

namespace Automatonymous
{
    public static class ProduceExtensions
    {
        public static EventActivityBinder<TInstance> Produce<TInstance, TMessage>(this EventActivityBinder<TInstance> source,
            EventStoreDbStreamNameProvider<TInstance> streamNameProvider, TMessage message, Action<EventStoreDbSendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new ProduceActivity<TInstance, TMessage>(streamNameProvider, x => Task.FromResult(message), contextCallback));
        }

        public static EventActivityBinder<TInstance> Produce<TInstance, TMessage>(this EventActivityBinder<TInstance> source,
            EventStoreDbStreamNameProvider<TInstance> streamNameProvider, Task<TMessage> message, Action<EventStoreDbSendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new ProduceActivity<TInstance, TMessage>(streamNameProvider, x => message, contextCallback));
        }

        public static EventActivityBinder<TInstance> Produce<TInstance, TMessage>(this EventActivityBinder<TInstance> source,
            EventStoreDbStreamNameProvider<TInstance> streamNameProvider, AsyncEventMessageFactory<TInstance, TMessage> messageFactory,
            Action<EventStoreDbSendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new ProduceActivity<TInstance, TMessage>(streamNameProvider, messageFactory, contextCallback));
        }

        public static EventActivityBinder<TInstance, TData> Produce<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> source,
            EventStoreDbStreamNameProvider<TInstance, TData> streamNameProvider, TMessage message, Action<EventStoreDbSendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new ProduceActivity<TInstance, TData, TMessage>(streamNameProvider, x => Task.FromResult(message), contextCallback));
        }

        public static EventActivityBinder<TInstance, TData> Produce<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> source,
            EventStoreDbStreamNameProvider<TInstance, TData> streamNameProvider, Task<TMessage> message, Action<EventStoreDbSendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new ProduceActivity<TInstance, TData, TMessage>(streamNameProvider, x => message, contextCallback));
        }

        public static EventActivityBinder<TInstance, TData> Produce<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> source,
            EventStoreDbStreamNameProvider<TInstance, TData> streamNameProvider, AsyncEventMessageFactory<TInstance, TData, TMessage> messageFactory,
            Action<EventStoreDbSendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new ProduceActivity<TInstance, TData, TMessage>(streamNameProvider, messageFactory, contextCallback));
        }

        public static ExceptionActivityBinder<TInstance, TException> Produce<TInstance, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TException> source, ExceptionEventStoreDbStreamNameProvider<TInstance, TException> streamNameProvider, TMessage message,
            Action<EventStoreDbSendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedProduceActivity<TInstance, TException, TMessage>(streamNameProvider, x => Task.FromResult(message), contextCallback));
        }

        public static ExceptionActivityBinder<TInstance, TException> Produce<TInstance, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TException> source, ExceptionEventStoreDbStreamNameProvider<TInstance, TException> streamNameProvider,
            Task<TMessage> message, Action<EventStoreDbSendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedProduceActivity<TInstance, TException, TMessage>(streamNameProvider, x => message, contextCallback));
        }

        public static ExceptionActivityBinder<TInstance, TException> Produce<TInstance, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TException> source, ExceptionEventStoreDbStreamNameProvider<TInstance, TException> streamNameProvider,
            AsyncEventExceptionMessageFactory<TInstance, TException, TMessage> messageFactory,
            Action<EventStoreDbSendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedProduceActivity<TInstance, TException, TMessage>(streamNameProvider, messageFactory, contextCallback));
        }

        public static ExceptionActivityBinder<TInstance, TData, TException> Produce<TInstance, TData, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TData, TException> source, ExceptionEventStoreDbStreamNameProvider<TInstance, TData, TException>
                streamNameProvider, TMessage message,
            Action<EventStoreDbSendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedProduceActivity<TInstance, TData, TException, TMessage>(streamNameProvider, x => Task.FromResult(message), contextCallback));
        }

        public static ExceptionActivityBinder<TInstance, TData, TException> Produce<TInstance, TData, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TData, TException> source, ExceptionEventStoreDbStreamNameProvider<TInstance, TData, TException> streamNameProvider,
            Task<TMessage> message, Action<EventStoreDbSendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedProduceActivity<TInstance, TData, TException, TMessage>(streamNameProvider, x => message, contextCallback));
        }

        public static ExceptionActivityBinder<TInstance, TData, TException> Produce<TInstance, TData, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TData, TException> source, ExceptionEventStoreDbStreamNameProvider<TInstance, TData, TException> streamNameProvider,
            AsyncEventExceptionMessageFactory<TInstance, TData, TException, TMessage> messageFactory,
            Action<EventStoreDbSendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedProduceActivity<TInstance, TData, TException, TMessage>(streamNameProvider, messageFactory, contextCallback));
        }
    }
}
