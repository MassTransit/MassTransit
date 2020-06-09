namespace MassTransit.EventHubIntegration.Contexts
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Azure.Messaging.EventHubs;
    using Azure.Messaging.EventHubs.Processor;
    using Context;
    using Util;


    public class ProcessorLockContext :
        IProcessorLockContext
    {
        readonly SingleThreadedDictionary<string, PartitionCheckpointData> _data = new SingleThreadedDictionary<string, PartitionCheckpointData>();
        readonly ILogContext _logContext;
        readonly ushort _maxCount;
        readonly TimeSpan _timeout;

        public ProcessorLockContext(EventProcessorClient client, ILogContext logContext, TimeSpan timeout, ushort maxCount)
        {
            _logContext = logContext;
            _timeout = timeout;
            _maxCount = maxCount;

            client.PartitionInitializingAsync += OnPartitionInitializing;
            client.PartitionClosingAsync += OnPartitionClosing;
        }

        public Task Complete(ProcessEventArgs eventArgs)
        {
            LogContext.SetCurrentIfNull(_logContext);

            return _data.TryGetValue(eventArgs.Partition.PartitionId, out var data)
                ? data.TryCheckpointAsync(eventArgs)
                : TaskUtil.Completed;
        }

        Task OnPartitionInitializing(PartitionInitializingEventArgs eventArgs)
        {
            LogContext.SetCurrentIfNull(_logContext);

            _data.TryAdd(eventArgs.PartitionId, _ => new PartitionCheckpointData(_timeout, _maxCount));
            LogContext.Info?.Log("Partition: {PartitionId} was initialized", eventArgs.PartitionId);
            return TaskUtil.Completed;
        }

        Task OnPartitionClosing(PartitionClosingEventArgs eventArgs)
        {
            LogContext.SetCurrentIfNull(_logContext);

            return _data.TryRemove(eventArgs.PartitionId, out var data) ? data.Close(eventArgs) : TaskUtil.Completed;
        }


        sealed class PartitionCheckpointData
        {
            readonly ushort _maxCount;
            readonly TimeSpan _timeout;
            readonly Stopwatch _timer;
            ProcessEventArgs _current;
            ushort _processed;

            public PartitionCheckpointData(TimeSpan timeout, ushort maxCount)
            {
                _timeout = timeout;
                _maxCount = maxCount;

                _processed = 0;
                _timer = Stopwatch.StartNew();
            }

            public async Task<bool> TryCheckpointAsync(ProcessEventArgs args)
            {
                void Reset()
                {
                    _current = default;
                    _processed = 0;
                    _timer.Restart();
                }

                _current = args;
                _processed += 1;

                if (_processed < _maxCount && _timer.Elapsed < _timeout)
                    return false;

                LogContext.Debug?.Log("Partition: {PartitionId} updating checkpoint with offset: {Offset}", _current.Partition.PartitionId,
                    _current.Data.Offset);
                await _current.UpdateCheckpointAsync().ConfigureAwait(false);
                Reset();
                return true;
            }

            public async Task Close(PartitionClosingEventArgs args)
            {
                try
                {
                    if (!_current.HasEvent || args.Reason != ProcessingStoppedReason.Shutdown)
                        return;

                    LogContext.Debug?.Log("Partition: {PartitionId} updating checkpoint with offset: {Offset}", _current.Partition.PartitionId,
                        _current.Data.Offset);
                    await _current.UpdateCheckpointAsync().ConfigureAwait(false);
                }
                finally
                {
                    _timer.Stop();
                    _current = default;
                    LogContext.Info?.Log("Partition: {PartitionId} was closed, reason: {Reason}", args.PartitionId, args.Reason);
                }
            }
        }
    }
}
