namespace MassTransit.QuartzIntegration
{
    using System.Threading.Tasks;
    using Quartz;
    using Scheduling;


    public class ResumeScheduledMessageConsumer :
        IConsumer<ResumeScheduledRecurringMessage>
    {
        readonly ISchedulerFactory _schedulerFactory;

        public ResumeScheduledMessageConsumer(ISchedulerFactory schedulerFactory)
        {
            _schedulerFactory = schedulerFactory;
        }

        public async Task Consume(ConsumeContext<ResumeScheduledRecurringMessage> context)
        {
            const string prependedValue = "Recurring.Trigger.";

            var scheduleId = context.Message.ScheduleId;

            if (!scheduleId.StartsWith(prependedValue))
                scheduleId = string.Concat(prependedValue, scheduleId);

            var scheduler = await _schedulerFactory.GetScheduler(context.CancellationToken).ConfigureAwait(false);

            await scheduler.ResumeTrigger(new TriggerKey(scheduleId, context.Message.ScheduleGroup), context.CancellationToken)
                .ConfigureAwait(false);

            LogContext.Debug?.Log("ResumeScheduledRecurringMessage: {ScheduleId}/{ScheduleGroup} at {Timestamp}", context.Message.ScheduleId,
                context.Message.ScheduleGroup, context.Message.Timestamp);
        }
    }
}
