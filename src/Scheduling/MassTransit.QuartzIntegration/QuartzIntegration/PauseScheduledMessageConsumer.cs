namespace MassTransit.QuartzIntegration
{
    using System.Threading.Tasks;
    using Quartz;
    using Scheduling;


    public class PauseScheduledMessageConsumer :
        IConsumer<PauseScheduledRecurringMessage>
    {
        readonly ISchedulerFactory _schedulerFactory;

        public PauseScheduledMessageConsumer(ISchedulerFactory schedulerFactory)
        {
            _schedulerFactory = schedulerFactory;
        }

        public async Task Consume(ConsumeContext<PauseScheduledRecurringMessage> context)
        {
            const string prependedValue = "Recurring.Trigger.";

            var scheduleId = context.Message.ScheduleId;

            if (!scheduleId.StartsWith(prependedValue))
                scheduleId = string.Concat(prependedValue, scheduleId);

            var scheduler = await _schedulerFactory.GetScheduler(context.CancellationToken).ConfigureAwait(false);

            await scheduler.PauseTrigger(new TriggerKey(scheduleId, context.Message.ScheduleGroup), context.CancellationToken)
                .ConfigureAwait(false);

            LogContext.Debug?.Log("PauseScheduledRecurringMessage: {ScheduleId}/{ScheduleGroup} at {Timestamp}", context.Message.ScheduleId,
                context.Message.ScheduleGroup, context.Message.Timestamp);
        }
    }
}
