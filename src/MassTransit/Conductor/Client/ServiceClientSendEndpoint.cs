namespace MassTransit.Conductor.Client
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Context.Converters;
    using Contexts;
    using GreenPipes;
    using MassTransit.Pipeline;
    using Util;


    /// <summary>
    /// For later, when regular Send is also supported by client
    /// </summary>
    public class ServiceClientSendEndpoint :
        ISendEndpoint
    {
        readonly ISendEndpoint _endpoint;
        readonly ServiceInstanceContext _context;

        public ServiceClientSendEndpoint(ISendEndpoint endpoint, ServiceInstanceContext context)
        {
            _endpoint = endpoint;
            _context = context;
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

            var replyToPipe = new InstancePipe<T>(_context);

            return _endpoint.Send(message, replyToPipe, cancellationToken);
        }

        public Task Send<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
            where T : class
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            var replyToPipe = new InstancePipe<T>(_context, pipe);

            return _endpoint.Send(message, replyToPipe, cancellationToken);
        }

        public Task Send<T>(T message, IPipe<SendContext> pipe, CancellationToken cancellationToken)
            where T : class
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            var replyToPipe = new InstancePipe<T>(_context, pipe);

            return _endpoint.Send(message, replyToPipe, cancellationToken);
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

            var replyToPipe = new InstancePipe<T>(_context);

            return _endpoint.Send<T>(values, replyToPipe, cancellationToken);
        }

        public Task Send<T>(object values, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
            where T : class
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));
            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            var replyToPipe = new InstancePipe<T>(_context, pipe);

            return _endpoint.Send<T>(values, replyToPipe, cancellationToken);
        }

        public Task Send<T>(object values, IPipe<SendContext> pipe, CancellationToken cancellationToken)
            where T : class
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));
            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            var replyToPipe = new InstancePipe<T>(_context, pipe);

            return _endpoint.Send<T>(values, replyToPipe, cancellationToken);
        }


        class InstancePipe<TMessage> :
            IPipe<SendContext<TMessage>>,
            ISendContextPipe
            where TMessage : class
        {
            readonly ServiceInstanceContext _context;
            readonly IPipe<SendContext<TMessage>> _pipe;
            readonly ISendContextPipe _sendContextPipe;

            public InstancePipe(ServiceInstanceContext context)
            {
                _context = context;

                _pipe = default;
                _sendContextPipe = default;
            }

            public InstancePipe(ServiceInstanceContext context, IPipe<SendContext<TMessage>> pipe)
            {
                _context = context;
                _pipe = pipe;
                _sendContextPipe = pipe as ISendContextPipe;
            }

            void IProbeSite.Probe(ProbeContext context)
            {
                _pipe?.Probe(context);
            }

            public Task Send(SendContext<TMessage> context)
            {
                if (_context != null)
                {
                    var instanceContext = _context;
                    context.GetOrAddPayload(() => instanceContext);
                }

                return _pipe.IsNotEmpty() ? _pipe.Send(context) : TaskUtil.Completed;
            }

            public Task Send<T>(SendContext<T> context)
                where T : class
            {
                return _sendContextPipe != null
                    ? _sendContextPipe.Send(context)
                    : TaskUtil.Completed;
            }
        }
    }
}
