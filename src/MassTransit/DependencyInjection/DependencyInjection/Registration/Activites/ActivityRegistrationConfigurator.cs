namespace MassTransit.DependencyInjection.Registration
{
    using System;
    using Configuration;


    public class ActivityRegistrationConfigurator<TActivity, TArguments, TLog> :
        IActivityRegistrationConfigurator<TActivity, TArguments, TLog>
        where TActivity : class, IActivity<TArguments, TLog>
        where TArguments : class
        where TLog : class
    {
        readonly IRegistrationConfigurator _configurator;
        readonly IActivityRegistration _registration;

        public ActivityRegistrationConfigurator(IRegistrationConfigurator configurator, IActivityRegistration registration)
        {
            _configurator = configurator;
            _registration = registration;
        }

        public void Endpoints(Action<IEndpointRegistrationConfigurator> configureExecute, Action<IEndpointRegistrationConfigurator> configureCompensate)
        {
            ExecuteEndpoint(configureExecute);
            CompensateEndpoint(configureCompensate);
        }

        public IActivityRegistrationConfigurator ExecuteEndpoint(Action<IEndpointRegistrationConfigurator> configureExecute)
        {
            if (!_registration.IncludeInConfigureEndpoints)
                throw new ConfigurationException("Activity is excluded from ConfigureEndpoints");

            var configurator = new EndpointRegistrationConfigurator<IExecuteActivity<TArguments>> { ConfigureConsumeTopology = false };

            configureExecute?.Invoke(configurator);

            _configurator.AddEndpoint<ExecuteActivityEndpointDefinition<TActivity, TArguments>, IExecuteActivity<TArguments>>(_registration,
                configurator.Settings);

            return this;
        }

        public IActivityRegistrationConfigurator CompensateEndpoint(Action<IEndpointRegistrationConfigurator> configureCompensate)
        {
            if (!_registration.IncludeInConfigureEndpoints)
                throw new ConfigurationException("Activity is excluded from ConfigureEndpoints");

            var compensateConfigurator = new EndpointRegistrationConfigurator<ICompensateActivity<TLog>> { ConfigureConsumeTopology = false };

            configureCompensate?.Invoke(compensateConfigurator);

            _configurator.AddEndpoint<CompensateActivityEndpointDefinition<TActivity, TLog>, ICompensateActivity<TLog>>(_registration,
                compensateConfigurator.Settings);

            return this;
        }

        public void ExcludeFromConfigureEndpoints()
        {
            _registration.IncludeInConfigureEndpoints = false;
        }
    }
}
