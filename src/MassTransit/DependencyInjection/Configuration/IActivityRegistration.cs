namespace MassTransit.Configuration
{
    using System;


    /// <summary>
    /// An activity, which must be configured on two separate receive endpoints
    /// </summary>
    public interface IActivityRegistration :
        IRegistration
    {
        void AddConfigureAction<T, TArguments>(Action<IRegistrationContext, IExecuteActivityConfigurator<T, TArguments>> configure)
            where T : class, IExecuteActivity<TArguments>
            where TArguments : class;

        void AddConfigureAction<T, TLog>(Action<IRegistrationContext, ICompensateActivityConfigurator<T, TLog>> configure)
            where T : class, ICompensateActivity<TLog>
            where TLog : class;

        void Configure(IReceiveEndpointConfigurator executeEndpointConfigurator, IReceiveEndpointConfigurator compensateEndpointConfigurator,
            IRegistrationContext context);

        IActivityDefinition GetDefinition(IRegistrationContext context);

        void ConfigureCompensate(IReceiveEndpointConfigurator configurator, IRegistrationContext context);

        void ConfigureExecute(IReceiveEndpointConfigurator configurator, IRegistrationContext context, Uri compensateAddress);
    }
}
