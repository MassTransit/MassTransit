namespace MassTransit.HangfireIntegration
{
    using System.Linq;
    using System.Threading.Tasks;
    using Hangfire;
    using Hangfire.Storage;
    using MassTransit.Scheduling;


    public class PauseScheduledRecurringMessageConsumer : IConsumer<PauseScheduledRecurringMessage>
    {
        readonly IRecurringJobManager _recurringJobManager;
        readonly JobStorage _jobStorage;

        public PauseScheduledRecurringMessageConsumer(IRecurringJobManager recurringJobManager, JobStorage jobStorage)
        {
            _recurringJobManager = recurringJobManager;
            _jobStorage = jobStorage;
        }

        public async Task Consume(ConsumeContext<PauseScheduledRecurringMessage> context)
        {
            var jobKey = JobKey.Create(context.Message.ScheduleId, context.Message.ScheduleGroup);

            using var connection = _jobStorage.GetConnection();

            var recurringJob = connection.GetRecurringJobs(new[] { jobKey }).FirstOrDefault();

            if (recurringJob == null)
            {
                LogContext.Warning?.Log("Job not found. PauseScheduledRecurringMessage: {ScheduleId}/{ScheduleGroup} at {Timestamp}", context.Message.ScheduleId,
                    context.Message.ScheduleGroup, context.Message.Timestamp);

                return;
            }

            using var transaction = connection.CreateWriteTransaction();

            connection.SetJobParameter(jobKey, "MT-OriginalCron", recurringJob.Cron);

            _recurringJobManager.AddOrUpdate(jobKey, recurringJob.Job, Cron.Never());

            transaction.Commit();

            LogContext.Debug?.Log("PauseScheduledRecurringMessage: {ScheduleId}/{ScheduleGroup} at {Timestamp}", context.Message.ScheduleId,
                context.Message.ScheduleGroup, context.Message.Timestamp);
        }
    }
}
