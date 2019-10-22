namespace MassTransit
{
    using System;
    using Autofac;
    using AutofacIntegration;
    using AutofacIntegration.Registration;
    using Automatonymous;
    using Automatonymous.SagaConfigurators;
    using Automatonymous.StateMachineConnectors;
    using GreenPipes;
    using Pipeline;
    using Registration;
    using Saga;


    public static class AutofacAutomatonymousReceiveEndpointExtensions
    {
        static readonly IStateMachineActivityFactory _activityFactory = new AutofacStateMachineActivityFactory();

        /// <summary>
        /// Subscribe a state machine saga to the endpoint
        /// </summary>
        /// <typeparam name="TInstance">The state machine instance type</typeparam>
        /// <param name="configurator"></param>
        /// <param name="stateMachine">The state machine</param>
        /// <param name="context">The Autofac root container to resolve the repository</param>
        /// <param name="configure">Optionally configure the saga</param>
        /// <param name="name">The name to use for the scope created for each message</param>
        /// <param name="configureScope">Configuration for scope container</param>
        /// <returns></returns>
        public static void StateMachineSaga<TInstance>(this IReceiveEndpointConfigurator configurator, SagaStateMachine<TInstance> stateMachine,
            IComponentContext context, Action<ISagaConfigurator<TInstance>> configure = null, string name = "message",
            Action<ContainerBuilder, ConsumeContext> configureScope = null)
            where TInstance : class, SagaStateMachineInstance
        {
            StateMachineSaga(configurator, stateMachine, context.Resolve<ILifetimeScope>(), configure, name, configureScope);
        }

        /// <summary>
        /// Subscribe a state machine saga to the endpoint
        /// </summary>
        /// <typeparam name="TInstance">The state machine instance type</typeparam>
        /// <param name="configurator"></param>
        /// <param name="stateMachine">The state machine</param>
        /// <param name="scope">The Autofac Lifetime container to resolve the repository</param>
        /// <param name="configure">Optionally configure the saga</param>
        /// <param name="name">The name to use for the scope created for each message</param>
        /// <param name="configureScope">Configuration for scope container</param>
        /// <returns></returns>
        public static void StateMachineSaga<TInstance>(this IReceiveEndpointConfigurator configurator, SagaStateMachine<TInstance> stateMachine,
            ILifetimeScope scope, Action<ISagaConfigurator<TInstance>> configure = null, string name = "message",
            Action<ContainerBuilder, ConsumeContext> configureScope = null)
            where TInstance : class, SagaStateMachineInstance
        {
            ISagaRepository<TInstance> repository = CreateSagaRepository<TInstance>(scope, name, configureScope);

            var stateMachineConfigurator = new StateMachineSagaConfigurator<TInstance>(stateMachine, repository, configurator);

            configure?.Invoke(stateMachineConfigurator);

            configurator.AddEndpointSpecification(stateMachineConfigurator);
        }

        /// <summary>
        /// Subscribe a state machine saga to the endpoint
        /// </summary>
        /// <typeparam name="TInstance">The state machine instance type</typeparam>
        /// <param name="configurator"></param>
        /// <param name="context">The Autofac root container to resolve the repository</param>
        /// <param name="configure">Optionally configure the saga</param>
        /// <param name="name">The name to use for the scope created for each message</param>
        /// <param name="configureScope">Configuration for scope container</param>
        /// <returns></returns>
        public static void StateMachineSaga<TInstance>(this IReceiveEndpointConfigurator configurator, IComponentContext context,
            Action<ISagaConfigurator<TInstance>> configure = null, string name = "message", Action<ContainerBuilder, ConsumeContext> configureScope = null)
            where TInstance : class, SagaStateMachineInstance
        {
            StateMachineSaga(configurator, context.Resolve<ILifetimeScope>(), configure, name, configureScope);
        }

        /// <summary>
        /// Subscribe a state machine saga to the endpoint
        /// </summary>
        /// <typeparam name="TInstance">The state machine instance type</typeparam>
        /// <param name="configurator"></param>
        /// <param name="scope">The Autofac Lifetime Container to resolve the repository</param>
        /// <param name="configure">Optionally configure the saga</param>
        /// <param name="name">The name to use for the scope created for each message</param>
        /// <param name="configureScope">Configuration for scope container</param>
        /// <returns></returns>
        public static void StateMachineSaga<TInstance>(this IReceiveEndpointConfigurator configurator, ILifetimeScope scope,
            Action<ISagaConfigurator<TInstance>> configure = null, string name = "message", Action<ContainerBuilder, ConsumeContext> configureScope = null)
            where TInstance : class, SagaStateMachineInstance
        {
            ISagaStateMachineFactory stateMachineFactory = new AutofacSagaStateMachineFactory(scope);

            SagaStateMachine<TInstance> stateMachine = stateMachineFactory.CreateStateMachine<TInstance>();

            StateMachineSaga(configurator, stateMachine, scope, configure, name, configureScope);
        }

        public static ConnectHandle ConnectStateMachineSaga<TInstance>(this IConsumePipeConnector pipe, SagaStateMachine<TInstance> stateMachine,
            ILifetimeScope scope, string name = "message", Action<ContainerBuilder, ConsumeContext> configureScope = null)
            where TInstance : class, SagaStateMachineInstance
        {
            var connector = new StateMachineConnector<TInstance>(stateMachine);

            ISagaRepository<TInstance> repository = CreateSagaRepository<TInstance>(scope, name, configureScope);

            ISagaSpecification<TInstance> specification = connector.CreateSagaSpecification<TInstance>();

            return connector.ConnectSaga(pipe, repository, specification);
        }

        public static ConnectHandle ConnectStateMachineSaga<TInstance>(this IConsumePipeConnector pipe, ILifetimeScope scope, string name = "message",
            Action<ContainerBuilder, ConsumeContext> configureScope = null)
            where TInstance : class, SagaStateMachineInstance
        {
            ISagaStateMachineFactory stateMachineFactory = new AutofacSagaStateMachineFactory(scope);

            SagaStateMachine<TInstance> stateMachine = stateMachineFactory.CreateStateMachine<TInstance>();

            return pipe.ConnectStateMachineSaga(stateMachine, scope, name, configureScope);
        }

        static ISagaRepository<TInstance> CreateSagaRepository<TInstance>(ILifetimeScope scope, string name,
            Action<ContainerBuilder, ConsumeContext> configureScope)
            where TInstance : class, SagaStateMachineInstance
        {
            ISagaRepositoryFactory repositoryFactory = new AutofacSagaRepositoryFactory(new SingleLifetimeScopeProvider(scope), name, configureScope);

            return repositoryFactory.CreateSagaRepository<TInstance>(AddStateMachineActivityFactory);
        }

        static void AddStateMachineActivityFactory(ConsumeContext context)
        {
            context.GetOrAddPayload(() => _activityFactory);
        }
    }
}
