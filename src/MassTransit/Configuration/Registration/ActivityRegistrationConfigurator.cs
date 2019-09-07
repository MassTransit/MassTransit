namespace MassTransit.Registration
{
    using System;
    using Courier;
    using Definition;


    public class ActivityRegistrationConfigurator<TActivity, TArguments, TLog> :
        IActivityRegistrationConfigurator<TActivity, TArguments, TLog>
        where TActivity : class, IActivity<TArguments, TLog>
        where TArguments : class
        where TLog : class
    {
        readonly IRegistrationConfigurator _configurator;
        readonly IActivityRegistration _registration;
        readonly IContainerRegistrar _registrar;

        public ActivityRegistrationConfigurator(IRegistrationConfigurator configurator, IActivityRegistration registration,
            IContainerRegistrar registrar)
        {
            _configurator = configurator;
            _registration = registration;
            _registrar = registrar;
        }

        public void Endpoints(Action<IExecuteActivityEndpointRegistrationConfigurator<TActivity, TArguments>> configureExecute,
            Action<ICompensateActivityEndpointRegistrationConfigurator<TActivity, TLog>> configureCompensate)
        {
            var configurator = new ExecuteActivityEndpointRegistrationConfigurator<TActivity, TArguments>();

            configureExecute?.Invoke(configurator);

            _configurator.AddEndpoint<ExecuteActivityEndpointDefinition<TActivity, TArguments>, IExecuteActivity<TArguments>>(configurator.Settings);

            var compensateConfigurator = new CompensateActivityEndpointRegistrationConfigurator<TActivity, TLog>();

            configureCompensate?.Invoke(compensateConfigurator);

            _configurator.AddEndpoint<CompensateActivityEndpointDefinition<TActivity, TLog>, ICompensateActivity<TLog>>(compensateConfigurator.Settings);

            _registrar.RegisterActivityDefinition<EndpointActivityDefinition<TActivity, TArguments, TLog>, TActivity, TArguments, TLog>();
        }
    }
}
