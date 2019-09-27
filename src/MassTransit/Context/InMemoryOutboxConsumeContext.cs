namespace MassTransit.Context
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using Util;


    public class InMemoryOutboxConsumeContext :
        ConsumeContextProxy,
        OutboxContext
    {
        readonly ConsumeContext _context;
        readonly TaskCompletionSource<InMemoryOutboxConsumeContext> _clearToSend;
        readonly List<Func<Task>> _pendingActions;

        public InMemoryOutboxConsumeContext(ConsumeContext context)
            : base(context)
        {
            _context = context;

            ReceiveContext = new InMemoryOutboxReceiveContext(this, context.ReceiveContext);

            _pendingActions = new List<Func<Task>>();
            _clearToSend = new TaskCompletionSource<InMemoryOutboxConsumeContext>();
        }

        public Task ClearToSend => _clearToSend.Task;

        public void Add(Func<Task> method)
        {
            lock (_pendingActions)
                _pendingActions.Add(method);
        }

        public override ReceiveContext ReceiveContext { get; }

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

        public Task DiscardPendingActions()
        {
            lock (_pendingActions)
                _pendingActions.Clear();

            return TaskUtil.Completed;
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
