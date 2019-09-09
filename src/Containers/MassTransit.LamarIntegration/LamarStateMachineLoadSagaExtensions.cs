namespace MassTransit
{
    using System;
    using System.Linq;
    using Automatonymous;
    using GreenPipes.Internals.Extensions;
    using Lamar;
    using LamarIntegration;
    using LamarIntegration.Registration;
    using Microsoft.Extensions.DependencyInjection;
    using Registration;


    public static class LamarStateMachineLoadSagaExtensions
    {
        /// <summary>
        /// Scans the lifetime scope and registers any state machines sagas which are found in the scope using the StructureMap saga repository
        /// and the appropriate state machine saga repository under the hood.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="container"></param>
        public static void LoadStateMachineSagas(this IReceiveEndpointConfigurator configurator, IContainer container)
        {
            var registrationConfigurator = new RegistrationConfigurator();

            container.Configure(x =>
            {
                if (container.TryGetInstance<ISagaStateMachineFactory>() == null)
                    x.AddSingleton<ISagaStateMachineFactory>(provider => new LamarSagaStateMachineFactory(provider));

                if (container.TryGetInstance<ISagaRepositoryFactory>() == null)
                    x.AddSingleton<ISagaRepositoryFactory>(provider => new LamarSagaRepositoryFactory(container));

                if (container.TryGetInstance<IStateMachineActivityFactory>() == null)
                    x.AddSingleton<IStateMachineActivityFactory>(provider => new LamarStateMachineActivityFactory());
            });

            registrationConfigurator.AddSagaStateMachines(FindStateMachineSagaTypes(container));

            var registration = registrationConfigurator.CreateRegistration(new LamarConfigurationServiceProvider(container));

            registration.ConfigureSagas(configurator);
        }

        public static Type[] FindStateMachineSagaTypes(IContainer container)
        {
            return container
                .Model
                .AllInstances.ToArray()
                .Where(x => x.ImplementationType.HasInterface(typeof(SagaStateMachine<>)))
                .Select(x => x.ImplementationType)
                .Distinct()
                .ToArray();
        }
    }
}
