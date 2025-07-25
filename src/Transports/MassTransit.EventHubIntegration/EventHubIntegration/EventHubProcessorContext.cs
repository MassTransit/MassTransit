namespace MassTransit.EventHubIntegration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Messaging.EventHubs;
    using Azure.Messaging.EventHubs.Processor;
    using Logging;
    using MassTransit.Configuration;
    using MassTransit.Middleware;


    public class EventHubProcessorContext :
        BasePipeContext,
        ProcessorContext
    {
        readonly EventProcessorClient _client;
        readonly IHostConfiguration _hostConfiguration;
        readonly Func<PartitionClosingEventArgs, Task> _partitionClosingHandler;
        readonly Func<PartitionInitializingEventArgs, Task> _partitionInitializingHandler;
        Action _releaseClient;

        public EventHubProcessorContext(IHostConfiguration hostConfiguration,
            EventProcessorClient client, Func<PartitionInitializingEventArgs, Task> partitionInitializingHandler,
            Func<PartitionClosingEventArgs, Task> partitionClosingHandler, CancellationToken cancellationToken)
            : base(cancellationToken)
        {
            _hostConfiguration = hostConfiguration;

            _client = client;

            _partitionInitializingHandler = partitionInitializingHandler;
            _partitionClosingHandler = partitionClosingHandler;
        }

        public ILogContext LogContext => _hostConfiguration.ReceiveLogContext;

        public EventProcessorClient GetClient(ProcessorClientBuilderContext context)
        {
            if (_releaseClient != null)
                throw new InvalidOperationException("The client has already been configured and will throw an exception if it is used again");

            Func<PartitionInitializingEventArgs, Task> OnPartitionInitializingAsync()
            {
                return async args =>
                {
                    await context.OnPartitionInitializing(args).ConfigureAwait(false);
                    if (_partitionInitializingHandler != null)
                        await _partitionInitializingHandler(args).ConfigureAwait(false);
                };
            }

            _client.PartitionInitializingAsync += OnPartitionInitializingAsync();

            Func<PartitionClosingEventArgs, Task> OnPartitionClosingAsync()
            {
                return async args =>
                {
                    if (_partitionClosingHandler != null)
                        await _partitionClosingHandler(args).ConfigureAwait(false);

                    await context.OnPartitionClosing(args).ConfigureAwait(false);
                };
            }

            _client.PartitionClosingAsync += OnPartitionClosingAsync();

            _releaseClient = () =>
            {
                _client.PartitionClosingAsync -= OnPartitionClosingAsync();
                _client.PartitionInitializingAsync -= OnPartitionInitializingAsync();

                _releaseClient = null;
            };

            return _client;
        }

        public void ReleaseClient(ProcessorClientBuilderContext processorLockContext)
        {
            _releaseClient?.Invoke();
        }
    }
}
