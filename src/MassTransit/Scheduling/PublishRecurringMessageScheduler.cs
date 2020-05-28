namespace MassTransit.Scheduling
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using Initializers;


    public class PublishRecurringMessageScheduler :
        IRecurringMessageScheduler
    {
        readonly IPublishEndpoint _publishEndpoint;

        public PublishRecurringMessageScheduler(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        Task<ScheduledRecurringMessage<T>> IRecurringMessageScheduler.ScheduleRecurringSend<T>(Uri destinationAddress, RecurringSchedule schedule, T message,
            CancellationToken cancellationToken)
        {
            if (destinationAddress == null)
                throw new ArgumentNullException(nameof(destinationAddress));

            if (message == null)
                throw new ArgumentNullException(nameof(message));

            return ScheduleRecurringSend(destinationAddress, schedule, Task.FromResult(message), cancellationToken);
        }

        Task<ScheduledRecurringMessage<T>> IRecurringMessageScheduler.ScheduleRecurringSend<T>(Uri destinationAddress, RecurringSchedule schedule, T message,
            IPipe<SendContext<T>> pipe,
            CancellationToken cancellationToken)
        {
            if (destinationAddress == null)
                throw new ArgumentNullException(nameof(destinationAddress));

            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            return ScheduleRecurringSend(destinationAddress, schedule, Task.FromResult(message), pipe, cancellationToken);
        }

        Task<ScheduledRecurringMessage<T>> IRecurringMessageScheduler.ScheduleRecurringSend<T>(Uri destinationAddress, RecurringSchedule schedule, T message,
            IPipe<SendContext> pipe,
            CancellationToken cancellationToken)
        {
            if (destinationAddress == null)
                throw new ArgumentNullException(nameof(destinationAddress));

            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            return ScheduleRecurringSend(destinationAddress, schedule, Task.FromResult(message), pipe, cancellationToken);
        }

        Task<ScheduledRecurringMessage> IRecurringMessageScheduler.ScheduleRecurringSend(Uri destinationAddress, RecurringSchedule schedule, object message,
            CancellationToken cancellationToken)
        {
            if (destinationAddress == null)
                throw new ArgumentNullException(nameof(destinationAddress));

            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var messageType = message.GetType();

            return MessageSchedulerConverterCache.ScheduleRecurringSend(this, destinationAddress, schedule, message, messageType, cancellationToken);
        }

        Task<ScheduledRecurringMessage> IRecurringMessageScheduler.ScheduleRecurringSend(Uri destinationAddress, RecurringSchedule schedule, object message,
            Type messageType,
            CancellationToken cancellationToken)
        {
            if (destinationAddress == null)
                throw new ArgumentNullException(nameof(destinationAddress));

            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (messageType == null)
                throw new ArgumentNullException(nameof(messageType));

            return MessageSchedulerConverterCache.ScheduleRecurringSend(this, destinationAddress, schedule, message, messageType, cancellationToken);
        }

        Task<ScheduledRecurringMessage> IRecurringMessageScheduler.ScheduleRecurringSend(Uri destinationAddress, RecurringSchedule schedule, object message,
            IPipe<SendContext> pipe,
            CancellationToken cancellationToken)
        {
            if (destinationAddress == null)
                throw new ArgumentNullException(nameof(destinationAddress));

            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            var messageType = message.GetType();

            return MessageSchedulerConverterCache.ScheduleRecurringSend(this, destinationAddress, schedule, message, messageType, pipe, cancellationToken);
        }

        Task<ScheduledRecurringMessage> IRecurringMessageScheduler.ScheduleRecurringSend(Uri destinationAddress, RecurringSchedule schedule, object message,
            Type messageType,
            IPipe<SendContext> pipe, CancellationToken cancellationToken)
        {
            if (destinationAddress == null)
                throw new ArgumentNullException(nameof(destinationAddress));

            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (messageType == null)
                throw new ArgumentNullException(nameof(messageType));

            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            return MessageSchedulerConverterCache.ScheduleRecurringSend(this, destinationAddress, schedule, message, messageType, pipe, cancellationToken);
        }

        Task<ScheduledRecurringMessage<T>> IRecurringMessageScheduler.ScheduleRecurringSend<T>(Uri destinationAddress, RecurringSchedule schedule,
            object values,
            CancellationToken cancellationToken)
        {
            if (destinationAddress == null)
                throw new ArgumentNullException(nameof(destinationAddress));

            if (values == null)
                throw new ArgumentNullException(nameof(values));

            Task<T> message = MessageInitializerCache<T>.InitializeMessage(values, cancellationToken);

            return ScheduleRecurringSend(destinationAddress, schedule, message, cancellationToken);
        }

        Task<ScheduledRecurringMessage<T>> IRecurringMessageScheduler.ScheduleRecurringSend<T>(Uri destinationAddress, RecurringSchedule schedule,
            object values,
            IPipe<SendContext<T>> pipe,
            CancellationToken cancellationToken)
        {
            if (destinationAddress == null)
                throw new ArgumentNullException(nameof(destinationAddress));

            if (values == null)
                throw new ArgumentNullException(nameof(values));

            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            Task<T> message = MessageInitializerCache<T>.InitializeMessage(values, cancellationToken);

            return ScheduleRecurringSend(destinationAddress, schedule, message, pipe, cancellationToken);
        }

        Task<ScheduledRecurringMessage<T>> IRecurringMessageScheduler.ScheduleRecurringSend<T>(Uri destinationAddress, RecurringSchedule schedule,
            object values,
            IPipe<SendContext> pipe,
            CancellationToken cancellationToken)
        {
            if (destinationAddress == null)
                throw new ArgumentNullException(nameof(destinationAddress));

            if (values == null)
                throw new ArgumentNullException(nameof(values));

            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            Task<T> message = MessageInitializerCache<T>.InitializeMessage(values, cancellationToken);

            return ScheduleRecurringSend(destinationAddress, schedule, message, pipe, cancellationToken);
        }

        public Task CancelScheduledRecurringSend(string scheduleId, string scheduleGroup)
        {
            var command = new CancelScheduledRecurringMessageCommand(scheduleId, scheduleGroup);

            return _publishEndpoint.Publish<CancelScheduledRecurringMessage>(command);
        }

        async Task<ScheduledRecurringMessage<T>> ScheduleRecurringSend<T>(Uri destinationAddress, RecurringSchedule schedule, Task<T> message,
            CancellationToken cancellationToken)
            where T : class
        {
            ScheduleRecurringMessage<T> command = await CreateCommand(destinationAddress, schedule, message).ConfigureAwait(false);

            await _publishEndpoint.Publish(command, cancellationToken).ConfigureAwait(false);

            return new ScheduledRecurringMessageHandle<T>(schedule, command.Destination, command.Payload);
        }

        async Task<ScheduledRecurringMessage<T>> ScheduleRecurringSend<T>(Uri destinationAddress, RecurringSchedule schedule, Task<T> message,
            IPipe<SendContext> pipe,
            CancellationToken cancellationToken)
            where T : class
        {
            ScheduleRecurringMessage<T> command = await CreateCommand(destinationAddress, schedule, message).ConfigureAwait(false);

            await _publishEndpoint.Publish(command, pipe, cancellationToken).ConfigureAwait(false);

            return new ScheduledRecurringMessageHandle<T>(schedule, command.Destination, command.Payload);
        }

        async Task<ScheduledRecurringMessage<T>> ScheduleRecurringSend<T>(Uri destinationAddress, RecurringSchedule schedule, Task<T> message,
            IPipe<SendContext<T>> pipe,
            CancellationToken cancellationToken)
            where T : class
        {
            ScheduleRecurringMessage<T> command = await CreateCommand(destinationAddress, schedule, message).ConfigureAwait(false);

            await _publishEndpoint.Publish(command, pipe, cancellationToken).ConfigureAwait(false);

            return new ScheduledRecurringMessageHandle<T>(schedule, command.Destination, command.Payload);
        }

        static async Task<ScheduleRecurringMessage<T>> CreateCommand<T>(Uri destinationAddress, RecurringSchedule schedule, Task<T> message)
            where T : class
        {
            var payload = await message.ConfigureAwait(false);

            return new ScheduleRecurringMessageCommand<T>(schedule, destinationAddress, payload);
        }
    }
}
