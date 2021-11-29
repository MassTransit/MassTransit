namespace MassTransit.Middleware.InMemoryOutbox
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Context;
    using Util;


    public class InMemoryOutboxConsumeContext :
        ConsumeContextProxy,
        OutboxContext
    {
        readonly TaskCompletionSource<InMemoryOutboxConsumeContext> _clearToSend;
        readonly InMemoryOutboxMessageSchedulerContext _outboxSchedulerContext;
        readonly List<Func<Task>> _pendingActions;

        public InMemoryOutboxConsumeContext(ConsumeContext context)
            : base(context)
        {
            CapturedContext = context;

            var outboxReceiveContext = new InMemoryOutboxReceiveContext(this, context.ReceiveContext);

            ReceiveContext = outboxReceiveContext;
            PublishEndpointProvider = outboxReceiveContext.PublishEndpointProvider;

            _pendingActions = new List<Func<Task>>();
            _clearToSend = TaskUtil.GetTask<InMemoryOutboxConsumeContext>();

            if (context.TryGetPayload(out MessageSchedulerContext schedulerContext))
            {
                _outboxSchedulerContext = new InMemoryOutboxMessageSchedulerContext(schedulerContext, _clearToSend.Task);
                context.AddOrUpdatePayload(() => _outboxSchedulerContext, _ => _outboxSchedulerContext);
            }
        }

        public ConsumeContext CapturedContext { get; }

        public Task ClearToSend => _clearToSend.Task;

        public Task Add(Func<Task> method)
        {
            if (_clearToSend.Task.IsCompleted)
                return method();

            lock (_pendingActions)
            {
                _pendingActions.Add(method);

                return Task.CompletedTask;
            }
        }

        public async Task ExecutePendingActions(bool concurrentMessageDelivery)
        {
            _clearToSend.TrySetResult(this);

            Func<Task>[] pendingActions;
            lock (_pendingActions)
                pendingActions = _pendingActions.ToArray();

            if (pendingActions.Length > 0)
            {
                if (concurrentMessageDelivery)
                {
                    var collection = new PendingTaskCollection(pendingActions.Length);

                    collection.Add(pendingActions.Select(action => action()));

                    await collection.Completed().ConfigureAwait(false);
                }
                else
                {
                    foreach (Func<Task> action in pendingActions)
                    {
                        var task = action();
                        if (task != null)
                            await task.ConfigureAwait(false);
                    }
                }
            }

            if (_outboxSchedulerContext != null)
            {
                try
                {
                    await _outboxSchedulerContext.ExecutePendingActions().ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    LogContext.Warning?.Log(e, "One or more messages could not be unscheduled.", e);
                }
            }
        }

        public async Task DiscardPendingActions()
        {
            lock (_pendingActions)
                _pendingActions.Clear();

            if (_outboxSchedulerContext != null)
            {
                try
                {
                    await _outboxSchedulerContext.CancelAllScheduledMessages().ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    LogContext.Warning?.Log(e, "One or more messages could not be unscheduled.", e);
                }
            }
        }
    }


    public class InMemoryOutboxConsumeContext<T> :
        InMemoryOutboxConsumeContext,
        ConsumeContext<T>
        where T : class
    {
        readonly ConsumeContext<T> _context;

        public InMemoryOutboxConsumeContext(ConsumeContext<T> context)
            : base(context)
        {
            _context = context;
        }

        public T Message => _context.Message;

        public virtual Task NotifyConsumed(TimeSpan duration, string consumerType)
        {
            return NotifyConsumed(this, duration, consumerType);
        }

        public virtual Task NotifyFaulted(TimeSpan duration, string consumerType, Exception exception)
        {
            return NotifyFaulted(this, duration, consumerType, exception);
        }
    }
}
