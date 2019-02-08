// Copyright 2007-2019 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the
// License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR
// CONDITIONS OF ANY KIND, either express or implied. See the License for the
// specific language governing permissions and limitations under the License.
namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Courier;
    using Definition;
    using Internals.Extensions;
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

            AddConsumers(configurator, types.FindTypes(TypeClassification.Concrete).ToArray());
        }

        /// <summary>
        /// Adds all consumers from the assembly containing the specified type that are in the same (or deeper) namespace.
        /// </summary>
        /// <param name="configurator"></param>
        /// <typeparam name="T">The anchor type</typeparam>
        public static void AddConsumersFromNamespaceContaining<T>(this IRegistrationConfigurator configurator)
        {
            AddConsumersFromNamespaceContaining(configurator, typeof(T));
        }

        /// <summary>
        /// Adds all consumers in the specified assemblies matching the namespace
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="type">The type to use to identify the assembly and namespace to scan</param>
        public static void AddConsumersFromNamespaceContaining(this IRegistrationConfigurator configurator, Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (type.Assembly == null || type.Namespace == null)
                throw new ArgumentException($"The type {TypeMetadataCache.GetShortName(type)} is not in an assembly with a valid namespace", nameof(type));

            IEnumerable<Type> types = FindTypesInNamespace(type, IsConsumerOrDefinition);

            AddConsumers(configurator, types.ToArray());
        }

        static bool IsConsumerOrDefinition(Type type)
        {
            return TypeMetadataCache.HasConsumerInterfaces(type) && !TypeMetadataCache.HasSagaInterfaces(type)
                || type.HasInterface(typeof(IConsumerDefinition<>));
        }

        /// <summary>
        /// Adds the specified consumer types
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="types">The state machine types to add</param>
        public static void AddConsumers(this IRegistrationConfigurator configurator, params Type[] types)
        {
            IEnumerable<Type> consumerTypes = types.Where(TypeMetadataCache.HasConsumerInterfaces);
            IEnumerable<Type> consumerDefinitionTypes = types.Where(x => x.HasInterface(typeof(IConsumerDefinition<>)));

            var consumers = from c in consumerTypes
                join d in consumerDefinitionTypes on c equals d.GetClosingArgument(typeof(IConsumerDefinition<>)) into dc
                from d in dc.DefaultIfEmpty()
                select new {ConsumerType = c, DefinitionType = d};

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

            AddSagas(configurator, types.FindTypes(TypeClassification.Concrete).ToArray());
        }

        static bool IsSagaTypeOrDefinition(Type type)
        {
            return TypeMetadataCache.HasSagaInterfaces(type)
                || type.HasInterface(typeof(ISagaDefinition<>));
        }

        /// <summary>
        /// Adds all sagas in the specified assemblies matching the namespace. If you are using both state machine and regular sagas, be
        /// sure to call AddSagaStateMachinesFromNamespaceContaining prior to calling this one.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="type">The type to use to identify the assembly and namespace to scan</param>
        public static void AddSagasFromNamespaceContaining(this IRegistrationConfigurator configurator, Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (type.Assembly == null || type.Namespace == null)
                throw new ArgumentException($"The type {TypeMetadataCache.GetShortName(type)} is not in an assembly with a valid namespace", nameof(type));

            IEnumerable<Type> types = FindTypesInNamespace(type, IsSagaTypeOrDefinition);

            AddSagas(configurator, types.ToArray());
        }

        /// <summary>
        /// Adds the specified saga types
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="types">The state machine types to add</param>
        public static void AddSagas(this IRegistrationConfigurator configurator, params Type[] types)
        {
            IEnumerable<Type> sagaTypes = types.Where(x => x.HasInterface<ISaga>());
            IEnumerable<Type> sagaDefinitionTypes = types.Where(x => x.HasInterface(typeof(ISagaDefinition<>)));

            var sagas = from c in sagaTypes
                join d in sagaDefinitionTypes on c equals d.GetClosingArgument(typeof(ISagaDefinition<>)) into dc
                from d in dc.DefaultIfEmpty()
                select new {SagaType = c, DefinitionType = d};

            foreach (var saga in sagas)
                configurator.AddSaga(saga.SagaType, saga.DefinitionType);
        }

        /// <summary>
        /// Adds all activities (including execute-only activities) in the specified assemblies. If using state machine sagas, they should be
        /// added first using AddSagaStateMachines.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="ns">The namespace to filter</param>
        /// <param name="assemblies">The assemblies to scan for consumers</param>
        public static void AddActivities(this IRegistrationConfigurator configurator, string ns, params Assembly[] assemblies)
        {
            if (assemblies.Length == 0)
                assemblies = AppDomain.CurrentDomain.GetAssemblies();

            var types = AssemblyTypeCache.FindTypes(assemblies, x => x.HasInterface(typeof(Activity<,>))
                && x.Namespace != null && x.Namespace.StartsWith(ns, StringComparison.OrdinalIgnoreCase)).GetAwaiter().GetResult();

            AddConsumers(configurator, types.FindTypes(TypeClassification.Concrete).ToArray());
        }

        /// <summary>
        /// Adds the specified activity types
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="types">The state machine types to add</param>
        public static void AddActivities(this IRegistrationConfigurator configurator, params Type[] types)
        {
            foreach (var type in types)
            {
                if (type.HasInterface(typeof(Activity<,>)))
                    configurator.AddActivity(type);
                else if (type.HasInterface(typeof(ExecuteActivity<>)))
                    configurator.AddExecuteActivity(type);
                else
                    throw new ArgumentException($"The type is not an activity: {TypeMetadataCache.GetShortName(type)}", nameof(types));
            }
        }

        static IEnumerable<Type> FindTypesInNamespace(Type type, Func<Type, bool> typeFilter)
        {
            bool Filter(Type candidate)
            {
                return typeFilter(candidate)
                    && candidate.Namespace != null
                    && candidate.Namespace.StartsWith(type.Namespace, StringComparison.OrdinalIgnoreCase);
            }

            return AssemblyTypeCache.FindTypes(type.Assembly, TypeClassification.Concrete, Filter).GetAwaiter().GetResult();
        }
    }
}
