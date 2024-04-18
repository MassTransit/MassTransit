namespace MassTransit
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Internals;
    using Metadata;


    public static class MessageTypeCache
    {
        static CachedType GetOrAdd(Type type)
        {
            return Cached.Instance.GetOrAdd(type, _ => Activation.Activate(type, new Factory()));
        }

        public static IEnumerable<PropertyInfo> GetProperties(Type type)
        {
            return GetOrAdd(type).Properties;
        }

        public static bool IsValidMessageType(Type type)
        {
            return GetOrAdd(type).IsValidMessageType;
        }

        public static string? InvalidMessageTypeReason(Type type)
        {
            return GetOrAdd(type).InvalidMessageTypeReason;
        }

        public static bool IsTemporaryMessageType(Type type)
        {
            return GetOrAdd(type).IsTemporaryMessageType;
        }

        public static Type[] GetMessageTypes(Type type)
        {
            return GetOrAdd(type).MessageTypes;
        }

        public static string[] GetMessageTypeNames(Type type)
        {
            return GetOrAdd(type).MessageTypeNames;
        }


        readonly struct Factory :
            IActivationType<CachedType>
        {
            public CachedType ActivateType<T>()
                where T : class
            {
                return new CachedType<T>();
            }
        }


        static class Cached
        {
            internal static readonly ConcurrentDictionary<Type, CachedType> Instance = new ConcurrentDictionary<Type, CachedType>();
        }


        interface CachedType
        {
            bool IsTemporaryMessageType { get; }
            bool IsValidMessageType { get; }
            string? InvalidMessageTypeReason { get; }
            Type[] MessageTypes { get; }
            string[] MessageTypeNames { get; }
            IEnumerable<PropertyInfo> Properties { get; }
        }


        class CachedType<T> :
            CachedType
        {
            bool CachedType.IsTemporaryMessageType => MessageTypeCache<T>.IsTemporaryMessageType;
            bool CachedType.IsValidMessageType => MessageTypeCache<T>.IsValidMessageType;
            string? CachedType.InvalidMessageTypeReason => MessageTypeCache<T>.InvalidMessageTypeReason;
            public Type[] MessageTypes => MessageTypeCache<T>.MessageTypes;
            public string[] MessageTypeNames => MessageTypeCache<T>.MessageTypeNames;

            public IEnumerable<PropertyInfo> Properties => MessageTypeCache<T>.Properties;

            public void Method1()
            {
            }

            public void Method2()
            {
            }
        }
    }


    public class MessageTypeCache<T> :
        IMessageTypeCache
    {
        readonly Lazy<string> _diagnosticAddress;
        readonly Lazy<bool> _isTemporaryMessageType;
        readonly Lazy<bool> _isValidMessageType;
        readonly Lazy<string[]> _messageTypeNames;
        string? _invalidMessageTypeReason;
        Type[]? _messageTypes;
        List<PropertyInfo>? _properties;

        MessageTypeCache()
        {
            _isValidMessageType = new Lazy<bool>(CheckIfValidMessageType);
            _isTemporaryMessageType = new Lazy<bool>(() => CheckIfTemporaryMessageType(typeof(T)));
            _messageTypeNames = new Lazy<string[]>(() => GetMessageTypeNames().ToArray());
            _diagnosticAddress = new Lazy<string>(GetDiagnosticAddress);
        }

        public static string DiagnosticAddress => Cached.Metadata.Value.DiagnosticAddress;
        public static IEnumerable<PropertyInfo> Properties => Cached.Metadata.Value.Properties;
        public static bool IsValidMessageType => Cached.Metadata.Value.IsValidMessageType;
        public static string? InvalidMessageTypeReason => Cached.Metadata.Value.InvalidMessageTypeReason;
        public static bool IsTemporaryMessageType => Cached.Metadata.Value.IsTemporaryMessageType;
        public static Type[] MessageTypes => Cached.Metadata.Value.MessageTypes;
        public static string[] MessageTypeNames => Cached.Metadata.Value.MessageTypeNames;

        bool IMessageTypeCache.IsTemporaryMessageType => _isTemporaryMessageType.Value;

        string[] IMessageTypeCache.MessageTypeNames => _messageTypeNames.Value;
        string IMessageTypeCache.DiagnosticAddress => _diagnosticAddress.Value;
        IEnumerable<PropertyInfo> IMessageTypeCache.Properties => _properties ??= PropertyListFactory();
        bool IMessageTypeCache.IsValidMessageType => _isValidMessageType.Value;
        string? IMessageTypeCache.InvalidMessageTypeReason => _invalidMessageTypeReason;

        Type[] IMessageTypeCache.MessageTypes => _messageTypes ??= GetMessageTypes().ToArray();

        static List<PropertyInfo> PropertyListFactory()
        {
            return typeof(T).GetAllProperties()
                .GroupBy(x => x.Name)
                .Select(x => x.Last())
                .ToList();
        }

        static bool CheckIfTemporaryMessageType(Type messageTypeInfo)
        {
            return (!messageTypeInfo.IsVisible && messageTypeInfo.IsClass)
                || (messageTypeInfo.IsGenericType && messageTypeInfo.GetGenericArguments().Any(x => CheckIfTemporaryMessageType(x)));
        }

        /// <summary>
        /// Returns all the message types that are available for the specified type. This will
        /// return any base classes or interfaces implemented by the type that are allowed
        /// message types.
        /// </summary>
        /// <returns>An enumeration of valid message types implemented by the specified type</returns>
        static IEnumerable<Type> GetMessageTypes()
        {
            if (IsValidMessageType)
                yield return typeof(T);

            if (typeof(T).ClosesType(typeof(Fault<>), out Type[] arguments))
            {
                foreach (var faultMessageType in MessageTypeCache.GetMessageTypes(arguments[0]))
                {
                    var faultInterfaceType = typeof(Fault<>).MakeGenericType(faultMessageType);
                    if (faultInterfaceType != typeof(T))
                        yield return faultInterfaceType;
                }
            }

            var baseType = typeof(T).BaseType;
            while (baseType != null && MessageTypeCache.IsValidMessageType(baseType))
            {
                yield return baseType;

                baseType = baseType.BaseType;
            }

            IEnumerable<Type>? interfaces = typeof(T)
                .GetInterfaces()
                .Where(MessageTypeCache.IsValidMessageType);

            foreach (var interfaceType in interfaces)
                yield return interfaceType;
        }

        /// <summary>
        /// Returns true if the specified type is an allowed message type, i.e.
        /// that it doesn't come from the .Net core assemblies or is without a namespace,
        /// amongst others.
        /// </summary>
        /// <returns>True if the message can be sent, otherwise false</returns>
        bool CheckIfValidMessageType()
        {
            var type = typeof(T);

            var ns = type.Namespace;
            if (ns == null)
            {
                if (type.IsAnonymousType())
                {
                    _invalidMessageTypeReason = $"Message types must not be anonymous types: {TypeCache<T>.ShortName}";
                    return false;
                }

                _invalidMessageTypeReason = $"Messages types must have a valid namespace: {TypeCache<T>.ShortName}";
                return false;
            }

            if (type is { Name: "JsonObject", Namespace: "System.Text.Json.Nodes" })
                return true;

            if (ns == "System" || ns.StartsWith("System."))
            {
                _invalidMessageTypeReason = $"Messages types must not be in the System namespace: {TypeCache<T>.ShortName}";
                return false;
            }

            if (typeof(object).Assembly.Equals(type.Assembly))
            {
                _invalidMessageTypeReason = $"Messages types must not be System types: {TypeCache<T>.ShortName}";
                return false;
            }

            if (type.HasInterface<SendContext>()
                || type.HasInterface<ConsumeContext>()
                || type.HasInterface<ReceiveContext>())
            {
                _invalidMessageTypeReason = $"ConsumeContext, ReceiveContext, and SendContext are not valid message types: {TypeCache<T>.ShortName}";
                return false;
            }

            if (type.IsGenericType)
            {
                var typeDefinition = type.GetGenericTypeDefinition();
                if (typeDefinition == typeof(CorrelatedBy<>))
                {
                    _invalidMessageTypeReason =
                        $"CorrelatedBy<{type.GetClosingArgument(typeof(CorrelatedBy<>)).Name}> is not a valid message type";

                    return false;
                }

                if (typeDefinition == typeof(Orchestrates<>))
                {
                    _invalidMessageTypeReason =
                        $"Orchestrates<{type.GetClosingArgument(typeof(Orchestrates<>)).Name}> is not a valid message type";

                    return false;
                }

                if (typeDefinition == typeof(InitiatedBy<>))
                {
                    _invalidMessageTypeReason =
                        $"InitiatedBy<{type.GetClosingArgument(typeof(InitiatedBy<>)).Name}> is not a valid message type";

                    return false;
                }

                if (typeDefinition == typeof(InitiatedByOrOrchestrates<>))
                {
                    _invalidMessageTypeReason =
                        $"InitiatedByOrOrchestrates<{type.GetClosingArgument(typeof(InitiatedByOrOrchestrates<>)).Name}> is not a valid message type";

                    return false;
                }

                if (typeDefinition == typeof(Observes<,>))
                {
                    Type[]? closingArguments = type.GetClosingArguments(typeof(Observes<,>)).ToArray();
                    _invalidMessageTypeReason = $"Observes<{closingArguments[0].Name},{closingArguments[1].Name}> is not a valid message type";
                    return false;
                }

                if (type.IsOpenGeneric())
                {
                    _invalidMessageTypeReason = $"Message types must not be open generic types: {TypeCache<T>.ShortName}";
                    return false;
                }
            }

            return true;
        }

        static IEnumerable<string> GetMessageTypeNames()
        {
            return MessageTypes.Select(MessageUrn.ForTypeString);
        }

        static string GetDiagnosticAddress()
        {
            const string activity = "Activity";

            if (typeof(T).HasInterface<IExecuteActivity>())
            {
                var activityName = typeof(T).Name;
                if (activityName.EndsWith(activity, StringComparison.InvariantCultureIgnoreCase))
                    activityName = activityName.Substring(0, activityName.Length - activity.Length);

                return activityName;
            }

            var (type, ns, _) = MessageUrn.ForType<T>();
            return $"{type}/{ns}";
        }


        static class Cached
        {
            internal static readonly Lazy<IMessageTypeCache> Metadata = new Lazy<IMessageTypeCache>(() => new MessageTypeCache<T>());
        }
    }
}
