#nullable enable
namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using Util;


    public static class SqlPublishTopologyConfigurationExtensions
    {
        /// <summary>
        /// Adds any valid message types found in the specified namespace to the publish topology
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        /// <param name="filter"></param>
        public static void AddPublishMessageTypesFromNamespaceContaining<T>(this ISqlBusFactoryConfigurator configurator,
            Action<ISqlMessagePublishTopologyConfigurator, Type>? configure = null, Func<Type, bool>? filter = null)
        {
            AddPublishMessageTypesFromNamespaceContaining(configurator, typeof(T), configure, filter);
        }

        /// <summary>
        /// Adds any valid message types found in the specified namespace to the publish topology
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="type">The type to use to identify the assembly and namespace to scan</param>
        /// <param name="configure"></param>
        /// <param name="filter"></param>
        public static void AddPublishMessageTypesFromNamespaceContaining(this ISqlBusFactoryConfigurator configurator, Type type,
            Action<ISqlMessagePublishTopologyConfigurator, Type>? configure = null, Func<Type, bool>? filter = null)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (type.Assembly == null || type.Namespace == null)
                throw new ArgumentException($"The type {TypeCache.GetShortName(type)} is not in an assembly with a valid namespace", nameof(type));

            IEnumerable<Type> types;

            const TypeClassification typeClassification = TypeClassification.Concrete | TypeClassification.Closed | TypeClassification.Abstract
                | TypeClassification.Interface;

            if (filter != null)
            {
                bool IsAllowed(Type candidate)
                {
                    return MessageTypeCache.IsValidMessageType(candidate) && filter(candidate);
                }

                types = AssemblyTypeCache.FindTypesInNamespace(type, IsAllowed, typeClassification);
            }
            else
                types = AssemblyTypeCache.FindTypesInNamespace(type, MessageTypeCache.IsValidMessageType, typeClassification);

            foreach (var messageType in types)
                configurator.Publish(messageType, x => configure?.Invoke(x, messageType));
        }

        /// <summary>
        /// Adds the specified message types to the publish topology
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="messageTypes"></param>
        /// <param name="configure"></param>
        public static void AddPublishMessageTypes(this ISqlBusFactoryConfigurator configurator, IEnumerable<Type> messageTypes,
            Action<ISqlMessagePublishTopologyConfigurator, Type>? configure = null)
        {
            foreach (var messageType in messageTypes)
                configurator.Publish(messageType, x => configure?.Invoke(x, messageType));
        }
    }
}
