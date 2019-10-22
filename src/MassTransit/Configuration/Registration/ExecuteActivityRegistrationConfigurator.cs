namespace MassTransit.Registration
{
    using System;
    using Courier;
    using Definition;


    public class ExecuteActivityRegistrationConfigurator<TActivity, TArguments> :
        IExecuteActivityRegistrationConfigurator<TActivity, TArguments>
        where TActivity : class, IExecuteActivity<TArguments>
        where TArguments : class
    {
        readonly IRegistrationConfigurator _configurator;
        readonly IExecuteActivityRegistration _registration;
        readonly IContainerRegistrar _registrar;

        public ExecuteActivityRegistrationConfigurator(IRegistrationConfigurator configurator, IExecuteActivityRegistration registration,
            IContainerRegistrar registrar)
        {
            _configurator = configurator;
            _registration = registration;
            _registrar = registrar;
        }

        public void Endpoint(Action<IExecuteActivityEndpointRegistrationConfigurator<TActivity, TArguments>> configure)
        {
            var configurator = new ExecuteActivityEndpointRegistrationConfigurator<TActivity, TArguments>();

            configure?.Invoke(configurator);

            _configurator.AddEndpoint<ExecuteActivityEndpointDefinition<TActivity, TArguments>, IExecuteActivity<TArguments>>(configurator.Settings);

            _registrar.RegisterExecuteActivityDefinition<EndpointExecuteActivityDefinition<TActivity, TArguments>, TActivity, TArguments>();
        }
    }
}
