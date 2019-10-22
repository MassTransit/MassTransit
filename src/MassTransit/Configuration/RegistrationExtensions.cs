namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Automatonymous;
    using Courier;
    using Definition;
    using Internals.Extensions;
    using Metadata;
    using Registration;
    using Saga;
    using Util;


    public static class RegistrationExtensions
    {
        /// <summary>
        /// Adds all consumers in the specified assemblies
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="assemblies">The assemblies to scan for consumers</param>
        public static void AddConsumers(this IRegistrationConfigurator configurator, params Assembly[] assemblies)
        {
            if (assemblies.Length == 0)
                assemblies = AppDomain.CurrentDomain.GetAssemblies();

            var types = AssemblyTypeCache.FindTypes(assemblies, IsConsumerOrDefinition).GetAwaiter().GetResult();

            AddConsumers(configurator, types.FindTypes(TypeClassification.Concrete | TypeClassification.Closed).ToArray());
        }

        /// <summary>
        /// Adds all consumers from the assembly containing the specified type that are in the same (or deeper) namespace.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="filter"></param>
        /// <typeparam name="T">The anchor type</typeparam>
        public static void AddConsumersFromNamespaceContaining<T>(this IRegistrationConfigurator configurator, Func<Type, bool> filter = null)
        {
            AddConsumersFromNamespaceContaining(configurator, typeof(T), filter);
        }

        /// <summary>
        /// Adds all consumers in the specified assemblies matching the namespace
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="type">The type to use to identify the assembly and namespace to scan</param>
        /// <param name="filter"></param>
        public static void AddConsumersFromNamespaceContaining(this IRegistrationConfigurator configurator, Type type, Func<Type, bool> filter = null)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (type.Assembly == null || type.Namespace == null)
                throw new ArgumentException($"The type {TypeMetadataCache.GetShortName(type)} is not in an assembly with a valid namespace", nameof(type));

            IEnumerable<Type> types;
            if (filter != null)
            {
                bool IsAllowed(Type candidate) => IsConsumerOrDefinition(candidate) && filter(candidate);

                types = FindTypesInNamespace(type, IsAllowed);
            }
            else
                types = FindTypesInNamespace(type, IsConsumerOrDefinition);

            AddConsumers(configurator, types.ToArray());
        }

        /// <summary>
        /// Adds the specified consumer types
        /// </summary>
        /// <param name="configurator"></param>ˆ
        /// <param name="types">The state machine types to add</param>
        public static void AddConsumers(this IRegistrationConfigurator configurator, params Type[] types)
        {
            IEnumerable<Type> consumerTypes = types.Where(TypeMetadataCache.HasConsumerInterfaces);
            IEnumerable<Type> consumerDefinitionTypes = types.Where(x => x.HasInterface(typeof(IConsumerDefinition<>)));

            var consumers = from c in consumerTypes
                join d in consumerDefinitionTypes on c equals d.GetClosingArgument(typeof(IConsumerDefinition<>)) into dc
                from d in dc.DefaultIfEmpty()
                select new
                {
                    ConsumerType = c,
                    DefinitionType = d
                };

            foreach (var consumer in consumers)
                configurator.AddConsumer(consumer.ConsumerType, consumer.DefinitionType);
        }

        /// <summary>
        /// Adds all sagas in the specified assemblies. If using state machine sagas, they should be added first using AddSagaStateMachines.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="assemblies">The assemblies to scan for consumers</param>
        public static void AddSagas(this IRegistrationConfigurator configurator, params Assembly[] assemblies)
        {
            if (assemblies.Length == 0)
                assemblies = AppDomain.CurrentDomain.GetAssemblies();

            var types = AssemblyTypeCache.FindTypes(assemblies, IsSagaTypeOrDefinition).GetAwaiter().GetResult();

            AddSagas(configurator, types.FindTypes(TypeClassification.Concrete | TypeClassification.Closed).ToArray());
        }

        /// <summary>
        /// Adds all sagas in the specified assemblies matching the namespace. If you are using both state machine and regular sagas, be
        /// sure to call AddSagaStateMachinesFromNamespaceContaining prior to calling this one.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="filter"></param>
        public static void AddSagasFromNamespaceContaining<T>(this IRegistrationConfigurator configurator, Func<Type, bool> filter = null)
        {
            AddSagasFromNamespaceContaining(configurator, typeof(T), filter);
        }

        /// <summary>
        /// Adds all sagas in the specified assemblies matching the namespace. If you are using both state machine and regular sagas, be
        /// sure to call AddSagaStateMachinesFromNamespaceContaining prior to calling this one.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="type">The type to use to identify the assembly and namespace to scan</param>
        /// <param name="filter"></param>
        public static void AddSagasFromNamespaceContaining(this IRegistrationConfigurator configurator, Type type, Func<Type, bool> filter = null)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (type.Assembly == null || type.Namespace == null)
                throw new ArgumentException($"The type {TypeMetadataCache.GetShortName(type)} is not in an assembly with a valid namespace", nameof(type));

            IEnumerable<Type> types;
            if (filter != null)
            {
                bool IsAllowed(Type candidate) => IsSagaTypeOrDefinition(candidate) && filter(candidate);

                types = FindTypesInNamespace(type, IsAllowed);
            }
            else
                types = FindTypesInNamespace(type, IsSagaTypeOrDefinition);

            AddSagas(configurator, types.ToArray());
        }

        /// <summary>
        /// Adds the specified saga types
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="types">The state machine types to add</param>
        public static void AddSagas(this IRegistrationConfigurator configurator, params Type[] types)
        {
            IEnumerable<Type> sagaTypes = types.Where(x => x.HasInterface<ISaga>() && !x.HasInterface<SagaStateMachineInstance>());
            IEnumerable<Type> sagaDefinitionTypes = types.Where(x => x.HasInterface(typeof(ISagaDefinition<>)));

            var sagas = from c in sagaTypes
                join d in sagaDefinitionTypes on c equals d.GetClosingArgument(typeof(ISagaDefinition<>)) into dc
                from d in dc.DefaultIfEmpty()
                select new
                {
                    SagaType = c,
                    DefinitionType = d
                };

            foreach (var saga in sagas)
                configurator.AddSaga(saga.SagaType, saga.DefinitionType);
        }

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
                join d in sagaDefinitionTypes on c.GetClosingArgument(typeof(SagaStateMachine<>))
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

        /// <summary>
        /// Adds all activities (including execute-only activities) in the specified assemblies.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="assemblies">The assemblies to scan for consumers</param>
        public static void AddActivities(this IRegistrationConfigurator configurator, params Assembly[] assemblies)
        {
            if (assemblies.Length == 0)
                assemblies = AppDomain.CurrentDomain.GetAssemblies();

            var types = AssemblyTypeCache.FindTypes(assemblies, IsActivityTypeOrDefinition).GetAwaiter().GetResult();

            AddActivities(configurator, types.FindTypes(TypeClassification.Concrete | TypeClassification.Closed).ToArray());
        }

        /// <summary>
        /// Adds all activities (including execute-only activities) in the specified assemblies matching the namespace.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="filter"></param>
        public static void AddActivitiesFromNamespaceContaining<T>(this IRegistrationConfigurator configurator, Func<Type, bool> filter = null)
        {
            AddActivitiesFromNamespaceContaining(configurator, typeof(T), filter);
        }

        /// <summary>
        /// Adds all activities (including execute-only activities) in the specified assemblies matching the namespace.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="type">The type to use to identify the assembly and namespace to scan</param>
        /// <param name="filter"></param>
        public static void AddActivitiesFromNamespaceContaining(this IRegistrationConfigurator configurator, Type type, Func<Type, bool> filter = null)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (type.Assembly == null || type.Namespace == null)
                throw new ArgumentException($"The type {TypeMetadataCache.GetShortName(type)} is not in an assembly with a valid namespace", nameof(type));

            IEnumerable<Type> types;
            if (filter != null)
            {
                bool IsAllowed(Type candidate) => IsActivityTypeOrDefinition(candidate) && filter(candidate);

                types = FindTypesInNamespace(type, IsAllowed);
            }
            else
                types = FindTypesInNamespace(type, IsActivityTypeOrDefinition);

            AddActivities(configurator, types.ToArray());
        }

        /// <summary>
        /// Adds the specified activity types
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="types">The state machine types to add</param>
        public static void AddActivities(this IRegistrationConfigurator configurator, params Type[] types)
        {
            IEnumerable<Type> activityTypes = types.Where(x => x.HasInterface(typeof(IActivity<,>)));
            IEnumerable<Type> activityDefinitionTypes = types.Where(x => x.HasInterface(typeof(IActivityDefinition<,,>)));

            var activities = from c in activityTypes
                join d in activityDefinitionTypes on c equals d.GetClosingArguments(typeof(IActivityDefinition<,,>)).First() into dc
                from d in dc.DefaultIfEmpty()
                select new
                {
                    ActivityType = c,
                    DefinitionType = d
                };

            foreach (var activity in activities)
                configurator.AddActivity(activity.ActivityType, activity.DefinitionType);

            IEnumerable<Type> executeActivityTypes = types.Where(x => x.HasInterface(typeof(IExecuteActivity<>))).Except(activityTypes);
            IEnumerable<Type> executeActivityDefinitionTypes = types.Where(x => x.HasInterface(typeof(IExecuteActivityDefinition<,>)));

            var executeActivities = from c in executeActivityTypes
                join d in executeActivityDefinitionTypes on c equals d.GetClosingArguments(typeof(IExecuteActivityDefinition<,>)).First() into dc
                from d in dc.DefaultIfEmpty()
                select new
                {
                    ActivityType = c,
                    DefinitionType = d
                };

            foreach (var executeActivity in executeActivities)
                configurator.AddExecuteActivity(executeActivity.ActivityType, executeActivity.DefinitionType);
        }

        static bool IsConsumerOrDefinition(Type type)
        {
            return TypeMetadataCache.HasConsumerInterfaces(type) && !TypeMetadataCache.HasSagaInterfaces(type)
                || type.HasInterface(typeof(IConsumerDefinition<>));
        }

        static bool IsSagaTypeOrDefinition(Type type)
        {
            return TypeMetadataCache.HasSagaInterfaces(type) || type.HasInterface(typeof(ISagaDefinition<>));
        }

        static bool IsSagaStateMachineOrDefinition(Type type)
        {
            return type.HasInterface(typeof(SagaStateMachine<>))
                || type.HasInterface(typeof(ISagaDefinition<>));
        }

        static bool IsActivityTypeOrDefinition(Type type)
        {
            return type.HasInterface(typeof(IExecuteActivity<>))
                || type.HasInterface(typeof(IActivityDefinition<,,>))
                || type.HasInterface(typeof(ICompensateActivity<>));
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
    }
}
