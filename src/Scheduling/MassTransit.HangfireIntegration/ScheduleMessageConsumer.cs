namespace MassTransit.HangfireIntegration
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Context;
    using Hangfire;
    using Hangfire.Storage;
    using Scheduling;


    public class ScheduleMessageConsumer :
        IConsumer<ScheduleMessage>,
        IConsumer<CancelScheduledMessage>
    {
        readonly IBackgroundJobClient _backgroundJobClient;
        readonly JobStorage _jobStorage;
        const string JobIdKey = "MT-JobId";

        public ScheduleMessageConsumer(IHangfireComponentResolver hangfireComponentResolver)
            : this(hangfireComponentResolver.BackgroundJobClient, hangfireComponentResolver.JobStorage)
        {
        }

        ScheduleMessageConsumer(IBackgroundJobClient backgroundJobClient, JobStorage jobStorage)
        {
            _backgroundJobClient = backgroundJobClient;
            _jobStorage = jobStorage;
        }

        bool TryRemoveJob(IStorageConnection connection, string id, out string jobId)
        {
            Dictionary<string, string> items = connection.GetAllEntriesFromHash(id);
            if (items != null)
                return items.TryGetValue(JobIdKey, out jobId) && _backgroundJobClient.Delete(jobId);
            jobId = null;
            return false;

        }

        public async Task Consume(ConsumeContext<ScheduleMessage> context)
        {
            var correlationId = context.Message.CorrelationId.ToString("N");
            var message = HangfireScheduledMessageData.Create(context);

            using var connection = _jobStorage.GetConnection();
            using var transaction = connection.CreateWriteTransaction();
            if (TryRemoveJob(connection, correlationId, out var jobId))
                LogContext.Debug?.Log("Cancelled Scheduled Message: {Id}", jobId);
            jobId = _backgroundJobClient.Schedule<ScheduleJob>(
                x => x.SendMessage(message, null),
                context.Message.ScheduledTime);
            connection.SetRangeInHash(correlationId, new[] {new KeyValuePair<string, string>(JobIdKey, jobId)});
            LogContext.Debug?.Log("Scheduled: {Id}", jobId);
        }

        public async Task Consume(ConsumeContext<CancelScheduledMessage> context)
        {
            using var connection = _jobStorage.GetConnection();
            var correlationId = context.Message.TokenId.ToString("N");
            if (TryRemoveJob(connection, correlationId, out var jobId))
                LogContext.Debug?.Log("Cancelled Scheduled Message: {Id} at {Timestamp}", jobId, context.Message.Timestamp);
            else
                LogContext.Debug?.Log("CancelScheduledMessage: no message found for {Id}", jobId);
        }
    }
}
