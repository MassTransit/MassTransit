namespace MassTransit.Transports
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Context.Converters;
    using GreenPipes;
    using Initializers;


    /// <summary>
    /// Generalized proxy for ISendEndpoint to intercept pipe/context
    /// </summary>
    public abstract class SendEndpointProxy :
        ISendEndpoint
    {
        readonly ISendEndpoint _endpoint;

        protected SendEndpointProxy(ISendEndpoint endpoint)
        {
            _endpoint = endpoint;
        }

        internal ISendEndpoint Endpoint => _endpoint;

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _endpoint.ConnectSendObserver(observer);
        }

        public virtual Task Send<T>(T message, CancellationToken cancellationToken)
            where T : class
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            return _endpoint.Send(message, GetPipeProxy<T>(), cancellationToken);
        }

        public virtual Task Send<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
            where T : class
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            return _endpoint.Send(message, GetPipeProxy(pipe), cancellationToken);
        }

        public virtual Task Send<T>(T message, IPipe<SendContext> pipe, CancellationToken cancellationToken)
            where T : class
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            return _endpoint.Send(message, GetPipeProxy<T>(pipe), cancellationToken);
        }

        public Task Send(object message, CancellationToken cancellationToken)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var messageType = message.GetType();

            return SendEndpointConverterCache.Send(this, message, messageType, cancellationToken);
        }

        public Task Send(object message, Type messageType, CancellationToken cancellationToken)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (messageType == null)
                throw new ArgumentNullException(nameof(messageType));

            return SendEndpointConverterCache.Send(this, message, messageType, cancellationToken);
        }

        public Task Send(object message, IPipe<SendContext> pipe, CancellationToken cancellationToken)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            var messageType = message.GetType();

            return SendEndpointConverterCache.Send(this, message, messageType, pipe, cancellationToken);
        }

        public Task Send(object message, Type messageType, IPipe<SendContext> pipe, CancellationToken cancellationToken)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (messageType == null)
                throw new ArgumentNullException(nameof(messageType));

            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            return SendEndpointConverterCache.Send(this, message, messageType, pipe, cancellationToken);
        }

        public Task Send<T>(object values, CancellationToken cancellationToken)
            where T : class
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            IMessageInitializer<T> initializer = MessageInitializerCache<T>.GetInitializer(values.GetType());

            InitializeContext<T> context = GetInitializeContext(initializer, cancellationToken);

            return initializer.Send(this, context, values);
        }

        public Task Send<T>(object values, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
            where T : class
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            IMessageInitializer<T> initializer = MessageInitializerCache<T>.GetInitializer(values.GetType());

            InitializeContext<T> context = GetInitializeContext(initializer, cancellationToken);

            return initializer.Send(this, context, values, pipe);
        }

        public Task Send<T>(object values, IPipe<SendContext> pipe, CancellationToken cancellationToken)
            where T : class
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            IMessageInitializer<T> initializer = MessageInitializerCache<T>.GetInitializer(values.GetType());

            InitializeContext<T> context = GetInitializeContext(initializer, cancellationToken);

            return initializer.Send(this, context, values, pipe);
        }

        protected abstract IPipe<SendContext<T>> GetPipeProxy<T>(IPipe<SendContext<T>> pipe = default)
            where T : class;

        protected virtual InitializeContext<T> GetInitializeContext<T>(IMessageInitializer<T> initializer, CancellationToken cancellationToken)
            where T : class
        {
            return initializer.Create(cancellationToken);
        }
    }
}
