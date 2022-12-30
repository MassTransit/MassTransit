namespace MassTransit.EventHubIntegration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Messaging.EventHubs;
    using Azure.Messaging.EventHubs.Processor;
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

        public event Func<ProcessErrorEventArgs, Task> ProcessError
        {
            add => _context.ProcessError += value;
            remove => _context.ProcessError -= value;
        }

        public EventProcessorClient GetClient(ProcessorClientBuilderContext context,
            Func<ProcessErrorEventArgs, Task> errorHandler)
        {
            return _context.GetClient(context, errorHandler);
        }
    }
}
