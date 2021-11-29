namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Courier;
    using DependencyInjection.Registration;
    using Internals;
    using Metadata;
    using Util;


    public static class RegistrationExtensions
    {
        /// <summary>
        /// Adds the consumer, allowing configuration when it is configured on an endpoint
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        /// <typeparam name="T">The consumer type</typeparam>
        /// <typeparam name="TDefinition">The consumer definition type</typeparam>
        public static IConsumerRegistrationConfigurator<T> AddConsumer<T, TDefinition>(this IRegistrationConfigurator configurator,
            Action<IConsumerConfigurator<T>> configure = null)
            where T : class, IConsumer
            where TDefinition : class, IConsumerDefinition<T>
        {
            return configurator.AddConsumer(typeof(TDefinition), configure);
        }

        /// <summary>
        /// Adds all consumers in the specified assemblies
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="assemblies">The assemblies to scan for consumers</param>
        public static void AddConsumers(this IRegistrationConfigurator configurator, params Assembly[] assemblies)
        {
            AddConsumers(configurator, null, assemblies);
        }

        /// <summary>
        /// Adds all consumers that match the given filter in the specified assemblies
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="filter"></param>
        /// <param name="assemblies">The assemblies to scan for consumers</param>
        public static void AddConsumers(this IRegistrationConfigurator configurator, Func<Type, bool> filter, params Assembly[] assemblies)
        {
            if (assemblies.Length == 0)
                assemblies = AppDomain.CurrentDomain.GetAssemblies();

            var types = AssemblyTypeCache.FindTypes(assemblies, RegistrationMetadata.IsConsumerOrDefinition).GetAwaiter().GetResult();

            AddConsumers(configurator, filter, types.FindTypes(TypeClassification.Concrete | TypeClassification.Closed).ToArray());
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
                throw new ArgumentException($"The type {TypeCache.GetShortName(type)} is not in an assembly with a valid namespace", nameof(type));

            IEnumerable<Type> types;
            if (filter != null)
            {
                bool IsAllowed(Type candidate)
                {
                    return RegistrationMetadata.IsConsumerOrDefinition(candidate) && filter(candidate);
                }

                types = FindTypesInNamespace(type, IsAllowed);
            }
            else
                types = FindTypesInNamespace(type, RegistrationMetadata.IsConsumerOrDefinition);

            AddConsumers(configurator, types.ToArray());
        }

        /// <summary>
        /// Adds the specified consumer types
        /// </summary>
        /// <param name="configurator"></param>
        /// ˆ
        /// <param name="types">The state machine types to add</param>
        public static void AddConsumers(this IRegistrationConfigurator configurator, params Type[] types)
        {
            AddConsumers(configurator, null, types);
        }

        /// <summary>
        /// Adds the specified consumer types which match the given filter
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="filter"></param>
        /// <param name="types">The consumer types to add</param>
        public static void AddConsumers(this IRegistrationConfigurator configurator, Func<Type, bool> filter, params Type[] types)
        {
            filter ??= t => true;

            IEnumerable<Type> consumerTypes = types.Where(MessageTypeCache.HasConsumerInterfaces);
            IEnumerable<Type> consumerDefinitionTypes = types.Where(x => x.HasInterface(typeof(IConsumerDefinition<>)));

            var consumers = from c in consumerTypes
                join d in consumerDefinitionTypes on c equals d.GetClosingArgument(typeof(IConsumerDefinition<>)) into dc
                from d in dc.DefaultIfEmpty()
                where filter(c)
                select new
                {
                    ConsumerType = c,
                    DefinitionType = d
                };

            foreach (var consumer in consumers)
                configurator.AddConsumer(consumer.ConsumerType, consumer.DefinitionType);
        }

        /// <summary>
        /// Adds the saga, allowing configuration when it is configured on the endpoint. This should not
        /// be used for state machine (Automatonymous) sagas.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        /// <typeparam name="T">The saga type</typeparam>
        /// <typeparam name="TDefinition">The saga definition type</typeparam>
        public static ISagaRegistrationConfigurator<T> AddSaga<T, TDefinition>(this IRegistrationConfigurator configurator,
            Action<ISagaConfigurator<T>> configure = null)
            where T : class, ISaga
            where TDefinition : class, ISagaDefinition<T>
        {
            return configurator.AddSaga(typeof(TDefinition), configure);
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

            var types = AssemblyTypeCache.FindTypes(assemblies, RegistrationMetadata.IsSagaOrDefinition).GetAwaiter().GetResult();

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
                throw new ArgumentException($"The type {TypeCache.GetShortName(type)} is not in an assembly with a valid namespace", nameof(type));

            IEnumerable<Type> types;
            if (filter != null)
            {
                bool IsAllowed(Type candidate)
                {
                    return RegistrationMetadata.IsSagaOrDefinition(candidate) && filter(candidate);
                }

                types = FindTypesInNamespace(type, IsAllowed);
            }
            else
                types = FindTypesInNamespace(type, RegistrationMetadata.IsSagaOrDefinition);

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
        /// Adds a SagaStateMachine to the registry and updates the registrar prior to registering so that the default
        /// saga registrar isn't notified.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        /// <typeparam name="TStateMachine">The state machine type</typeparam>
        /// <typeparam name="T">The state machine instance type</typeparam>
        /// <typeparam name="TDefinition">The saga definition type</typeparam>
        public static ISagaRegistrationConfigurator<T> AddSagaStateMachine<TStateMachine, T, TDefinition>(this IRegistrationConfigurator configurator,
            Action<ISagaConfigurator<T>> configure = null)
            where T : class, SagaStateMachineInstance
            where TStateMachine : class, SagaStateMachine<T>
            where TDefinition : class, ISagaDefinition<T>
        {
            return configurator.AddSagaStateMachine<TStateMachine, T>(typeof(TDefinition), configure);
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

            var types = AssemblyTypeCache.FindTypes(assemblies,
                    type => RegistrationMetadata.IsSagaStateMachineOrDefinition(type) && !RegistrationMetadata.IsFutureOrDefinition(type))
                .GetAwaiter().GetResult();

            configurator.AddSagaStateMachines(types.FindTypes(TypeClassification.Concrete | TypeClassification.Closed).ToArray());
        }

        /// <summary>
        /// Adds all saga state machines in the specified assemblies matching the namespace. If you are using both state machine and regular sagas, be
        /// sure to call AddSagasFromNamespaceContaining after calling this one.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="filter"></param>
        public static void AddSagaStateMachinesFromNamespaceContaining<T>(this IRegistrationConfigurator configurator, Func<Type, bool> filter = null)
        {
            AddSagaStateMachinesFromNamespaceContaining(configurator, typeof(T), filter);
        }

        /// <summary>
        /// Adds all saga state machines in the specified assemblies matching the namespace. If you are using both state machine and regular sagas, be
        /// sure to call AddSagasFromNamespaceContaining after calling this one.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="type">The type to use to identify the assembly and namespace to scan</param>
        /// <param name="filter"></param>
        public static void AddSagaStateMachinesFromNamespaceContaining(this IRegistrationConfigurator configurator, Type type, Func<Type, bool> filter = null)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (type.Assembly == null || type.Namespace == null)
                throw new ArgumentException($"The type {TypeCache.GetShortName(type)} is not in an assembly with a valid namespace", nameof(type));

            IEnumerable<Type> types;
            if (filter != null)
            {
                bool IsAllowed(Type candidate)
                {
                    return RegistrationMetadata.IsSagaStateMachineOrDefinition(candidate)
                        && !RegistrationMetadata.IsFutureOrDefinition(type)
                        && filter(candidate);
                }

                types = FindTypesInNamespace(type, IsAllowed);
            }
            else
                types = FindTypesInNamespace(type, RegistrationMetadata.IsSagaStateMachineOrDefinition);

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

        /// <summary>
        /// Adds an execute activity (Courier), allowing configuration when it is configured on the endpoint.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        /// <typeparam name="TActivity">The activity type</typeparam>
        /// <typeparam name="TArguments">The argument type</typeparam>
        /// <typeparam name="TDefinition">The activity definition type</typeparam>
        /// <returns></returns>
        public static IExecuteActivityRegistrationConfigurator<TActivity, TArguments> AddExecuteActivity<TActivity, TArguments, TDefinition>(
            this IRegistrationConfigurator configurator, Action<IExecuteActivityConfigurator<TActivity, TArguments>> configure = null)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
            where TDefinition : class, IExecuteActivityDefinition<TActivity, TArguments>
        {
            return configurator.AddExecuteActivity(typeof(TDefinition), configure);
        }

        /// <summary>
        /// Adds an activity (Courier), allowing configuration when it is configured on the endpoint.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configureExecute">The execute configuration callback</param>
        /// <param name="configureCompensate">The compensate configuration callback</param>
        /// <typeparam name="TActivity">The activity type</typeparam>
        /// <typeparam name="TArguments">The argument type</typeparam>
        /// <typeparam name="TLog">The log type</typeparam>
        /// <typeparam name="TDefinition">The activity definition type</typeparam>
        /// <returns></returns>
        public static IActivityRegistrationConfigurator<TActivity, TArguments, TLog> AddActivity<TActivity, TArguments, TLog, TDefinition>(
            this IRegistrationConfigurator configurator,
            Action<IExecuteActivityConfigurator<TActivity, TArguments>> configureExecute = null,
            Action<ICompensateActivityConfigurator<TActivity, TLog>> configureCompensate = null)
            where TActivity : class, IActivity<TArguments, TLog>
            where TArguments : class
            where TDefinition : class, IActivityDefinition<TActivity, TArguments, TLog>
            where TLog : class
        {
            return configurator.AddActivity(typeof(TDefinition), configureExecute, configureCompensate);
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

            var types = AssemblyTypeCache.FindTypes(assemblies, RegistrationMetadata.IsActivityOrDefinition).GetAwaiter().GetResult();

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
                throw new ArgumentException($"The type {TypeCache.GetShortName(type)} is not in an assembly with a valid namespace", nameof(type));

            IEnumerable<Type> types;
            if (filter != null)
            {
                bool IsAllowed(Type candidate)
                {
                    return RegistrationMetadata.IsActivityOrDefinition(candidate) && filter(candidate);
                }

                types = FindTypesInNamespace(type, IsAllowed);
            }
            else
                types = FindTypesInNamespace(type, RegistrationMetadata.IsActivityOrDefinition);

            AddActivities(configurator, types.ToArray());
        }

        /// <summary>
        /// Adds the specified activity types
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="types">The state machine types to add</param>
        public static void AddActivities(this IRegistrationConfigurator configurator, params Type[] types)
        {
            IEnumerable<Type> activityTypes = types.Where(x => x.HasInterface(typeof(IActivity<,>))).ToList();
            IEnumerable<Type> activityDefinitionTypes = types.Where(x => x.HasInterface(typeof(IActivityDefinition<,,>))).ToList();

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

            IEnumerable<Type> executeActivityTypes = types.Where(x => x.HasInterface(typeof(IExecuteActivity<>))).Except(activityTypes).ToList();
            IEnumerable<Type> executeActivityDefinitionTypes = types.Where(x => x.HasInterface(typeof(IExecuteActivityDefinition<,>))).ToList();

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

        /// <summary>
        /// Configure the default endpoint name formatter in the container
        /// </summary>
        /// <param name="configurator"></param>
        public static void SetDefaultEndpointNameFormatter(this IRegistrationConfigurator configurator)
        {
            configurator.SetEndpointNameFormatter(DefaultEndpointNameFormatter.Instance);
        }

        public static void SetSnakeCaseEndpointNameFormatter(this IRegistrationConfigurator configurator)
        {
            configurator.SetEndpointNameFormatter(SnakeCaseEndpointNameFormatter.Instance);
        }

        /// <summary>
        /// Configure the Kebab Case endpoint name formatter
        /// </summary>
        /// <param name="configurator"></param>
        public static void SetKebabCaseEndpointNameFormatter(this IRegistrationConfigurator configurator)
        {
            configurator.SetEndpointNameFormatter(KebabCaseEndpointNameFormatter.Instance);
        }

        static IEnumerable<Type> FindTypesInNamespace(Type type, Func<Type, bool> typeFilter)
        {
            if (type.Namespace == null)
                throw new ArgumentException("The type must have a valid namespace", nameof(type));

            var dottedNamespace = type.Namespace + ".";

            bool Filter(Type candidate)
            {
                return typeFilter(candidate)
                    && candidate.Namespace != null
                    && (candidate.Namespace.StartsWith(dottedNamespace, StringComparison.OrdinalIgnoreCase)
                        || candidate.Namespace.Equals(type.Namespace, StringComparison.OrdinalIgnoreCase));
            }

            return AssemblyTypeCache.FindTypes(type.Assembly, TypeClassification.Concrete | TypeClassification.Closed, Filter).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Adds the consumer, allowing configuration when it is configured on an endpoint
        /// </summary>
        /// <param name="configurator"></param>
        /// <typeparam name="T">The consumer type</typeparam>
        /// <typeparam name="TDefinition">The consumer definition type</typeparam>
        public static IFutureRegistrationConfigurator<T> AddFuture<T, TDefinition>(this IRegistrationConfigurator configurator)
            where T : MassTransitStateMachine<FutureState>
            where TDefinition : class, IFutureDefinition<T>
        {
            return configurator.AddFuture<T>(typeof(TDefinition));
        }

        /// <summary>
        /// Adds a combined consumer/future, where the future handles the requests and the consumer is only known to the future.
        /// This is a shortcut method,
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        /// <typeparam name="TFuture">The consumer type</typeparam>
        /// <typeparam name="TConsumer"></typeparam>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        public static IFutureRegistrationConfigurator<TFuture> AddFutureRequestConsumer<TFuture, TConsumer, TRequest, TResponse>(
            this IRegistrationConfigurator configurator, Action<IConsumerConfigurator<TConsumer>> configure = null)
            where TFuture : Future<TRequest, TResponse>
            where TRequest : class
            where TResponse : class
            where TConsumer : class, IConsumer<TRequest>
        {
            configurator.AddConsumer<TConsumer, FutureRequestConsumerDefinition<TConsumer, TRequest>>(configure);

            return configurator.AddFuture<TFuture, RequestConsumerFutureDefinition<TFuture, TConsumer, TRequest, TResponse>>();
        }

        /// <summary>
        /// Adds all futures in the specified assemblies
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="assemblies">The assemblies to scan for futures</param>
        public static void AddFutures(this IRegistrationConfigurator configurator, params Assembly[] assemblies)
        {
            AddFutures(configurator, null, assemblies);
        }

        /// <summary>
        /// Adds all futures that match the given filter in the specified assemblies
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="filter"></param>
        /// <param name="assemblies">The assemblies to scan for futures</param>
        public static void AddFutures(this IRegistrationConfigurator configurator, Func<Type, bool> filter, params Assembly[] assemblies)
        {
            if (assemblies.Length == 0)
                assemblies = AppDomain.CurrentDomain.GetAssemblies();

            var types = AssemblyTypeCache.FindTypes(assemblies, RegistrationMetadata.IsFutureOrDefinition).GetAwaiter().GetResult();

            AddFutures(configurator, filter, types.FindTypes(TypeClassification.Concrete | TypeClassification.Closed).ToArray());
        }

        /// <summary>
        /// Adds all futures from the assembly containing the specified type that are in the same (or deeper) namespace.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="filter"></param>
        /// <typeparam name="T">The anchor type</typeparam>
        public static void AddFuturesFromNamespaceContaining<T>(this IRegistrationConfigurator configurator, Func<Type, bool> filter = null)
        {
            AddFuturesFromNamespaceContaining(configurator, typeof(T), filter);
        }

        /// <summary>
        /// Adds all futures in the specified assemblies matching the namespace
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="type">The type to use to identify the assembly and namespace to scan</param>
        /// <param name="filter"></param>
        public static void AddFuturesFromNamespaceContaining(this IRegistrationConfigurator configurator, Type type, Func<Type, bool> filter = null)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (type.Assembly == null || type.Namespace == null)
                throw new ArgumentException($"The type {TypeCache.GetShortName(type)} is not in an assembly with a valid namespace", nameof(type));

            IEnumerable<Type> types;
            if (filter != null)
            {
                bool IsAllowed(Type candidate)
                {
                    return RegistrationMetadata.IsFutureOrDefinition(candidate) && filter(candidate);
                }

                types = FindTypesInNamespace(type, IsAllowed);
            }
            else
                types = FindTypesInNamespace(type, RegistrationMetadata.IsFutureOrDefinition);

            AddFutures(configurator, types.ToArray());
        }

        /// <summary>
        /// Adds the specified consumer types
        /// </summary>
        /// <param name="configurator"></param>
        /// ˆ
        /// <param name="types">The state machine types to add</param>
        public static void AddFutures(this IRegistrationConfigurator configurator, params Type[] types)
        {
            AddFutures(configurator, null, types);
        }

        /// <summary>
        /// Adds the specified consumer types which match the given filter
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="filter"></param>
        /// <param name="types">The consumer types to add</param>
        public static void AddFutures(this IRegistrationConfigurator configurator, Func<Type, bool> filter, params Type[] types)
        {
            filter ??= t => true;

            IEnumerable<Type> consumerTypes = types.Where(x => x.HasInterface(typeof(SagaStateMachine<FutureState>)));
            IEnumerable<Type> consumerDefinitionTypes = types.Where(x => x.HasInterface(typeof(IFutureDefinition<>)));

            var futures = from c in consumerTypes
                join d in consumerDefinitionTypes on c equals d.GetClosingArgument(typeof(IFutureDefinition<>)) into dc
                from d in dc.DefaultIfEmpty()
                where filter(c)
                select new
                {
                    FutureType = c,
                    DefinitionType = d
                };

            foreach (var future in futures)
                configurator.AddFuture(future.FutureType, future.DefinitionType);
        }
    }
}
