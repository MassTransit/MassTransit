namespace MassTransit.Registration
{
    using System;
    using Courier;
    using Definition;


    /// <summary>
    /// An activity, which must be configured on two separate receive endpoints
    /// </summary>
    public interface IActivityRegistration
    {
        void AddConfigureAction<T, TArguments>(Action<IExecuteActivityConfigurator<T, TArguments>> configure)
            where T : class, IExecuteActivity<TArguments>
            where TArguments : class;

        void AddConfigureAction<T, TLog>(Action<ICompensateActivityConfigurator<T, TLog>> configure)
            where T : class, ICompensateActivity<TLog>
            where TLog : class;

        void Configure(IReceiveEndpointConfigurator executeEndpointConfigurator,
            IReceiveEndpointConfigurator compensateEndpointConfigurator, IConfigurationServiceProvider scopeProvider);

        IActivityDefinition GetDefinition(IConfigurationServiceProvider provider);

        void ConfigureCompensate(IReceiveEndpointConfigurator configurator, IConfigurationServiceProvider configurationServiceProvider);

        void ConfigureExecute(IReceiveEndpointConfigurator configurator, IConfigurationServiceProvider configurationServiceProvider, Uri compensateAddress);
    }
}
