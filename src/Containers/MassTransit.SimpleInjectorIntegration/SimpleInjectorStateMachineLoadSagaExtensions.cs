namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Automatonymous;
    using GreenPipes.Internals.Extensions;
    using Registration;
    using SimpleInjector;
    using SimpleInjectorIntegration;
    using SimpleInjectorIntegration.Registration;


    public static class SimpleInjectorStateMachineLoadSagaExtensions
    {
        /// <summary>
        /// Specify that the service bus should load the StateMachineSagas from the container passed as argument
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="container"></param>
        public static void LoadStateMachineSagas(this IReceiveEndpointConfigurator configurator, Container container)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            if (container == null)
                throw new ArgumentNullException(nameof(container));

            var registrationConfigurator = new RegistrationConfigurator();

            container.RegisterInstance<ISagaStateMachineFactory>(new SimpleInjectorSagaStateMachineFactory(container));
            container.RegisterInstance<IStateMachineActivityFactory>(new SimpleInjectorStateMachineActivityFactory());
            container.RegisterInstance<ISagaRepositoryFactory>(new SimpleInjectorSagaRepositoryFactory(container));

            IEnumerable<Type> sagaTypes = FindStateMachineSagaTypes(container);
            foreach (var sagaType in sagaTypes)
                ((IRegistrationConfigurator)registrationConfigurator).AddSagaStateMachine(sagaType);

            var registration = registrationConfigurator.CreateRegistration(new SimpleInjectorConfigurationServiceProvider(container));

            registration.ConfigureSagas(configurator);
        }

        static IEnumerable<Type> FindStateMachineSagaTypes(Container container)
        {
            return
                container.GetCurrentRegistrations()
                    .Where(r => r.Registration.ImplementationType.HasInterface(typeof(SagaStateMachine<>)))
                    .Select(x => x.Registration.ImplementationType)
                    .ToList();
        }
    }
}
