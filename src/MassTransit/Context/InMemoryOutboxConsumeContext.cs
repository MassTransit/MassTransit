namespace MassTransit.Context
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Util;


    public class InMemoryOutboxConsumeContext :
        ConsumeContextProxy,
        OutboxContext
    {
        readonly TaskCompletionSource<InMemoryOutboxConsumeContext> _clearToSend;
        readonly List<Func<Task>> _pendingActions;
        readonly InMemoryOutboxMessageSchedulerContext _outboxSchedulerContext;

        public InMemoryOutboxConsumeContext(ConsumeContext context)
            : base(context)
        {
            var outboxReceiveContext = new InMemoryOutboxReceiveContext(this, context.ReceiveContext);

            ReceiveContext = outboxReceiveContext;
            PublishEndpointProvider = outboxReceiveContext.PublishEndpointProvider;

            _pendingActions = new List<Func<Task>>();
            _clearToSend = TaskUtil.GetTask<InMemoryOutboxConsumeContext>();

            if (context.TryGetPayload(out MessageSchedulerContext schedulerContext))
            {
                _outboxSchedulerContext = new InMemoryOutboxMessageSchedulerContext(schedulerContext);
                context.AddOrUpdatePayload(() => _outboxSchedulerContext, _ => _outboxSchedulerContext);
            }
        }

        public Task ClearToSend => _clearToSend.Task;

        public void Add(Func<Task> method)
        {
            lock (_pendingActions)
                _pendingActions.Add(method);
        }

        public async Task ExecutePendingActions()
        {
            _clearToSend.TrySetResult(this);

            Func<Task>[] pendingActions;
            lock (_pendingActions)
                pendingActions = _pendingActions.ToArray();

            foreach (var action in pendingActions)
            {
                var task = action();
                if (task != null)
                    await task.ConfigureAwait(false);
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
            return base.NotifyConsumed(this, duration, consumerType);
        }

        public virtual Task NotifyFaulted(TimeSpan duration, string consumerType, Exception exception)
        {
            return base.NotifyFaulted(this, duration, consumerType, exception);
        }
    }
}
