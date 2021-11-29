namespace MassTransit.Configuration
{
    using System;


    public interface ISagaRegistration :
        IRegistration
    {
        void AddConfigureAction<T>(Action<ISagaConfigurator<T>> configure)
            where T : class, ISaga;

        void Configure(IReceiveEndpointConfigurator configurator, IServiceProvider provider);

        ISagaDefinition GetDefinition(IServiceProvider provider);
    }
}
