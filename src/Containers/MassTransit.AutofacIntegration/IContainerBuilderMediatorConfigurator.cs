namespace MassTransit.AutofacIntegration
{
    using System;
    using Autofac;


    public interface IContainerBuilderMediatorConfigurator :
        IMediatorRegistrationConfigurator
    {
        ContainerBuilder Builder { get; }

        string ScopeName { set; }

        Action<ContainerBuilder, ConsumeContext> ConfigureScope { set; }
    }
}
