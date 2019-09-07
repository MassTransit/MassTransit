namespace MassTransit.Registration
{
    using System;
    using Courier;
    using Definition;


    /// <summary>
    /// An execute activity, which doesn't have compensation
    /// </summary>
    public interface IExecuteActivityRegistration
    {
        void AddConfigureAction<T, TArguments>(Action<IExecuteActivityConfigurator<T, TArguments>> configure)
            where T : class, IExecuteActivity<TArguments>
            where TArguments : class;

        void Configure(IReceiveEndpointConfigurator configurator, IConfigurationServiceProvider scopeProvider);

        IExecuteActivityDefinition GetDefinition(IConfigurationServiceProvider provider);
    }
}
