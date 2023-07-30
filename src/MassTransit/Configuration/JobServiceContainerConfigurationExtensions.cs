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

        /// <summary>
        /// Configure the job server saga repositories to resolve from the container.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="provider">The bus registration context provided during configuration</param>
        /// <returns></returns>
        [Obsolete("Use the IRegistrationContext overload instead. Visit https://masstransit.io/obsolete for details.")]
        public static IJobServiceConfigurator ConfigureSagaRepositories(this IJobServiceConfigurator configurator, IServiceProvider provider)
        {
            configurator.Repository = new DependencyInjectionSagaRepository<JobTypeSaga>(provider, LegacySetScopedConsumeContext.Instance);
            configurator.JobRepository = new DependencyInjectionSagaRepository<JobSaga>(provider, LegacySetScopedConsumeContext.Instance);
            configurator.JobAttemptRepository = new DependencyInjectionSagaRepository<JobAttemptSaga>(provider, LegacySetScopedConsumeContext.Instance);

            return configurator;
        }
    }
}
