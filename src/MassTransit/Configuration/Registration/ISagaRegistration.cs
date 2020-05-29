namespace MassTransit.Registration
{
    using System;
    using Definition;
    using Saga;


    public interface ISagaRegistration
    {
        void AddConfigureAction<T>(Action<ISagaConfigurator<T>> configure)
            where T : class, ISaga;

        void Configure(IReceiveEndpointConfigurator configurator, IConfigurationServiceProvider repositoryFactory);

        ISagaDefinition GetDefinition(IConfigurationServiceProvider provider);
    }
}
