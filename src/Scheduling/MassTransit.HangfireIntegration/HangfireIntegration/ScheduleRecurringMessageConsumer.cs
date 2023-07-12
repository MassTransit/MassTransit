namespace MassTransit.HangfireIntegration
{
    using System;
    using System.Threading.Tasks;
    using Hangfire;
    using Scheduling;


    public class ScheduleRecurringMessageConsumer :
        IConsumer<ScheduleRecurringMessage>,
        IConsumer<CancelScheduledRecurringMessage>
    {
        readonly IRecurringJobManager _recurringJobManager;
        readonly ITimeZoneResolver _timeZoneResolver;

        public ScheduleRecurringMessageConsumer(IRecurringJobManager recurringJobManager, ITimeZoneResolver timeZoneResolver)
        {
            _recurringJobManager = recurringJobManager;
            _timeZoneResolver = timeZoneResolver;
        }

        public Task Consume(ConsumeContext<CancelScheduledRecurringMessage> context)
        {
            var jobKey = JobKey.Create(context.Message.ScheduleId, context.Message.ScheduleGroup);
            _recurringJobManager.RemoveIfExists(jobKey);

            LogContext.Debug?.Log("Canceled Recurring Message: {Key}", jobKey);
            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<ScheduleRecurringMessage> context)
        {
            var jobKey = JobKey.Create(context.Message.Schedule.ScheduleId, context.Message.Schedule.ScheduleGroup);
            var message = HangfireRecurringScheduledMessageData.Create(context, jobKey);

            var tz = TimeZoneInfo.Local;
            if (!string.IsNullOrWhiteSpace(context.Message.Schedule.TimeZoneId) && context.Message.Schedule.TimeZoneId != tz.Id)
                tz = _timeZoneResolver.GetTimeZoneById(context.Message.Schedule.TimeZoneId);

            _recurringJobManager.AddOrUpdate<ScheduleJob>(
                jobKey,
                x => x.SendMessage(message, null!),
                context.Message.Schedule.CronExpression,
                new RecurringJobOptions { TimeZone = tz });

            LogContext.Debug?.Log("Scheduled: {Key}", jobKey);
            return Task.CompletedTask;
        }
    }
}
