namespace MassTransit.Configuration
{
    using System;


    public interface IFutureRegistration :
        IRegistration
    {
        void Configure(IReceiveEndpointConfigurator configurator, IServiceProvider provider);

        IFutureDefinition GetDefinition(IServiceProvider provider);
    }
}
