namespace MassTransit.AzureServiceBusTransport.Contexts
{
    using System;
    using System.Threading;
    using GreenPipes;
    using Microsoft.ServiceBus.Messaging;


    public class SharedMessagingFactoryContext :
        ProxyPipeContext,
        MessagingFactoryContext
    {
        readonly MessagingFactoryContext _context;

        public SharedMessagingFactoryContext(MessagingFactoryContext context, CancellationToken cancellationToken)
            : base(context)
        {
            _context = context;
            CancellationToken = cancellationToken;
        }

        public override CancellationToken CancellationToken { get; }

        MessagingFactory MessagingFactoryContext.MessagingFactory => _context.MessagingFactory;
        Uri MessagingFactoryContext.ServiceAddress => _context.ServiceAddress;
    }
}
