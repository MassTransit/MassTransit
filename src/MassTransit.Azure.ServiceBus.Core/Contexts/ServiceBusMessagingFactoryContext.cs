namespace MassTransit.Azure.ServiceBus.Core.Contexts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using GreenPipes.Payloads;


    public class ServiceBusMessagingFactoryContext :
        BasePipeContext,
        MessagingFactoryContext,
        IAsyncDisposable
    {
        readonly MessagingFactory _messagingFactory;

        public ServiceBusMessagingFactoryContext(MessagingFactory messagingFactory, CancellationToken cancellationToken)
            : base(new PayloadCache(), cancellationToken)
        {
            _messagingFactory = messagingFactory;
        }

        public async Task DisposeAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            await _messagingFactory.CloseAsync().ConfigureAwait(false);

            LogContext.Debug?.Log("Closed messaging factory: {Host}", _messagingFactory.Address);
        }

        MessagingFactory MessagingFactoryContext.MessagingFactory => _messagingFactory;

        public Uri ServiceAddress => _messagingFactory.Address;
    }
}
