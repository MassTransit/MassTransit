namespace MassTransit.SimpleInjectorIntegration.Registration
{
    using System;
    using System.Linq;
    using Automatonymous;
    using Courier;
    using Definition;
    using MassTransit.Registration;
    using Saga;
    using ScopeProviders;
    using Scoping;
    using SimpleInjector;


    public class SimpleInjectorContainerRegistar :
        IContainerRegistrar
    {
        readonly Container _container;
        readonly Lifestyle _hybridLifestyle;

        public SimpleInjectorContainerRegistar(Container container)
        {
            _container = container;

            _hybridLifestyle = Lifestyle.CreateHybrid(_container.Options.DefaultScopedLifestyle, Lifestyle.Singleton);
        }

        public void RegisterConsumer<T>()
            where T : class, IConsumer
        {
            _container.Register<T>(Lifestyle.Scoped);
        }

        public void RegisterConsumerDefinition<TDefinition, TConsumer>()
            where TDefinition : class, IConsumerDefinition<TConsumer>
            where TConsumer : class, IConsumer
        {
            _container.Register<IConsumerDefinition<TConsumer>, TDefinition>(Lifestyle.Transient);
        }

        public void RegisterSaga<T>()
            where T : class, ISaga
        {
        }

        public void RegisterStateMachineSaga<TStateMachine, TInstance>()
            where TStateMachine : class, SagaStateMachine<TInstance>
            where TInstance : class, SagaStateMachineInstance
        {
            _container.RegisterSingleton<IStateMachineActivityFactory, SimpleInjectorStateMachineActivityFactory>();
            _container.RegisterSingleton<ISagaStateMachineFactory, SimpleInjectorSagaStateMachineFactory>();

            _container.RegisterSingleton<TStateMachine>();
            _container.RegisterSingleton<SagaStateMachine<TInstance>>(() => _container.GetInstance<TStateMachine>());
        }

        public void RegisterSagaDefinition<TDefinition, TSaga>()
            where TDefinition : class, ISagaDefinition<TSaga>
            where TSaga : class, ISaga
        {
            _container.Register<ISagaDefinition<TSaga>, TDefinition>(Lifestyle.Transient);
        }

        public void RegisterExecuteActivity<TActivity, TArguments>()
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            RegisterIfNotRegistered<TActivity>();

            _container.Register<IExecuteActivityScopeProvider<TActivity, TArguments>,
                SimpleInjectorExecuteActivityScopeProvider<TActivity, TArguments>>(Lifestyle.Transient);
        }

        public void RegisterActivityDefinition<TDefinition, TActivity, TArguments, TLog>()
            where TDefinition : class, IActivityDefinition<TActivity, TArguments, TLog>
            where TActivity : class, IActivity<TArguments, TLog>
            where TArguments : class
            where TLog : class
        {
            _container.Register<IActivityDefinition<TActivity, TArguments, TLog>, TDefinition>(Lifestyle.Transient);
        }

        public void RegisterExecuteActivityDefinition<TDefinition, TActivity, TArguments>()
            where TDefinition : class, IExecuteActivityDefinition<TActivity, TArguments>
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            _container.Register<IExecuteActivityDefinition<TActivity, TArguments>, TDefinition>(Lifestyle.Transient);
        }

        public void RegisterEndpointDefinition<TDefinition, T>(IEndpointSettings<IEndpointDefinition<T>> settings)
            where TDefinition : class, IEndpointDefinition<T>
            where T : class
        {
            _container.Register<IEndpointDefinition<T>, TDefinition>(Lifestyle.Transient);

            if (settings != null)
                _container.RegisterInstance(settings);
        }

        public void RegisterRequestClient<T>(RequestTimeout timeout)
            where T : class
        {
            _container.Register(() =>
            {
                var clientFactory = _container.GetInstance<IClientFactory>();

                var consumeContext = _container.GetConsumeContext();
                return consumeContext != null
                    ? clientFactory.CreateRequestClient<T>(consumeContext, timeout)
                    : clientFactory.CreateRequestClient<T>(timeout);
            }, _hybridLifestyle);
        }

        public void RegisterRequestClient<T>(Uri destinationAddress, RequestTimeout timeout)
            where T : class
        {
            _container.Register(() =>
            {
                var clientFactory = _container.GetInstance<IClientFactory>();

                var consumeContext = _container.GetConsumeContext();
                return consumeContext != null
                    ? clientFactory.CreateRequestClient<T>(consumeContext, destinationAddress, timeout)
                    : clientFactory.CreateRequestClient<T>(destinationAddress, timeout);
            }, _hybridLifestyle);
        }

        public void RegisterCompensateActivity<TActivity, TLog>()
            where TActivity : class, ICompensateActivity<TLog>
            where TLog : class
        {
            RegisterIfNotRegistered<TActivity>();

            _container.Register<ICompensateActivityScopeProvider<TActivity, TLog>,
                SimpleInjectorCompensateActivityScopeProvider<TActivity, TLog>>(Lifestyle.Transient);
        }

        void RegisterIfNotRegistered<TActivity>()
            where TActivity : class
        {
            var notExists = _container.GetCurrentRegistrations().SingleOrDefault(r => r.Registration.ImplementationType == typeof(TActivity)) == null;

            if (notExists)
                _container.Register<TActivity>(Lifestyle.Scoped);
        }
    }
}
