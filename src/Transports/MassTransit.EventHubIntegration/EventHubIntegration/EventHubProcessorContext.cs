namespace MassTransit.EventHubIntegration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Messaging.EventHubs;
    using Azure.Messaging.EventHubs.Processor;
    using MassTransit.Configuration;
    using MassTransit.Middleware;


    public class EventHubProcessorContext :
        BasePipeContext,
        ProcessorContext
    {
        readonly IProcessorLockContext _lockContext;
        readonly EventProcessorClient _client;

        public EventHubProcessorContext(IHostConfiguration hostConfiguration, ReceiveSettings receiveSettings,
            EventProcessorClient client, Func<PartitionInitializingEventArgs, Task> partitionInitializingHandler,
            Func<PartitionClosingEventArgs, Task> partitionClosingHandler, CancellationToken cancellationToken)
            : base(cancellationToken)
        {
            var lockContext = new ProcessorLockContext(hostConfiguration, receiveSettings);

            client.PartitionInitializingAsync += async args =>
            {
                await lockContext.OnPartitionInitializing(args).ConfigureAwait(false);
                if (partitionInitializingHandler != null)
                    await partitionInitializingHandler(args).ConfigureAwait(false);
            };
            client.PartitionClosingAsync += async args =>
            {
                if (partitionClosingHandler != null)
                    await partitionClosingHandler(args).ConfigureAwait(false);

                await lockContext.OnPartitionClosing(args).ConfigureAwait(false);
            };

            ReceiveSettings = receiveSettings;
            _lockContext = lockContext;
            _client = client;
        }

        public event Func<ProcessErrorEventArgs, Task> ProcessError;

        public EventProcessorClient GetClient(Func<ProcessErrorEventArgs, Task> onError)
        {
            _client.ProcessErrorAsync += async args =>
            {
                if (ProcessError != null)
                    await ProcessError.Invoke(args).ConfigureAwait(false);

                if (onError != null)
                    await onError.Invoke(args).ConfigureAwait(false);
            };
            return _client;
        }

        public ReceiveSettings ReceiveSettings { get; }

        public Task Pending(ProcessEventArgs eventArgs)
        {
            return _lockContext.Pending(eventArgs);
        }

        public Task Faulted(ProcessEventArgs eventArgs, Exception exception)
        {
            return _lockContext.Faulted(eventArgs, exception);
        }

        public Task Complete(ProcessEventArgs eventArgs)
        {
            return _lockContext.Complete(eventArgs);
        }

        public ValueTask DisposeAsync()
        {
            return _lockContext.DisposeAsync();
        }

        public Task Push(ProcessEventArgs args, Func<Task> method, CancellationToken cancellationToken = default)
        {
            return _lockContext.Push(args, method, cancellationToken);
        }

        public Task Run(ProcessEventArgs args, Func<Task> method, CancellationToken cancellationToken = default)
        {
            return _lockContext.Run(args, method, cancellationToken);
        }
    }
}
