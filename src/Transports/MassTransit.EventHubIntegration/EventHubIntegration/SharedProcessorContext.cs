namespace MassTransit.EventHubIntegration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Messaging.EventHubs.Processor;
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

        public event Func<ProcessEventArgs, Task> ProcessEvent
        {
            add => _context.ProcessEvent += value;
            remove => _context.ProcessEvent -= value;
        }

        public event Func<ProcessErrorEventArgs, Task> ProcessError
        {
            add => _context.ProcessError += value;
            remove => _context.ProcessError -= value;
        }

        public ReceiveSettings ReceiveSettings => _context.ReceiveSettings;

        public Task<bool> CreateBlobIfNotExistsAsync(CancellationToken cancellationToken = default)
        {
            return _context.CreateBlobIfNotExistsAsync(cancellationToken);
        }

        public Task StartProcessingAsync(CancellationToken cancellationToken = default)
        {
            return _context.StartProcessingAsync(cancellationToken);
        }

        public Task StopProcessingAsync(CancellationToken cancellationToken = default)
        {
            return _context.StopProcessingAsync(cancellationToken);
        }

        public Task Pending(ProcessEventArgs eventArgs)
        {
            return _context.Pending(eventArgs);
        }

        public Task Faulted(ProcessEventArgs eventArgs, Exception exception)
        {
            return _context.Faulted(eventArgs, exception);
        }

        public Task Complete(ProcessEventArgs eventArgs)
        {
            return _context.Complete(eventArgs);
        }
    }
}
