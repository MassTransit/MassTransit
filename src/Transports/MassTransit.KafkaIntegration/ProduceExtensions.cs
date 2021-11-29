namespace MassTransit
{
    using System;
    using System.Threading.Tasks;
    using KafkaIntegration.Activities;
    using SagaStateMachine;


    public static class ProduceExtensions
    {
        public static EventActivityBinder<TInstance> Produce<TInstance, TMessage>(this EventActivityBinder<TInstance> source,
            TMessage message, Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new ProduceActivity<TInstance, TMessage>(MessageFactory<TMessage>.Create(message, contextCallback)));
        }

        public static EventActivityBinder<TInstance> Produce<TInstance, TMessage>(this EventActivityBinder<TInstance> source,
            Task<TMessage> message, Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new ProduceActivity<TInstance, TMessage>(MessageFactory<TMessage>.Create(message, contextCallback)));
        }

        public static EventActivityBinder<TInstance> Produce<TInstance, TMessage>(this EventActivityBinder<TInstance> source,
            AsyncEventMessageFactory<TInstance, TMessage> messageFactory, Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new ProduceActivity<TInstance, TMessage>(MessageFactory<TMessage>.Create(messageFactory, contextCallback)));
        }

        public static EventActivityBinder<TInstance> Produce<TInstance, TMessage>(this EventActivityBinder<TInstance> source,
            Func<BehaviorContext<TInstance>, Task<SendTuple<TMessage>>> messageFactory, Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new ProduceActivity<TInstance, TMessage>(MessageFactory<TMessage>.Create(messageFactory, contextCallback)));
        }

        public static EventActivityBinder<TInstance, TData> Produce<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> source,
            TMessage message, Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new ProduceActivity<TInstance, TData, TMessage>(MessageFactory<TMessage>.Create(message, contextCallback)));
        }

        public static EventActivityBinder<TInstance, TData> Produce<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> source,
            Task<TMessage> message, Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new ProduceActivity<TInstance, TData, TMessage>(MessageFactory<TMessage>.Create(message, contextCallback)));
        }

        public static EventActivityBinder<TInstance, TData> Produce<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> source,
            AsyncEventMessageFactory<TInstance, TData, TMessage> messageFactory, Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new ProduceActivity<TInstance, TData, TMessage>(MessageFactory<TMessage>.Create(messageFactory, contextCallback)));
        }

        public static EventActivityBinder<TInstance, TData> Produce<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> source,
            Func<BehaviorContext<TInstance, TData>, Task<SendTuple<TMessage>>> messageFactory, Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new ProduceActivity<TInstance, TData, TMessage>(MessageFactory<TMessage>.Create(messageFactory, contextCallback)));
        }

        public static ExceptionActivityBinder<TInstance, TException> Produce<TInstance, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TException> source, TMessage message,
            Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedProduceActivity<TInstance, TException, TMessage>(MessageFactory<TMessage>.Create(message, contextCallback)));
        }

        public static ExceptionActivityBinder<TInstance, TException> Produce<TInstance, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TException> source, Task<TMessage> message,
            Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedProduceActivity<TInstance, TException, TMessage>(MessageFactory<TMessage>.Create(message, contextCallback)));
        }

        public static ExceptionActivityBinder<TInstance, TException> Produce<TInstance, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TException> source,
            AsyncEventExceptionMessageFactory<TInstance, TException, TMessage> messageFactory,
            Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedProduceActivity<TInstance, TException, TMessage>(MessageFactory<TMessage>.Create(messageFactory, contextCallback)));
        }

        public static ExceptionActivityBinder<TInstance, TException> Produce<TInstance, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TException> source,
            Func<BehaviorExceptionContext<TInstance, TException>, Task<SendTuple<TMessage>>> messageFactory,
            Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedProduceActivity<TInstance, TException, TMessage>(MessageFactory<TMessage>.Create(messageFactory, contextCallback)));
        }

        public static ExceptionActivityBinder<TInstance, TData, TException> Produce<TInstance, TData, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TData, TException> source, TMessage message,
            Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedProduceActivity<TInstance, TData, TException, TMessage>(MessageFactory<TMessage>.Create(message, contextCallback)));
        }

        public static ExceptionActivityBinder<TInstance, TData, TException> Produce<TInstance, TData, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TData, TException> source, Task<TMessage> message,
            Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedProduceActivity<TInstance, TData, TException, TMessage>(MessageFactory<TMessage>.Create(message, contextCallback)));
        }

        public static ExceptionActivityBinder<TInstance, TData, TException> Produce<TInstance, TData, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TData, TException> source,
            AsyncEventExceptionMessageFactory<TInstance, TData, TException, TMessage> messageFactory,
            Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedProduceActivity<TInstance, TData, TException, TMessage>(
                MessageFactory<TMessage>.Create(messageFactory, contextCallback)));
        }

        public static ExceptionActivityBinder<TInstance, TData, TException> Produce<TInstance, TData, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TData, TException> source,
            Func<BehaviorExceptionContext<TInstance, TData, TException>, Task<SendTuple<TMessage>>> messageFactory,
            Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedProduceActivity<TInstance, TData, TException, TMessage>(
                MessageFactory<TMessage>.Create(messageFactory, contextCallback)));
        }
    }
}
