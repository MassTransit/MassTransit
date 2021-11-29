namespace MassTransit.Configuration
{
    using System;
    using Courier;


    /// <summary>
    /// An execute activity, which doesn't have compensation
    /// </summary>
    public interface IExecuteActivityRegistration :
        IRegistration
    {
        void AddConfigureAction<T, TArguments>(Action<IExecuteActivityConfigurator<T, TArguments>> configure)
            where T : class, IExecuteActivity<TArguments>
            where TArguments : class;

        void Configure(IReceiveEndpointConfigurator configurator, IServiceProvider scopeProvider);

        IExecuteActivityDefinition GetDefinition(IServiceProvider provider);
    }
}
