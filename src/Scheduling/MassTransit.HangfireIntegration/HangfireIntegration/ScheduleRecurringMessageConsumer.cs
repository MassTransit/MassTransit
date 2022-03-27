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

        public async Task Consume(ConsumeContext<CancelScheduledRecurringMessage> context)
        {
            var jobKey = GetJobKey(context.Message.ScheduleId, context.Message.ScheduleGroup);
            _recurringJobManager.RemoveIfExists(jobKey);

            LogContext.Debug?.Log("Canceled Recurring Message: {Key}", jobKey);
        }

        public async Task Consume(ConsumeContext<ScheduleRecurringMessage> context)
        {
            var jobKey = GetJobKey(context.Message.Schedule.ScheduleId, context.Message.Schedule.ScheduleGroup);
            var message = HangfireRecurringScheduledMessageData.Create(context, jobKey);

            var tz = TimeZoneInfo.Local;
            if (!string.IsNullOrWhiteSpace(context.Message.Schedule.TimeZoneId) && context.Message.Schedule.TimeZoneId != tz.Id)
                tz = _timeZoneResolver.GetTimeZoneById(context.Message.Schedule.TimeZoneId);

            _recurringJobManager.AddOrUpdate<ScheduleJob>(
                jobKey,
                x => x.SendMessage(message, null!),
                context.Message.Schedule.CronExpression,
                tz);

            LogContext.Debug?.Log("Scheduled: {Key}", jobKey);
        }

        static string GetJobKey(string scheduleId, string scheduleGroup)
        {
            return string.IsNullOrEmpty(scheduleGroup)
                ? scheduleId
                : $"{scheduleId}-{scheduleGroup}";
        }
    }
}
