namespace MassTransit.DependencyInjection.Registration
{
    using System;
    using System.Collections.Generic;
    using Configuration;
    using Internals;
    using Transports;


    public class ExecuteActivityRegistration<TActivity, TArguments> :
        IExecuteActivityRegistration
        where TActivity : class, IExecuteActivity<TArguments>
        where TArguments : class
    {
        readonly List<Action<IRegistrationContext, IExecuteActivityConfigurator<TActivity, TArguments>>> _configureActions;
        readonly IContainerSelector _selector;
        IExecuteActivityDefinition<TActivity, TArguments> _definition;

        public ExecuteActivityRegistration(IContainerSelector selector)
        {
            _selector = selector;
            _configureActions = new List<Action<IRegistrationContext, IExecuteActivityConfigurator<TActivity, TArguments>>>();
            IncludeInConfigureEndpoints = !Type.HasAttribute<ExcludeFromConfigureEndpointsAttribute>();
        }

        public Type Type => typeof(TActivity);

        public bool IncludeInConfigureEndpoints { get; set; }

        void IExecuteActivityRegistration.AddConfigureAction<T, TArgs>(Action<IRegistrationContext, IExecuteActivityConfigurator<T, TArgs>> configure)
        {
            if (configure is Action<IRegistrationContext, IExecuteActivityConfigurator<TActivity, TArguments>> action)
                _configureActions.Add(action);
        }

        public void Configure(IReceiveEndpointConfigurator configurator, IRegistrationContext context)
        {
            var executeActivityScopeProvider = new ExecuteActivityScopeProvider<TActivity, TArguments>(context);

            var executeActivityFactory = new ScopeExecuteActivityFactory<TActivity, TArguments>(executeActivityScopeProvider);

            var specification = new ExecuteActivityHostConfigurator<TActivity, TArguments>(executeActivityFactory, configurator);

            configurator.ConfigureConsumeTopology = false;

            GetActivityDefinition(context)
                .Configure(configurator, specification, context);

            foreach (Action<IRegistrationContext, IExecuteActivityConfigurator<TActivity, TArguments>> action in _configureActions)
                action(context, specification);

            LogContext.Info?.Log("Configured endpoint {Endpoint}, Execute Activity: {ActivityType}", configurator.InputAddress.GetEndpointName(),
                TypeCache<TActivity>.ShortName);

            configurator.AddEndpointSpecification(specification);
        }

        IExecuteActivityDefinition IExecuteActivityRegistration.GetDefinition(IRegistrationContext context)
        {
            return GetActivityDefinition(context);
        }

        IExecuteActivityDefinition<TActivity, TArguments> GetActivityDefinition(IServiceProvider provider)
        {
            if (_definition != null)
                return _definition;

            _definition = _selector.GetDefinition<IExecuteActivityDefinition<TActivity, TArguments>>(provider)
                ?? new DefaultExecuteActivityDefinition<TActivity, TArguments>();

            IEndpointDefinition<IExecuteActivity<TArguments>> executeEndpointDefinition =
                _selector.GetEndpointDefinition<IExecuteActivity<TArguments>>(provider);
            if (executeEndpointDefinition != null)
                _definition.ExecuteEndpointDefinition = executeEndpointDefinition;

            return _definition;
        }
    }
}
