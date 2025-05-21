#nullable enable

namespace MassTransit.DapperIntegration.JobSagas
{
    using System;
    using Saga;

    public class JobAttemptSagaDatabaseContext : JobSagaBaseContext<JobAttemptSaga, DbJobAttemptModel>, DatabaseContext<JobAttemptSaga>
    {
        public JobAttemptSagaDatabaseContext(
            DatabaseContext<DbJobAttemptModel> databaseContext, 
            DapperSagaSerializer<JobAttemptSaga, DbJobAttemptModel> serializer
        ) : base(databaseContext, serializer)
        { }
    }

    public class JobAttemptSerializer : SystemTextJsonSagaSerializerBase<JobAttemptSaga, DbJobAttemptModel>
    {
        public override DbJobAttemptModel FromSaga(JobAttemptSaga instance)
        {
            var model = new DbJobAttemptModel
            {
                CorrelationId = instance.CorrelationId,
                CurrentState = instance.CurrentState,
                JobId = instance.JobId,
                Started = instance.Started,
                Faulted = instance.Faulted,
                StatusCheckTokenId = instance.StatusCheckTokenId,
                RetryAttempt = instance.RetryAttempt,
                ServiceAddress = instance.ServiceAddress?.ToString(),
                InstanceAddress = instance.InstanceAddress?.ToString(),
            };

            return model;
        }

        public override JobAttemptSaga? FromModel(DbJobAttemptModel? model)
        {
            if (model is null)
                return null;

            var instance = new JobAttemptSaga
            {
                CorrelationId = model.CorrelationId,
                CurrentState = model.CurrentState,
                JobId = model.JobId,
                Started = model.Started,
                Faulted = model.Faulted,
                StatusCheckTokenId = model.StatusCheckTokenId,
                RetryAttempt = model.RetryAttempt,
                ServiceAddress = UriOrDefault(model.ServiceAddress),
                InstanceAddress = UriOrDefault(model.InstanceAddress),
            };

            return instance;
        }
    }

    public class DbJobAttemptModel : ISaga
    {
        public Guid CorrelationId { get; set; }
        public int CurrentState { get; set; }

        public Guid JobId { get; set; }

        public DateTime? Started { get; set; }
        public DateTime? Faulted { get; set; }
        public Guid? StatusCheckTokenId { get; set; }

        public int RetryAttempt { get; set; }
        public string? ServiceAddress { get; set; }
        public string? InstanceAddress { get; set; }
    }
}
#nullable restore
