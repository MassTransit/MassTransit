namespace MassTransit.DependencyInjection.Registration
{
    using System;
    using Configuration;
    using Courier;


    public class ExecuteActivityRegistrationConfigurator<TActivity, TArguments> :
        IExecuteActivityRegistrationConfigurator<TActivity, TArguments>
        where TActivity : class, IExecuteActivity<TArguments>
        where TArguments : class
    {
        readonly IRegistrationConfigurator _configurator;

        public ExecuteActivityRegistrationConfigurator(IRegistrationConfigurator configurator)
        {
            _configurator = configurator;
        }

        public void Endpoint(Action<IEndpointRegistrationConfigurator> configure)
        {
            var configurator = new EndpointRegistrationConfigurator<IExecuteActivity<TArguments>> { ConfigureConsumeTopology = false };

            configure?.Invoke(configurator);

            _configurator.AddEndpoint<ExecuteActivityEndpointDefinition<TActivity, TArguments>, IExecuteActivity<TArguments>>(configurator.Settings);
        }
    }
}
