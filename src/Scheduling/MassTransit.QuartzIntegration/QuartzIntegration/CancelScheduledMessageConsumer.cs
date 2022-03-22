namespace MassTransit.QuartzIntegration
{
    using System.Threading.Tasks;
    using Quartz;
    using Scheduling;


    public class CancelScheduledMessageConsumer :
        IConsumer<CancelScheduledMessage>,
        IConsumer<CancelScheduledRecurringMessage>
    {
        readonly ISchedulerFactory _schedulerFactory;

        public CancelScheduledMessageConsumer(ISchedulerFactory schedulerFactory)
        {
            _schedulerFactory = schedulerFactory;
        }

        public async Task Consume(ConsumeContext<CancelScheduledMessage> context)
        {
            var correlationId = context.Message.TokenId.ToString("N");

            var jobKey = new JobKey(correlationId);

            var scheduler = await _schedulerFactory.GetScheduler(context.CancellationToken).ConfigureAwait(false);

            var deletedJob = await scheduler.DeleteJob(jobKey, context.CancellationToken).ConfigureAwait(false);

            if (deletedJob)
                LogContext.Debug?.Log("Canceled Scheduled Message: {Id} at {Timestamp}", jobKey, context.Message.Timestamp);
            else
                LogContext.Debug?.Log("CancelScheduledMessage: no message found for {Id}", jobKey);
        }

        public async Task Consume(ConsumeContext<CancelScheduledRecurringMessage> context)
        {
            const string prependedValue = "Recurring.Trigger.";

            var scheduleId = context.Message.ScheduleId;

            if (!scheduleId.StartsWith(prependedValue))
                scheduleId = string.Concat(prependedValue, scheduleId);

            var scheduler = await _schedulerFactory.GetScheduler(context.CancellationToken).ConfigureAwait(false);

            var unscheduledJob = await scheduler.UnscheduleJob(new TriggerKey(scheduleId, context.Message.ScheduleGroup), context.CancellationToken)
                .ConfigureAwait(false);

            if (unscheduledJob)
            {
                LogContext.Debug?.Log("CancelRecurringScheduledMessage: {ScheduleId}/{ScheduleGroup} at {Timestamp}", context.Message.ScheduleId,
                    context.Message.ScheduleGroup, context.Message.Timestamp);
            }
            else
            {
                LogContext.Debug?.Log("CancelRecurringScheduledMessage: no message found {ScheduleId}/{ScheduleGroup}", context.Message.ScheduleId,
                    context.Message.ScheduleGroup);
            }
        }
    }
}
