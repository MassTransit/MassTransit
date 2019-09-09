namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Automatonymous;
    using Definition;
    using Internals.Extensions;
    using Registration;
    using Util;


    public static class AutomatonymousRegistrationExtensions
    {
        /// <summary>
        /// Adds SagaStateMachines to the registry, using the factory method, and updates the registrar prior to registering so that the default
        /// saga registrar isn't notified.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="assemblies">The assemblies to scan for state machines</param>
        public static void AddSagaStateMachines(this IRegistrationConfigurator configurator, params Assembly[] assemblies)
        {
            if (assemblies.Length == 0)
                assemblies = AppDomain.CurrentDomain.GetAssemblies();

            var types = AssemblyTypeCache.FindTypes(assemblies, IsSagaStateMachineOrDefinition).GetAwaiter().GetResult();

            configurator.AddSagaStateMachines(types.FindTypes(TypeClassification.Concrete | TypeClassification.Closed).ToArray());
        }

        /// <summary>
        /// Adds all sagas in the specified assemblies matching the namespace. If you are using both state machine and regular sagas, be
        /// sure to call AddSagaStateMachinesFromNamespaceContaining prior to calling this one.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="type">The type to use to identify the assembly and namespace to scan</param>
        /// <param name="filter"></param>
        public static void AddSagaStateMachinesFromNamespaceContaining(this IRegistrationConfigurator configurator, Type type, Func<Type, bool> filter = null)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (type.Assembly == null || type.Namespace == null)
                throw new ArgumentException($"The type {TypeMetadataCache.GetShortName(type)} is not in an assembly with a valid namespace", nameof(type));

            IEnumerable<Type> types;
            if (filter != null)
            {
                bool IsAllowed(Type candidate)
                {
                    return IsSagaStateMachineOrDefinition(candidate) && filter(candidate);
                }

                types = FindTypesInNamespace(type, IsAllowed);
            }
            else
                types = FindTypesInNamespace(type, IsSagaStateMachineOrDefinition);

            AddSagaStateMachines(configurator, types.ToArray());
        }

        static bool IsSagaStateMachineOrDefinition(Type type)
        {
            return type.HasInterface(typeof(SagaStateMachine<>))
                || type.HasInterface(typeof(ISagaDefinition<>));
        }

        static IEnumerable<Type> FindTypesInNamespace(Type type, Func<Type, bool> typeFilter)
        {
            bool Filter(Type candidate)
            {
                return typeFilter(candidate)
                    && candidate.Namespace != null
                    && candidate.Namespace.StartsWith(type.Namespace, StringComparison.OrdinalIgnoreCase);
            }

            return AssemblyTypeCache.FindTypes(type.Assembly, TypeClassification.Concrete | TypeClassification.Closed, Filter).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Adds SagaStateMachines to the registry, using the factory method, and updates the registrar prior to registering so that the default
        /// saga registrar isn't notified.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="types">The state machine types to add</param>
        public static void AddSagaStateMachines(this IRegistrationConfigurator configurator, params Type[] types)
        {
            IEnumerable<Type> sagaTypes = types.Where(x => x.HasInterface(typeof(SagaStateMachine<>)));
            IEnumerable<Type> sagaDefinitionTypes = types.Where(x => x.HasInterface(typeof(ISagaDefinition<>)));

            var sagas = from c in sagaTypes
                join d in sagaDefinitionTypes on c.GetClosingArguments(typeof(SagaStateMachine<>)).Single()
                    equals d.GetClosingArguments(typeof(ISagaDefinition<>)).Single() into dc
                from d in dc.DefaultIfEmpty()
                select new
                {
                    SagaType = c,
                    DefinitionType = d
                };

            foreach (var saga in sagas)
                configurator.AddSagaStateMachine(saga.SagaType, saga.DefinitionType);
        }

        public static void RegisterSagaStateMachines(this IContainerRegistrar registrar, params Assembly[] assemblies)
        {
            if (assemblies.Length == 0)
                assemblies = AppDomain.CurrentDomain.GetAssemblies();

            var types = AssemblyTypeCache.FindTypes(assemblies, x => x.HasInterface(typeof(SagaStateMachine<>))).GetAwaiter().GetResult();

            registrar.RegisterSagaStateMachines(types.FindTypes(TypeClassification.Concrete | TypeClassification.Closed).ToArray());
        }

        public static void RegisterSagaStateMachines(this IContainerRegistrar registrar, params Type[] types)
        {
            foreach (var type in types)
            {
                if (!type.HasInterface(typeof(SagaStateMachine<>)))
                    throw new ArgumentException($"The type is not a saga state machine: {TypeMetadataCache.GetShortName(type)}", nameof(types));

                SagaStateMachineRegistrationCache.Register(type, registrar);
            }
        }
    }
}
