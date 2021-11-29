namespace MassTransit.DependencyInjection.Registration
{
    using System;
    using System.Collections.Generic;
    using Configuration;
    using Courier;
    using Microsoft.Extensions.DependencyInjection;
    using Transports;


    public class ExecuteActivityRegistration<TActivity, TArguments> :
        IExecuteActivityRegistration
        where TActivity : class, IExecuteActivity<TArguments>
        where TArguments : class
    {
        readonly List<Action<IExecuteActivityConfigurator<TActivity, TArguments>>> _configureActions;
        IExecuteActivityDefinition<TActivity, TArguments> _definition;

        public ExecuteActivityRegistration()
        {
            _configureActions = new List<Action<IExecuteActivityConfigurator<TActivity, TArguments>>>();
        }

        public Type Type => typeof(TActivity);

        void IExecuteActivityRegistration.AddConfigureAction<T, TArgs>(Action<IExecuteActivityConfigurator<T, TArgs>> configure)
        {
            if (configure is Action<IExecuteActivityConfigurator<TActivity, TArguments>> action)
                _configureActions.Add(action);
        }

        public void Configure(IReceiveEndpointConfigurator configurator, IServiceProvider configurationServiceProvider)
        {
            var executeActivityScopeProvider = configurationServiceProvider.GetRequiredService<IExecuteActivityScopeProvider<TActivity, TArguments>>();

            var executeActivityFactory = new ScopeExecuteActivityFactory<TActivity, TArguments>(executeActivityScopeProvider);

            var specification = new ExecuteActivityHostConfigurator<TActivity, TArguments>(executeActivityFactory, configurator);

            configurator.ConfigureConsumeTopology = false;

            GetActivityDefinition(configurationServiceProvider)
                .Configure(configurator, specification);

            foreach (Action<IExecuteActivityConfigurator<TActivity, TArguments>> action in _configureActions)
                action(specification);

            LogContext.Info?.Log("Configured endpoint {Endpoint}, Execute Activity: {ActivityType}", configurator.InputAddress.GetEndpointName(),
                TypeCache<TActivity>.ShortName);

            configurator.AddEndpointSpecification(specification);
        }

        IExecuteActivityDefinition IExecuteActivityRegistration.GetDefinition(IServiceProvider provider)
        {
            return GetActivityDefinition(provider);
        }

        IExecuteActivityDefinition<TActivity, TArguments> GetActivityDefinition(IServiceProvider provider)
        {
            if (_definition != null)
                return _definition;

            _definition = provider.GetService<IExecuteActivityDefinition<TActivity, TArguments>>()
                ?? new DefaultExecuteActivityDefinition<TActivity, TArguments>();

            var executeEndpointDefinition = provider.GetService<IEndpointDefinition<IExecuteActivity<TArguments>>>();
            if (executeEndpointDefinition != null)
                _definition.ExecuteEndpointDefinition = executeEndpointDefinition;

            return _definition;
        }
    }
}
