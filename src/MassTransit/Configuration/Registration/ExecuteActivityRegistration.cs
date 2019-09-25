namespace MassTransit.Registration
{
    using System;
    using System.Collections.Generic;
    using Courier;
    using Definition;
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
            return _definition ?? (_definition = provider.GetService<IExecuteActivityDefinition<TActivity, TArguments>>()
                ?? new DefaultExecuteActivityDefinition<TActivity, TArguments>());
        }
    }
}
