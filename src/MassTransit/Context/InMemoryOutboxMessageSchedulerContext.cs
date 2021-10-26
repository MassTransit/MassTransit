namespace MassTransit.Context
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using Scheduling;
    using Util;


    public class InMemoryOutboxMessageSchedulerContext :
        MessageSchedulerContext
    {
        readonly List<Func<Task>> _cancelMessages;
        readonly Task _clearToSend;
        readonly MessageSchedulerContext _context;
        readonly object _listLock = new object();
        readonly List<ScheduledMessage> _scheduledMessages;

        public InMemoryOutboxMessageSchedulerContext(MessageSchedulerContext context, Task clearToSend)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _clearToSend = clearToSend;

            _scheduledMessages = new List<ScheduledMessage>();
            _cancelMessages = new List<Func<Task>>();
        }

        async Task<ScheduledMessage<T>> IMessageScheduler.ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, T message,
            CancellationToken cancellationToken)
        {
            ScheduledMessage<T> scheduledMessage =
                await _context.ScheduleSend(destinationAddress, scheduledTime, message, cancellationToken).ConfigureAwait(false);

            AddScheduledMessage(scheduledMessage);

            return scheduledMessage;
        }

        async Task<ScheduledMessage<T>> IMessageScheduler.ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, T message, IPipe<SendContext<T>> pipe,
            CancellationToken cancellationToken)
        {
            ScheduledMessage<T> scheduledMessage =
                await _context.ScheduleSend(destinationAddress, scheduledTime, message, pipe, cancellationToken).ConfigureAwait(false);

            AddScheduledMessage(scheduledMessage);

            return scheduledMessage;
        }

        async Task<ScheduledMessage<T>> IMessageScheduler.ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, T message, IPipe<SendContext> pipe,
            CancellationToken cancellationToken)
        {
            ScheduledMessage<T> scheduledMessage =
                await _context.ScheduleSend(destinationAddress, scheduledTime, message, pipe, cancellationToken).ConfigureAwait(false);

            AddScheduledMessage(scheduledMessage);

            return scheduledMessage;
        }

        async Task<ScheduledMessage> IMessageScheduler.ScheduleSend(Uri destinationAddress, DateTime scheduledTime, object message,
            CancellationToken cancellationToken)
        {
            var scheduledMessage = await _context.ScheduleSend(destinationAddress, scheduledTime, message, cancellationToken).ConfigureAwait(false);

            AddScheduledMessage(scheduledMessage);

            return scheduledMessage;
        }

        async Task<ScheduledMessage> IMessageScheduler.ScheduleSend(Uri destinationAddress, DateTime scheduledTime, object message, Type messageType,
            CancellationToken cancellationToken)
        {
            var scheduledMessage =
                await _context.ScheduleSend(destinationAddress, scheduledTime, message, messageType, cancellationToken).ConfigureAwait(false);

            AddScheduledMessage(scheduledMessage);

            return scheduledMessage;
        }

        async Task<ScheduledMessage> IMessageScheduler.ScheduleSend(Uri destinationAddress, DateTime scheduledTime, object message, IPipe<SendContext> pipe,
            CancellationToken cancellationToken)
        {
            var scheduledMessage = await _context.ScheduleSend(destinationAddress, scheduledTime, message, pipe, cancellationToken).ConfigureAwait(false);

            AddScheduledMessage(scheduledMessage);

            return scheduledMessage;
        }

        async Task<ScheduledMessage> IMessageScheduler.ScheduleSend(Uri destinationAddress, DateTime scheduledTime, object message, Type messageType,
            IPipe<SendContext> pipe, CancellationToken cancellationToken)
        {
            var scheduledMessage = await _context.ScheduleSend(destinationAddress, scheduledTime, message, messageType, pipe, cancellationToken)
                .ConfigureAwait(false);

            AddScheduledMessage(scheduledMessage);

            return scheduledMessage;
        }

        async Task<ScheduledMessage<T>> IMessageScheduler.ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, object values,
            CancellationToken cancellationToken)
        {
            ScheduledMessage<T> scheduledMessage =
                await _context.ScheduleSend<T>(destinationAddress, scheduledTime, values, cancellationToken).ConfigureAwait(false);

            AddScheduledMessage(scheduledMessage);

            return scheduledMessage;
        }

        async Task<ScheduledMessage<T>> IMessageScheduler.ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, object values,
            IPipe<SendContext<T>> pipe,
            CancellationToken cancellationToken)
        {
            ScheduledMessage<T> scheduledMessage =
                await _context.ScheduleSend(destinationAddress, scheduledTime, values, pipe, cancellationToken).ConfigureAwait(false);

            AddScheduledMessage(scheduledMessage);

            return scheduledMessage;
        }

        async Task<ScheduledMessage<T>> IMessageScheduler.ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, object values,
            IPipe<SendContext> pipe,
            CancellationToken cancellationToken)
        {
            ScheduledMessage<T> scheduledMessage =
                await _context.ScheduleSend<T>(destinationAddress, scheduledTime, values, pipe, cancellationToken).ConfigureAwait(false);

            AddScheduledMessage(scheduledMessage);

            return scheduledMessage;
        }

        async Task<ScheduledMessage<T>> MessageSchedulerContext.ScheduleSend<T>(DateTime scheduledTime, T message, CancellationToken cancellationToken)
        {
            ScheduledMessage<T> scheduledMessage = await _context.ScheduleSend(scheduledTime, message, cancellationToken).ConfigureAwait(false);

            AddScheduledMessage(scheduledMessage);

            return scheduledMessage;
        }

        async Task<ScheduledMessage<T>> MessageSchedulerContext.ScheduleSend<T>(DateTime scheduledTime, T message, IPipe<SendContext<T>> pipe,
            CancellationToken cancellationToken)
        {
            ScheduledMessage<T> scheduledMessage = await _context.ScheduleSend(scheduledTime, message, pipe, cancellationToken).ConfigureAwait(false);

            AddScheduledMessage(scheduledMessage);

            return scheduledMessage;
        }

        async Task<ScheduledMessage<T>> MessageSchedulerContext.ScheduleSend<T>(DateTime scheduledTime, T message, IPipe<SendContext> pipe,
            CancellationToken cancellationToken)
        {
            ScheduledMessage<T> scheduledMessage = await _context.ScheduleSend(scheduledTime, message, pipe, cancellationToken).ConfigureAwait(false);

            AddScheduledMessage(scheduledMessage);

            return scheduledMessage;
        }

        async Task<ScheduledMessage> MessageSchedulerContext.ScheduleSend(DateTime scheduledTime, object message, CancellationToken cancellationToken)
        {
            var scheduledMessage = await _context.ScheduleSend(scheduledTime, message, cancellationToken).ConfigureAwait(false);

            AddScheduledMessage(scheduledMessage);

            return scheduledMessage;
        }

        async Task<ScheduledMessage> MessageSchedulerContext.ScheduleSend(DateTime scheduledTime, object message, Type messageType,
            CancellationToken cancellationToken)
        {
            var scheduledMessage = await _context.ScheduleSend(scheduledTime, message, messageType, cancellationToken).ConfigureAwait(false);

            AddScheduledMessage(scheduledMessage);

            return scheduledMessage;
        }

        async Task<ScheduledMessage> MessageSchedulerContext.ScheduleSend(DateTime scheduledTime, object message, IPipe<SendContext> pipe,
            CancellationToken cancellationToken)
        {
            var scheduledMessage = await _context.ScheduleSend(scheduledTime, message, pipe, cancellationToken).ConfigureAwait(false);

            AddScheduledMessage(scheduledMessage);

            return scheduledMessage;
        }

        async Task<ScheduledMessage> MessageSchedulerContext.ScheduleSend(DateTime scheduledTime, object message, Type messageType, IPipe<SendContext> pipe,
            CancellationToken cancellationToken)
        {
            var scheduledMessage = await _context.ScheduleSend(scheduledTime, message, messageType, pipe, cancellationToken).ConfigureAwait(false);

            AddScheduledMessage(scheduledMessage);

            return scheduledMessage;
        }

        async Task<ScheduledMessage<T>> MessageSchedulerContext.ScheduleSend<T>(DateTime scheduledTime, object values, CancellationToken cancellationToken)
        {
            ScheduledMessage<T> scheduledMessage = await _context.ScheduleSend<T>(scheduledTime, values, cancellationToken).ConfigureAwait(false);

            AddScheduledMessage(scheduledMessage);

            return scheduledMessage;
        }

        async Task<ScheduledMessage<T>> MessageSchedulerContext.ScheduleSend<T>(DateTime scheduledTime, object values, IPipe<SendContext<T>> pipe,
            CancellationToken cancellationToken)
        {
            ScheduledMessage<T> scheduledMessage = await _context.ScheduleSend(scheduledTime, values, pipe, cancellationToken).ConfigureAwait(false);

            AddScheduledMessage(scheduledMessage);

            return scheduledMessage;
        }

        async Task<ScheduledMessage<T>> MessageSchedulerContext.ScheduleSend<T>(DateTime scheduledTime, object values, IPipe<SendContext> pipe,
            CancellationToken cancellationToken)
        {
            ScheduledMessage<T> scheduledMessage = await _context.ScheduleSend<T>(scheduledTime, values, pipe, cancellationToken).ConfigureAwait(false);

            AddScheduledMessage(scheduledMessage);

            return scheduledMessage;
        }

        async Task<ScheduledMessage<T>> IMessageScheduler.SchedulePublish<T>(DateTime scheduledTime, T message, CancellationToken cancellationToken)
        {
            ScheduledMessage<T> scheduledMessage = await _context.SchedulePublish(scheduledTime, message, cancellationToken).ConfigureAwait(false);

            AddScheduledMessage(scheduledMessage);

            return scheduledMessage;
        }

        async Task<ScheduledMessage<T>> IMessageScheduler.SchedulePublish<T>(DateTime scheduledTime, T message, IPipe<SendContext<T>> pipe,
            CancellationToken cancellationToken)
        {
            ScheduledMessage<T> scheduledMessage = await _context.SchedulePublish(scheduledTime, message, pipe, cancellationToken).ConfigureAwait(false);

            AddScheduledMessage(scheduledMessage);

            return scheduledMessage;
        }

        async Task<ScheduledMessage<T>> IMessageScheduler.SchedulePublish<T>(DateTime scheduledTime, T message, IPipe<SendContext> pipe,
            CancellationToken cancellationToken)
        {
            ScheduledMessage<T> scheduledMessage = await _context.SchedulePublish(scheduledTime, message, pipe, cancellationToken).ConfigureAwait(false);

            AddScheduledMessage(scheduledMessage);

            return scheduledMessage;
        }

        async Task<ScheduledMessage> IMessageScheduler.SchedulePublish(DateTime scheduledTime, object message, CancellationToken cancellationToken)
        {
            var scheduledMessage = await _context.SchedulePublish(scheduledTime, message, cancellationToken).ConfigureAwait(false);

            AddScheduledMessage(scheduledMessage);

            return scheduledMessage;
        }

        async Task<ScheduledMessage> IMessageScheduler.SchedulePublish(DateTime scheduledTime, object message, Type messageType,
            CancellationToken cancellationToken)
        {
            var scheduledMessage = await _context.SchedulePublish(scheduledTime, message, messageType, cancellationToken).ConfigureAwait(false);

            AddScheduledMessage(scheduledMessage);

            return scheduledMessage;
        }

        async Task<ScheduledMessage> IMessageScheduler.SchedulePublish(DateTime scheduledTime, object message, IPipe<SendContext> pipe,
            CancellationToken cancellationToken)
        {
            var scheduledMessage = await _context.SchedulePublish(scheduledTime, message, pipe, cancellationToken).ConfigureAwait(false);

            AddScheduledMessage(scheduledMessage);

            return scheduledMessage;
        }

        async Task<ScheduledMessage> IMessageScheduler.SchedulePublish(DateTime scheduledTime, object message, Type messageType, IPipe<SendContext> pipe,
            CancellationToken cancellationToken)
        {
            var scheduledMessage = await _context.SchedulePublish(scheduledTime, message, messageType, pipe, cancellationToken).ConfigureAwait(false);

            AddScheduledMessage(scheduledMessage);

            return scheduledMessage;
        }

        async Task<ScheduledMessage<T>> IMessageScheduler.SchedulePublish<T>(DateTime scheduledTime, object values, CancellationToken cancellationToken)
        {
            ScheduledMessage<T> scheduledMessage = await _context.SchedulePublish<T>(scheduledTime, values, cancellationToken).ConfigureAwait(false);

            AddScheduledMessage(scheduledMessage);

            return scheduledMessage;
        }

        async Task<ScheduledMessage<T>> IMessageScheduler.SchedulePublish<T>(DateTime scheduledTime, object values, IPipe<SendContext<T>> pipe,
            CancellationToken cancellationToken)
        {
            ScheduledMessage<T> scheduledMessage = await _context.SchedulePublish(scheduledTime, values, pipe, cancellationToken).ConfigureAwait(false);

            AddScheduledMessage(scheduledMessage);

            return scheduledMessage;
        }

        async Task<ScheduledMessage<T>> IMessageScheduler.SchedulePublish<T>(DateTime scheduledTime, object values, IPipe<SendContext> pipe,
            CancellationToken cancellationToken)
        {
            ScheduledMessage<T> scheduledMessage = await _context.SchedulePublish<T>(scheduledTime, values, pipe, cancellationToken).ConfigureAwait(false);

            AddScheduledMessage(scheduledMessage);

            return scheduledMessage;
        }

        public Task CancelScheduledPublish<T>(Guid tokenId)
            where T : class
        {
            return AddCancelMessage(() => _context.CancelScheduledPublish<T>(tokenId));
        }

        public Task CancelScheduledPublish(Type messageType, Guid tokenId)
        {
            return AddCancelMessage(() => _context.CancelScheduledPublish(messageType, tokenId));
        }

        public Task CancelScheduledSend(Uri destinationAddress, Guid tokenId)
        {
            return AddCancelMessage(() => _context.CancelScheduledSend(destinationAddress, tokenId));
        }

        void AddScheduledMessage(ScheduledMessage scheduledMessage)
        {
            if (_clearToSend.IsCompleted)
                return;

            lock (_listLock)
                _scheduledMessages.Add(scheduledMessage);
        }

        Task AddCancelMessage(Func<Task> cancel)
        {
            if (_clearToSend.IsCompleted)
                return cancel();

            lock (_listLock)
                _cancelMessages.Add(cancel);

            return Task.CompletedTask;
        }

        public Task CancelAllScheduledMessages()
        {
            ScheduledMessage[] scheduledMessages;

            lock (_listLock)
            {
                if (_scheduledMessages.Count == 0)
                    return TaskUtil.Completed;

                scheduledMessages = _scheduledMessages.ToArray();
            }

            var tasks = new PendingTaskCollection(scheduledMessages.Length);
            foreach (var scheduledMessage in scheduledMessages)
                tasks.Add(_context.CancelScheduledSend(scheduledMessage.Destination, scheduledMessage.TokenId));

            return tasks.Completed();
        }

        public Task ExecutePendingActions()
        {
            Func<Task>[] cancelMessages;
            lock (_listLock)
            {
                if (_cancelMessages.Count == 0)
                    return TaskUtil.Completed;

                cancelMessages = _cancelMessages.ToArray();
            }

            var tasks = new PendingTaskCollection(cancelMessages.Length);
            foreach (Func<Task> cancel in cancelMessages)
                tasks.Add(cancel());

            return tasks.Completed();
        }
    }
}
