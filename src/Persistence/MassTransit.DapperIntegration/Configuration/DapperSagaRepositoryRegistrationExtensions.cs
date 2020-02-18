namespace MassTransit.DapperIntegration
{
    using System;
    using Configurators;
    using MassTransit.Configurators;
    using Saga;


    public static class DapperSagaRepositoryRegistrationExtensions
    {
        /// <summary>
        /// Adds a Redis saga repository to the registration
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="connectionString"></param>
        /// <param name="configure"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static ISagaRegistrationConfigurator<T> DapperRepository<T>(this ISagaRegistrationConfigurator<T> configurator,
            string connectionString, Action<IDapperSagaRepositoryConfigurator<T>> configure = null)
            where T : class, ISaga
        {
            var repositoryConfigurator = new DapperSagaRepositoryConfigurator<T>(connectionString);

            configure?.Invoke(repositoryConfigurator);

            BusConfigurationResult.CompileResults(repositoryConfigurator.Validate());

            configurator.Repository(x => repositoryConfigurator.Register(x));

            return configurator;
        }
    }
}
