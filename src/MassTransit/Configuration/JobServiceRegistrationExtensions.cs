#nullable enable
namespace MassTransit
{
    using System;
    using Configuration;
    using DependencyInjection.Registration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;


    public static class JobServiceRegistrationExtensions
    {
        /// <summary>
        /// Set the job consumer options (optional, not required to use job consumers)
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure">Configure the job consumer options using this callback</param>
        /// <returns></returns>
        public static IJobServiceRegistrationConfigurator SetJobConsumerOptions(this IBusRegistrationConfigurator configurator,
            Action<JobConsumerOptions>? configure = null)
        {
            var registration = configurator.RegisterJobService(configurator.Registrar);

            registration.AddConfigureAction(configure);

            var registrationConfigurator = new JobServiceRegistrationConfigurator(configurator, registration);

            return registrationConfigurator;
        }

        /// <summary>
        /// Add registrations for the job service saga state machines
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure">Configure the job saga options</param>
        public static IJobSagaRegistrationConfigurator AddJobSagaStateMachines(this IBusRegistrationConfigurator configurator,
            Action<JobSagaOptions>? configure = null)
        {
            var registrationConfigurator = new JobSagaRegistrationConfigurator(configurator, configure);

            return registrationConfigurator;
        }

        /// <summary>
        /// Register a custom job distribution strategy for the job saga state machines
        /// </summary>
        /// <param name="services"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IServiceCollection TryAddJobDistributionStrategy<T>(this IServiceCollection services)
            where T : class, IJobDistributionStrategy
        {
            services.TryAddScoped<IJobDistributionStrategy, T>();

            return services;
        }
    }
}
