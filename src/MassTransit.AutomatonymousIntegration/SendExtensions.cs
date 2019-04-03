namespace Automatonymous
{
    using System;
    using System.Threading.Tasks;
    using Activities;
    using Binders;
    using MassTransit;


    public static class SendExtensions
    {
        public static EventActivityBinder<TInstance> Send<TInstance, TMessage>(this EventActivityBinder<TInstance> source, Uri destinationAddress,
            TMessage message, Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new SendActivity<TInstance, TMessage>(new SendEndpointAddressProvider<TInstance>(_ => destinationAddress), x => message,
                contextCallback));
        }

        public static EventActivityBinder<TInstance> SendAsync<TInstance, TMessage>(this EventActivityBinder<TInstance> source, Uri destinationAddress,
            Task<TMessage> message, Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new SendActivity<TInstance, TMessage>(new SendEndpointAddressProvider<TInstance>(_ => destinationAddress), x => message,
                contextCallback));
        }

        public static EventActivityBinder<TInstance> Send<TInstance, TMessage>(this EventActivityBinder<TInstance> source,
            SendEndpointAddressProvider<TInstance> sendEndpointAddressProvider, TMessage message, Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new SendActivity<TInstance, TMessage>(sendEndpointAddressProvider, x => message, contextCallback));
        }

        public static EventActivityBinder<TInstance> SendAsync<TInstance, TMessage>(this EventActivityBinder<TInstance> source,
            SendEndpointAddressProvider<TInstance> sendEndpointAddressProvider, Task<TMessage> message, Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new SendActivity<TInstance, TMessage>(sendEndpointAddressProvider, x => message, contextCallback));
        }

        public static EventActivityBinder<TInstance> Send<TInstance, TMessage>(this EventActivityBinder<TInstance> source, Uri destinationAddress,
            EventMessageFactory<TInstance, TMessage> messageFactory, Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new SendActivity<TInstance, TMessage>(new SendEndpointAddressProvider<TInstance>(_ => destinationAddress), messageFactory,
                contextCallback));
        }

        public static EventActivityBinder<TInstance> SendAsync<TInstance, TMessage>(this EventActivityBinder<TInstance> source, Uri destinationAddress,
            AsyncEventMessageFactory<TInstance, TMessage> messageFactory, Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new SendActivity<TInstance, TMessage>(new SendEndpointAddressProvider<TInstance>(_ => destinationAddress), messageFactory,
                contextCallback));
        }

        public static EventActivityBinder<TInstance> Send<TInstance, TMessage>(this EventActivityBinder<TInstance> source,
            SendEndpointAddressProvider<TInstance> sendEndpointAddressProvider, EventMessageFactory<TInstance, TMessage> messageFactory,
            Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new SendActivity<TInstance, TMessage>(sendEndpointAddressProvider, messageFactory, contextCallback));
        }

        public static EventActivityBinder<TInstance> SendAsync<TInstance, TMessage>(this EventActivityBinder<TInstance> source,
            SendEndpointAddressProvider<TInstance> sendEndpointAddressProvider, AsyncEventMessageFactory<TInstance, TMessage> messageFactory,
            Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
        {
            return source.Add(new SendActivity<TInstance, TMessage>(sendEndpointAddressProvider, messageFactory, contextCallback));
        }

        public static EventActivityBinder<TInstance, TData> Send<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> source,
            Uri destinationAddress, TMessage message, Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new SendActivity<TInstance, TData, TMessage>(_ => destinationAddress, x => message, contextCallback));
        }

        public static EventActivityBinder<TInstance, TData> SendAsync<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> source,
            Uri destinationAddress, Task<TMessage> message, Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new SendActivity<TInstance, TData, TMessage>(_ => destinationAddress, x => message, contextCallback));
        }

        public static EventActivityBinder<TInstance, TData> Send<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> source,
            SendEndpointAddressProvider<TInstance, TData> sendEndpointAddressProvider, TMessage message, Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new SendActivity<TInstance, TData, TMessage>(sendEndpointAddressProvider, x => message, contextCallback));
        }

        public static EventActivityBinder<TInstance, TData> SendAsync<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> source,
            SendEndpointAddressProvider<TInstance, TData> sendEndpointAddressProvider, Task<TMessage> message,
            Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new SendActivity<TInstance, TData, TMessage>(sendEndpointAddressProvider, x => message, contextCallback));
        }

        public static EventActivityBinder<TInstance, TData> Send<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> source,
            Uri destinationAddress, EventMessageFactory<TInstance, TData, TMessage> messageFactory, Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new SendActivity<TInstance, TData, TMessage>(_ => destinationAddress, messageFactory, contextCallback));
        }

        public static EventActivityBinder<TInstance, TData> SendAsync<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> source,
            Uri destinationAddress, AsyncEventMessageFactory<TInstance, TData, TMessage> messageFactory, Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new SendActivity<TInstance, TData, TMessage>(_ => destinationAddress, messageFactory, contextCallback));
        }

        public static EventActivityBinder<TInstance, TData> Send<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> source,
            SendEndpointAddressProvider<TInstance, TData> sendEndpointAddressProvider, EventMessageFactory<TInstance, TData, TMessage> messageFactory,
            Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new SendActivity<TInstance, TData, TMessage>(sendEndpointAddressProvider, messageFactory, contextCallback));
        }

        public static EventActivityBinder<TInstance, TData> SendAsync<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> source,
            SendEndpointAddressProvider<TInstance, TData> sendEndpointAddressProvider, AsyncEventMessageFactory<TInstance, TData, TMessage> messageFactory,
            Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
        {
            return source.Add(new SendActivity<TInstance, TData, TMessage>(sendEndpointAddressProvider, messageFactory, contextCallback));
        }

        public static ExceptionActivityBinder<TInstance, TException> Send<TInstance, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TException> source, Uri destinationAddress, TMessage message,
            Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedSendActivity<TInstance, TException, TMessage>(_ => destinationAddress, x => message, contextCallback));
        }

        public static ExceptionActivityBinder<TInstance, TException> SendAsync<TInstance, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TException> source, Uri destinationAddress, Task<TMessage> message,
            Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedSendActivity<TInstance, TException, TMessage>(_ => destinationAddress, x => message, contextCallback));
        }

        [Obsolete("Use method with SendEndpointAddressProvider instead")]
        public static ExceptionActivityBinder<TInstance, TException> Send<TInstance, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TException> source, DestinationAddressProvider<TInstance> sendEndpointAddressProvider,
            TMessage message, Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new SendActivity<TInstance, TMessage>(sendEndpointAddressProvider, x => message, contextCallback));
        }

        public static ExceptionActivityBinder<TInstance, TException> Send<TInstance, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TException> source, SendEndpointAddressProvider<TInstance> sendEndpointAddressProvider,
            TMessage message, Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new SendActivity<TInstance, TMessage>(sendEndpointAddressProvider, x => message, contextCallback));
        }

        [Obsolete("Use method with SendEndpointAddressProvider instead")]
        public static ExceptionActivityBinder<TInstance, TException> SendAsync<TInstance, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TException> source, DestinationAddressProvider<TInstance> sendEndpointAddressProvider,
            Task<TMessage> message, Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new SendActivity<TInstance, TMessage>(sendEndpointAddressProvider, x => message, contextCallback));
        }

        public static ExceptionActivityBinder<TInstance, TException> SendAsync<TInstance, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TException> source, SendEndpointAddressProvider<TInstance> sendEndpointAddressProvider,
            Task<TMessage> message, Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new SendActivity<TInstance, TMessage>(sendEndpointAddressProvider, x => message, contextCallback));
        }

        public static ExceptionActivityBinder<TInstance, TException> Send<TInstance, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TException> source, Uri destinationAddress,
            EventExceptionMessageFactory<TInstance, TException, TMessage> messageFactory, Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedSendActivity<TInstance, TException, TMessage>(_ => destinationAddress, messageFactory, contextCallback));
        }

        public static ExceptionActivityBinder<TInstance, TException> SendAsync<TInstance, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TException> source, Uri destinationAddress,
            AsyncEventExceptionMessageFactory<TInstance, TException, TMessage> messageFactory, Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedSendActivity<TInstance, TException, TMessage>(_ => destinationAddress, messageFactory, contextCallback));
        }

        public static ExceptionActivityBinder<TInstance, TData, TException> Send<TInstance, TData, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TData, TException> source, Uri destinationAddress, TMessage message,
            Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedSendActivity<TInstance, TData, TException, TMessage>(_ => destinationAddress, x => message, contextCallback));
        }

        public static ExceptionActivityBinder<TInstance, TData, TException> SendAsync<TInstance, TData, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TData, TException> source, Uri destinationAddress, Task<TMessage> message,
            Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedSendActivity<TInstance, TData, TException, TMessage>(_ => destinationAddress, x => message, contextCallback));
        }

        [Obsolete("Use method with SendEndpointAddressProvider instead")]
        public static ExceptionActivityBinder<TInstance, TData, TException> Send<TInstance, TData, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TData, TException> source, DestinationAddressProvider<TInstance, TData> sendEndpointAddressProvider,
            TMessage message, Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new SendActivity<TInstance, TData, TMessage>(sendEndpointAddressProvider, x => message, contextCallback));
        }

        public static ExceptionActivityBinder<TInstance, TData, TException> Send<TInstance, TData, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TData, TException> source, SendEndpointAddressProvider<TInstance, TData> sendEndpointAddressProvider,
            TMessage message, Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new SendActivity<TInstance, TData, TMessage>(sendEndpointAddressProvider, x => message, contextCallback));
        }

        [Obsolete("Use method with SendEndpointAddressProvider instead")]
        public static ExceptionActivityBinder<TInstance, TData, TException> SendAsync<TInstance, TData, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TData, TException> source, DestinationAddressProvider<TInstance, TData> destinationAddressProvider,
            Task<TMessage> message, Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new SendActivity<TInstance, TData, TMessage>(destinationAddressProvider, x => message, contextCallback));
        }

        public static ExceptionActivityBinder<TInstance, TData, TException> SendAsync<TInstance, TData, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TData, TException> source, SendEndpointAddressProvider<TInstance, TData> sendEndpointAddressProvider,
            Task<TMessage> message, Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new SendActivity<TInstance, TData, TMessage>(sendEndpointAddressProvider, x => message, contextCallback));
        }

        public static ExceptionActivityBinder<TInstance, TData, TException> Send<TInstance, TData, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TData, TException> source, Uri destinationAddress,
            EventExceptionMessageFactory<TInstance, TData, TException, TMessage> messageFactory, Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedSendActivity<TInstance, TData, TException, TMessage>(_ => destinationAddress, messageFactory, contextCallback));
        }

        public static ExceptionActivityBinder<TInstance, TData, TException> SendAsync<TInstance, TData, TException, TMessage>(
            this ExceptionActivityBinder<TInstance, TData, TException> source, Uri destinationAddress,
            AsyncEventExceptionMessageFactory<TInstance, TData, TException, TMessage> messageFactory, Action<SendContext<TMessage>> contextCallback = null)
            where TInstance : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
            where TException : Exception
        {
            return source.Add(new FaultedSendActivity<TInstance, TData, TException, TMessage>(_ => destinationAddress, messageFactory, contextCallback));
        }
    }
}
