namespace MassTransit
{
    using System;
    using System.Reflection;
    using Automatonymous;
    using Castle.MicroKernel;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;


    public static class WindsorStateMachineRegisterSagaExtensions
    {
        /// <summary>
        /// Register the state machine sagas found in the specified assemblies using the Container provided. The
        /// machines are registered using their SagaStateMachine&lt;&gt; type, as well as the concrete type for use by
        /// the application.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="assemblies"></param>
        public static void RegisterStateMachineSagas(this IWindsorContainer container, params Assembly[] assemblies)
        {
            if (container == null)
                throw new ArgumentNullException(nameof(container));

            RegisterStateMachineSagas(container.Kernel, assemblies);
        }

        /// <summary>
        /// Register the state machine sagas found in the specified assemblies using the Container provided. The
        /// machines are registered using their SagaStateMachine&lt;&gt; type, as well as the concrete type for use by
        /// the application.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="assemblies"></param>
        public static void RegisterStateMachineSagas(this IKernel container, params Assembly[] assemblies)
        {
            if (container == null)
                throw new ArgumentNullException(nameof(container));

            if (assemblies == null)
                throw new ArgumentNullException(nameof(assemblies));

            foreach (var assembly in assemblies)
            {
                container.Register(Classes.FromAssembly(assembly)
                    .BasedOn(typeof(SagaStateMachine<>))
                    .WithServiceAllInterfaces()
                    .WithServiceSelf()
                    .LifestyleTransient()
                );
            }
        }
    }
}
