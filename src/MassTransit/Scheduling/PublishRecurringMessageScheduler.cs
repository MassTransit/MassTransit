namespace MassTransit.Scheduling
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Initializers;


    public class PublishRecurringMessageScheduler :
        IRecurringMessageScheduler
    {
        readonly IPublishEndpoint _publishEndpoint;

        public PublishRecurringMessageScheduler(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public Task<ScheduledRecurringMessage<T>> ScheduleRecurringSend<T>(Uri destinationAddress, RecurringSchedule schedule, T message,
            CancellationToken cancellationToken)
            where T : class
        {
            if (destinationAddress == null)
                throw new ArgumentNullException(nameof(destinationAddress));

            if (message == null)
                throw new ArgumentNullException(nameof(message));

            return Schedule(destinationAddress, schedule, message, cancellationToken);
        }

        public Task<ScheduledRecurringMessage<T>> ScheduleRecurringSend<T>(Uri destinationAddress, RecurringSchedule schedule, T message,
            IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
            where T : class
        {
            if (destinationAddress == null)
                throw new ArgumentNullException(nameof(destinationAddress));

            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            return Schedule(destinationAddress, schedule, message, pipe, cancellationToken);
        }

        public Task<ScheduledRecurringMessage<T>> ScheduleRecurringSend<T>(Uri destinationAddress, RecurringSchedule schedule, T message,
            IPipe<SendContext> pipe, CancellationToken cancellationToken)
            where T : class
        {
            if (destinationAddress == null)
                throw new ArgumentNullException(nameof(destinationAddress));

            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            return Schedule(destinationAddress, schedule, message, pipe, cancellationToken);
        }

        public Task<ScheduledRecurringMessage> ScheduleRecurringSend(Uri destinationAddress, RecurringSchedule schedule, object message,
            CancellationToken cancellationToken)
        {
            if (destinationAddress == null)
                throw new ArgumentNullException(nameof(destinationAddress));

            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var messageType = message.GetType();

            return MessageSchedulerConverterCache.ScheduleRecurringSend(this, destinationAddress, schedule, message, messageType, cancellationToken);
        }

        public Task<ScheduledRecurringMessage> ScheduleRecurringSend(Uri destinationAddress, RecurringSchedule schedule, object message,
            Type messageType, CancellationToken cancellationToken)
        {
            if (destinationAddress == null)
                throw new ArgumentNullException(nameof(destinationAddress));

            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (messageType == null)
                throw new ArgumentNullException(nameof(messageType));

            return MessageSchedulerConverterCache.ScheduleRecurringSend(this, destinationAddress, schedule, message, messageType, cancellationToken);
        }

        public Task<ScheduledRecurringMessage> ScheduleRecurringSend(Uri destinationAddress, RecurringSchedule schedule, object message,
            IPipe<SendContext> pipe, CancellationToken cancellationToken)
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

        public Task<ScheduledRecurringMessage> ScheduleRecurringSend(Uri destinationAddress, RecurringSchedule schedule, object message,
            Type messageType, IPipe<SendContext> pipe, CancellationToken cancellationToken)
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

        public async Task<ScheduledRecurringMessage<T>> ScheduleRecurringSend<T>(Uri destinationAddress, RecurringSchedule schedule,
            object values, CancellationToken cancellationToken)
            where T : class
        {
            if (destinationAddress == null)
                throw new ArgumentNullException(nameof(destinationAddress));

            if (values == null)
                throw new ArgumentNullException(nameof(values));

            SendTuple<T> send = await MessageInitializerCache<T>.InitializeMessage(values, cancellationToken).ConfigureAwait(false);

            return await Schedule(destinationAddress, schedule, send.Message, send.Pipe, cancellationToken).ConfigureAwait(false);
        }

        public async Task<ScheduledRecurringMessage<T>> ScheduleRecurringSend<T>(Uri destinationAddress, RecurringSchedule schedule,
            object values, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
            where T : class
        {
            if (destinationAddress == null)
                throw new ArgumentNullException(nameof(destinationAddress));

            if (values == null)
                throw new ArgumentNullException(nameof(values));

            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            SendTuple<T> send = await MessageInitializerCache<T>.InitializeMessage(values, pipe, cancellationToken).ConfigureAwait(false);

            return await Schedule(destinationAddress, schedule, send.Message, send.Pipe, cancellationToken).ConfigureAwait(false);
        }

        public async Task<ScheduledRecurringMessage<T>> ScheduleRecurringSend<T>(Uri destinationAddress, RecurringSchedule schedule,
            object values, IPipe<SendContext> pipe, CancellationToken cancellationToken)
            where T : class
        {
            if (destinationAddress == null)
                throw new ArgumentNullException(nameof(destinationAddress));

            if (values == null)
                throw new ArgumentNullException(nameof(values));

            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            SendTuple<T> send = await MessageInitializerCache<T>.InitializeMessage(values, pipe, cancellationToken).ConfigureAwait(false);

            return await Schedule(destinationAddress, schedule, send.Message, send.Pipe, cancellationToken).ConfigureAwait(false);
        }

        public Task CancelScheduledRecurringSend(string scheduleId, string scheduleGroup)
        {
            var command = new CancelScheduledRecurringMessageCommand(scheduleId, scheduleGroup);

            return _publishEndpoint.Publish<CancelScheduledRecurringMessage>(command);
        }

        async Task<ScheduledRecurringMessage<T>> Schedule<T>(Uri destinationAddress, RecurringSchedule schedule, T message, CancellationToken cancellationToken)
            where T : class
        {
            var command = CreateCommand(destinationAddress, schedule, message);

            await _publishEndpoint.Publish(command, cancellationToken).ConfigureAwait(false);

            return new ScheduledRecurringMessageHandle<T>(schedule, command.Destination, message);
        }

        async Task<ScheduledRecurringMessage<T>> Schedule<T>(Uri destinationAddress, RecurringSchedule schedule, T message, IPipe<SendContext> pipe,
            CancellationToken cancellationToken)
            where T : class
        {
            var command = CreateCommand(destinationAddress, schedule, message);

            await _publishEndpoint.Publish(command, pipe, cancellationToken).ConfigureAwait(false);

            return new ScheduledRecurringMessageHandle<T>(schedule, command.Destination, message);
        }

        async Task<ScheduledRecurringMessage<T>> Schedule<T>(Uri destinationAddress, RecurringSchedule schedule, T message, IPipe<SendContext<T>> pipe,
            CancellationToken cancellationToken)
            where T : class
        {
            var command = CreateCommand(destinationAddress, schedule, message);

            var scheduleMessagePipe = new ScheduleRecurringMessageContextPipe<T>(message, pipe);

            await _publishEndpoint.Publish(command, scheduleMessagePipe, cancellationToken).ConfigureAwait(false);

            return new ScheduledRecurringMessageHandle<T>(schedule, command.Destination, message);
        }

        static ScheduleRecurringMessage CreateCommand<T>(Uri destinationAddress, RecurringSchedule schedule, T message)
            where T : class
        {
            if (!MessageTypeCache<T>.IsValidMessageType)
                throw new ArgumentException(MessageTypeCache<T>.InvalidMessageTypeReason, nameof(T));

            return new ScheduleRecurringMessageCommand<T>(schedule, destinationAddress, message);
        }


        class ScheduleRecurringMessageContextPipe<T> :
            IPipe<PublishContext<ScheduleRecurringMessage>>
            where T : class
        {
            readonly T _payload;
            readonly IPipe<PublishContext<T>> _pipe;

            public ScheduleRecurringMessageContextPipe(T payload, IPipe<PublishContext<T>> pipe)
            {
                _payload = payload;
                _pipe = pipe;
            }

            public async Task Send(PublishContext<ScheduleRecurringMessage> context)
            {
                if (_pipe.IsNotEmpty())
                {
                    var proxy = new PublishContextProxy<T>(context, _payload);

                    await _pipe.Send(proxy).ConfigureAwait(false);
                }
            }

            void IProbeSite.Probe(ProbeContext context)
            {
                _pipe?.Probe(context);
            }
        }
    }
}
