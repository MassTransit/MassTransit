namespace MassTransit.EventHubIntegration.Contexts
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Messaging.EventHubs.Processor;
    using Context;
    using Util;


    public class ProcessorLockContext :
        IProcessorLockContext
    {
        readonly Dictionary<string, PartitionData> _data = new Dictionary<string, PartitionData>();
        readonly ILogContext _logContext;
        readonly ushort _maxCount;
        readonly TimeSpan _timeout;

        public ProcessorLockContext(ILogContext logContext, TimeSpan timeout, ushort maxCount)
        {
            _logContext = logContext;
            _timeout = timeout;
            _maxCount = maxCount;
        }

        public Task Complete(ProcessEventArgs eventArgs, CancellationToken cancellationToken)
        {
            return _data.TryGetValue(eventArgs.Partition.PartitionId, out var data)
                ? data.TryCheckpointAsync(eventArgs, cancellationToken)
                : TaskUtil.Completed;
        }

        public Task OnPartitionInitializing(PartitionInitializingEventArgs eventArgs)
        {
            if (_data.TryGetValue(eventArgs.PartitionId, out _))
                return TaskUtil.Completed;

            _data[eventArgs.PartitionId] = new PartitionData(_logContext, _timeout, _maxCount);
            return TaskUtil.Completed;
        }

        public Task OnPartitionClosing(PartitionClosingEventArgs eventArgs)
        {
            if (!_data.TryGetValue(eventArgs.PartitionId, out var data))
                return TaskUtil.Completed;

            _data.Remove(eventArgs.PartitionId);
            return data.CheckpointAsync(eventArgs.CancellationToken);
        }


        sealed class PartitionData
        {
            readonly SemaphoreSlim _lock;
            readonly ILogContext _logContext;
            readonly ushort _maxCount;
            readonly TimeSpan _timeout;
            readonly Stopwatch _timer;
            ProcessEventArgs _current;
            ushort _processed;

            public PartitionData(ILogContext logContext, TimeSpan timeout, ushort maxCount)
            {
                _logContext = logContext;
                _timeout = timeout;
                _maxCount = maxCount;

                _processed = 0;
                _lock = new SemaphoreSlim(1);
                _timer = Stopwatch.StartNew();
            }

            public async Task TryCheckpointAsync(ProcessEventArgs args, CancellationToken cancellationToken)
            {
                LogContext.SetCurrentIfNull(_logContext);
                try
                {
                    void Reset()
                    {
                        _current = default;
                        _processed = 0;
                        _timer.Restart();
                    }

                    await _lock.WaitAsync(args.CancellationToken).ConfigureAwait(false);

                    _processed += 1;
                    _current = args;

                    if (_timer.Elapsed >= _timeout || _processed >= _maxCount)
                    {
                        LogContext.Info?.Log("Partition: {PartitionId} updating checkpoint", _current.Partition.PartitionId);
                        await _current.UpdateCheckpointAsync(cancellationToken).ConfigureAwait(false);
                        Reset();
                    }
                }
                catch (OperationCanceledException e) when (e.CancellationToken == args.CancellationToken)
                {
                }
                finally
                {
                    _lock.Release();
                }
            }

            public async Task CheckpointAsync(CancellationToken cancellationToken)
            {
                LogContext.SetCurrentIfNull(_logContext);
                try
                {
                    await _lock.WaitAsync(cancellationToken).ConfigureAwait(false);

                    if (_current.HasEvent)
                    {
                        LogContext.Info?.Log("Partition: {PartitionId} updating checkpoint", _current.Partition.PartitionId);
                        await _current.UpdateCheckpointAsync(cancellationToken).ConfigureAwait(false);
                    }
                }
                catch (OperationCanceledException e) when (e.CancellationToken == cancellationToken)
                {
                }
                finally
                {
                    _lock.Release();
                    _timer.Stop();
                    _lock.Dispose();
                    _current = default;
                }
            }
        }
    }
}
