namespace MassTransit.Azure.ServiceBus.Core.Contexts
{
    using System;
    using System.Threading;
    using GreenPipes;


    public class SharedMessagingFactoryContext :
        ProxyPipeContext,
        MessagingFactoryContext
    {
        readonly MessagingFactoryContext _context;

        public SharedMessagingFactoryContext(MessagingFactoryContext context, CancellationToken cancellationToken)
            : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            CancellationToken = cancellationToken;
        }

        public override CancellationToken CancellationToken { get; }

        MessagingFactory MessagingFactoryContext.MessagingFactory => _context.MessagingFactory;
        Uri MessagingFactoryContext.ServiceAddress => _context.ServiceAddress;
    }
}
