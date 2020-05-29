namespace MassTransit.Scheduling
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;


    /// <summary>
    /// Calls the generic version of the ISendEndpoint.Send method with the object's type
    /// </summary>
    public interface IMessageSchedulerConverter
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
}
