namespace MassTransit.EventHubIntegration.Contexts
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Messaging.EventHubs.Processor;
    using Configuration;
    using Context;
    using Util;


    public class ProcessorLockContext :
        IProcessorLockContext
    {
        readonly SingleThreadedDictionary<string, PartitionCheckpointData> _data = new SingleThreadedDictionary<string, PartitionCheckpointData>();
        readonly IHostConfiguration _hostConfiguration;
        readonly ushort _maxCount;
        readonly TimeSpan _timeout;

        public ProcessorLockContext(IHostConfiguration hostConfiguration, ReceiveSettings receiveSettings)
        {
            _hostConfiguration = hostConfiguration;
            _timeout = receiveSettings.CheckpointInterval;
            _maxCount = receiveSettings.CheckpointMessageCount;
        }

        public Task Complete(ProcessEventArgs eventArgs)
        {
            LogContext.SetCurrentIfNull(_hostConfiguration.ReceiveLogContext);

            return _data.TryGetValue(eventArgs.Partition.PartitionId, out var data)
                ? data.TryCheckpointAsync(eventArgs)
                : TaskUtil.Completed;
        }

        public async Task OnPartitionInitializing(PartitionInitializingEventArgs eventArgs)
        {
            LogContext.SetCurrentIfNull(_hostConfiguration.ReceiveLogContext);

            _data.TryAdd(eventArgs.PartitionId, _ => new PartitionCheckpointData(_timeout, _maxCount));
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
            readonly SemaphoreSlim _lock;
            readonly ushort _maxCount;
            readonly TimeSpan _timeout;
            readonly Stopwatch _timer;
            bool _commitIsRequired;
            ProcessEventArgs _current;
            ushort _processed;

            public PartitionCheckpointData(TimeSpan timeout, ushort maxCount)
            {
                _timeout = timeout;
                _maxCount = maxCount;
                _processed = 0;
                _timer = Stopwatch.StartNew();
                _lock = new SemaphoreSlim(1);
                _commitIsRequired = false;
            }

            public async Task TryCheckpointAsync(ProcessEventArgs args)
            {
                void Reset()
                {
                    _processed = 0;
                    _commitIsRequired = false;
                    _timer.Restart();
                }

                try
                {
                    //if we can't acquire lock with this token, commit will likely to throw
                    await _lock.WaitAsync(args.CancellationToken).ConfigureAwait(false);

                    if (_current.Data == null || _current.Data.Offset < args.Data.Offset)
                    {
                        _current = args;
                        _commitIsRequired = true;
                    }

                    _processed += 1;

                    if (_processed < _maxCount && _timer.Elapsed < _timeout)
                        return;

                    await CommitIfRequired().ConfigureAwait(false);
                    Reset();
                }
                finally
                {
                    _lock.Release();
                }
            }

            public async Task Close(PartitionClosingEventArgs args)
            {
                try
                {
                    //if we can't acquire lock with this token, commit will likely to throw
                    await _lock.WaitAsync(args.CancellationToken).ConfigureAwait(false);

                    if (args.Reason != ProcessingStoppedReason.Shutdown)
                        return;

                    await CommitIfRequired().ConfigureAwait(false);
                }
                finally
                {
                    _timer.Stop();
                    _current = default;
                    _lock.Dispose();

                    LogContext.Info?.Log("Partition: {PartitionId} was closed, reason: {Reason}", args.PartitionId, args.Reason);
                }
            }

            Task CommitIfRequired()
            {
                if (!_commitIsRequired)
                    return TaskUtil.Completed;

                LogContext.Debug?.Log("Partition: {PartitionId} updating checkpoint with offset: {Offset}", _current.Partition.PartitionId,
                    _current.Data.Offset);
                return _current.UpdateCheckpointAsync();
            }
        }
    }
}
