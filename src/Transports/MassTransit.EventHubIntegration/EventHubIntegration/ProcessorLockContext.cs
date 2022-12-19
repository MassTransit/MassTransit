namespace MassTransit.EventHubIntegration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Messaging.EventHubs.Processor;
    using Checkpoints;
    using MassTransit.Configuration;
    using Util;


    public class ProcessorLockContext :
        IProcessorLockContext
    {
        readonly SingleThreadedDictionary<string, PartitionCheckpointData> _data = new SingleThreadedDictionary<string, PartitionCheckpointData>();
        readonly IHostConfiguration _hostConfiguration;
        readonly ReceiveSettings _receiveSettings;

        public ProcessorLockContext(IHostConfiguration hostConfiguration, ReceiveSettings receiveSettings)
        {
            _hostConfiguration = hostConfiguration;
            _receiveSettings = receiveSettings;
        }

        public async Task Pending(ProcessEventArgs eventArgs)
        {
            LogContext.SetCurrentIfNull(_hostConfiguration.ReceiveLogContext);

            if (_data.TryGetValue(eventArgs.Partition.PartitionId, out var data))
                await data.Pending(eventArgs).ConfigureAwait(false);
        }

        public Task Faulted(ProcessEventArgs eventArgs, Exception exception)
        {
            LogContext.SetCurrentIfNull(_hostConfiguration.ReceiveLogContext);

            if (_data.TryGetValue(eventArgs.Partition.PartitionId, out var data))
                data.Faulted(eventArgs, exception);

            return Task.CompletedTask;
        }

        public Task Complete(ProcessEventArgs eventArgs)
        {
            LogContext.SetCurrentIfNull(_hostConfiguration.ReceiveLogContext);

            if (_data.TryGetValue(eventArgs.Partition.PartitionId, out var data))
                data.Complete(eventArgs);

            return Task.CompletedTask;
        }

        public async Task OnPartitionInitializing(PartitionInitializingEventArgs eventArgs)
        {
            LogContext.SetCurrentIfNull(_hostConfiguration.ReceiveLogContext);

            if (!_data.TryAdd(eventArgs.PartitionId, _ => new PartitionCheckpointData(_receiveSettings)))
                return;

            LogContext.Info?.Log("Partition: {PartitionId} was initialized", eventArgs.PartitionId);
        }

        public async Task OnPartitionClosing(PartitionClosingEventArgs eventArgs)
        {
            LogContext.SetCurrentIfNull(_hostConfiguration.ReceiveLogContext);

            if (!_data.TryGetValue(eventArgs.PartitionId, out var data))
                return;

            await data.Close(eventArgs).ConfigureAwait(false);
            _data.TryRemove(eventArgs.PartitionId, out _);
        }


        sealed class PartitionCheckpointData
        {
            readonly ChannelExecutor _executor;
            readonly PendingConfirmationCollection _pending;
            readonly ICheckpointer _checkpointer;
            readonly CancellationTokenSource _cancellationTokenSource;

            public PartitionCheckpointData(ReceiveSettings settings)
            {
                _cancellationTokenSource = new CancellationTokenSource();
                _executor = new ChannelExecutor(settings.PrefetchCount, settings.ConcurrentMessageLimit);
                _checkpointer = new BatchCheckpointer(settings, _cancellationTokenSource.Token);
                _pending = new PendingConfirmationCollection(settings.EventHubName);
            }

            public Task Pending(ProcessEventArgs eventArgs)
            {
                var pendingConfirmation = _pending.Add(eventArgs);
                return _checkpointer.Pending(pendingConfirmation);
            }

            public void Complete(ProcessEventArgs eventArgs)
            {
                _pending.Complete(eventArgs.Data.Offset);
            }

            public void Faulted(ProcessEventArgs eventArgs, Exception exception)
            {
                _pending.Faulted(eventArgs.Data.Offset, exception);
            }

            public async Task Close(PartitionClosingEventArgs args)
            {
                if (args.Reason != ProcessingStoppedReason.Shutdown)
                    _cancellationTokenSource.Cancel();

                await _executor.DisposeAsync().ConfigureAwait(false);
                await _checkpointer.DisposeAsync().ConfigureAwait(false);

                LogContext.Info?.Log("Partition: {PartitionId} was closed, reason: {Reason}", args.PartitionId, args.Reason);

                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
            }

            public Task Push(Func<Task> method)
            {
                return _executor.Push(method, _cancellationTokenSource.Token);
            }

            public Task Run(Func<Task> method)
            {
                return _executor.Run(method, _cancellationTokenSource.Token);
            }
        }


        public async ValueTask DisposeAsync()
        {
        }

        public Task Push(ProcessEventArgs partition, Func<Task> method, CancellationToken cancellationToken = default)
        {
            return _data[partition.Partition.PartitionId].Push(method);
        }

        public Task Run(ProcessEventArgs partition, Func<Task> method, CancellationToken cancellationToken = default)
        {
            return _data[partition.Partition.PartitionId].Run(method);
        }
    }
}
