namespace MassTransit
{
    using System;
    using System.Reflection;
    using Automatonymous;
    using ExtensionsDependencyInjectionIntegration.Registration;
    using Microsoft.Extensions.DependencyInjection;
    using Registration;


    public static class DependencyInjectionStateMachineSagaRegistrationExtensions
    {
        /// <summary>
        /// Add the state machine sagas in the specified assembly to the service collection
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="type">The state machine saga type</param>
        public static void RegisterSagaStateMachine(this IServiceCollection collection, Type type)
        {
            var registrar = new DependencyInjectionContainerRegistrar(collection);

            SagaStateMachineRegistrationCache.Register(type, registrar);
        }

        /// <summary>
        /// Add the state machine sagas in the specified assembly to the service collection
        /// </summary>
        /// <param name="collection"></param>
        public static void RegisterSagaStateMachine<TStateMachine, TInstance>(this IServiceCollection collection)
            where TStateMachine : class, SagaStateMachine<TInstance>
            where TInstance : class, SagaStateMachineInstance
        {
            var registrar = new DependencyInjectionContainerRegistrar(collection);

            SagaStateMachineRegistrationCache.Register(typeof(TStateMachine), registrar);
        }

        /// <summary>
        /// Add the state machine sagas in the specified assembly to the service collection
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="assemblies">If specified, only the specified assemblies are scanned</param>
        public static void RegisterSagaStateMachines(this IServiceCollection collection, params Assembly[] assemblies)
        {
            var registrar = new DependencyInjectionContainerRegistrar(collection);

            registrar.RegisterSagaStateMachines(assemblies);
        }

        /// <summary>
        /// Add the state machine sagas by type
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="types">If specified, only the specified assemblies are scanned</param>
        public static void RegisterSagaStateMachines(this IServiceCollection collection, params Type[] types)
        {
            var registrar = new DependencyInjectionContainerRegistrar(collection);

            registrar.RegisterSagaStateMachines(types);
        }
    }
}
