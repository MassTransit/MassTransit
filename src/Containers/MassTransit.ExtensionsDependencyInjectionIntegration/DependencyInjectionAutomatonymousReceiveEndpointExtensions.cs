namespace MassTransit
{
    using System;
    using Automatonymous;
    using ExtensionsDependencyInjectionIntegration;
    using ExtensionsDependencyInjectionIntegration.Registration;
    using GreenPipes;
    using Microsoft.Extensions.DependencyInjection;
    using Pipeline;
    using Registration;
    using Saga;


    public static class DependencyInjectionAutomatonymousReceiveEndpointExtensions
    {
        static readonly IStateMachineActivityFactory _activityFactory = new DependencyInjectionStateMachineActivityFactory();

        /// <summary>
        /// Subscribe a state machine saga to the endpoint
        /// </summary>
        /// <typeparam name="TInstance">The state machine instance type</typeparam>
        /// <param name="configurator"></param>
        /// <param name="stateMachine"></param>
        /// <param name="serviceProvider">The Container reference to resolve the repository</param>
        /// <param name="configure">Optionally configure the saga</param>
        /// <returns></returns>
        public static void StateMachineSaga<TInstance>(this IReceiveEndpointConfigurator configurator, SagaStateMachine<TInstance> stateMachine,
            IServiceProvider serviceProvider, Action<ISagaConfigurator<TInstance>> configure = null)
            where TInstance : class, SagaStateMachineInstance
        {
            ISagaRepository<TInstance> repository = CreateSagaRepository<TInstance>(serviceProvider);

            configurator.StateMachineSaga(stateMachine, repository, configure);
        }

        /// <summary>
        /// Subscribe a state machine saga to the endpoint
        /// </summary>
        /// <typeparam name="TInstance">The state machine instance type</typeparam>
        /// <param name="configurator"></param>
        /// <param name="serviceProvider">The Container reference to resolve the repository</param>
        /// <param name="configure">Optionally configure the saga</param>
        /// <returns></returns>
        public static void StateMachineSaga<TInstance>(this IReceiveEndpointConfigurator configurator, IServiceProvider serviceProvider,
            Action<ISagaConfigurator<TInstance>> configure = null)
            where TInstance : class, SagaStateMachineInstance
        {
            var stateMachine = serviceProvider.GetRequiredService<SagaStateMachine<TInstance>>();

            ISagaRepository<TInstance> repository = CreateSagaRepository<TInstance>(serviceProvider);

            configurator.StateMachineSaga(stateMachine, repository, configure);
        }

        public static ConnectHandle ConnectStateMachineSaga<TInstance>(this IConsumePipeConnector bus, SagaStateMachine<TInstance> stateMachine,
            IServiceProvider serviceProvider, Action<ISagaConfigurator<TInstance>> configure = null)
            where TInstance : class, SagaStateMachineInstance
        {
            ISagaRepository<TInstance> repository = CreateSagaRepository<TInstance>(serviceProvider);

            return bus.ConnectStateMachineSaga(stateMachine, repository, configure);
        }

        public static ConnectHandle ConnectStateMachineSaga<TInstance>(this IConsumePipeConnector bus, IServiceProvider serviceProvider,
            Action<ISagaConfigurator<TInstance>> configure = null)
            where TInstance : class, SagaStateMachineInstance
        {
            var stateMachine = serviceProvider.GetRequiredService<SagaStateMachine<TInstance>>();

            ISagaRepository<TInstance> repository = CreateSagaRepository<TInstance>(serviceProvider);

            return bus.ConnectStateMachineSaga(stateMachine, repository, configure);
        }

        static ISagaRepository<TInstance> CreateSagaRepository<TInstance>(IServiceProvider serviceProvider)
            where TInstance : class, SagaStateMachineInstance
        {
            ISagaRepositoryFactory repositoryFactory = new DependencyInjectionSagaRepositoryFactory(serviceProvider);

            return repositoryFactory.CreateSagaRepository<TInstance>(AddStateMachineActivityFactory);
        }

        static void AddStateMachineActivityFactory(ConsumeContext context)
        {
            context.GetOrAddPayload(() => _activityFactory);
        }
    }
}
