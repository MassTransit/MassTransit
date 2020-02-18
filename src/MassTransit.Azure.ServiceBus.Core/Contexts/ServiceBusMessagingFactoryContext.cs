namespace MassTransit.Azure.ServiceBus.Core.Contexts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using Transports;


    public class ServiceBusMessagingFactoryContext :
        BasePipeContext,
        MessagingFactoryContext,
        IAsyncDisposable
    {
        readonly MessagingFactory _messagingFactory;

        public ServiceBusMessagingFactoryContext(MessagingFactory messagingFactory, CancellationToken cancellationToken)
            : base(cancellationToken)
        {
            _messagingFactory = messagingFactory;
        }

        public async Task DisposeAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            var address = _messagingFactory.Address.ToString();

            TransportLogMessages.DisconnectHost(address);

            await _messagingFactory.CloseAsync().ConfigureAwait(false);

            TransportLogMessages.DisconnectedHost(address);
        }

        MessagingFactory MessagingFactoryContext.MessagingFactory => _messagingFactory;

        public Uri ServiceAddress => _messagingFactory.Address;
    }
}
