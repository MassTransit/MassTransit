#nullable enable

namespace MassTransit.DapperIntegration.JobSagas
{
    using System.Collections.Generic;
    using System;
    using Saga;

    public class JobTypeSagaDatabaseContext : JobSagaBaseContext<JobTypeSaga, DbJobTypeModel>, DatabaseContext<JobTypeSaga>
    {
        public JobTypeSagaDatabaseContext(
            DatabaseContext<DbJobTypeModel> databaseContext,
            DapperSagaSerializer<JobTypeSaga, DbJobTypeModel> serializer
        ) : base(databaseContext, serializer)
        { }
    }

    public class JobTypeSerializer : SystemTextJsonSagaSerializerBase<JobTypeSaga, DbJobTypeModel>
    {
        public override DbJobTypeModel FromSaga(JobTypeSaga instance)
        {
            return new DbJobTypeModel
            {
                CorrelationId = instance.CorrelationId,

                Name = instance.Name,
                CurrentState = instance.CurrentState,
                ActiveJobCount = instance.ActiveJobCount,
                ConcurrentJobLimit = instance.ConcurrentJobLimit,
                OverrideJobLimit = instance.OverrideJobLimit,
                OverrideLimitExpiration = instance.OverrideLimitExpiration,
                GlobalConcurrentJobLimit = instance.GlobalConcurrentJobLimit,

                ActiveJobs = Serialize(instance.ActiveJobs),
                Instances = Serialize(instance.Instances),
                Properties = Serialize(instance.Properties),
            };
        }

        public override JobTypeSaga? FromModel(DbJobTypeModel? model)
        {
            if (model is null)
                return null;

            return new JobTypeSaga
            {
                CorrelationId = model.CorrelationId,

                Name = model.Name!,
                CurrentState = model.CurrentState,
                ActiveJobCount = model.ActiveJobCount,
                ConcurrentJobLimit = model.ConcurrentJobLimit,
                OverrideJobLimit = model.OverrideJobLimit,
                OverrideLimitExpiration = model.OverrideLimitExpiration,
                GlobalConcurrentJobLimit = model.GlobalConcurrentJobLimit,

                ActiveJobs = Deserialize<List<ActiveJob>>(model.ActiveJobs),
                Instances = Deserialize<Dictionary<Uri, JobTypeInstance>>(model.Instances),
                Properties = Deserialize<Dictionary<string, object>>(model.Properties),
            };
        }
    }

    public class DbJobTypeModel : ISaga
    {
        public Guid CorrelationId { get; set; }
        public string? Name { get; set; }
        public int CurrentState { get; set; }

        public int ActiveJobCount { get; set; }
        public int ConcurrentJobLimit { get; set; }
        public int? OverrideJobLimit { get; set; }
        public DateTime? OverrideLimitExpiration { get; set; }
        public int? GlobalConcurrentJobLimit { get; set; }

        public string? ActiveJobs { get; set; }
        public string? Instances { get; set; }
        public string? Properties { get; set; }
    }
}
#nullable restore
