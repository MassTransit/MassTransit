namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Automatonymous;
    using Castle.MicroKernel;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using Registration;
    using WindsorIntegration;
    using WindsorIntegration.Registration;


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

            var registrationConfigurator = new RegistrationConfigurator();

            container.Register(Component.For<ISagaStateMachineFactory>().Instance(new WindsorSagaStateMachineFactory(container)));
            container.Register(Component.For<IStateMachineActivityFactory>().Instance(new WindsorStateMachineActivityFactory()));
            container.Register(Component.For<ISagaRepositoryFactory>().Instance(new WindsorSagaRepositoryFactory(container)));

            IEnumerable<Type> sagaTypes = FindStateMachineSagaTypes(container);
            foreach (var sagaType in sagaTypes)
                ((IRegistrationConfigurator)registrationConfigurator).AddSagaStateMachine(sagaType);

            var registration = registrationConfigurator.CreateRegistration(new WindsorConfigurationServiceProvider(container));

            registration.ConfigureSagas(configurator);
        }

        static IEnumerable<Type> FindStateMachineSagaTypes(IKernel container)
        {
            List<Type> types = container.GetAssignableHandlers(typeof(StateMachine))
                .Where(x => x.HasInterface(typeof(SagaStateMachine<>)))
                .Select(x => x.ComponentModel.Implementation)
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
