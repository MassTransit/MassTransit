namespace MassTransit.Configuration
{
    using System;
    using Courier;


    /// <summary>
    /// An activity, which must be configured on two separate receive endpoints
    /// </summary>
    public interface IActivityRegistration :
        IRegistration
    {
        void AddConfigureAction<T, TArguments>(Action<IExecuteActivityConfigurator<T, TArguments>> configure)
            where T : class, IExecuteActivity<TArguments>
            where TArguments : class;

        void AddConfigureAction<T, TLog>(Action<ICompensateActivityConfigurator<T, TLog>> configure)
            where T : class, ICompensateActivity<TLog>
            where TLog : class;

        void Configure(IReceiveEndpointConfigurator executeEndpointConfigurator, IReceiveEndpointConfigurator compensateEndpointConfigurator,
            IServiceProvider scopeProvider);

        IActivityDefinition GetDefinition(IServiceProvider provider);

        void ConfigureCompensate(IReceiveEndpointConfigurator configurator, IServiceProvider configurationServiceProvider);

        void ConfigureExecute(IReceiveEndpointConfigurator configurator, IServiceProvider configurationServiceProvider, Uri compensateAddress);
    }
}
