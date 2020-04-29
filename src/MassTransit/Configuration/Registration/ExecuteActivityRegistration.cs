namespace MassTransit.Registration
{
    using System;
    using System.Collections.Generic;
    using Context;
    using Courier;
    using Definition;
    using Metadata;
    using PipeConfigurators;
    using Scoping;


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

        void IExecuteActivityRegistration.AddConfigureAction<T, TArgs>(Action<IExecuteActivityConfigurator<T, TArgs>> configure)
        {
            if (configure is Action<IExecuteActivityConfigurator<TActivity, TArguments>> action)
                _configureActions.Add(action);
        }

        public void Configure(IReceiveEndpointConfigurator configurator, IConfigurationServiceProvider configurationServiceProvider)
        {
            var executeActivityScopeProvider = configurationServiceProvider.GetRequiredService<IExecuteActivityScopeProvider<TActivity, TArguments>>();

            var executeActivityFactory = new ScopeExecuteActivityFactory<TActivity, TArguments>(executeActivityScopeProvider);

            var specification = new ExecuteActivityHostSpecification<TActivity, TArguments>(executeActivityFactory, configurator);

            LogContext.Debug?.Log("Configuring endpoint {Endpoint}, Execute Activity: {ActivityType}", configurator.InputAddress.GetLastPart(),
                TypeMetadataCache<TActivity>.ShortName);

            configurator.ConfigureConsumeTopology = false;

            GetActivityDefinition(configurationServiceProvider)
                .Configure(configurator, specification);

            foreach (var action in _configureActions)
                action(specification);

            configurator.AddEndpointSpecification(specification);
        }

        IExecuteActivityDefinition IExecuteActivityRegistration.GetDefinition(IConfigurationServiceProvider provider)
        {
            return GetActivityDefinition(provider);
        }

        IExecuteActivityDefinition<TActivity, TArguments> GetActivityDefinition(IConfigurationServiceProvider provider)
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
