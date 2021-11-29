namespace MassTransit.DependencyInjection.Registration
{
    using System;
    using Configuration;
    using Courier;


    public class ActivityRegistrationConfigurator<TActivity, TArguments, TLog> :
        IActivityRegistrationConfigurator<TActivity, TArguments, TLog>
        where TActivity : class, IActivity<TArguments, TLog>
        where TArguments : class
        where TLog : class
    {
        readonly IRegistrationConfigurator _configurator;

        public ActivityRegistrationConfigurator(IRegistrationConfigurator configurator)
        {
            _configurator = configurator;
        }

        public void Endpoints(Action<IEndpointRegistrationConfigurator> configureExecute, Action<IEndpointRegistrationConfigurator> configureCompensate)
        {
            ExecuteEndpoint(configureExecute);
            CompensateEndpoint(configureCompensate);
        }

        public IActivityRegistrationConfigurator ExecuteEndpoint(Action<IEndpointRegistrationConfigurator> configureExecute)
        {
            var configurator = new EndpointRegistrationConfigurator<IExecuteActivity<TArguments>> { ConfigureConsumeTopology = false };

            configureExecute?.Invoke(configurator);

            _configurator.AddEndpoint<ExecuteActivityEndpointDefinition<TActivity, TArguments>, IExecuteActivity<TArguments>>(configurator.Settings);

            return this;
        }

        public IActivityRegistrationConfigurator CompensateEndpoint(Action<IEndpointRegistrationConfigurator> configureCompensate)
        {
            var compensateConfigurator = new EndpointRegistrationConfigurator<ICompensateActivity<TLog>> { ConfigureConsumeTopology = false };

            configureCompensate?.Invoke(compensateConfigurator);

            _configurator.AddEndpoint<CompensateActivityEndpointDefinition<TActivity, TLog>, ICompensateActivity<TLog>>(compensateConfigurator.Settings);

            return this;
        }
    }
}
