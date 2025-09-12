namespace MassTransit.Middleware.InMemoryOutbox
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Batching;
    using Context;
    using Util;


    public class InMemoryOutboxConsumeContext :
        ConsumeContextProxy,
        OutboxContext
    {
        readonly TaskCompletionSource<InMemoryOutboxConsumeContext> _clearToSend;
        readonly InMemoryOutboxDeferredMethodCollection _deferredMethods;
        readonly InMemoryOutboxMessageSchedulerContext _outboxSchedulerContext;

        protected InMemoryOutboxConsumeContext(ConsumeContext context)
            : base(context)
        {
            CapturedContext = context;

            var outboxReceiveContext = new InMemoryOutboxReceiveContext(this, context.ReceiveContext);

            ReceiveContext = outboxReceiveContext;
            PublishEndpointProvider = outboxReceiveContext.PublishEndpointProvider;

            _clearToSend = TaskUtil.GetTask<InMemoryOutboxConsumeContext>();

            _deferredMethods = new InMemoryOutboxDeferredMethodCollection(_clearToSend.Task);

            if (context.TryGetPayload(out MessageSchedulerContext schedulerContext))
            {
                _outboxSchedulerContext = (InMemoryOutboxMessageSchedulerContext)context.AddOrUpdatePayload<MessageSchedulerContext>(
                    () => new InMemoryOutboxMessageSchedulerContext(context, schedulerContext.SchedulerFactory, _clearToSend.Task),
                    existing => new InMemoryOutboxMessageSchedulerContext(context, existing.SchedulerFactory, _clearToSend.Task));
            }
        }

        public ConsumeContext CapturedContext { get; }

        public Task ClearToSend => _clearToSend.Task;

        public Task Add(Func<Task> method)
        {
            return _deferredMethods.Add(method);
        }

        public virtual async Task ExecutePendingActions(bool concurrentMessageDelivery)
        {
            _clearToSend.TrySetResult(this);

            await _deferredMethods.Execute(concurrentMessageDelivery).ConfigureAwait(false);

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

        public virtual async Task DiscardPendingActions()
        {
            await _deferredMethods.Discard().ConfigureAwait(false);

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

        public void Method1()
        {
        }

        public void Method2()
        {
        }

        public void Method3()
        {
        }


        public class Batch :
            InMemoryOutboxConsumeContext,
            ConsumeContext<Batch<T>>
        {
            readonly MessageBatch<T> _batch;
            readonly List<InMemoryOutboxConsumeContext<T>> _messages;

            public Batch(ConsumeContext<Batch<T>> context)
                : base(context)
            {
                Batch<T> batch = context.Message;
                _messages = batch.Select(x => new InMemoryOutboxConsumeContext<T>(x)).ToList();
                _batch = new MessageBatch<T>(batch.FirstMessageReceived, batch.LastMessageReceived, batch.Mode, _messages);
            }

            public Batch<T> Message => _batch;

            public Task NotifyConsumed(TimeSpan duration, string consumerType)
            {
                return NotifyConsumed(this, duration, consumerType);
            }

            public Task NotifyFaulted(TimeSpan duration, string consumerType, Exception exception)
            {
                return NotifyFaulted(this, duration, consumerType, exception);
            }

            public override async Task ExecutePendingActions(bool concurrentMessageDelivery)
            {
                await base.ExecutePendingActions(concurrentMessageDelivery).ConfigureAwait(false);

                await Task.WhenAll(_messages.Select(x => x.ExecutePendingActions(concurrentMessageDelivery))).ConfigureAwait(false);
            }

            public override async Task DiscardPendingActions()
            {
                await base.DiscardPendingActions().ConfigureAwait(false);

                await Task.WhenAll(_messages.Select(x => x.DiscardPendingActions())).ConfigureAwait(false);
            }
        }
    }
}
