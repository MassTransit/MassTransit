namespace MassTransit.Registration
{
    using System;
    using ConsumeConfigurators;
    using Definition;


    public interface IConsumerRegistration
    {
        Type ConsumerType { get; }

        void Configure(IReceiveEndpointConfigurator configurator, IConfigurationServiceProvider scopeProvider);

        IConsumerDefinition GetDefinition(IConfigurationServiceProvider provider);
    }


    public interface IConsumerRegistration<T> :
        IConsumerRegistration
        where T : class, IConsumer
    {
        void AddConfigureAction(Action<IConsumerConfigurator<T>> configure);
    }


    public interface IConsumerConfiguratorAction<T>
        where T : class, IConsumer
    {
        void Configure(IConsumerConfigurator<T> configurator);
    }
}
