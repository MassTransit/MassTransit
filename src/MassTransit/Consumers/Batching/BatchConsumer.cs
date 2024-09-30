namespace MassTransit.Batching
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Logging;
    using Util;


    public class BatchConsumer<TMessage> :
        IConsumer<TMessage>
        where TMessage : class
    {
        readonly TaskCompletionSource<DateTime> _completed;
        readonly IPipe<ConsumeContext<Batch<TMessage>>> _consumerPipe;
        readonly TaskExecutor _dispatcher;
        readonly TaskExecutor _executor;
        readonly DateTime _firstMessage;
        readonly Dictionary<Guid, BatchEntry> _messages;
        readonly BatchOptions _options;
        readonly Timer _timer;
        Activity _currentActivity;
        DateTime _lastMessage;
        ILogContext _logContext;

        public BatchConsumer(BatchOptions options, TaskExecutor executor, TaskExecutor dispatcher, IPipe<ConsumeContext<Batch<TMessage>>> consumerPipe)
        {
            _executor = executor;
            _consumerPipe = consumerPipe;
            _dispatcher = dispatcher;
            _messages = new Dictionary<Guid, BatchEntry>();
            _completed = TaskUtil.GetTask<DateTime>();
            _firstMessage = DateTime.UtcNow;
            _options = options;

            _timer = new Timer(TimeLimitExpired, null, _options.TimeLimit, TimeSpan.FromMilliseconds(-1));
        }

        public bool IsCompleted { get; private set; }

        public async Task Consume(ConsumeContext<TMessage> context)
        {
            try
            {
                await _completed.Task.ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (context.CancellationToken.IsCancellationRequested)
            {
                throw new OperationCanceledException(context.CancellationToken);
            }
            catch
            {
                // if this message was marked as successfully delivered, do not fault it
                if (context.ReceiveContext.IsDelivered)
                    return;

                throw;
            }
        }

        void TimeLimitExpired(object state)
        {
            Task.Run(() => _executor.Push(() =>
            {
                if (IsCompleted)
                    return Task.CompletedTask;

                IsCompleted = true;

                if (_messages.Count <= 0)
                    return Task.CompletedTask;

                List<ConsumeContext<TMessage>> messages = GetMessageBatchInOrder();

                return _dispatcher.Push(() => Deliver(messages[messages.Count - 1], messages, BatchCompletionMode.Time));
            }));
        }

        public Task Add(ConsumeContext<TMessage> context, Activity currentActivity)
        {
            _logContext ??= LogContext.Current;
            if (currentActivity != null)
                _currentActivity = currentActivity;

            var messageId = context.MessageId ?? NewId.NextGuid();

            ulong? sequenceNumber = context.ReceiveContext.TryGetPayload<ITransportSequenceNumber>(out var payload) ? payload.SequenceNumber : null;            
            ulong sentTimeAsSequenceFallback() => (ulong)(context.SentTime ?? context.ReceiveContext.GetSentTime() ?? DateTime.UtcNow).Ticks;

            var batchEntry = new BatchEntry(
                context,
                sequenceNumber ?? sentTimeAsSequenceFallback(),
                () => RemoveCanceledMessage(messageId));

            if (!_messages.ContainsKey(messageId))
                _messages.Add(messageId, batchEntry);
            else
                batchEntry.Unregister();

            if (_options.TimeLimitStart == BatchTimeLimitStart.FromLast)
                _timer.Change(_options.TimeLimit, TimeSpan.FromMilliseconds(-1));

            _lastMessage = DateTime.UtcNow;

            if (IsReadyToDeliver(context))
            {
                IsCompleted = true;

                List<ConsumeContext<TMessage>> messageList = GetMessageBatchInOrder();

                return messageList.Count == 0
                    ? Task.CompletedTask
                    : _dispatcher.Push(() => Deliver(context, messageList, BatchCompletionMode.Size));
            }

            return Task.CompletedTask;
        }

        void RemoveCanceledMessage(Guid messageId)
        {
            Task.Run(() => _executor.Push(() =>
            {
                if (IsCompleted)
                    return Task.CompletedTask;

                if (_messages.TryGetValue(messageId, out var batchEntry))
                {
                    batchEntry.Unregister();

                    _messages.Remove(messageId);

                    if (_messages.Count == 0)
                    {
                        IsCompleted = true;

                        _completed.TrySetCanceled();
                    }
                }

                return Task.CompletedTask;
            }));
        }

        bool IsReadyToDeliver(ConsumeContext context)
        {
            if (context.GetRetryAttempt() > 0)
                return true;

            return _messages.Count == _options.MessageLimit;
        }

        public Task ForceComplete()
        {
            IsCompleted = true;

            List<ConsumeContext<TMessage>> consumeContexts = GetMessageBatchInOrder();
            return consumeContexts.Count == 0
                ? Task.CompletedTask
                : _dispatcher.Push(() => Deliver(consumeContexts[consumeContexts.Count - 1], consumeContexts, BatchCompletionMode.Forced));
        }

        async Task Deliver(ConsumeContext context, IReadOnlyList<ConsumeContext<TMessage>> messages, BatchCompletionMode batchCompletionMode)
        {
            _timer.Dispose();

            foreach (var batchEntry in _messages.Values)
                batchEntry.Unregister();

            LogContext.SetCurrentIfNull(_logContext);

            Activity.Current = _currentActivity;

            Batch<TMessage> batch = new MessageBatch<TMessage>(_firstMessage, _lastMessage, batchCompletionMode, messages);

            ConsumeContext<Batch<TMessage>> batchConsumeContext = new BatchConsumeContext<TMessage>(context, batch);

            try
            {
                await _consumerPipe.Send(batchConsumeContext).ConfigureAwait(false);

                _completed.TrySetResult(DateTime.UtcNow);
            }
            catch (OperationCanceledException exception) when (exception.CancellationToken == context.CancellationToken)
            {
                _completed.TrySetCanceled();
            }
            catch (Exception exception)
            {
                if (batchConsumeContext.TryGetPayload(out RetryContext<ConsumeContext<Batch<TMessage>>> retryContext))
                {
                    for (var i = 0; i < messages.Count; i++)
                        messages[i].GetOrAddPayload(() => retryContext);
                }

                _completed.TrySetException(exception);
            }
        }

        List<ConsumeContext<TMessage>> GetMessageBatchInOrder()
        {
            return _messages.Values.OrderBy(x => x.Index).Select(x => x.Context).ToList();
        }


        readonly struct BatchEntry
        {
            public readonly ConsumeContext<TMessage> Context;
            public readonly ulong Index;
            readonly CancellationTokenRegistration _registration;

            public BatchEntry(ConsumeContext<TMessage> context, ulong index, Action canceled)
            {
                Context = context;
                Index = index;

                if (context.CancellationToken.CanBeCanceled)
                    _registration = context.CancellationToken.Register(() => canceled());
            }

            public void Unregister()
            {
                _registration.Dispose();
            }
        }
    }
}
