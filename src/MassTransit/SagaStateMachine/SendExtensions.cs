namespace MassTransit
{
    using System;
    using System.Threading.Tasks;
    using SagaStateMachine;


    public static class SendExtensions
    {
        public static EventActivityBinder<TSaga> Send<TSaga, TMessage>(this EventActivityBinder<TSaga> source, Uri destinationAddress,
            TMessage message, Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new SendActivity<TSaga, TMessage>(_ => destinationAddress, MessageFactory<TMessage>.Create(message, callback)));
        }

        public static EventActivityBinder<TSaga> SendAsync<TSaga, TMessage>(this EventActivityBinder<TSaga> source, Uri destinationAddress,
            Task<TMessage> message, Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new SendActivity<TSaga, TMessage>(_ => destinationAddress, MessageFactory<TMessage>.Create(message, callback)));
        }

        public static EventActivityBinder<TSaga> Send<TSaga, TMessage>(this EventActivityBinder<TSaga> source,
            DestinationAddressProvider<TSaga> destinationAddressProvider, TMessage message, Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new SendActivity<TSaga, TMessage>(destinationAddressProvider, MessageFactory<TMessage>.Create(message, callback)));
        }

        public static EventActivityBinder<TSaga> SendAsync<TSaga, TMessage>(this EventActivityBinder<TSaga> source,
            DestinationAddressProvider<TSaga> destinationAddressProvider, Task<TMessage> message, Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new SendActivity<TSaga, TMessage>(destinationAddressProvider, MessageFactory<TMessage>.Create(message, callback)));
        }

        public static EventActivityBinder<TSaga> Send<TSaga, TMessage>(this EventActivityBinder<TSaga> source, Uri destinationAddress,
            EventMessageFactory<TSaga, TMessage> messageFactory, Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new SendActivity<TSaga, TMessage>(_ => destinationAddress, MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static EventActivityBinder<TSaga> SendAsync<TSaga, TMessage>(this EventActivityBinder<TSaga> source, Uri destinationAddress,
            AsyncEventMessageFactory<TSaga, TMessage> messageFactory, Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new SendActivity<TSaga, TMessage>(_ => destinationAddress, MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static EventActivityBinder<TSaga> SendAsync<TSaga, TMessage>(this EventActivityBinder<TSaga> source, Uri destinationAddress,
            Func<BehaviorContext<TSaga>, Task<SendTuple<TMessage>>> messageFactory, Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new SendActivity<TSaga, TMessage>(_ => destinationAddress, MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static EventActivityBinder<TSaga> Send<TSaga, TMessage>(this EventActivityBinder<TSaga> source,
            DestinationAddressProvider<TSaga> destinationAddressProvider, EventMessageFactory<TSaga, TMessage> messageFactory,
            Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new SendActivity<TSaga, TMessage>(destinationAddressProvider, MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static EventActivityBinder<TSaga> SendAsync<TSaga, TMessage>(this EventActivityBinder<TSaga> source,
            DestinationAddressProvider<TSaga> destinationAddressProvider, AsyncEventMessageFactory<TSaga, TMessage> messageFactory,
            Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new SendActivity<TSaga, TMessage>(destinationAddressProvider, MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static EventActivityBinder<TSaga> SendAsync<TSaga, TMessage>(this EventActivityBinder<TSaga> source,
            DestinationAddressProvider<TSaga> destinationAddressProvider, Func<BehaviorContext<TSaga>, Task<SendTuple<TMessage>>> messageFactory,
            Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new SendActivity<TSaga, TMessage>(destinationAddressProvider, MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static EventActivityBinder<TSaga, TData> Send<TSaga, TData, TMessage>(this EventActivityBinder<TSaga, TData> source,
            Uri destinationAddress, TMessage message, Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new SendActivity<TSaga, TData, TMessage>(_ => destinationAddress, MessageFactory<TMessage>.Create(message, callback)));
        }

        public static EventActivityBinder<TSaga, TData> SendAsync<TSaga, TData, TMessage>(this EventActivityBinder<TSaga, TData> source,
            Uri destinationAddress, Task<TMessage> message, Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new SendActivity<TSaga, TData, TMessage>(_ => destinationAddress, MessageFactory<TMessage>.Create(message, callback)));
        }

        public static EventActivityBinder<TSaga, TData> Send<TSaga, TData, TMessage>(this EventActivityBinder<TSaga, TData> source,
            DestinationAddressProvider<TSaga, TData> destinationAddressProvider, TMessage message,
            Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new SendActivity<TSaga, TData, TMessage>(destinationAddressProvider, MessageFactory<TMessage>.Create(message, callback)));
        }

        public static EventActivityBinder<TSaga, TData> SendAsync<TSaga, TData, TMessage>(this EventActivityBinder<TSaga, TData> source,
            DestinationAddressProvider<TSaga, TData> destinationAddressProvider, Task<TMessage> message,
            Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new SendActivity<TSaga, TData, TMessage>(destinationAddressProvider, MessageFactory<TMessage>.Create(message, callback)));
        }

        public static EventActivityBinder<TSaga, TData> Send<TSaga, TData, TMessage>(this EventActivityBinder<TSaga, TData> source,
            Uri destinationAddress, EventMessageFactory<TSaga, TData, TMessage> messageFactory, Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new SendActivity<TSaga, TData, TMessage>(_ => destinationAddress, MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static EventActivityBinder<TSaga, TData> SendAsync<TSaga, TData, TMessage>(this EventActivityBinder<TSaga, TData> source,
            Uri destinationAddress, AsyncEventMessageFactory<TSaga, TData, TMessage> messageFactory, Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new SendActivity<TSaga, TData, TMessage>(_ => destinationAddress, MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static EventActivityBinder<TSaga, TData> SendAsync<TSaga, TData, TMessage>(this EventActivityBinder<TSaga, TData> source,
            Uri destinationAddress, Func<BehaviorContext<TSaga, TData>, Task<SendTuple<TMessage>>> messageFactory,
            Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new SendActivity<TSaga, TData, TMessage>(_ => destinationAddress, MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static EventActivityBinder<TSaga, TData> Send<TSaga, TData, TMessage>(this EventActivityBinder<TSaga, TData> source,
            DestinationAddressProvider<TSaga, TData> destinationAddressProvider, EventMessageFactory<TSaga, TData, TMessage> messageFactory,
            Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new SendActivity<TSaga, TData, TMessage>(destinationAddressProvider, MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static EventActivityBinder<TSaga, TData> SendAsync<TSaga, TData, TMessage>(this EventActivityBinder<TSaga, TData> source,
            DestinationAddressProvider<TSaga, TData> destinationAddressProvider,
            AsyncEventMessageFactory<TSaga, TData, TMessage> messageFactory,
            Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new SendActivity<TSaga, TData, TMessage>(destinationAddressProvider, MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static EventActivityBinder<TSaga, TData> SendAsync<TSaga, TData, TMessage>(this EventActivityBinder<TSaga, TData> source,
            DestinationAddressProvider<TSaga, TData> destinationAddressProvider,
            Func<BehaviorContext<TSaga, TData>, Task<SendTuple<TMessage>>> messageFactory,
            Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new SendActivity<TSaga, TData, TMessage>(destinationAddressProvider, MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static ExceptionActivityBinder<TSaga, TException> Send<TSaga, TException, TMessage>(this ExceptionActivityBinder<TSaga, TException> source,
            Uri destinationAddress, TMessage message,
            Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TMessage : class
            where TException : Exception
        {
            return source.Add(
                new FaultedSendActivity<TSaga, TException, TMessage>(_ => destinationAddress, MessageFactory<TMessage>.Create(message, callback)));
        }

        public static ExceptionActivityBinder<TSaga, TException> SendAsync<TSaga, TException, TMessage>(this ExceptionActivityBinder<TSaga, TException> source,
            Uri destinationAddress, Task<TMessage> message,
            Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TMessage : class
            where TException : Exception
        {
            return source.Add(
                new FaultedSendActivity<TSaga, TException, TMessage>(_ => destinationAddress, MessageFactory<TMessage>.Create(message, callback)));
        }

        public static ExceptionActivityBinder<TSaga, TException> Send<TSaga, TException, TMessage>(this ExceptionActivityBinder<TSaga, TException> source,
            DestinationAddressProvider<TSaga> destinationAddressProvider,
            TMessage message, Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TMessage : class
            where TException : Exception
        {
            return source.Add(
                new FaultedSendActivity<TSaga, TException, TMessage>(destinationAddressProvider, MessageFactory<TMessage>.Create(message, callback)));
        }

        public static ExceptionActivityBinder<TSaga, TException> SendAsync<TSaga, TException, TMessage>(this ExceptionActivityBinder<TSaga, TException> source,
            DestinationAddressProvider<TSaga> destinationAddressProvider,
            Task<TMessage> message, Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TMessage : class
            where TException : Exception
        {
            return source.Add(
                new FaultedSendActivity<TSaga, TException, TMessage>(destinationAddressProvider, MessageFactory<TMessage>.Create(message, callback)));
        }

        public static ExceptionActivityBinder<TSaga, TException> Send<TSaga, TException, TMessage>(this ExceptionActivityBinder<TSaga, TException> source,
            Uri destinationAddress,
            EventExceptionMessageFactory<TSaga, TException, TMessage> messageFactory, Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedSendActivity<TSaga, TException, TMessage>(_ => destinationAddress,
                MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static ExceptionActivityBinder<TSaga, TException> SendAsync<TSaga, TException, TMessage>(this ExceptionActivityBinder<TSaga, TException> source,
            Uri destinationAddress,
            AsyncEventExceptionMessageFactory<TSaga, TException, TMessage> messageFactory, Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedSendActivity<TSaga, TException, TMessage>(_ => destinationAddress,
                MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static ExceptionActivityBinder<TSaga, TException> SendAsync<TSaga, TException, TMessage>(this ExceptionActivityBinder<TSaga, TException> source,
            Uri destinationAddress,
            Func<BehaviorExceptionContext<TSaga, TException>, Task<SendTuple<TMessage>>> messageFactory, Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedSendActivity<TSaga, TException, TMessage>(_ => destinationAddress,
                MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static ExceptionActivityBinder<TSaga, TException> Send<TSaga, TException, TMessage>(this ExceptionActivityBinder<TSaga, TException> source,
            DestinationAddressProvider<TSaga> destinationAddressProvider,
            EventExceptionMessageFactory<TSaga, TException, TMessage> messageFactory,
            Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedSendActivity<TSaga, TException, TMessage>(destinationAddressProvider,
                MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static ExceptionActivityBinder<TSaga, TException> SendAsync<TSaga, TException, TMessage>(this ExceptionActivityBinder<TSaga, TException> source,
            DestinationAddressProvider<TSaga> destinationAddressProvider,
            AsyncEventExceptionMessageFactory<TSaga, TException, TMessage> messageFactory,
            Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedSendActivity<TSaga, TException, TMessage>(destinationAddressProvider,
                MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static ExceptionActivityBinder<TSaga, TException> SendAsync<TSaga, TException, TMessage>(this ExceptionActivityBinder<TSaga, TException> source,
            DestinationAddressProvider<TSaga> destinationAddressProvider,
            Func<BehaviorExceptionContext<TSaga, TException>, Task<SendTuple<TMessage>>> messageFactory,
            Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedSendActivity<TSaga, TException, TMessage>(destinationAddressProvider,
                MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static ExceptionActivityBinder<TSaga, TData, TException> Send<TSaga, TData, TException, TMessage>(
            this ExceptionActivityBinder<TSaga, TData, TException> source, Uri destinationAddress, TMessage message,
            Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
            where TException : Exception
        {
            return source.Add(
                new FaultedSendActivity<TSaga, TData, TException, TMessage>(_ => destinationAddress, MessageFactory<TMessage>.Create(message, callback)));
        }

        public static ExceptionActivityBinder<TSaga, TData, TException> SendAsync<TSaga, TData, TException, TMessage>(
            this ExceptionActivityBinder<TSaga, TData, TException> source, Uri destinationAddress, Task<TMessage> message,
            Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
            where TException : Exception
        {
            return source.Add(
                new FaultedSendActivity<TSaga, TData, TException, TMessage>(_ => destinationAddress, MessageFactory<TMessage>.Create(message, callback)));
        }

        public static ExceptionActivityBinder<TSaga, TData, TException> Send<TSaga, TData, TException, TMessage>(
            this ExceptionActivityBinder<TSaga, TData, TException> source, DestinationAddressProvider<TSaga, TData> destinationAddressProvider,
            TMessage message, Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedSendActivity<TSaga, TData, TException, TMessage>(destinationAddressProvider,
                MessageFactory<TMessage>.Create(message, callback)));
        }

        public static ExceptionActivityBinder<TSaga, TData, TException> SendAsync<TSaga, TData, TException, TMessage>(
            this ExceptionActivityBinder<TSaga, TData, TException> source, DestinationAddressProvider<TSaga, TData> destinationAddressProvider,
            Task<TMessage> message, Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedSendActivity<TSaga, TData, TException, TMessage>(destinationAddressProvider,
                MessageFactory<TMessage>.Create(message, callback)));
        }

        public static ExceptionActivityBinder<TSaga, TData, TException> Send<TSaga, TData, TException, TMessage>(
            this ExceptionActivityBinder<TSaga, TData, TException> source, Uri destinationAddress,
            EventExceptionMessageFactory<TSaga, TData, TException, TMessage> messageFactory, Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedSendActivity<TSaga, TData, TException, TMessage>(_ => destinationAddress,
                MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static ExceptionActivityBinder<TSaga, TData, TException> Send<TSaga, TData, TException, TMessage>(
            this ExceptionActivityBinder<TSaga, TData, TException> source,
            DestinationAddressProvider<TSaga, TData> destinationAddressProvider,
            EventExceptionMessageFactory<TSaga, TData, TException, TMessage> messageFactory,
            Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedSendActivity<TSaga, TData, TException, TMessage>(
                destinationAddressProvider, MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static ExceptionActivityBinder<TSaga, TData, TException> SendAsync<TSaga, TData, TException, TMessage>(
            this ExceptionActivityBinder<TSaga, TData, TException> source, Uri destinationAddress,
            AsyncEventExceptionMessageFactory<TSaga, TData, TException, TMessage> messageFactory, Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedSendActivity<TSaga, TData, TException, TMessage>(_ => destinationAddress,
                MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static ExceptionActivityBinder<TSaga, TData, TException> SendAsync<TSaga, TData, TException, TMessage>(
            this ExceptionActivityBinder<TSaga, TData, TException> source, Uri destinationAddress,
            Func<BehaviorExceptionContext<TSaga, TData, TException>, Task<SendTuple<TMessage>>> messageFactory, Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedSendActivity<TSaga, TData, TException, TMessage>(_ => destinationAddress,
                MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static ExceptionActivityBinder<TSaga, TData, TException> SendAsync<TSaga, TData, TException, TMessage>(
            this ExceptionActivityBinder<TSaga, TData, TException> source, DestinationAddressProvider<TSaga, TData> destinationAddressProvider,
            AsyncEventExceptionMessageFactory<TSaga, TData, TException, TMessage> messageFactory, Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedSendActivity<TSaga, TData, TException, TMessage>(destinationAddressProvider,
                MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static ExceptionActivityBinder<TSaga, TData, TException> SendAsync<TSaga, TData, TException, TMessage>(
            this ExceptionActivityBinder<TSaga, TData, TException> source, DestinationAddressProvider<TSaga, TData> destinationAddressProvider,
            Func<BehaviorExceptionContext<TSaga, TData, TException>, Task<SendTuple<TMessage>>> messageFactory, Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedSendActivity<TSaga, TData, TException, TMessage>(destinationAddressProvider,
                MessageFactory<TMessage>.Create(messageFactory, callback)));
        }
    }


    public static class SendCallbackExtensions
    {
        public static EventActivityBinder<TSaga> Send<TSaga, TMessage>(this EventActivityBinder<TSaga> source, Uri destinationAddress,
            TMessage message, SendContextCallback<TSaga, TMessage> callback)
            where TSaga : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new SendActivity<TSaga, TMessage>(_ => destinationAddress, MessageFactory<TMessage>.Create(message, callback)));
        }

        public static EventActivityBinder<TSaga> SendAsync<TSaga, TMessage>(this EventActivityBinder<TSaga> source, Uri destinationAddress,
            Task<TMessage> message, SendContextCallback<TSaga, TMessage> callback)
            where TSaga : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new SendActivity<TSaga, TMessage>(_ => destinationAddress, MessageFactory<TMessage>.Create(message, callback)));
        }

        public static EventActivityBinder<TSaga> Send<TSaga, TMessage>(this EventActivityBinder<TSaga> source,
            DestinationAddressProvider<TSaga> destinationAddressProvider, TMessage message, SendContextCallback<TSaga, TMessage> callback)
            where TSaga : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new SendActivity<TSaga, TMessage>(destinationAddressProvider, MessageFactory<TMessage>.Create(message, callback)));
        }

        public static EventActivityBinder<TSaga> SendAsync<TSaga, TMessage>(this EventActivityBinder<TSaga> source,
            DestinationAddressProvider<TSaga> destinationAddressProvider, Task<TMessage> message, SendContextCallback<TSaga, TMessage> callback)
            where TSaga : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new SendActivity<TSaga, TMessage>(destinationAddressProvider, MessageFactory<TMessage>.Create(message, callback)));
        }

        public static EventActivityBinder<TSaga> Send<TSaga, TMessage>(this EventActivityBinder<TSaga> source, Uri destinationAddress,
            EventMessageFactory<TSaga, TMessage> messageFactory, SendContextCallback<TSaga, TMessage> callback)
            where TSaga : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new SendActivity<TSaga, TMessage>(_ => destinationAddress, MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static EventActivityBinder<TSaga> SendAsync<TSaga, TMessage>(this EventActivityBinder<TSaga> source, Uri destinationAddress,
            AsyncEventMessageFactory<TSaga, TMessage> messageFactory, SendContextCallback<TSaga, TMessage> callback)
            where TSaga : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new SendActivity<TSaga, TMessage>(_ => destinationAddress, MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static EventActivityBinder<TSaga> SendAsync<TSaga, TMessage>(this EventActivityBinder<TSaga> source, Uri destinationAddress,
            Func<BehaviorContext<TSaga>, Task<SendTuple<TMessage>>> messageFactory, SendContextCallback<TSaga, TMessage> callback)
            where TSaga : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new SendActivity<TSaga, TMessage>(_ => destinationAddress, MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static EventActivityBinder<TSaga> Send<TSaga, TMessage>(this EventActivityBinder<TSaga> source,
            DestinationAddressProvider<TSaga> destinationAddressProvider, EventMessageFactory<TSaga, TMessage> messageFactory,
            SendContextCallback<TSaga, TMessage> callback)
            where TSaga : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new SendActivity<TSaga, TMessage>(destinationAddressProvider, MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static EventActivityBinder<TSaga> SendAsync<TSaga, TMessage>(this EventActivityBinder<TSaga> source,
            DestinationAddressProvider<TSaga> destinationAddressProvider, AsyncEventMessageFactory<TSaga, TMessage> messageFactory,
            SendContextCallback<TSaga, TMessage> callback)
            where TSaga : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new SendActivity<TSaga, TMessage>(destinationAddressProvider, MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static EventActivityBinder<TSaga> SendAsync<TSaga, TMessage>(this EventActivityBinder<TSaga> source,
            DestinationAddressProvider<TSaga> destinationAddressProvider, Func<BehaviorContext<TSaga>, Task<SendTuple<TMessage>>> messageFactory,
            SendContextCallback<TSaga, TMessage> callback)
            where TSaga : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new SendActivity<TSaga, TMessage>(destinationAddressProvider, MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static EventActivityBinder<TSaga, TData> Send<TSaga, TData, TMessage>(this EventActivityBinder<TSaga, TData> source,
            Uri destinationAddress, TMessage message, SendContextCallback<TSaga, TData, TMessage> callback)
            where TSaga : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new SendActivity<TSaga, TData, TMessage>(_ => destinationAddress, MessageFactory<TMessage>.Create(message, callback)));
        }

        public static EventActivityBinder<TSaga, TData> SendAsync<TSaga, TData, TMessage>(this EventActivityBinder<TSaga, TData> source,
            Uri destinationAddress, Task<TMessage> message, SendContextCallback<TSaga, TData, TMessage> callback)
            where TSaga : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new SendActivity<TSaga, TData, TMessage>(_ => destinationAddress, MessageFactory<TMessage>.Create(message, callback)));
        }

        public static EventActivityBinder<TSaga, TData> Send<TSaga, TData, TMessage>(this EventActivityBinder<TSaga, TData> source,
            DestinationAddressProvider<TSaga, TData> destinationAddressProvider, TMessage message,
            SendContextCallback<TSaga, TData, TMessage> callback)
            where TSaga : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new SendActivity<TSaga, TData, TMessage>(destinationAddressProvider, MessageFactory<TMessage>.Create(message, callback)));
        }

        public static EventActivityBinder<TSaga, TData> SendAsync<TSaga, TData, TMessage>(this EventActivityBinder<TSaga, TData> source,
            DestinationAddressProvider<TSaga, TData> destinationAddressProvider, Task<TMessage> message,
            SendContextCallback<TSaga, TData, TMessage> callback)
            where TSaga : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new SendActivity<TSaga, TData, TMessage>(destinationAddressProvider, MessageFactory<TMessage>.Create(message, callback)));
        }

        public static EventActivityBinder<TSaga, TData> Send<TSaga, TData, TMessage>(this EventActivityBinder<TSaga, TData> source,
            Uri destinationAddress, EventMessageFactory<TSaga, TData, TMessage> messageFactory,
            SendContextCallback<TSaga, TData, TMessage> callback)
            where TSaga : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new SendActivity<TSaga, TData, TMessage>(_ => destinationAddress, MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static EventActivityBinder<TSaga, TData> SendAsync<TSaga, TData, TMessage>(this EventActivityBinder<TSaga, TData> source,
            Uri destinationAddress, AsyncEventMessageFactory<TSaga, TData, TMessage> messageFactory,
            SendContextCallback<TSaga, TData, TMessage> callback)
            where TSaga : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new SendActivity<TSaga, TData, TMessage>(_ => destinationAddress, MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static EventActivityBinder<TSaga, TData> SendAsync<TSaga, TData, TMessage>(this EventActivityBinder<TSaga, TData> source,
            Uri destinationAddress, Func<BehaviorContext<TSaga, TData>, Task<SendTuple<TMessage>>> messageFactory,
            SendContextCallback<TSaga, TData, TMessage> callback)
            where TSaga : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new SendActivity<TSaga, TData, TMessage>(_ => destinationAddress, MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static EventActivityBinder<TSaga, TData> Send<TSaga, TData, TMessage>(this EventActivityBinder<TSaga, TData> source,
            DestinationAddressProvider<TSaga, TData> destinationAddressProvider, EventMessageFactory<TSaga, TData, TMessage> messageFactory,
            SendContextCallback<TSaga, TData, TMessage> callback)
            where TSaga : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new SendActivity<TSaga, TData, TMessage>(destinationAddressProvider, MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static EventActivityBinder<TSaga, TData> SendAsync<TSaga, TData, TMessage>(this EventActivityBinder<TSaga, TData> source,
            DestinationAddressProvider<TSaga, TData> destinationAddressProvider,
            AsyncEventMessageFactory<TSaga, TData, TMessage> messageFactory,
            SendContextCallback<TSaga, TData, TMessage> callback)
            where TSaga : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new SendActivity<TSaga, TData, TMessage>(destinationAddressProvider, MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static EventActivityBinder<TSaga, TData> SendAsync<TSaga, TData, TMessage>(this EventActivityBinder<TSaga, TData> source,
            DestinationAddressProvider<TSaga, TData> destinationAddressProvider,
            Func<BehaviorContext<TSaga, TData>, Task<SendTuple<TMessage>>> messageFactory,
            SendContextCallback<TSaga, TData, TMessage> callback)
            where TSaga : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new SendActivity<TSaga, TData, TMessage>(destinationAddressProvider, MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static ExceptionActivityBinder<TSaga, TException> Send<TSaga, TException, TMessage>(this ExceptionActivityBinder<TSaga, TException> source,
            Uri destinationAddress, TMessage message, SendExceptionContextCallback<TSaga, TException, TMessage> callback)
            where TSaga : class, SagaStateMachineInstance
            where TMessage : class
            where TException : Exception
        {
            return source.Add(
                new FaultedSendActivity<TSaga, TException, TMessage>(_ => destinationAddress, MessageFactory<TMessage>.Create(message, callback)));
        }

        public static ExceptionActivityBinder<TSaga, TException> SendAsync<TSaga, TException, TMessage>(this ExceptionActivityBinder<TSaga, TException> source,
            Uri destinationAddress, Task<TMessage> message, SendExceptionContextCallback<TSaga, TException, TMessage> callback)
            where TSaga : class, SagaStateMachineInstance
            where TMessage : class
            where TException : Exception
        {
            return source.Add(
                new FaultedSendActivity<TSaga, TException, TMessage>(_ => destinationAddress, MessageFactory<TMessage>.Create(message, callback)));
        }

        public static ExceptionActivityBinder<TSaga, TException> Send<TSaga, TException, TMessage>(this ExceptionActivityBinder<TSaga, TException> source,
            DestinationAddressProvider<TSaga> destinationAddressProvider, TMessage message,
            SendExceptionContextCallback<TSaga, TException, TMessage> callback)
            where TSaga : class, SagaStateMachineInstance
            where TMessage : class
            where TException : Exception
        {
            return source.Add(
                new FaultedSendActivity<TSaga, TException, TMessage>(destinationAddressProvider, MessageFactory<TMessage>.Create(message, callback)));
        }

        public static ExceptionActivityBinder<TSaga, TException> SendAsync<TSaga, TException, TMessage>(this ExceptionActivityBinder<TSaga, TException> source,
            DestinationAddressProvider<TSaga> destinationAddressProvider, Task<TMessage> message,
            SendExceptionContextCallback<TSaga, TException, TMessage> callback)
            where TSaga : class, SagaStateMachineInstance
            where TMessage : class
            where TException : Exception
        {
            return source.Add(
                new FaultedSendActivity<TSaga, TException, TMessage>(destinationAddressProvider, MessageFactory<TMessage>.Create(message, callback)));
        }

        public static ExceptionActivityBinder<TSaga, TException> Send<TSaga, TException, TMessage>(this ExceptionActivityBinder<TSaga, TException> source,
            Uri destinationAddress,
            EventExceptionMessageFactory<TSaga, TException, TMessage> messageFactory, SendExceptionContextCallback<TSaga, TException, TMessage> callback)
            where TSaga : class, SagaStateMachineInstance
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedSendActivity<TSaga, TException, TMessage>(_ => destinationAddress,
                MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static ExceptionActivityBinder<TSaga, TException> Send<TSaga, TException, TMessage>(this ExceptionActivityBinder<TSaga, TException> source,
            DestinationAddressProvider<TSaga> destinationAddressProvider,
            EventExceptionMessageFactory<TSaga, TException, TMessage> messageFactory,
            SendExceptionContextCallback<TSaga, TException, TMessage> callback)
            where TSaga : class, SagaStateMachineInstance
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedSendActivity<TSaga, TException, TMessage>(destinationAddressProvider,
                MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static ExceptionActivityBinder<TSaga, TException> SendAsync<TSaga, TException, TMessage>(this ExceptionActivityBinder<TSaga, TException> source,
            Uri destinationAddress,
            AsyncEventExceptionMessageFactory<TSaga, TException, TMessage> messageFactory, SendExceptionContextCallback<TSaga, TException, TMessage> callback)
            where TSaga : class, SagaStateMachineInstance
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedSendActivity<TSaga, TException, TMessage>(_ => destinationAddress,
                MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static ExceptionActivityBinder<TSaga, TException> SendAsync<TSaga, TException, TMessage>(this ExceptionActivityBinder<TSaga, TException> source,
            Uri destinationAddress,
            Func<BehaviorExceptionContext<TSaga, TException>, Task<SendTuple<TMessage>>> messageFactory,
            SendExceptionContextCallback<TSaga, TException, TMessage> callback)
            where TSaga : class, SagaStateMachineInstance
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedSendActivity<TSaga, TException, TMessage>(_ => destinationAddress,
                MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static ExceptionActivityBinder<TSaga, TException> SendAsync<TSaga, TException, TMessage>(this ExceptionActivityBinder<TSaga, TException> source,
            DestinationAddressProvider<TSaga> destinationAddressProvider,
            AsyncEventExceptionMessageFactory<TSaga, TException, TMessage> messageFactory,
            SendExceptionContextCallback<TSaga, TException, TMessage> callback)
            where TSaga : class, SagaStateMachineInstance
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedSendActivity<TSaga, TException, TMessage>(destinationAddressProvider,
                MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static ExceptionActivityBinder<TSaga, TException> SendAsync<TSaga, TException, TMessage>(this ExceptionActivityBinder<TSaga, TException> source,
            DestinationAddressProvider<TSaga> destinationAddressProvider,
            Func<BehaviorExceptionContext<TSaga, TException>, Task<SendTuple<TMessage>>> messageFactory,
            SendExceptionContextCallback<TSaga, TException, TMessage> callback)
            where TSaga : class, SagaStateMachineInstance
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedSendActivity<TSaga, TException, TMessage>(destinationAddressProvider,
                MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static ExceptionActivityBinder<TSaga, TData, TException> Send<TSaga, TData, TException, TMessage>(
            this ExceptionActivityBinder<TSaga, TData, TException> source, Uri destinationAddress, TMessage message,
            SendExceptionContextCallback<TSaga, TData, TException, TMessage> callback)
            where TSaga : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
            where TException : Exception
        {
            return source.Add(
                new FaultedSendActivity<TSaga, TData, TException, TMessage>(_ => destinationAddress, MessageFactory<TMessage>.Create(message, callback)));
        }

        public static ExceptionActivityBinder<TSaga, TData, TException> SendAsync<TSaga, TData, TException, TMessage>(
            this ExceptionActivityBinder<TSaga, TData, TException> source, Uri destinationAddress, Task<TMessage> message,
            SendExceptionContextCallback<TSaga, TData, TException, TMessage> callback)
            where TSaga : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedSendActivity<TSaga, TData, TException, TMessage>(_ => destinationAddress,
                MessageFactory<TMessage>.Create(message, callback)));
        }

        public static ExceptionActivityBinder<TSaga, TData, TException> Send<TSaga, TData, TException, TMessage>(
            this ExceptionActivityBinder<TSaga, TData, TException> source, DestinationAddressProvider<TSaga, TData> destinationAddressProvider,
            TMessage message, SendExceptionContextCallback<TSaga, TData, TException, TMessage> callback)
            where TSaga : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedSendActivity<TSaga, TData, TException, TMessage>(destinationAddressProvider,
                MessageFactory<TMessage>.Create(message, callback)));
        }

        public static ExceptionActivityBinder<TSaga, TData, TException> SendAsync<TSaga, TData, TException, TMessage>(
            this ExceptionActivityBinder<TSaga, TData, TException> source, DestinationAddressProvider<TSaga, TData> destinationAddressProvider,
            Task<TMessage> message, SendExceptionContextCallback<TSaga, TData, TException, TMessage> callback)
            where TSaga : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedSendActivity<TSaga, TData, TException, TMessage>(destinationAddressProvider,
                MessageFactory<TMessage>.Create(message, callback)));
        }

        public static ExceptionActivityBinder<TSaga, TData, TException> Send<TSaga, TData, TException, TMessage>(
            this ExceptionActivityBinder<TSaga, TData, TException> source, Uri destinationAddress,
            EventExceptionMessageFactory<TSaga, TData, TException, TMessage> messageFactory,
            SendExceptionContextCallback<TSaga, TData, TException, TMessage> callback)
            where TSaga : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedSendActivity<TSaga, TData, TException, TMessage>(_ => destinationAddress,
                MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static ExceptionActivityBinder<TSaga, TData, TException> SendAsync<TSaga, TData, TException, TMessage>(
            this ExceptionActivityBinder<TSaga, TData, TException> source, Uri destinationAddress,
            AsyncEventExceptionMessageFactory<TSaga, TData, TException, TMessage> messageFactory,
            SendExceptionContextCallback<TSaga, TData, TException, TMessage> callback)
            where TSaga : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedSendActivity<TSaga, TData, TException, TMessage>(_ => destinationAddress,
                MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static ExceptionActivityBinder<TSaga, TData, TException> SendAsync<TSaga, TData, TException, TMessage>(
            this ExceptionActivityBinder<TSaga, TData, TException> source, Uri destinationAddress,
            Func<BehaviorExceptionContext<TSaga, TData, TException>, Task<SendTuple<TMessage>>> messageFactory,
            SendExceptionContextCallback<TSaga, TData, TException, TMessage> callback)
            where TSaga : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedSendActivity<TSaga, TData, TException, TMessage>(_ => destinationAddress,
                MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static ExceptionActivityBinder<TSaga, TData, TException> Send<TSaga, TData, TException, TMessage>(
            this ExceptionActivityBinder<TSaga, TData, TException> source,
            DestinationAddressProvider<TSaga, TData> destinationAddressProvider,
            EventExceptionMessageFactory<TSaga, TData, TException, TMessage> messageFactory,
            SendExceptionContextCallback<TSaga, TData, TException, TMessage> callback)
            where TSaga : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedSendActivity<TSaga, TData, TException, TMessage>(destinationAddressProvider,
                MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static ExceptionActivityBinder<TSaga, TData, TException> SendAsync<TSaga, TData, TException, TMessage>(
            this ExceptionActivityBinder<TSaga, TData, TException> source,
            DestinationAddressProvider<TSaga, TData> destinationAddressProvider,
            AsyncEventExceptionMessageFactory<TSaga, TData, TException, TMessage> messageFactory,
            SendExceptionContextCallback<TSaga, TData, TException, TMessage> callback)
            where TSaga : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedSendActivity<TSaga, TData, TException, TMessage>(destinationAddressProvider,
                MessageFactory<TMessage>.Create(messageFactory, callback)));
        }

        public static ExceptionActivityBinder<TSaga, TData, TException> SendAsync<TSaga, TData, TException, TMessage>(
            this ExceptionActivityBinder<TSaga, TData, TException> source,
            DestinationAddressProvider<TSaga, TData> destinationAddressProvider,
            Func<BehaviorExceptionContext<TSaga, TData, TException>, Task<SendTuple<TMessage>>> messageFactory,
            SendExceptionContextCallback<TSaga, TData, TException, TMessage> callback)
            where TSaga : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedSendActivity<TSaga, TData, TException, TMessage>(destinationAddressProvider,
                MessageFactory<TMessage>.Create(messageFactory, callback)));
        }
    }
}
