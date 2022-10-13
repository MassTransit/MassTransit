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
        readonly ChannelExecutor _dispatcher;
        readonly ChannelExecutor _executor;
        readonly DateTime _firstMessage;
        readonly int _messageLimit;
        readonly SortedDictionary<Guid, ConsumeContext<TMessage>> _messages;
        readonly TimeSpan _timeLimit;
        readonly BatchTimeLimitStart _timeLimitStart;
        readonly Timer _timer;
        Activity _currentActivity;
        DateTime _lastMessage;
        ILogContext _logContext;

        public BatchConsumer(int messageLimit, TimeSpan timeLimit, BatchTimeLimitStart timeLimitStart, ChannelExecutor executor, ChannelExecutor dispatcher,
            IPipe<ConsumeContext<Batch<TMessage>>> consumerPipe)
        {
            _messageLimit = messageLimit;
            _executor = executor;
            _consumerPipe = consumerPipe;
            _dispatcher = dispatcher;
            _messages = new SortedDictionary<Guid, ConsumeContext<TMessage>>();
            _completed = TaskUtil.GetTask<DateTime>();
            _firstMessage = DateTime.UtcNow;
            _timeLimit = timeLimit;
            _timeLimitStart = timeLimitStart;

            _timer = new Timer(TimeLimitExpired, null, timeLimit, TimeSpan.FromMilliseconds(-1));
        }

        public bool IsCompleted { get; private set; }

        async Task IConsumer<TMessage>.Consume(ConsumeContext<TMessage> context)
        {
            try
            {
                await _completed.Task.ConfigureAwait(false);
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
            _messages.Add(messageId, context);

            if (_timeLimitStart == BatchTimeLimitStart.FromLast)
                _timer.Change(_timeLimit, TimeSpan.FromMilliseconds(-1));

            _lastMessage = DateTime.UtcNow;

            if (IsReadyToDeliver(context))
            {
                IsCompleted = true;

                List<ConsumeContext<TMessage>> messageList = GetMessageBatchInOrder();

                return _dispatcher.Push(() => Deliver(context, messageList, BatchCompletionMode.Size));
            }

            return Task.CompletedTask;
        }

        bool IsReadyToDeliver(ConsumeContext context)
        {
            if (context.GetRetryAttempt() > 0)
                return true;

            return _messages.Count == _messageLimit;
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
            return _messages.Values
                .OrderBy(x => x.SentTime ?? x.MessageId?.ToNewId().Timestamp ?? default)
                .ToList();
        }
    }
}
