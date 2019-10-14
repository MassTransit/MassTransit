namespace MassTransit.Conductor.Client
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Clients;
    using GreenPipes;
    using GreenPipes.Internals.Extensions;
    using Initializers;


    public class ServiceClientRequestSendEndpoint<TMessage> :
        IRequestSendEndpoint<TMessage>
        where TMessage : class
    {
        readonly Task<IMessageClient<TMessage>> _messageClient;
        readonly ConsumeContext _consumeContext;
        readonly ISendEndpointProvider _sendEndpointProvider;

        public ServiceClientRequestSendEndpoint(Task<IMessageClient<TMessage>> messageClient, ISendEndpointProvider sendEndpointProvider)
        {
            _messageClient = messageClient;
            _sendEndpointProvider = sendEndpointProvider;
        }

        public ServiceClientRequestSendEndpoint(Task<IMessageClient<TMessage>> messageClient, ConsumeContext consumeContext)
        {
            _messageClient = messageClient;
            _sendEndpointProvider = consumeContext;
            _consumeContext = consumeContext;
        }

        public Task<InitializeContext<TMessage>> CreateMessage(object values, CancellationToken cancellationToken)
        {
            var initializer = MessageInitializerCache<TMessage>.GetInitializer(values.GetType());

            return _consumeContext != null
                ? initializer.Initialize(initializer.Create(_consumeContext), values)
                : initializer.Initialize(values, cancellationToken);
        }

        public async Task Send(TMessage message, IPipe<SendContext<TMessage>> pipe, CancellationToken cancellationToken)
        {
            var messageClient = await _messageClient.OrCanceled(cancellationToken).ConfigureAwait(false);

            var sendEndpoint = await messageClient.GetServiceSendEndpoint(_sendEndpointProvider, message, cancellationToken).ConfigureAwait(false);

            var clientIdPipe = new ClientIdPipe<TMessage>(messageClient.ClientId, pipe);

            await sendEndpoint.Send(message, clientIdPipe, cancellationToken).ConfigureAwait(false);
        }


        readonly struct ClientIdPipe<T> :
            IPipe<SendContext<T>>
            where T : class
        {
            readonly Guid _clientId;
            readonly IPipe<SendContext<T>> _pipe;

            public ClientIdPipe(Guid clientId, IPipe<SendContext<T>> pipe)
            {
                _clientId = clientId;
                _pipe = pipe;
            }

            public Task Send(SendContext<T> context)
            {
                context.Headers.Set(MessageHeaders.ClientId, _clientId.ToString("D"));

                return _pipe.Send(context);
            }

            public void Probe(ProbeContext context)
            {
                _pipe.Probe(context);
            }
        }
    }
}
