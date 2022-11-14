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
            var triggerKey = new TriggerKey(correlationId);

            var scheduler = await _schedulerFactory.GetScheduler(context.CancellationToken).ConfigureAwait(false);

            var unscheduleJob = await scheduler.UnscheduleJob(triggerKey, context.CancellationToken).ConfigureAwait(false);

            if (unscheduleJob)
                LogContext.Debug?.Log("Canceled Scheduled Message: {Id} at {Timestamp}", triggerKey, context.Message.Timestamp);
            else
                LogContext.Debug?.Log("CancelScheduledMessage: no message found for {Id}", triggerKey);
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
