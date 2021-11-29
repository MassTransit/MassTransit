namespace MassTransit.Configuration
{
    using System;


    public interface IConsumerRegistration :
        IRegistration
    {
        void AddConfigureAction<T>(Action<IConsumerConfigurator<T>> configure)
            where T : class, IConsumer;

        void Configure(IReceiveEndpointConfigurator configurator, IServiceProvider scopeProvider);

        IConsumerDefinition GetDefinition(IServiceProvider provider);

        IConsumerRegistrationConfigurator GetConsumerRegistrationConfigurator(IRegistrationConfigurator registrationConfigurator);
    }
}
