namespace MassTransit.Scheduling
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using Metadata;


    /// <summary>
    /// Converts the object type message to the appropriate generic type and invokes the send method with that
    /// generic overload.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MessageSchedulerConverter<T> :
        IMessageSchedulerConverter
        where T : class
    {
        public async Task<ScheduledMessage> ScheduleSend(IMessageScheduler scheduler, Uri destinationAddress, DateTime scheduledTime, object message,
            CancellationToken cancellationToken = default)
        {
            if (scheduler == null)
                throw new ArgumentNullException(nameof(scheduler));
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (message is T msg)
                return await scheduler.ScheduleSend(destinationAddress, scheduledTime, msg, cancellationToken).ConfigureAwait(false);

            throw new ArgumentException("Unexpected message type: " + TypeMetadataCache.GetShortName(message.GetType()));
        }

        public async Task<ScheduledMessage> ScheduleSend(IMessageScheduler scheduler, Uri destinationAddress, DateTime scheduledTime, object message,
            IPipe<SendContext> pipe, CancellationToken cancellationToken = default)
        {
            if (scheduler == null)
                throw new ArgumentNullException(nameof(scheduler));
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            if (message is T msg)
                return await scheduler.ScheduleSend(destinationAddress, scheduledTime, msg, pipe, cancellationToken).ConfigureAwait(false);

            throw new ArgumentException("Unexpected message type: " + TypeMetadataCache.GetShortName(message.GetType()));

        }

        public async Task<ScheduledRecurringMessage> ScheduleRecurringSend(IRecurringMessageScheduler scheduler, Uri destinationAddress,
            RecurringSchedule schedule, object message, CancellationToken cancellationToken)
        {
            if (scheduler == null)
                throw new ArgumentNullException(nameof(scheduler));
            if (schedule == null)
                throw new ArgumentNullException(nameof(schedule));
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (message is T msg)
                return await scheduler.ScheduleRecurringSend(destinationAddress, schedule, msg, cancellationToken).ConfigureAwait(false);

            throw new ArgumentException("Unexpected message type: " + TypeMetadataCache.GetShortName(message.GetType()));
        }

        public async Task<ScheduledRecurringMessage> ScheduleRecurringSend(IRecurringMessageScheduler scheduler, Uri destinationAddress,
            RecurringSchedule schedule, object message, IPipe<SendContext> pipe, CancellationToken cancellationToken)
        {
            if (scheduler == null)
                throw new ArgumentNullException(nameof(scheduler));
            if (schedule == null)
                throw new ArgumentNullException(nameof(schedule));
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            if (message is T msg)
                return await scheduler.ScheduleRecurringSend(destinationAddress, schedule, msg, pipe, cancellationToken).ConfigureAwait(false);

            throw new ArgumentException("Unexpected message type: " + TypeMetadataCache.GetShortName(message.GetType()));
        }
    }
}
