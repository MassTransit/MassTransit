namespace MassTransit
{
    using System;
    using System.Threading.Tasks;
    using SagaStateMachine;


    public static class SendByConventionExtensions
    {
        public static EventActivityBinder<TInstance> Send<TInstance, TMessage>(this EventActivityBinder<TInstance> source,
            TMessage message, Action<SendContext<TMessage>> callback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new SendActivity<TInstance, TMessage>(_ => GetDestinationAddress<TMessage>(),
                MessageFactory<TMessage>.Create(message, callback)));
        }

        public static EventActivityBinder<TInstance> SendAsync<TInstance, TMessage>(this EventActivityBinder<TInstance> source,
            Task<TMessage> message, Action<SendContext<TMessage>> callback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new SendActivity<TInstance, TMessage>(_ => GetDestinationAddress<TMessage>(),
                MessageFactory<TMessage>.Create(message, callback)));
        }

        public static EventActivityBinder<TInstance> Send<TInstance, TMessage>(this EventActivityBinder<TInstance> source,
            EventMessageFactory<TInstance, TMessage> messageFactory, Action<SendContext<TMessage>> callback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new SendActivity<TInstance, TMessage>(_ => GetDestinationAddress<TMessage>(),
                MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static EventActivityBinder<TInstance> SendAsync<TInstance, TMessage>(this EventActivityBinder<TInstance> source,
            AsyncEventMessageFactory<TInstance, TMessage> messageFactory, Action<SendContext<TMessage>> callback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new SendActivity<TInstance, TMessage>(_ => GetDestinationAddress<TMessage>(),
                MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static EventActivityBinder<TInstance> SendAsync<TInstance, TMessage>(this EventActivityBinder<TInstance> source,
            Func<BehaviorContext<TInstance>, Task<SendTuple<TMessage>>> messageFactory, Action<SendContext<TMessage>> callback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new SendActivity<TInstance, TMessage>(_ => GetDestinationAddress<TMessage>(),
                MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static EventActivityBinder<TInstance, TData> Send<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> source,
            TMessage message, Action<SendContext<TMessage>> callback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new SendActivity<TInstance, TData, TMessage>(_ => GetDestinationAddress<TMessage>(),
                MessageFactory<TMessage>.Create(message, callback)));
        }

        public static EventActivityBinder<TInstance, TData> SendAsync<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> source,
            Task<TMessage> message, Action<SendContext<TMessage>> callback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new SendActivity<TInstance, TData, TMessage>(_ => GetDestinationAddress<TMessage>(),
                MessageFactory<TMessage>.Create(message, callback)));
        }

        public static EventActivityBinder<TInstance, TData> Send<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> source,
            EventMessageFactory<TInstance, TData, TMessage> messageFactory, Action<SendContext<TMessage>> callback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new SendActivity<TInstance, TData, TMessage>(_ => GetDestinationAddress<TMessage>(),
                MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static EventActivityBinder<TInstance, TData> SendAsync<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> source,
            AsyncEventMessageFactory<TInstance, TData, TMessage> messageFactory, Action<SendContext<TMessage>> callback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new SendActivity<TInstance, TData, TMessage>(_ => GetDestinationAddress<TMessage>(),
                MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static EventActivityBinder<TInstance, TData> SendAsync<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> source,
            Func<BehaviorContext<TInstance, TData>, Task<SendTuple<TMessage>>> messageFactory, Action<SendContext<TMessage>> callback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new SendActivity<TInstance, TData, TMessage>(_ => GetDestinationAddress<TMessage>(),
                MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static ExceptionActivityBinder<TInstance, TException> Send<TInstance, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TException> source, TMessage message,
            Action<SendContext<TMessage>> callback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedSendActivity<TInstance, TException, TMessage>(_ => GetDestinationAddress<TMessage>(),
                MessageFactory<TMessage>.Create(message, callback)));
        }

        public static ExceptionActivityBinder<TInstance, TException> SendAsync<TInstance, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TException> source, Task<TMessage> message,
            Action<SendContext<TMessage>> callback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedSendActivity<TInstance, TException, TMessage>(_ => GetDestinationAddress<TMessage>(),
                MessageFactory<TMessage>.Create(message, callback)));
        }

        public static ExceptionActivityBinder<TInstance, TException> Send<TInstance, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TException> source,
            EventExceptionMessageFactory<TInstance, TException, TMessage> messageFactory, Action<SendContext<TMessage>> callback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedSendActivity<TInstance, TException, TMessage>(_ => GetDestinationAddress<TMessage>(),
                MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static ExceptionActivityBinder<TInstance, TException> SendAsync<TInstance, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TException> source,
            AsyncEventExceptionMessageFactory<TInstance, TException, TMessage> messageFactory, Action<SendContext<TMessage>> callback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedSendActivity<TInstance, TException, TMessage>(_ => GetDestinationAddress<TMessage>(),
                MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static ExceptionActivityBinder<TInstance, TException> SendAsync<TInstance, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TException> source,
            Func<BehaviorExceptionContext<TInstance, TException>, Task<SendTuple<TMessage>>> messageFactory,
            Action<SendContext<TMessage>> callback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedSendActivity<TInstance, TException, TMessage>(_ => GetDestinationAddress<TMessage>(),
                MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static ExceptionActivityBinder<TInstance, TData, TException> Send<TInstance, TData, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TData, TException> source, TMessage message,
            Action<SendContext<TMessage>> callback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedSendActivity<TInstance, TData, TException, TMessage>(_ => GetDestinationAddress<TMessage>(),
                MessageFactory<TMessage>.Create(message, callback)));
        }

        public static ExceptionActivityBinder<TInstance, TData, TException> SendAsync<TInstance, TData, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TData, TException> source, Task<TMessage> message,
            Action<SendContext<TMessage>> callback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedSendActivity<TInstance, TData, TException, TMessage>(_ => GetDestinationAddress<TMessage>(),
                MessageFactory<TMessage>.Create(message, callback)));
        }

        public static ExceptionActivityBinder<TInstance, TData, TException> Send<TInstance, TData, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TData, TException> source,
            EventExceptionMessageFactory<TInstance, TData, TException, TMessage> messageFactory, Action<SendContext<TMessage>> callback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedSendActivity<TInstance, TData, TException, TMessage>(_ => GetDestinationAddress<TMessage>(),
                MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static ExceptionActivityBinder<TInstance, TData, TException> SendAsync<TInstance, TData, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TData, TException> source,
            AsyncEventExceptionMessageFactory<TInstance, TData, TException, TMessage> messageFactory, Action<SendContext<TMessage>> callback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedSendActivity<TInstance, TData, TException, TMessage>(_ => GetDestinationAddress<TMessage>(),
                MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static ExceptionActivityBinder<TInstance, TData, TException> SendAsync<TInstance, TData, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TData, TException> source,
            Func<BehaviorExceptionContext<TInstance, TData, TException>, Task<SendTuple<TMessage>>> messageFactory,
            Action<SendContext<TMessage>> callback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedSendActivity<TInstance, TData, TException, TMessage>(_ => GetDestinationAddress<TMessage>(),
                MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static EventActivityBinder<TInstance> Send<TInstance, TMessage>(this EventActivityBinder<TInstance> source,
            TMessage message, SendContextCallback<TInstance, TMessage> callback)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new SendActivity<TInstance, TMessage>(_ => GetDestinationAddress<TMessage>(),
                MessageFactory<TMessage>.Create(message, callback)));
        }

        public static EventActivityBinder<TInstance> SendAsync<TInstance, TMessage>(this EventActivityBinder<TInstance> source,
            Task<TMessage> message, SendContextCallback<TInstance, TMessage> callback)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new SendActivity<TInstance, TMessage>(_ => GetDestinationAddress<TMessage>(),
                MessageFactory<TMessage>.Create(message, callback)));
        }

        public static EventActivityBinder<TInstance> Send<TInstance, TMessage>(this EventActivityBinder<TInstance> source,
            EventMessageFactory<TInstance, TMessage> messageFactory, SendContextCallback<TInstance, TMessage> callback)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new SendActivity<TInstance, TMessage>(_ => GetDestinationAddress<TMessage>(),
                MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static EventActivityBinder<TInstance> SendAsync<TInstance, TMessage>(this EventActivityBinder<TInstance> source,
            AsyncEventMessageFactory<TInstance, TMessage> messageFactory, SendContextCallback<TInstance, TMessage> callback)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new SendActivity<TInstance, TMessage>(_ => GetDestinationAddress<TMessage>(),
                MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static EventActivityBinder<TInstance> SendAsync<TInstance, TMessage>(this EventActivityBinder<TInstance> source,
            Func<BehaviorContext<TInstance>, Task<SendTuple<TMessage>>> messageFactory, SendContextCallback<TInstance, TMessage> callback)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new SendActivity<TInstance, TMessage>(_ => GetDestinationAddress<TMessage>(),
                MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static EventActivityBinder<TInstance, TData> Send<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> source,
            TMessage message, SendContextCallback<TInstance, TData, TMessage> callback)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new SendActivity<TInstance, TData, TMessage>(_ => GetDestinationAddress<TMessage>(),
                MessageFactory<TMessage>.Create(message, callback)));
        }

        public static EventActivityBinder<TInstance, TData> SendAsync<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> source,
            Task<TMessage> message, SendContextCallback<TInstance, TData, TMessage> callback)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new SendActivity<TInstance, TData, TMessage>(_ => GetDestinationAddress<TMessage>(),
                MessageFactory<TMessage>.Create(message, callback)));
        }

        public static EventActivityBinder<TInstance, TData> Send<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> source,
            EventMessageFactory<TInstance, TData, TMessage> messageFactory, SendContextCallback<TInstance, TData, TMessage> callback)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new SendActivity<TInstance, TData, TMessage>(_ => GetDestinationAddress<TMessage>(),
                MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static EventActivityBinder<TInstance, TData> SendAsync<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> source,
            AsyncEventMessageFactory<TInstance, TData, TMessage> messageFactory, SendContextCallback<TInstance, TData, TMessage> callback)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new SendActivity<TInstance, TData, TMessage>(_ => GetDestinationAddress<TMessage>(),
                MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static EventActivityBinder<TInstance, TData> SendAsync<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> source,
            Func<BehaviorContext<TInstance, TData>, Task<SendTuple<TMessage>>> messageFactory, SendContextCallback<TInstance, TData, TMessage> callback)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new SendActivity<TInstance, TData, TMessage>(_ => GetDestinationAddress<TMessage>(),
                MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static ExceptionActivityBinder<TInstance, TException> Send<TInstance, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TException> source, TMessage message,
            SendExceptionContextCallback<TInstance, TException, TMessage> callback)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedSendActivity<TInstance, TException, TMessage>(_ => GetDestinationAddress<TMessage>(),
                MessageFactory<TMessage>.Create(message, callback)));
        }

        public static ExceptionActivityBinder<TInstance, TException> SendAsync<TInstance, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TException> source, Task<TMessage> message,
            SendExceptionContextCallback<TInstance, TException, TMessage> callback)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedSendActivity<TInstance, TException, TMessage>(_ => GetDestinationAddress<TMessage>(),
                MessageFactory<TMessage>.Create(message, callback)));
        }

        public static ExceptionActivityBinder<TInstance, TException> Send<TInstance, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TException> source,
            EventExceptionMessageFactory<TInstance, TException, TMessage> messageFactory,
            SendExceptionContextCallback<TInstance, TException, TMessage> callback)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedSendActivity<TInstance, TException, TMessage>(_ => GetDestinationAddress<TMessage>(),
                MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static ExceptionActivityBinder<TInstance, TException> SendAsync<TInstance, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TException> source,
            AsyncEventExceptionMessageFactory<TInstance, TException, TMessage> messageFactory,
            SendExceptionContextCallback<TInstance, TException, TMessage> callback)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedSendActivity<TInstance, TException, TMessage>(_ => GetDestinationAddress<TMessage>(),
                MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static ExceptionActivityBinder<TInstance, TException> SendAsync<TInstance, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TException> source,
            Func<BehaviorExceptionContext<TInstance, TException>, Task<SendTuple<TMessage>>> messageFactory,
            SendExceptionContextCallback<TInstance, TException, TMessage> callback)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedSendActivity<TInstance, TException, TMessage>(_ => GetDestinationAddress<TMessage>(),
                MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static ExceptionActivityBinder<TInstance, TData, TException> Send<TInstance, TData, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TData, TException> source, TMessage message,
            SendExceptionContextCallback<TInstance, TData, TException, TMessage> callback)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedSendActivity<TInstance, TData, TException, TMessage>(_ => GetDestinationAddress<TMessage>(),
                MessageFactory<TMessage>.Create(message, callback)));
        }

        public static ExceptionActivityBinder<TInstance, TData, TException> SendAsync<TInstance, TData, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TData, TException> source, Task<TMessage> message,
            SendExceptionContextCallback<TInstance, TData, TException, TMessage> callback)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedSendActivity<TInstance, TData, TException, TMessage>(_ => GetDestinationAddress<TMessage>(),
                MessageFactory<TMessage>.Create(message, callback)));
        }

        public static ExceptionActivityBinder<TInstance, TData, TException> Send<TInstance, TData, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TData, TException> source,
            EventExceptionMessageFactory<TInstance, TData, TException, TMessage> messageFactory,
            SendExceptionContextCallback<TInstance, TData, TException, TMessage> callback)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedSendActivity<TInstance, TData, TException, TMessage>(_ => GetDestinationAddress<TMessage>(),
                MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static ExceptionActivityBinder<TInstance, TData, TException> SendAsync<TInstance, TData, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TData, TException> source,
            AsyncEventExceptionMessageFactory<TInstance, TData, TException, TMessage> messageFactory,
            SendExceptionContextCallback<TInstance, TData, TException, TMessage> callback)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedSendActivity<TInstance, TData, TException, TMessage>(_ => GetDestinationAddress<TMessage>(),
                MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static ExceptionActivityBinder<TInstance, TData, TException> SendAsync<TInstance, TData, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TData, TException> source,
            Func<BehaviorExceptionContext<TInstance, TData, TException>, Task<SendTuple<TMessage>>> messageFactory,
            SendExceptionContextCallback<TInstance, TData, TException, TMessage> callback)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedSendActivity<TInstance, TData, TException, TMessage>(_ => GetDestinationAddress<TMessage>(),
                MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        static Uri GetDestinationAddress<T>()
            where T : class
        {
            return EndpointConvention.TryGetDestinationAddress<T>(out var destinationAddress)
                ? destinationAddress
                : throw new ArgumentException($"A convention for the message type {TypeCache<T>.ShortName} was not found");
        }
    }
}
