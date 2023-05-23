namespace MassTransit.DependencyInjection.Registration
{
    using System;
    using Configuration;


    public class ExecuteActivityRegistrationConfigurator<TActivity, TArguments> :
        IExecuteActivityRegistrationConfigurator<TActivity, TArguments>
        where TActivity : class, IExecuteActivity<TArguments>
        where TArguments : class
    {
        readonly IRegistrationConfigurator _configurator;
        readonly IExecuteActivityRegistration _registration;

        public ExecuteActivityRegistrationConfigurator(IRegistrationConfigurator configurator, IExecuteActivityRegistration registration)
        {
            _configurator = configurator;
            _registration = registration;
        }

        public void Endpoint(Action<IEndpointRegistrationConfigurator> configure)
        {
            if (!_registration.IncludeInConfigureEndpoints)
                throw new ConfigurationException("ExecuteActivity is excluded from ConfigureEndpoints");

            var configurator = new EndpointRegistrationConfigurator<IExecuteActivity<TArguments>> { ConfigureConsumeTopology = false };

            configure?.Invoke(configurator);

            _configurator.AddEndpoint<ExecuteActivityEndpointDefinition<TActivity, TArguments>, IExecuteActivity<TArguments>>(_registration,
                configurator.Settings);
        }

        public void ExcludeFromConfigureEndpoints()
        {
            _registration.IncludeInConfigureEndpoints = false;
        }
    }
}
