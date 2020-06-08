namespace MassTransit.AutofacIntegration
{
    using System;
    using Autofac;


    public interface IContainerBuilderBusConfigurator :
        IBusRegistrationConfigurator
    {
        ContainerBuilder Builder { get; }

        string ScopeName { set; }

        Action<ContainerBuilder, ConsumeContext> ConfigureScope { set; }
    }
}
