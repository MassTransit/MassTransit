namespace MassTransit.HangfireIntegration
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Hangfire;
    using Hangfire.Storage;
    using Scheduling;


    public class ScheduleMessageConsumer :
        IConsumer<ScheduleMessage>,
        IConsumer<CancelScheduledMessage>
    {
        const string JobIdKey = "MT-JobId";
        readonly IBackgroundJobClient _backgroundJobClient;
        readonly JobStorage _jobStorage;

        public ScheduleMessageConsumer(IBackgroundJobClient backgroundJobClient, JobStorage jobStorage)
        {
            _backgroundJobClient = backgroundJobClient;
            _jobStorage = jobStorage;
        }

        public async Task Consume(ConsumeContext<CancelScheduledMessage> context)
        {
            using var connection = _jobStorage.GetConnection();
            using var transaction = connection.CreateWriteTransaction();
            var id = context.Message.TokenId.ToString("N");
            if (TryRemoveJob(connection, id, out var jobId))
            {
                LogContext.Debug?.Log("Canceled Scheduled Message: {Id} at {Timestamp}", jobId, context.Message.Timestamp);
                transaction.RemoveHash(id);
            }
            else
                LogContext.Debug?.Log("CancelScheduledMessage: no message found for {Id}", jobId);

            transaction.Commit();
        }

        public async Task Consume(ConsumeContext<ScheduleMessage> context)
        {
            var id = context.Message.CorrelationId.ToString("N");
            var message = HashedHangfireScheduledMessageData.Create(context, id);

            using var connection = _jobStorage.GetConnection();
            using var transaction = connection.CreateWriteTransaction();
            if (TryRemoveJob(connection, id, out var jobId))
                LogContext.Debug?.Log("Canceled Scheduled Message: {Id}", jobId);

            jobId = _backgroundJobClient.Schedule<ScheduleJob>(x => x.SendMessage(message, null!), context.Message.ScheduledTime);

            transaction.SetRangeInHash(id, new Dictionary<string, string> { [JobIdKey] = jobId });
            LogContext.Debug?.Log("Scheduled: {Id}", jobId);
            transaction.Commit();
        }

        bool TryRemoveJob(IStorageConnection connection, string id, out string? jobId)
        {
            Dictionary<string, string> items = connection.GetAllEntriesFromHash(id);
            if (items != null)
                return items.TryGetValue(JobIdKey, out jobId) && _backgroundJobClient.Delete(jobId);

            jobId = null;
            return false;
        }
    }
}
