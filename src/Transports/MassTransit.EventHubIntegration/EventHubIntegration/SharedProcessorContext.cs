namespace MassTransit.EventHubIntegration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Messaging.EventHubs;
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

        public event Func<ProcessErrorEventArgs, Task> ProcessError
        {
            add => _context.ProcessError += value;
            remove => _context.ProcessError -= value;
        }

        public EventProcessorClient CreateClient(Func<ProcessErrorEventArgs, Task> onError)
        {
            return _context.CreateClient(onError);
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

        public ValueTask DisposeAsync()
        {
            return _context.DisposeAsync();
        }

        public Task Push(ProcessEventArgs partition, Func<Task> method, CancellationToken cancellationToken = default)
        {
            return _context.Push(partition, method, cancellationToken);
        }

        public Task Run(ProcessEventArgs partition, Func<Task> method, CancellationToken cancellationToken = default)
        {
            return _context.Run(partition, method, cancellationToken);
        }
    }
}
