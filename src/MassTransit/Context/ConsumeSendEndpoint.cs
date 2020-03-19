namespace MassTransit.Context
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using Converters;
    using GreenPipes;
    using Initializers;


    /// <summary>
    /// Intercepts the ISendEndpoint and makes it part of the current consume context
    /// </summary>
    public class ConsumeSendEndpoint :
        ISendEndpoint
    {
        readonly ConsumeContext _context;
        readonly ISendEndpoint _endpoint;
        readonly Guid? _requestId;

        public ConsumeSendEndpoint(ISendEndpoint endpoint, ConsumeContext context, Guid? requestId)
        {
            _endpoint = endpoint;
            _context = context;
            _requestId = requestId;
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _endpoint.ConnectSendObserver(observer);
        }

        public Task Send<T>(T message, CancellationToken cancellationToken)
            where T : class
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var sendContextPipe = new ConsumeSendEndpointPipe<T>(_context, _requestId);

            return ConsumeTask(_endpoint.Send(message, sendContextPipe, cancellationToken));
        }

        public Task Send<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
            where T : class
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            var sendContextPipe = new ConsumeSendEndpointPipe<T>(_context, pipe, _requestId);

            return ConsumeTask(_endpoint.Send(message, sendContextPipe, cancellationToken));
        }

        public Task Send<T>(T message, IPipe<SendContext> pipe, CancellationToken cancellationToken)
            where T : class
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            var sendContextPipe = new ConsumeSendEndpointPipe<T>(_context, pipe, _requestId);

            return ConsumeTask(_endpoint.Send(message, sendContextPipe, cancellationToken));
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

            var initializer = MessageInitializerCache<T>.GetInitializer(values.GetType());

            return initializer.Send(this, initializer.Create(_context), values);
        }

        public Task Send<T>(object values, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
            where T : class
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            var initializer = MessageInitializerCache<T>.GetInitializer(values.GetType());

            return initializer.Send(this, initializer.Create(_context), values, pipe);
        }

        public Task Send<T>(object values, IPipe<SendContext> pipe, CancellationToken cancellationToken)
            where T : class
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            var initializer = MessageInitializerCache<T>.GetInitializer(values.GetType());

            return initializer.Send(this, initializer.Create(_context), values, pipe);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        Task ConsumeTask(Task task)
        {
            _context.AddConsumeTask(task);
            return task;
        }
    }
}
