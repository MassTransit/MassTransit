namespace MassTransit
{
    using System;
    using System.Linq;
    using Automatonymous;
    using Internals.Extensions;
    using Registration;
    using StructureMap;
    using StructureMapIntegration;
    using StructureMapIntegration.Registration;


    public static class StructureMapStateMachineLoadSagaExtensions
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
                    x.For<ISagaStateMachineFactory>().Use(provider => new StructureMapSagaStateMachineFactory(container)).Singleton();

                if (container.TryGetInstance<ISagaRepositoryFactory>() == null)
                    x.For<ISagaRepositoryFactory>().Use(provider => new StructureMapSagaRepositoryFactory(container)).Singleton();

                if (container.TryGetInstance<IStateMachineActivityFactory>() == null)
                    x.For<IStateMachineActivityFactory>().Use(provider => new StructureMapStateMachineActivityFactory()).ContainerScoped();
            });

            registrationConfigurator.AddSagaStateMachines(FindStateMachineSagaTypes(container));

            var registration = registrationConfigurator.CreateRegistration(new StructureMapConfigurationServiceProvider(container));

            registration.ConfigureSagas(configurator);
        }

        public static Type[] FindStateMachineSagaTypes(IContainer container)
        {
            return container
                .Model
                .PluginTypes
                .Where(x => x.PluginType.HasInterface(typeof(SagaStateMachine<>)))
                .Select(i => i.PluginType)
                .Distinct()
                .ToArray();
        }
    }
}
