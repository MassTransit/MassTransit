namespace MassTransit.HangfireIntegration
{
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Hangfire;
    using Scheduling;


    public class ScheduleRecurringMessageConsumer :
        IConsumer<ScheduleRecurringMessage>,
        IConsumer<CancelScheduledRecurringMessage>
    {
        readonly IRecurringJobManager _recurringJobManager;
        readonly ITimeZoneResolver _timeZoneResolver;

        ScheduleRecurringMessageConsumer(IRecurringJobManager recurringJobManager, ITimeZoneResolver timeZoneResolver)
        {
            _recurringJobManager = recurringJobManager;
            _timeZoneResolver = timeZoneResolver;
        }

        public ScheduleRecurringMessageConsumer(IHangfireComponentResolver hangfireComponentResolver)
            : this(hangfireComponentResolver.RecurringJobManager, hangfireComponentResolver.TimeZoneResolver)
        {
        }

        static string GetJobKey(string scheduleId, string scheduleGroup) => $"{scheduleId}-{scheduleGroup}";

        public async Task Consume(ConsumeContext<ScheduleRecurringMessage> context)
        {
            var jobKey = GetJobKey(context.Message.Schedule.ScheduleId, context.Message.Schedule.ScheduleGroup);
            var message = HangfireSerializedMessage.Create(context, jobKey);
            var tz = _timeZoneResolver.GetTimeZoneById(context.Message.Schedule.TimeZoneId);
            _recurringJobManager.AddOrUpdate<ScheduleJob>(
                jobKey,
                x => x.SendMessage(message, CancellationToken.None),
                context.Message.Schedule.CronExpression,
                tz);

            LogContext.Debug?.Log("Scheduled: {Key}", jobKey);
        }

        public async Task Consume(ConsumeContext<CancelScheduledRecurringMessage> context)
        {
            var jobKey = GetJobKey(context.Message.ScheduleId, context.Message.ScheduleGroup);
            _recurringJobManager.RemoveIfExists(jobKey);

            LogContext.Debug?.Log("Cancelled Recurring Message: {Key}", jobKey);
        }
    }
}
