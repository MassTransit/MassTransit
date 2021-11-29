namespace MassTransit
{
    using System;
    using AzureTable;
    using AzureTable.Saga;
    using Microsoft.Azure.Cosmos.Table;


    public static class AzureTableJobServiceConfigurationExtensions
    {
        public static void UseAzureTableSagaRepository(this IJobServiceConfigurator configurator,
            Func<CloudTable> contextFactory,
            ISagaKeyFormatter<JobTypeSaga> jobTypeKeyFormatter,
            ISagaKeyFormatter<JobSaga> jobKeyFormatter,
            ISagaKeyFormatter<JobAttemptSaga> jobAttemptKeyFormatter)
        {
            configurator.Repository = AzureTableSagaRepository<JobTypeSaga>.Create(contextFactory, jobTypeKeyFormatter);

            configurator.JobRepository = AzureTableSagaRepository<JobSaga>.Create(contextFactory, jobKeyFormatter);

            configurator.JobAttemptRepository = AzureTableSagaRepository<JobAttemptSaga>.Create(contextFactory, jobAttemptKeyFormatter);
        }

        public static void UseAzureTableSagaRepository(this IJobServiceConfigurator configurator,
            Func<CloudTable> contextFactory)
        {
            UseAzureTableSagaRepository(configurator, contextFactory,
                new ConstPartitionSagaKeyFormatter<JobTypeSaga>(nameof(JobTypeSaga)),
                new ConstPartitionSagaKeyFormatter<JobSaga>(nameof(JobSaga)),
                new ConstPartitionSagaKeyFormatter<JobAttemptSaga>(nameof(JobAttemptSaga)));
        }
    }
}
