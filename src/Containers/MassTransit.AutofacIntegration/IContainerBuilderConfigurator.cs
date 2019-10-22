namespace MassTransit.AutofacIntegration
{
    using System;
    using Autofac;


    public interface IContainerBuilderConfigurator :
        IRegistrationConfigurator
    {
        ContainerBuilder Builder { get; }

        string ScopeName { set; }

        Action<ContainerBuilder, ConsumeContext> ConfigureScope { set; }

        /// <summary>
        /// Add the bus to the container, configured properly
        /// </summary>
        /// <param name="busFactory"></param>
        void AddBus(Func<IComponentContext, IBusControl> busFactory);
    }
}
