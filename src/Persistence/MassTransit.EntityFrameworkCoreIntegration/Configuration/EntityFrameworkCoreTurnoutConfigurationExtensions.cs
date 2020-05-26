namespace MassTransit.EntityFrameworkCoreIntegration
{
    using System;
    using MassTransit.Turnout.Components.StateMachines;
    using MassTransit.Turnout.Configuration;
    using Saga;
    using Turnout;


    public static class EntityFrameworkCoreTurnoutConfigurationExtensions
    {
        public static void UseEntityFrameworkCoreSagaRepository(this ITurnoutConfigurator configurator, Func<TurnoutSagaDbContext> contextFactory,
            ILockStatementProvider lockStatementProvider = default)
        {
            configurator.Repository = EntityFrameworkSagaRepository<TurnoutJobTypeState>.CreatePessimistic(contextFactory, lockStatementProvider);

            configurator.JobRepository = EntityFrameworkSagaRepository<TurnoutJobState>.CreatePessimistic(contextFactory, lockStatementProvider);

            configurator.JobAttemptRepository = EntityFrameworkSagaRepository<TurnoutJobAttemptState>.CreatePessimistic(contextFactory, lockStatementProvider);
        }
    }
}
