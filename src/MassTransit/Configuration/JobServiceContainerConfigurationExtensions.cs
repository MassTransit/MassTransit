namespace MassTransit
{
    using System;
    using DependencyInjection;


    public static class JobServiceContainerConfigurationExtensions
    {
        /// <summary>
        /// Configure the job server saga repositories to resolve from the container.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="context">The bus registration context provided during configuration</param>
        /// <returns></returns>
        public static IJobServiceConfigurator ConfigureSagaRepositories(this IJobServiceConfigurator configurator, IRegistrationContext context)
        {
            configurator.Repository = new DependencyInjectionSagaRepository<JobTypeSaga>(context);
            configurator.JobRepository = new DependencyInjectionSagaRepository<JobSaga>(context);
            configurator.JobAttemptRepository = new DependencyInjectionSagaRepository<JobAttemptSaga>(context);

            return configurator;
        }

        [Obsolete(
            "Use the IRegistrationContext overload to ensure message scope is properly handled. For more information, visit https://masstransit.io/support/upgrade#version-8.1")]
        /// <summary>
        /// Configure the job server saga repositories to resolve from the container.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="provider">The bus registration context provided during configuration</param>
        /// <returns></returns>
        public static IJobServiceConfigurator ConfigureSagaRepositories(this IJobServiceConfigurator configurator, IServiceProvider provider)
        {
            configurator.Repository = new DependencyInjectionSagaRepository<JobTypeSaga>(provider, LegacySetScopedConsumeContext.Instance);
            configurator.JobRepository = new DependencyInjectionSagaRepository<JobSaga>(provider, LegacySetScopedConsumeContext.Instance);
            configurator.JobAttemptRepository = new DependencyInjectionSagaRepository<JobAttemptSaga>(provider, LegacySetScopedConsumeContext.Instance);

            return configurator;
        }
    }
}
