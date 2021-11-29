namespace MassTransit.DependencyInjection.Registration
{
    using System;
    using System.Collections.Generic;
    using Configuration;
    using Courier;
    using Microsoft.Extensions.DependencyInjection;
    using Transports;


    public class ActivityRegistration<TActivity, TArguments, TLog> :
        IActivityRegistration
        where TActivity : class, IActivity<TArguments, TLog>
        where TArguments : class
        where TLog : class
    {
        readonly List<Action<ICompensateActivityConfigurator<TActivity, TLog>>> _compensateActions;
        readonly List<Action<IExecuteActivityConfigurator<TActivity, TArguments>>> _executeActions;
        IActivityDefinition<TActivity, TArguments, TLog> _definition;

        public ActivityRegistration()
        {
            _executeActions = new List<Action<IExecuteActivityConfigurator<TActivity, TArguments>>>();
            _compensateActions = new List<Action<ICompensateActivityConfigurator<TActivity, TLog>>>();
        }

        public Type Type => typeof(TActivity);

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
            IServiceProvider scopeProvider)
        {
            ConfigureCompensate(compensateEndpointConfigurator, scopeProvider);

            ConfigureExecute(executeEndpointConfigurator, scopeProvider, compensateEndpointConfigurator.InputAddress);
        }

        IActivityDefinition IActivityRegistration.GetDefinition(IServiceProvider provider)
        {
            return GetActivityDefinition(provider);
        }

        public void ConfigureCompensate(IReceiveEndpointConfigurator configurator, IServiceProvider configurationServiceProvider)
        {
            var activityScopeProvider = configurationServiceProvider.GetRequiredService<ICompensateActivityScopeProvider<TActivity, TLog>>();

            var activityFactory = new ScopeCompensateActivityFactory<TActivity, TLog>(activityScopeProvider);

            var specification = new CompensateActivityHostConfigurator<TActivity, TLog>(activityFactory, configurator);

            configurator.ConfigureConsumeTopology = false;

            GetActivityDefinition(configurationServiceProvider)
                .Configure(configurator, specification);

            foreach (Action<ICompensateActivityConfigurator<TActivity, TLog>> action in _compensateActions)
                action(specification);

            LogContext.Info?.Log("Configured endpoint {Endpoint}, Compensate Activity: {ActivityType}", configurator.InputAddress.GetEndpointName(),
                TypeCache<TActivity>.ShortName);

            configurator.AddEndpointSpecification(specification);
        }

        public void ConfigureExecute(IReceiveEndpointConfigurator configurator, IServiceProvider configurationServiceProvider,
            Uri compensateAddress)
        {
            var activityScopeProvider = configurationServiceProvider.GetRequiredService<IExecuteActivityScopeProvider<TActivity, TArguments>>();

            var activityFactory = new ScopeExecuteActivityFactory<TActivity, TArguments>(activityScopeProvider);

            var specification = new ExecuteActivityHostConfigurator<TActivity, TArguments>(activityFactory, compensateAddress, configurator);

            configurator.ConfigureConsumeTopology = false;

            GetActivityDefinition(configurationServiceProvider)
                .Configure(configurator, specification);

            foreach (Action<IExecuteActivityConfigurator<TActivity, TArguments>> action in _executeActions)
                action(specification);

            LogContext.Info?.Log("Configured endpoint {Endpoint}, Execute Activity: {ActivityType}", configurator.InputAddress.GetEndpointName(),
                TypeCache<TActivity>.ShortName);

            configurator.AddEndpointSpecification(specification);
        }

        IActivityDefinition<TActivity, TArguments, TLog> GetActivityDefinition(IServiceProvider provider)
        {
            if (_definition != null)
                return _definition;

            _definition = provider.GetService<IActivityDefinition<TActivity, TArguments, TLog>>()
                ?? new DefaultActivityDefinition<TActivity, TArguments, TLog>();

            var executeEndpointDefinition = provider.GetService<IEndpointDefinition<IExecuteActivity<TArguments>>>();
            if (executeEndpointDefinition != null)
                _definition.ExecuteEndpointDefinition = executeEndpointDefinition;

            var compensateEndpointDefinition = provider.GetService<IEndpointDefinition<ICompensateActivity<TLog>>>();
            if (compensateEndpointDefinition != null)
                _definition.CompensateEndpointDefinition = compensateEndpointDefinition;

            return _definition;
        }
    }
}
