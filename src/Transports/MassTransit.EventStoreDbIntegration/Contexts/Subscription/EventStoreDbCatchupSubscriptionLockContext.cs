using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;
using MassTransit.Configuration;
using MassTransit.Context;

namespace MassTransit.EventStoreDbIntegration.Contexts
{
    public sealed class EventStoreDbCatchupSubscriptionLockContext :
        ISubscriptionLockContext
    {
        readonly SubscriptionCheckpointData _data;
        readonly IHostConfiguration _hostConfiguration;
        readonly ushort _maxCount;
        readonly TimeSpan _timeout;
        readonly ICheckpointStore _checkpointStore;
        
        public EventStoreDbCatchupSubscriptionLockContext(IHostConfiguration hostConfiguration, SubscriptionSettings subscriptionSettings,
            ICheckpointStore checkpointStore)
        {
            _hostConfiguration = hostConfiguration;
            _timeout = subscriptionSettings.CheckpointInterval;
            _maxCount = subscriptionSettings.CheckpointMessageCount;
            _checkpointStore = checkpointStore;
            _data = new SubscriptionCheckpointData(_checkpointStore, subscriptionSettings.SubscriptionName, subscriptionSettings.StreamName,
                _timeout, _maxCount);
        }

        public async Task Complete(ResolvedEvent resolvedEvent)
        {
            LogContext.SetCurrentIfNull(_hostConfiguration.ReceiveLogContext);

            _ = await _data.TryCheckpointAsync(resolvedEvent).ConfigureAwait(false);
        }

        public async Task CheckpointReached(StreamSubscription streamSubscription, Position position, CancellationToken cancellationToken)
        {
            LogContext.SetCurrentIfNull(_hostConfiguration.ReceiveLogContext);

            await _data.TryCheckpointAsync(position).ConfigureAwait(false);
        }

        public async Task OnSubscriptionDropped(SubscriptionDroppedReason reason)
        {
            LogContext.SetCurrentIfNull(_hostConfiguration.ReceiveLogContext);

            await _data.Close(reason).ConfigureAwait(false);
        }

        sealed class SubscriptionCheckpointData
        {
            readonly ICheckpointStore _checkpointStore;
            readonly string _subscriptionName;
            readonly StreamName _streamName;
            readonly ushort _maxCount;
            readonly TimeSpan _timeout;
            readonly Stopwatch _timer;

            bool _commitIsRequired; 
            ulong? _current;
            ushort _processed;

            public SubscriptionCheckpointData(ICheckpointStore checkpointStore, string subscriptionName, StreamName streamName,
                TimeSpan timeout, ushort maxCount)
            {
                _checkpointStore = checkpointStore;
                _subscriptionName = subscriptionName;
                _streamName = streamName;
                _timeout = timeout;
                _maxCount = maxCount;
                _processed = 0;
                _timer = Stopwatch.StartNew();
                _commitIsRequired = false;
                _current = null;
            }

            public async Task<ulong?> GetLastCheckpointAsync() => _current ??= await _checkpointStore.GetLastCheckpoint().ConfigureAwait(false);

            public async Task<bool> TryCheckpointAsync(ResolvedEvent resolvedEvent)
            {
                ulong? commitPosition = _streamName.IsAllStream
                    ? resolvedEvent.OriginalPosition?.CommitPosition
                    : resolvedEvent.Event.EventNumber.ToUInt64();

                if (!commitPosition.HasValue || _current >= commitPosition)
                    return false;

                _current = commitPosition;
                _commitIsRequired = true;
                ++_processed;

                return await TryCheckpointAsync().ConfigureAwait(false);
            }

            public async Task<bool> TryCheckpointAsync(Position position)
            {
                if (_current >= position.CommitPosition)
                    return false;

                _current = position.CommitPosition;
                _commitIsRequired = true;
                ++_processed;

                return await TryCheckpointAsync().ConfigureAwait(false);
            }

            async Task<bool> TryCheckpointAsync()
            {
                if (_processed < _maxCount && _timer.Elapsed < _timeout)
                    return false;

                await CommitCheckpoint().ConfigureAwait(false);
                Reset();
                return true;
            }

            public async Task Close(SubscriptionDroppedReason reason)
            {
                try
                {
                    if (_commitIsRequired)
                        await CommitCheckpoint().ConfigureAwait(false);
                }
                finally
                {
                    _timer.Stop();
                    _current = null;

                    LogContext.Info?.Log("Subscription: {SubscriptionName} for stream: {StreamName} was closed, reason: {Reason}",
                        _subscriptionName, _streamName, reason.ToString());
                }
            }

            Task CommitCheckpoint()
            {
                LogContext.Debug?.Log(
                    "Subscription: {StreamName} updating checkpoint with position: {Position}",
                    _streamName, _current);

                return _checkpointStore.StoreCheckpoint(_current);
            }

            void Reset()
            {
                _processed = 0;
                _commitIsRequired = false;
                _timer.Restart();
            }
        }
    }
}
