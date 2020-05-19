namespace MassTransit.AutofacIntegration
{
    using System;
    using Autofac;


    public interface IContainerBuilderMediatorConfigurator :
        IMediatorConfigurator<IComponentContext>
    {
        ContainerBuilder Builder { get; }

        string ScopeName { set; }

        Action<ContainerBuilder, ConsumeContext> ConfigureScope { set; }
    }
}
