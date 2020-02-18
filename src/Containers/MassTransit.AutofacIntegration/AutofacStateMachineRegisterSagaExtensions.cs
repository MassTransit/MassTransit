namespace MassTransit
{
    using System;
    using System.Reflection;
    using Autofac;
    using AutofacIntegration.Registration;
    using Automatonymous;
    using Registration;


    public static class AutofacStateMachineRegisterSagaExtensions
    {
        /// <summary>
        /// Add the state machine sagas in the specified assembly to the service collection
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="type">The state machine saga type</param>
        public static void RegisterSagaStateMachine(this ContainerBuilder builder, Type type)
        {
            var registrar = new AutofacContainerRegistrar(builder);

            SagaStateMachineRegistrationCache.Register(type, registrar);
        }

        /// <summary>
        /// Add the state machine sagas in the specified assembly to the service collection
        /// </summary>
        /// <param name="builder"></param>
        public static void RegisterSagaStateMachine<TStateMachine, TInstance>(this ContainerBuilder builder)
            where TStateMachine : class, SagaStateMachine<TInstance>
            where TInstance : class, SagaStateMachineInstance
        {
            var registrar = new AutofacContainerRegistrar(builder);

            SagaStateMachineRegistrationCache.Register(typeof(TStateMachine), registrar);
        }

        /// <summary>
        /// Add the state machine sagas in the specified assembly to the service collection
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="assemblies">If specified, only the specified assemblies are scanned</param>
        public static void RegisterSagaStateMachines(this ContainerBuilder builder, params Assembly[] assemblies)
        {
            var registrar = new AutofacContainerRegistrar(builder);

            registrar.RegisterSagaStateMachines(assemblies);
        }

        /// <summary>
        /// Add the state machine sagas by type
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="types">If specified, only the specified assemblies are scanned</param>
        public static void RegisterSagaStateMachines(this ContainerBuilder builder, params Type[] types)
        {
            var registrar = new AutofacContainerRegistrar(builder);

            registrar.RegisterSagaStateMachines(types);
        }
    }
}
