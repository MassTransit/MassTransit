namespace MassTransit.Azure.Table
{
    using System;
    using JobService;
    using JobService.Components.StateMachines;
    using JobService.Configuration;
    using Microsoft.Azure.Cosmos.Table;
    using Saga;


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
            UseAzureTableSagaRepository(configurator,
                contextFactory,
                new ConstPartitionSagaKeyFormatter<JobTypeSaga>(typeof(JobTypeSaga).Name),
                new ConstPartitionSagaKeyFormatter<JobSaga>(typeof(JobSaga).Name),
                new ConstPartitionSagaKeyFormatter<JobAttemptSaga>(typeof(JobAttemptSaga).Name));
        }
    }
}
