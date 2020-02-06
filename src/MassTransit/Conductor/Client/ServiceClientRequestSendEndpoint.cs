namespace MassTransit.Conductor.Client
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Clients;
    using GreenPipes;
    using GreenPipes.Internals.Extensions;


    public class ServiceClientRequestSendEndpoint<TMessage> :
        IRequestSendEndpoint<TMessage>
        where TMessage : class
    {
        readonly Task<IMessageClient<TMessage>> _messageClient;
        readonly ClientFactoryContext _clientFactoryContext;
        readonly ConsumeContext _consumeContext;

        public ServiceClientRequestSendEndpoint(Task<IMessageClient<TMessage>> messageClient, ClientFactoryContext clientFactoryContext)
        {
            _messageClient = messageClient;
            _clientFactoryContext = clientFactoryContext;
        }

        public ServiceClientRequestSendEndpoint(Task<IMessageClient<TMessage>> messageClient, ClientFactoryContext clientFactoryContext,
            ConsumeContext consumeContext)
        {
            _messageClient = messageClient;
            _clientFactoryContext = clientFactoryContext;
            _consumeContext = consumeContext;
        }

        public async Task<TMessage> Send(Guid requestId, object values, IPipe<SendContext<TMessage>> pipe, CancellationToken cancellationToken)
        {
            var messageClient = await _messageClient.OrCanceled(cancellationToken).ConfigureAwait(false);

            var sendEndpoint = await messageClient.GetServiceSendEndpoint(_clientFactoryContext, default, _consumeContext, cancellationToken)
                .ConfigureAwait(false);

            var clientIdPipe = new ClientIdPipe<TMessage>(messageClient.ClientId, pipe);

            return await sendEndpoint.Send(requestId, values, clientIdPipe, cancellationToken).ConfigureAwait(false);
        }

        public async Task Send(Guid requestId, TMessage message, IPipe<SendContext<TMessage>> pipe, CancellationToken cancellationToken)
        {
            var messageClient = await _messageClient.OrCanceled(cancellationToken).ConfigureAwait(false);

            var sendEndpoint = await messageClient.GetServiceSendEndpoint(_clientFactoryContext, message, _consumeContext, cancellationToken)
                .ConfigureAwait(false);

            var clientIdPipe = new ClientIdPipe<TMessage>(messageClient.ClientId, pipe);

            await sendEndpoint.Send(requestId, message, clientIdPipe, cancellationToken).ConfigureAwait(false);
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
