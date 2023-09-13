namespace MassTransit.Configuration
{
    using System;


    /// <summary>
    /// An execute activity, which doesn't have compensation
    /// </summary>
    public interface IExecuteActivityRegistration :
        IRegistration
    {
        void AddConfigureAction<T, TArguments>(Action<IRegistrationContext, IExecuteActivityConfigurator<T, TArguments>> configure)
            where T : class, IExecuteActivity<TArguments>
            where TArguments : class;

        void Configure(IReceiveEndpointConfigurator configurator, IRegistrationContext context);

        IExecuteActivityDefinition GetDefinition(IRegistrationContext context);
    }
}
