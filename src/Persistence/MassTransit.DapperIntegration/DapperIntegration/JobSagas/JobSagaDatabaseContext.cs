#nullable enable
namespace MassTransit.DapperIntegration.JobSagas
{
    using System;
    using Saga;
    using System.Collections.Generic;

    public class JobSagaDatabaseContext : JobSagaBaseContext<JobSaga, DbJobModel>, DatabaseContext<JobSaga>
    {
        public JobSagaDatabaseContext(
            DatabaseContext<DbJobModel> databaseContext,
            DapperSagaSerializer<JobSaga, DbJobModel> serializer
        ) : base(databaseContext, serializer)
        { }
    }

    public class JobSerializer : SystemTextJsonSagaSerializerBase<JobSaga, DbJobModel>
    {
        public override DbJobModel FromSaga(JobSaga instance)
        {
            var model = new DbJobModel
            {
                CorrelationId = instance.CorrelationId,
                CurrentState = instance.CurrentState,
                Completed = instance.Completed,
                Faulted = instance.Faulted,
                Started = instance.Started,
                Submitted = instance.Submitted,
                EndDate = instance.EndDate,
                NextStartDate = instance.NextStartDate,
                StartDate = instance.StartDate,
                AttemptId = instance.AttemptId,
                JobTypeId = instance.JobTypeId,
                JobRetryDelayToken = instance.JobRetryDelayToken,
                JobSlotWaitToken = instance.JobSlotWaitToken,
                RetryAttempt = instance.RetryAttempt,
                LastProgressLimit = instance.LastProgressLimit,
                LastProgressSequenceNumber = instance.LastProgressSequenceNumber,
                LastProgressValue = instance.LastProgressValue,
                CronExpression = instance.CronExpression,
                Reason = instance.Reason,
                TimeZoneId = instance.TimeZoneId,
                Duration = instance.Duration,
                JobTimeout = instance.JobTimeout,
                ServiceAddress = instance.ServiceAddress.ToString(),

                IncompleteAttempts = Serialize(instance.IncompleteAttempts),
                Job = Serialize(instance.Job),
                JobProperties = Serialize(instance.JobProperties),
                JobState = Serialize(instance.JobState),
            };

            return model;
        }

        public override JobSaga? FromModel(DbJobModel? model)
        {
            if (model is null)
                return null;

            var instance = new JobSaga
            {
                CorrelationId = model.CorrelationId,
                CurrentState = model.CurrentState,
                Completed = model.Completed,
                Faulted = model.Faulted,
                Started = model.Started,
                Submitted = model.Submitted,
                EndDate = model.EndDate,
                NextStartDate = model.NextStartDate,
                StartDate = model.StartDate,
                AttemptId = model.AttemptId,
                JobTypeId = model.JobTypeId,
                JobRetryDelayToken = model.JobRetryDelayToken,
                JobSlotWaitToken = model.JobSlotWaitToken,
                RetryAttempt = model.RetryAttempt,
                LastProgressLimit = model.LastProgressLimit,
                LastProgressSequenceNumber = model.LastProgressSequenceNumber,
                LastProgressValue = model.LastProgressValue,
                CronExpression = model.CronExpression,
                Reason = model.Reason,
                TimeZoneId = model.TimeZoneId,
                Duration = model.Duration,
                JobTimeout = model.JobTimeout,
                ServiceAddress = UriOrDefault(model.ServiceAddress),

                IncompleteAttempts = Deserialize<List<Guid>>(model.IncompleteAttempts),
                Job = Deserialize<Dictionary<string, object>>(model.Job),
                JobProperties = Deserialize<Dictionary<string, object>>(model.JobProperties),
                JobState = Deserialize<Dictionary<string, object>>(model.JobState)
            };

            return instance;
        }
    }

    public class DbJobModel : ISaga
    {
        public Guid CorrelationId { get; set; }
        public int CurrentState { get; set; }

        public DateTime? Completed { get; set; }
        public DateTime? Faulted { get; set; }
        public DateTime? Started { get; set; }
        public DateTime? Submitted { get; set; }

        public DateTimeOffset? EndDate { get; set; }
        public DateTimeOffset? NextStartDate { get; set; }
        public DateTimeOffset? StartDate { get; set; }

        public Guid AttemptId { get; set; }
        public Guid JobTypeId { get; set; }
        public Guid? JobRetryDelayToken { get; set; }
        public Guid? JobSlotWaitToken { get; set; }

        public int RetryAttempt { get; set; }
        public long? LastProgressLimit { get; set; }
        public long? LastProgressSequenceNumber { get; set; }
        public long? LastProgressValue { get; set; }
        public string? CronExpression { get; set; }
        public string? Reason { get; set; }
        public string? TimeZoneId { get; set; }

        public TimeSpan? Duration { get; set; }
        public TimeSpan? JobTimeout { get; set; }
        public string? ServiceAddress { get; set; }

        public string? IncompleteAttempts { get; set; }

        public string? Job { get; set; }
        public string? JobProperties { get; set; }
        public string? JobState { get; set; }
    }
}
#nullable restore
