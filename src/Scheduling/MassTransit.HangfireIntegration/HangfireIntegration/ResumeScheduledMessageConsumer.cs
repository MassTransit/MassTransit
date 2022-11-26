namespace MassTransit.HangfireIntegration
{
    using System.Linq;
    using System.Threading.Tasks;
    using Hangfire;
    using Hangfire.Storage;
    using MassTransit.Scheduling;

    public class ResumeScheduledRecurringMessageConsumer : IConsumer<ResumeScheduledRecurringMessage>
    {
        readonly IRecurringJobManager _recurringJobManager;
        readonly JobStorage _jobStorage;

        public ResumeScheduledRecurringMessageConsumer(IRecurringJobManager recurringJobManager, JobStorage jobStorage)
        {
            _recurringJobManager = recurringJobManager;
            _jobStorage = jobStorage;
        }

        public async Task Consume(ConsumeContext<ResumeScheduledRecurringMessage> context)
        {
            var jobKey = JobKey.Create(context.Message.ScheduleId, context.Message.ScheduleGroup);

            using var connection = _jobStorage.GetConnection();

            var recurringJob = connection.GetRecurringJobs(new[] { jobKey }).FirstOrDefault();

            if (recurringJob == null)
            {
                LogContext.Warning?.Log("Job not found. ResumeScheduledRecurringMessage: {ScheduleId}/{ScheduleGroup} at {Timestamp}", context.Message.ScheduleId,
                    context.Message.ScheduleGroup, context.Message.Timestamp);

                return;
            }

            var cron = connection.GetJobParameter(jobKey, "MT-OriginalCron");

            _recurringJobManager.AddOrUpdate(jobKey, recurringJob.Job, cron);

            LogContext.Debug?.Log("ResumeScheduledRecurringMessage: {ScheduleId}/{ScheduleGroup} at {Timestamp}", context.Message.ScheduleId,
                context.Message.ScheduleGroup, context.Message.Timestamp);
        }
    }
}
