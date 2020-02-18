namespace MassTransit
{
    using System;
    using Configurators;
    using EntityFrameworkIntegration;
    using EntityFrameworkIntegration.Configurators;
    using Saga;


    public static class EntityFrameworkSagaRepositoryRegistrationExtensions
    {
        /// <summary>
        /// Adds a EntityFramework saga repository to the registration
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        /// <typeparam name="TSaga"></typeparam>
        /// <returns></returns>
        public static ISagaRegistrationConfigurator<TSaga> EntityFrameworkRepository<TSaga>(this ISagaRegistrationConfigurator<TSaga> configurator,
            Action<IEntityFrameworkSagaRepositoryConfigurator<TSaga>> configure)
            where TSaga : class, ISaga
        {
            var entityFrameworkSagaRepositoryConfigurator = new EntityFrameworkSagaRepositoryConfigurator<TSaga>();

            configure?.Invoke(entityFrameworkSagaRepositoryConfigurator);

            BusConfigurationResult.CompileResults(entityFrameworkSagaRepositoryConfigurator.Validate());

            configurator.Repository(x => entityFrameworkSagaRepositoryConfigurator.Register(x));

            return configurator;
        }

    }
}
