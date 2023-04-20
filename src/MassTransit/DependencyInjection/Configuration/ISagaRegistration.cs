namespace MassTransit.Configuration
{
    using System;


    public interface ISagaRegistration :
        IRegistration
    {
        void AddConfigureAction<T>(Action<ISagaConfigurator<T>> configure)
            where T : class, ISaga;

        void Configure(IReceiveEndpointConfigurator configurator, IRegistrationContext context);

        ISagaDefinition GetDefinition(IRegistrationContext context);
    }
}
