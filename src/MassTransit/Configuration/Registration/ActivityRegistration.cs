namespace MassTransit.Registration
{
    using System;
    using System.Collections.Generic;
    using Courier;
    using Definition;
    using PipeConfigurators;
    using Scoping;


    public class ActivityRegistration<TActivity, TArguments, TLog> :
        IActivityRegistration
        where TActivity : class, IActivity<TArguments, TLog>
        where TArguments : class
        where TLog : class
    {
        readonly List<Action<IExecuteActivityConfigurator<TActivity, TArguments>>> _executeActions;
        readonly List<Action<ICompensateActivityConfigurator<TActivity, TLog>>> _compensateActions;
        IActivityDefinition<TActivity, TArguments, TLog> _definition;

        public ActivityRegistration()
        {
            _executeActions = new List<Action<IExecuteActivityConfigurator<TActivity, TArguments>>>();
            _compensateActions = new List<Action<ICompensateActivityConfigurator<TActivity, TLog>>>();
        }

        public void AddConfigureAction<T, TA>(Action<IExecuteActivityConfigurator<T, TA>> configure)
            where T : class, IExecuteActivity<TA>
            where TA : class
        {
            if (configure is Action<IExecuteActivityConfigurator<TActivity, TArguments>> action)
                _executeActions.Add(action);
        }

        public void AddConfigureAction<T, TL>(Action<ICompensateActivityConfigurator<T, TL>> configure)
            where T : class, ICompensateActivity<TL>
            where TL : class
        {
            if (configure is Action<ICompensateActivityConfigurator<TActivity, TLog>> action)
                _compensateActions.Add(action);
        }

        public void Configure(IReceiveEndpointConfigurator executeEndpointConfigurator, IReceiveEndpointConfigurator compensateEndpointConfigurator,
            IConfigurationServiceProvider scopeProvider)
        {
            ConfigureCompensate(compensateEndpointConfigurator, scopeProvider);

            ConfigureExecute(executeEndpointConfigurator, scopeProvider, compensateEndpointConfigurator.InputAddress);
        }

        IActivityDefinition IActivityRegistration.GetDefinition(IConfigurationServiceProvider provider)
        {
            return GetActivityDefinition(provider);
        }

        IActivityDefinition<TActivity, TArguments, TLog> GetActivityDefinition(IConfigurationServiceProvider provider)
        {
            return _definition ?? (_definition = provider.GetService<IActivityDefinition<TActivity, TArguments, TLog>>()
                ?? new DefaultActivityDefinition<TActivity, TArguments, TLog>());
        }

        void ConfigureCompensate(IReceiveEndpointConfigurator configurator, IConfigurationServiceProvider configurationServiceProvider)
        {
            var activityScopeProvider = configurationServiceProvider.GetRequiredService<ICompensateActivityScopeProvider<TActivity, TLog>>();

            var activityFactory = new ScopeCompensateActivityFactory<TActivity, TLog>(activityScopeProvider);

            var specification = new CompensateActivityHostSpecification<TActivity, TLog>(activityFactory, configurator);

            GetActivityDefinition(configurationServiceProvider)
                .Configure(configurator, specification);

            foreach (var action in _compensateActions)
                action(specification);

            configurator.AddEndpointSpecification(specification);
        }

        void ConfigureExecute(IReceiveEndpointConfigurator configurator, IConfigurationServiceProvider configurationServiceProvider, Uri compensateAddress)
        {
            var activityScopeProvider = configurationServiceProvider.GetRequiredService<IExecuteActivityScopeProvider<TActivity, TArguments>>();

            var activityFactory = new ScopeExecuteActivityFactory<TActivity, TArguments>(activityScopeProvider);

            var specification = new ExecuteActivityHostSpecification<TActivity, TArguments>(activityFactory, compensateAddress, configurator);

            GetActivityDefinition(configurationServiceProvider)
                .Configure(configurator, specification);

            foreach (var action in _executeActions)
                action(specification);

            configurator.AddEndpointSpecification(specification);
        }
    }
}
