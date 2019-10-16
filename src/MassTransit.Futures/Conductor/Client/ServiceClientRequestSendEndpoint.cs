namespace MassTransit.Conductor.Client
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Clients;
    using GreenPipes;
    using Initializers;


    public class ServiceClientRequestSendEndpoint<T> :
        IRequestSendEndpoint<T>
        where T : class
    {
        readonly IServiceClient _serviceClient;
        readonly ISendEndpointProvider _sendEndpointProvider;

        public ServiceClientRequestSendEndpoint(IServiceClient serviceClient, ISendEndpointProvider sendEndpointProvider)
        {
            _serviceClient = serviceClient;
            _sendEndpointProvider = sendEndpointProvider;
        }

        public async Task<T> CreateMessage(object values, CancellationToken cancellationToken)
        {
            var initializer = MessageInitializerCache<T>.GetInitializer(values.GetType());

            if (_sendEndpointProvider is ConsumeContext context)
                return (await initializer.Initialize(initializer.Create(context), values).ConfigureAwait(false)).Message;

            return (await initializer.Initialize(values, cancellationToken).ConfigureAwait(false)).Message;
        }

        public async Task Send(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
        {
            var sendEndpoint = await _serviceClient.GetServiceSendEndpoint<T>(_sendEndpointProvider, cancellationToken).ConfigureAwait(false);

            var clientIdPipe = new ClientIdPipe(_serviceClient.ClientId, pipe);

            await sendEndpoint.Send(message, clientIdPipe, cancellationToken).ConfigureAwait(false);
        }


        readonly struct ClientIdPipe :
            IPipe<SendContext<T>>
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
