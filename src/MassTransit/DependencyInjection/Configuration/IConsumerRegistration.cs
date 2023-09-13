namespace MassTransit.Configuration
{
    using System;


    public interface IConsumerRegistration :
        IRegistration
    {
        void AddConfigureAction<T>(Action<IRegistrationContext, IConsumerConfigurator<T>> configure)
            where T : class, IConsumer;

        void Configure(IReceiveEndpointConfigurator configurator, IRegistrationContext context);

        IConsumerDefinition GetDefinition(IRegistrationContext context);

        IConsumerRegistrationConfigurator GetConsumerRegistrationConfigurator(IRegistrationConfigurator registrationConfigurator);
    }
}
