namespace MassTransit.RabbitMqTransport.Contexts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Context.Converters;
    using GreenPipes;
    using MassTransit.Pipeline;
    using Util;


    public class ReplyToSendEndpoint :
        ISendEndpoint
    {
        readonly ISendEndpoint _endpoint;
        readonly string _queueName;

        public ReplyToSendEndpoint(ISendEndpoint endpoint, string queueName)
        {
            _endpoint = endpoint;
            _queueName = queueName;
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

            var replyToPipe = new ReplyToPipe<T>(_queueName);

            return _endpoint.Send(message, replyToPipe, cancellationToken);
        }

        public Task Send<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
            where T : class
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            var replyToPipe = new ReplyToPipe<T>(_queueName, pipe);

            return _endpoint.Send(message, replyToPipe, cancellationToken);
        }

        public Task Send<T>(T message, IPipe<SendContext> pipe, CancellationToken cancellationToken)
            where T : class
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            var replyToPipe = new ReplyToPipe<T>(_queueName, pipe);

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

            var replyToPipe = new ReplyToPipe<T>(_queueName);

            return _endpoint.Send<T>(values, replyToPipe, cancellationToken);
        }

        public Task Send<T>(object values, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
            where T : class
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));
            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            var replyToPipe = new ReplyToPipe<T>(_queueName, pipe);

            return _endpoint.Send<T>(values, replyToPipe, cancellationToken);
        }

        public Task Send<T>(object values, IPipe<SendContext> pipe, CancellationToken cancellationToken)
            where T : class
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));
            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            var replyToPipe = new ReplyToPipe<T>(_queueName, pipe);

            return _endpoint.Send<T>(values, replyToPipe, cancellationToken);
        }


        class ReplyToPipe<TMessage> :
            IPipe<SendContext<TMessage>>,
            ISendContextPipe
            where TMessage : class
        {
            readonly string _queueName;
            readonly IPipe<SendContext<TMessage>> _pipe;
            readonly ISendContextPipe _sendContextPipe;

            public ReplyToPipe(string queueName)
            {
                _queueName = queueName;

                _pipe = default;
                _sendContextPipe = default;
            }

            public ReplyToPipe(string queueName, IPipe<SendContext<TMessage>> pipe)
            {
                _queueName = queueName;
                _pipe = pipe;
                _sendContextPipe = pipe as ISendContextPipe;
            }

            void IProbeSite.Probe(ProbeContext context)
            {
                _pipe?.Probe(context);
            }

            public Task Send(SendContext<TMessage> context)
            {
                context.SetRoutingKey(_queueName);

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
