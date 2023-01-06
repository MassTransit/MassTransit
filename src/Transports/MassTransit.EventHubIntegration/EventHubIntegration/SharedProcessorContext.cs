namespace MassTransit.EventHubIntegration
{
    using System.Threading;
    using Azure.Messaging.EventHubs;
    using Logging;
    using MassTransit.Middleware;


    public class SharedProcessorContext :
        ProxyPipeContext,
        ProcessorContext
    {
        readonly ProcessorContext _context;

        public SharedProcessorContext(ProcessorContext context, CancellationToken cancellationToken)
            : base(context)
        {
            _context = context;
            CancellationToken = cancellationToken;
        }

        public override CancellationToken CancellationToken { get; }

        public ILogContext LogContext => _context.LogContext;

        public EventProcessorClient GetClient(ProcessorClientBuilderContext context)
        {
            return _context.GetClient(context);
        }
    }
}
