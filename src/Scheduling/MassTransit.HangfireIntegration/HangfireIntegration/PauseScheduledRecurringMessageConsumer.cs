namespace MassTransit.HangfireIntegration
{
    using System.Linq;
    using System.Threading.Tasks;
    using Hangfire;
    using Hangfire.Storage;
    using Scheduling;


    public class PauseScheduledRecurringMessageConsumer :
        IConsumer<PauseScheduledRecurringMessage>
    {
        readonly JobStorage _jobStorage;
        readonly IRecurringJobManager _recurringJobManager;

        public PauseScheduledRecurringMessageConsumer(IRecurringJobManager recurringJobManager, JobStorage jobStorage)
        {
            _recurringJobManager = recurringJobManager;
            _jobStorage = jobStorage;
        }

        public Task Consume(ConsumeContext<PauseScheduledRecurringMessage> context)
        {
            var jobKey = JobKey.Create(context.Message.ScheduleId, context.Message.ScheduleGroup);

            using var connection = _jobStorage.GetConnection();

            var recurringJob = connection.GetRecurringJobs(new[] { jobKey }).FirstOrDefault();

            if (recurringJob == null)
            {
                LogContext.Warning?.Log("Job not found. PauseScheduledRecurringMessage: {ScheduleId}/{ScheduleGroup} at {Timestamp}",
                    context.Message.ScheduleId,
                    context.Message.ScheduleGroup, context.Message.Timestamp);

                return Task.CompletedTask;
            }

            using var transaction = connection.CreateWriteTransaction();

            connection.SetJobParameter(jobKey, "MT-OriginalCron", recurringJob.Cron);

            _recurringJobManager.AddOrUpdate(jobKey, recurringJob.Job, Cron.Never());

            transaction.Commit();

            LogContext.Debug?.Log("PauseScheduledRecurringMessage: {ScheduleId}/{ScheduleGroup} at {Timestamp}", context.Message.ScheduleId,
                context.Message.ScheduleGroup, context.Message.Timestamp);

            return Task.CompletedTask;
        }
    }
}
