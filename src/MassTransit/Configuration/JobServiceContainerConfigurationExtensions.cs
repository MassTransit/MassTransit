namespace MassTransit
{
    using System;
    using Microsoft.Extensions.DependencyInjection;


    public static class JobServiceContainerConfigurationExtensions
    {
        /// <summary>
        /// Configure the job server saga repositories to resolve from the container.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="provider">The bus registration context provided during configuration</param>
        /// <returns></returns>
        public static IJobServiceConfigurator ConfigureSagaRepositories(this IJobServiceConfigurator configurator, IServiceProvider provider)
        {
            configurator.Repository = provider.GetRequiredService<ISagaRepository<JobTypeSaga>>();
            configurator.JobRepository = provider.GetRequiredService<ISagaRepository<JobSaga>>();
            configurator.JobAttemptRepository = provider.GetRequiredService<ISagaRepository<JobAttemptSaga>>();

            return configurator;
        }
    }
}
