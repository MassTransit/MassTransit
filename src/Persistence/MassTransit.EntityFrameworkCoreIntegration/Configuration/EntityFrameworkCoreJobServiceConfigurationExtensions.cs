namespace MassTransit.EntityFrameworkCoreIntegration
{
    using System;
    using JobService;
    using MassTransit.JobService;
    using MassTransit.JobService.Components.StateMachines;
    using Saga;


    public static class EntityFrameworkCoreJobServiceConfigurationExtensions
    {
        public static void UseEntityFrameworkCoreSagaRepository(this IJobServiceConfigurator configurator, Func<JobServiceSagaDbContext> contextFactory,
            ILockStatementProvider lockStatementProvider = default)
        {
            configurator.Repository = EntityFrameworkSagaRepository<JobTypeSaga>.CreatePessimistic(contextFactory, lockStatementProvider);

            configurator.JobRepository = EntityFrameworkSagaRepository<JobSaga>.CreatePessimistic(contextFactory, lockStatementProvider);

            configurator.JobAttemptRepository = EntityFrameworkSagaRepository<JobAttemptSaga>.CreatePessimistic(contextFactory, lockStatementProvider);
        }
    }
}
