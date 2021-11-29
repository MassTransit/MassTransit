namespace MassTransit.EventHubIntegration
{
    using System;
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

            _data.TryAdd(eventArgs.PartitionId, _ => new PartitionCheckpointData(_receiveSettings));
            LogContext.Info?.Log("Partition: {PartitionId} was initialized", eventArgs.PartitionId);
        }

        public async Task OnPartitionClosing(PartitionClosingEventArgs eventArgs)
        {
            LogContext.SetCurrentIfNull(_hostConfiguration.ReceiveLogContext);

            if (_data.TryRemove(eventArgs.PartitionId, out var data))
                await data.Close(eventArgs).ConfigureAwait(false);
        }


        sealed class PartitionCheckpointData
        {
            readonly ChannelExecutor _executor;
            readonly PendingConfirmationCollection _pending;
            readonly ICheckpointer _receiver;

            public PartitionCheckpointData(ReceiveSettings settings)
            {
                _executor = new ChannelExecutor(1);
                _receiver = new BatchCheckpointer(_executor, settings);
                _pending = new PendingConfirmationCollection(settings.EventHubName);
            }

            public Task Pending(ProcessEventArgs eventArgs)
            {
                var pendingConfirmation = _pending.Add(eventArgs);
                return _receiver.Pending(pendingConfirmation);
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
                await _receiver.Close(args.Reason).ConfigureAwait(false);
                await _executor.DisposeAsync().ConfigureAwait(false);

                LogContext.Info?.Log("Partition: {PartitionId} was closed, reason: {Reason}", args.PartitionId, args.Reason);
            }
        }
    }
}
