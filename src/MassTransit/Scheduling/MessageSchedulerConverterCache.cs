namespace MassTransit.Scheduling
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Threading.Tasks;


    /// <summary>
    /// Caches the converters that allow a raw object to be published using the object's type through
    /// the generic Send method.
    /// </summary>
    public class MessageSchedulerConverterCache
    {
        readonly ConcurrentDictionary<Type, Lazy<IMessageSchedulerConverter>> _types = new ConcurrentDictionary<Type, Lazy<IMessageSchedulerConverter>>();

        IMessageSchedulerConverter this[Type type] => _types.GetOrAdd(type, CreateTypeConverter).Value;

        public static Task<ScheduledMessage> ScheduleSend(IMessageScheduler scheduler, Uri destinationAddress, DateTime scheduledTime, object message,
            Type messageType, CancellationToken cancellationToken)
        {
            return Cached.Converters.Value[messageType].ScheduleSend(scheduler, destinationAddress, scheduledTime, message, cancellationToken);
        }

        public static Task<ScheduledMessage> ScheduleSend(IMessageScheduler scheduler, Uri destinationAddress, DateTime scheduledTime, object message,
            Type messageType, IPipe<SendContext> pipe, CancellationToken cancellationToken)
        {
            return Cached.Converters.Value[messageType].ScheduleSend(scheduler, destinationAddress, scheduledTime, message, pipe, cancellationToken);
        }

        public static Task<ScheduledRecurringMessage> ScheduleRecurringSend(IRecurringMessageScheduler scheduler, Uri destinationAddress,
            RecurringSchedule schedule, object message,
            Type messageType, IPipe<SendContext> pipe, CancellationToken cancellationToken)
        {
            return Cached.Converters.Value[messageType].ScheduleRecurringSend(scheduler, destinationAddress, schedule, message, pipe, cancellationToken);
        }

        public static Task<ScheduledRecurringMessage> ScheduleRecurringSend(IRecurringMessageScheduler scheduler, Uri destinationAddress,
            RecurringSchedule schedule, object message,
            Type messageType, CancellationToken cancellationToken)
        {
            return Cached.Converters.Value[messageType].ScheduleRecurringSend(scheduler, destinationAddress, schedule, message, cancellationToken);
        }

        static Lazy<IMessageSchedulerConverter> CreateTypeConverter(Type type)
        {
            return new Lazy<IMessageSchedulerConverter>(() => CreateConverter(type));
        }

        static IMessageSchedulerConverter CreateConverter(Type type)
        {
            var converterType = typeof(MessageSchedulerConverter<>).MakeGenericType(type);

            return (IMessageSchedulerConverter)Activator.CreateInstance(converterType);
        }


        /// <summary>
        /// Calls the generic version of the ISendEndpoint.Send method with the object's type
        /// </summary>
        interface IMessageSchedulerConverter
        {
            Task<ScheduledMessage> ScheduleSend(IMessageScheduler scheduler, Uri destinationAddress, DateTime scheduledTime, object message,
                CancellationToken cancellationToken);

            Task<ScheduledMessage> ScheduleSend(IMessageScheduler scheduler, Uri destinationAddress, DateTime scheduledTime, object message,
                IPipe<SendContext> pipe,
                CancellationToken cancellationToken);

            Task<ScheduledRecurringMessage> ScheduleRecurringSend(IRecurringMessageScheduler scheduler, Uri destinationAddress, RecurringSchedule schedule,
                object message, IPipe<SendContext> pipe,
                CancellationToken cancellationToken);

            Task<ScheduledRecurringMessage> ScheduleRecurringSend(IRecurringMessageScheduler scheduler, Uri destinationAddress,
                RecurringSchedule schedule, object message, CancellationToken cancellationToken);
        }


        /// <summary>
        /// Converts the object type message to the appropriate generic type and invokes the send method with that
        /// generic overload.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        class MessageSchedulerConverter<T> :
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

                throw new ArgumentException("Unexpected message type: " + TypeCache.GetShortName(message.GetType()));
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

                throw new ArgumentException("Unexpected message type: " + TypeCache.GetShortName(message.GetType()));
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

                throw new ArgumentException("Unexpected message type: " + TypeCache.GetShortName(message.GetType()));
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

                throw new ArgumentException("Unexpected message type: " + TypeCache.GetShortName(message.GetType()));
            }
        }


        static class Cached
        {
            internal static readonly Lazy<MessageSchedulerConverterCache> Converters =
                new Lazy<MessageSchedulerConverterCache>(() => new MessageSchedulerConverterCache(), LazyThreadSafetyMode.PublicationOnly);
        }
    }
}
