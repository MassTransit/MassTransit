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
        readonly MessageSchedulerContext _context;
        readonly object _listLock = new object();
        readonly List<ScheduledMessage> _scheduledMessages;

        public InMemoryOutboxMessageSchedulerContext(MessageSchedulerContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            _scheduledMessages = new List<ScheduledMessage>();
        }

        void AddScheduledMessage(ScheduledMessage scheduledMessage)
        {
            lock (_listLock)
                _scheduledMessages.Add(scheduledMessage);
        }

        public Task CancelAllScheduledMessages()
        {
            ScheduledMessage[] scheduledMessages;

            lock (_listLock)
                scheduledMessages = _scheduledMessages.ToArray();

            var tasks = new PendingTaskCollection(scheduledMessages.Length);
            foreach (var scheduledMessage in scheduledMessages)
            {
                tasks.Add(_context.CancelScheduledSend(scheduledMessage.Destination, scheduledMessage.TokenId));
            }

            return tasks.Completed();
        }

        Task IMessageScheduler.CancelScheduledSend(Guid tokenId)
        {
            return _context.CancelScheduledSend(tokenId);
        }

        public Task CancelScheduledSend(Uri destinationAddress, Guid tokenId)
        {
            return _context.CancelScheduledSend(destinationAddress, tokenId);
        }

        async Task<ScheduledMessage<T>> IMessageScheduler.ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, T message,
            CancellationToken cancellationToken)
        {
            var scheduledMessage = await _context.ScheduleSend(destinationAddress, scheduledTime, message, cancellationToken).ConfigureAwait(false);

            AddScheduledMessage(scheduledMessage);

            return scheduledMessage;
        }

        async Task<ScheduledMessage<T>> IMessageScheduler.ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, T message, IPipe<SendContext<T>> pipe,
            CancellationToken cancellationToken)
        {
            var scheduledMessage = await _context.ScheduleSend(destinationAddress, scheduledTime, message, pipe, cancellationToken).ConfigureAwait(false);

            AddScheduledMessage(scheduledMessage);

            return scheduledMessage;
        }

        async Task<ScheduledMessage<T>> IMessageScheduler.ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, T message, IPipe<SendContext> pipe,
            CancellationToken cancellationToken)
        {
            var scheduledMessage = await _context.ScheduleSend(destinationAddress, scheduledTime, message, pipe, cancellationToken).ConfigureAwait(false);

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
            var scheduledMessage = await _context.ScheduleSend(destinationAddress, scheduledTime, message, messageType, cancellationToken).ConfigureAwait(false);

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
            var scheduledMessage = await _context.ScheduleSend(destinationAddress, scheduledTime, message, messageType, pipe, cancellationToken).ConfigureAwait(false);

            AddScheduledMessage(scheduledMessage);

            return scheduledMessage;
        }

        async Task<ScheduledMessage<T>> IMessageScheduler.ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, object values,
            CancellationToken cancellationToken)
        {
            var scheduledMessage = await _context.ScheduleSend<T>(destinationAddress, scheduledTime, values, cancellationToken).ConfigureAwait(false);

            AddScheduledMessage(scheduledMessage);

            return scheduledMessage;
        }

        async Task<ScheduledMessage<T>> IMessageScheduler.ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, object values, IPipe<SendContext<T>> pipe,
            CancellationToken cancellationToken)
        {
            var scheduledMessage = await _context.ScheduleSend(destinationAddress, scheduledTime, values, pipe, cancellationToken).ConfigureAwait(false);

            AddScheduledMessage(scheduledMessage);

            return scheduledMessage;
        }

        async Task<ScheduledMessage<T>> IMessageScheduler.ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, object values, IPipe<SendContext> pipe,
            CancellationToken cancellationToken)
        {
            var scheduledMessage = await _context.ScheduleSend<T>(destinationAddress, scheduledTime, values, pipe, cancellationToken).ConfigureAwait(false);

            AddScheduledMessage(scheduledMessage);

            return scheduledMessage;
        }

        async Task<ScheduledMessage<T>> MessageSchedulerContext.ScheduleSend<T>(DateTime scheduledTime, T message, CancellationToken cancellationToken)
        {
            var scheduledMessage = await _context.ScheduleSend(scheduledTime, message, cancellationToken).ConfigureAwait(false);

            AddScheduledMessage(scheduledMessage);

            return scheduledMessage;
        }

        async Task<ScheduledMessage<T>> MessageSchedulerContext.ScheduleSend<T>(DateTime scheduledTime, T message, IPipe<SendContext<T>> pipe,
            CancellationToken cancellationToken)
        {
            var scheduledMessage = await _context.ScheduleSend(scheduledTime, message, pipe, cancellationToken).ConfigureAwait(false);

            AddScheduledMessage(scheduledMessage);

            return scheduledMessage;
        }

        async Task<ScheduledMessage<T>> MessageSchedulerContext.ScheduleSend<T>(DateTime scheduledTime, T message, IPipe<SendContext> pipe,
            CancellationToken cancellationToken)
        {
            var scheduledMessage = await _context.ScheduleSend(scheduledTime, message, pipe, cancellationToken).ConfigureAwait(false);

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
            var scheduledMessage = await _context.ScheduleSend<T>(scheduledTime, values, cancellationToken).ConfigureAwait(false);

            AddScheduledMessage(scheduledMessage);

            return scheduledMessage;
        }

        async Task<ScheduledMessage<T>> MessageSchedulerContext.ScheduleSend<T>(DateTime scheduledTime, object values, IPipe<SendContext<T>> pipe,
            CancellationToken cancellationToken)
        {
            var scheduledMessage = await _context.ScheduleSend(scheduledTime, values, pipe, cancellationToken).ConfigureAwait(false);

            AddScheduledMessage(scheduledMessage);

            return scheduledMessage;
        }

        async Task<ScheduledMessage<T>> MessageSchedulerContext.ScheduleSend<T>(DateTime scheduledTime, object values, IPipe<SendContext> pipe,
            CancellationToken cancellationToken)
        {
            var scheduledMessage = await _context.ScheduleSend<T>(scheduledTime, values, pipe, cancellationToken).ConfigureAwait(false);

            AddScheduledMessage(scheduledMessage);

            return scheduledMessage;
        }
    }
}
