namespace MassTransit.Registration
{
    using System;


    public interface IFutureRegistration
    {
        Type FutureType { get; }

        void Configure(IReceiveEndpointConfigurator configurator, IConfigurationServiceProvider repositoryFactory);

        IFutureDefinition GetDefinition(IConfigurationServiceProvider provider);
    }
}
