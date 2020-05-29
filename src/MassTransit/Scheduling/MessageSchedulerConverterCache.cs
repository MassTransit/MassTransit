namespace MassTransit.Scheduling
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;


    /// <summary>
    /// Caches the converters that allow a raw object to be published using the object's type through
    /// the generic Send method.
    /// </summary>
    public class MessageSchedulerConverterCache
    {
        readonly ConcurrentDictionary<Type, Lazy<IMessageSchedulerConverter>> _types =
            new ConcurrentDictionary<Type, Lazy<IMessageSchedulerConverter>>();

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


        static class Cached
        {
            internal static readonly Lazy<MessageSchedulerConverterCache> Converters =
                new Lazy<MessageSchedulerConverterCache>(() => new MessageSchedulerConverterCache(), LazyThreadSafetyMode.PublicationOnly);
        }
    }
}
