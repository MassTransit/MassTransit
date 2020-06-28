namespace MassTransit.Azure.Table
{
    using System;
    using JobService.Components.StateMachines;
    using JobService.Configuration;
    using Microsoft.Azure.Cosmos.Table;
    using Saga;


    public static class AzureTableJobServiceConfigurationExtensions
    {
        public static void UseAzureTableSagaRepository(this IJobServiceConfigurator configurator,
            Func<CloudTable> contextFactory)
        {
            configurator.Repository = AzureTableSagaRepository<JobTypeSaga>.Create(contextFactory);

            configurator.JobRepository = AzureTableSagaRepository<JobSaga>.Create(contextFactory);

            configurator.JobAttemptRepository = AzureTableSagaRepository<JobAttemptSaga>.Create(contextFactory);
        }
    }
}
