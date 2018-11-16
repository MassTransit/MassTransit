using System;
using System.Collections.Generic;
using System.Linq;

namespace MassTransit.AutomatonymousWindsorIntegration
{
    using Automatonymous;
    using Automatonymous.Scoping;
    using Castle.MicroKernel;
    using Castle.Windsor;
    using GreenPipes.Internals.Extensions;


    public static class WindsorStateMachineLoadSagaExtensions
    {
        /// <summary>
        /// Specify that the service bus should load the StateMachineSagas from the container passed as argument
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="container"></param>
        public static void LoadStateMachineSagas(this IReceiveEndpointConfigurator configurator, IWindsorContainer container)
        {
            if (container == null)
                throw new ArgumentNullException(nameof(container));

            LoadStateMachineSagas(configurator, container.Kernel);
        }

        /// <summary>
        /// Specify that the service bus should load the StateMachineSagas from the container passed as argument
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="container"></param>
        public static void LoadStateMachineSagas(this IReceiveEndpointConfigurator configurator, IKernel container)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (container == null)
                throw new ArgumentNullException(nameof(container));

            IEnumerable<Type> sagaTypes = FindStateMachineSagaTypes(container);

            var stateMachineFactory = new WindsorSagaStateMachineFactory(container);

            var repositoryFactory = new WindsorStateMachineSagaRepositoryFactory(container);

            foreach (var sagaType in sagaTypes)
            {
                StateMachineSagaConfiguratorCache.Configure(sagaType, configurator, stateMachineFactory, repositoryFactory);
            }
        }

        static IEnumerable<Type> FindStateMachineSagaTypes(IKernel container)
        {
            var types = container.GetAssignableHandlers(typeof(StateMachine))
                 .Where(x => x.HasInterface(typeof(SagaStateMachine<>)))
                 .Select(x => x.ComponentModel.Implementation.GetClosingArguments(typeof(SagaStateMachine<>)).First())
                 .Distinct()
                 .ToList();

            return types;
        }

        static bool HasInterface(this IHandler handler, Type type)
        {
            return handler.ComponentModel.Implementation.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == type);
        }
    }
}
