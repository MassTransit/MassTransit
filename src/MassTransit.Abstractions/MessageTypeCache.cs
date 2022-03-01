namespace MassTransit
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using Internals;


    public static class MessageTypeCache
    {
        static CachedType GetOrAdd(Type type)
        {
            return Cached.Instance.GetOrAdd(type, _ => (CachedType)Activator.CreateInstance(typeof(CachedType<>).MakeGenericType(type)));
        }

        public static IEnumerable<PropertyInfo> GetProperties(Type type)
        {
            return GetOrAdd(type).Properties;
        }

        public static bool IsValidMessageType(Type type)
        {
            return GetOrAdd(type).IsValidMessageType;
        }

        public static bool IsTemporaryMessageType(Type type)
        {
            return GetOrAdd(type).IsTemporaryMessageType;
        }

        public static bool HasConsumerInterfaces(Type type)
        {
            return GetOrAdd(type).HasConsumerInterfaces;
        }

        public static bool HasSagaInterfaces(Type type)
        {
            return GetOrAdd(type).HasSagaInterfaces;
        }

        public static Type[] GetMessageTypes(Type type)
        {
            return GetOrAdd(type).MessageTypes;
        }

        public static string[] GetMessageTypeNames(Type type)
        {
            return GetOrAdd(type).MessageTypeNames;
        }


        static class Cached
        {
            internal static readonly ConcurrentDictionary<Type, CachedType> Instance = new ConcurrentDictionary<Type, CachedType>();
        }


        interface CachedType
        {
            bool HasConsumerInterfaces { get; }
            bool HasSagaInterfaces { get; }
            bool IsTemporaryMessageType { get; }
            bool IsValidMessageType { get; }
            Type[] MessageTypes { get; }
            string[] MessageTypeNames { get; }
            IEnumerable<PropertyInfo> Properties { get; }
        }


        class CachedType<T> :
            CachedType
        {
            bool CachedType.HasConsumerInterfaces => MessageTypeCache<T>.HasConsumerInterfaces;
            bool CachedType.HasSagaInterfaces => MessageTypeCache<T>.HasSagaInterfaces;
            bool CachedType.IsTemporaryMessageType => MessageTypeCache<T>.IsTemporaryMessageType;
            bool CachedType.IsValidMessageType => MessageTypeCache<T>.IsValidMessageType;
            public Type[] MessageTypes => MessageTypeCache<T>.MessageTypes;
            public string[] MessageTypeNames => MessageTypeCache<T>.MessageTypeNames;

            public IEnumerable<PropertyInfo> Properties => MessageTypeCache<T>.Properties;
        }
    }


    public class MessageTypeCache<T> :
        IMessageTypeCache
    {
        readonly Lazy<string> _diagnosticAddress;
        readonly Lazy<bool> _hasConsumerInterfaces;
        readonly Lazy<bool> _hasSagaInterfaces;
        readonly Lazy<bool> _isTemporaryMessageType;
        readonly Lazy<bool> _isValidMessageType;
        readonly Lazy<string[]> _messageTypeNames;
        readonly Lazy<Type[]> _messageTypes;
        readonly Lazy<List<PropertyInfo>> _properties;
        string? _invalidMessageTypeReason;

        MessageTypeCache()
        {
            _hasSagaInterfaces = new Lazy<bool>(ScanForSagaInterfaces, LazyThreadSafetyMode.PublicationOnly);
            _hasConsumerInterfaces = new Lazy<bool>(() => !_hasSagaInterfaces.Value && ScanForConsumerInterfaces(), LazyThreadSafetyMode.PublicationOnly);

            static List<PropertyInfo> PropertyListFactory()
            {
                return typeof(T).GetAllProperties()
                    .GroupBy(x => x.Name)
                    .Select(x => x.Last())
                    .ToList();
            }

            _properties = new Lazy<List<PropertyInfo>>(PropertyListFactory);

            _isValidMessageType = new Lazy<bool>(CheckIfValidMessageType);
            _isTemporaryMessageType = new Lazy<bool>(() => CheckIfTemporaryMessageType(typeof(T).GetTypeInfo()));
            _messageTypes = new Lazy<Type[]>(() => GetMessageTypes().ToArray());
            _messageTypeNames = new Lazy<string[]>(() => GetMessageTypeNames().ToArray());
            _diagnosticAddress = new Lazy<string>(GetDiagnosticAddress);
        }

        public static string DiagnosticAddress => Cached.Metadata.Value.DiagnosticAddress;
        public static bool HasSagaInterfaces => Cached.Metadata.Value.HasSagaInterfaces;
        public static bool HasConsumerInterfaces => Cached.Metadata.Value.HasConsumerInterfaces;
        public static IEnumerable<PropertyInfo> Properties => Cached.Metadata.Value.Properties;
        public static bool IsValidMessageType => Cached.Metadata.Value.IsValidMessageType;
        public static string? InvalidMessageTypeReason => Cached.Metadata.Value.InvalidMessageTypeReason;
        public static bool IsTemporaryMessageType => Cached.Metadata.Value.IsTemporaryMessageType;
        public static Type[] MessageTypes => Cached.Metadata.Value.MessageTypes;
        public static string[] MessageTypeNames => Cached.Metadata.Value.MessageTypeNames;

        bool IMessageTypeCache.IsTemporaryMessageType => _isTemporaryMessageType.Value;
        string[] IMessageTypeCache.MessageTypeNames => _messageTypeNames.Value;
        string IMessageTypeCache.DiagnosticAddress => _diagnosticAddress.Value;
        IEnumerable<PropertyInfo> IMessageTypeCache.Properties => _properties.Value;
        bool IMessageTypeCache.IsValidMessageType => _isValidMessageType.Value;
        string? IMessageTypeCache.InvalidMessageTypeReason => _invalidMessageTypeReason;
        Type[] IMessageTypeCache.MessageTypes => _messageTypes.Value;
        bool IMessageTypeCache.HasConsumerInterfaces => _hasConsumerInterfaces.Value;
        bool IMessageTypeCache.HasSagaInterfaces => _hasSagaInterfaces.Value;

        static bool CheckIfTemporaryMessageType(Type messageTypeInfo)
        {
            return !messageTypeInfo.IsVisible && messageTypeInfo.IsClass
                || messageTypeInfo.IsGenericType && messageTypeInfo.GetGenericArguments().Any(x => CheckIfTemporaryMessageType(x.GetTypeInfo()));
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

            var baseType = typeof(T).GetTypeInfo().BaseType;
            while (baseType != null && MessageTypeCache.IsValidMessageType(baseType))
            {
                yield return baseType;

                baseType = baseType.GetTypeInfo().BaseType;
            }

            IEnumerable<Type> interfaces = typeof(T)
                .GetTypeInfo()
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
            var typeInfo = typeof(T).GetTypeInfo();

            if (typeInfo.IsAnonymousType())
            {
                _invalidMessageTypeReason = $"Message types must not be anonymous types: {TypeCache<T>.ShortName}";
                return false;
            }

            if (typeInfo.Namespace == null)
            {
                _invalidMessageTypeReason = $"Messages types must have a valid namespace: {TypeCache<T>.ShortName}";
                return false;
            }

            if (typeof(object).GetTypeInfo().Assembly.Equals(typeInfo.Assembly))
            {
                _invalidMessageTypeReason = $"Messages types must not be System types: {TypeCache<T>.ShortName}";
                return false;
            }

            if (typeInfo.Namespace == "System")
            {
                _invalidMessageTypeReason = $"Messages types must not be in the System namespace: {TypeCache<T>.ShortName}";
                return false;
            }

            var ns = typeInfo.Namespace;
            if (ns != null && ns.StartsWith("System."))
            {
                _invalidMessageTypeReason = $"Messages types must not be in the System namespace: {TypeCache<T>.ShortName}";
                return false;
            }

            if (typeInfo.HasInterface<SendContext>()
                || typeInfo.HasInterface<ConsumeContext>()
                || typeInfo.HasInterface<ReceiveContext>())
            {
                _invalidMessageTypeReason = $"ConsumeContext, ReceiveContext, and SendContext are not valid message types: {TypeCache<T>.ShortName}";
                return false;
            }

            if (typeInfo.IsGenericType)
            {
                var typeDefinition = typeInfo.GetGenericTypeDefinition();
                if (typeDefinition == typeof(CorrelatedBy<>))
                {
                    _invalidMessageTypeReason =
                        $"CorrelatedBy<{typeof(T).GetClosingArgument(typeof(CorrelatedBy<>)).Name}> is not a valid message type";

                    return false;
                }

                if (typeDefinition == typeof(Orchestrates<>))
                {
                    _invalidMessageTypeReason =
                        $"Orchestrates<{typeof(T).GetClosingArgument(typeof(Orchestrates<>)).Name}> is not a valid message type";

                    return false;
                }

                if (typeDefinition == typeof(InitiatedBy<>))
                {
                    _invalidMessageTypeReason =
                        $"InitiatedBy<{typeof(T).GetClosingArgument(typeof(InitiatedBy<>)).Name}> is not a valid message type";

                    return false;
                }

                if (typeDefinition == typeof(InitiatedByOrOrchestrates<>))
                {
                    _invalidMessageTypeReason =
                        $"InitiatedByOrOrchestrates<{typeof(T).GetClosingArgument(typeof(InitiatedByOrOrchestrates<>)).Name}> is not a valid message type";

                    return false;
                }

                if (typeDefinition == typeof(Observes<,>))
                {
                    Type[] closingArguments = typeof(T).GetClosingArguments(typeof(Observes<,>)).ToArray();
                    _invalidMessageTypeReason = $"Observes<{closingArguments[0].Name},{closingArguments[1].Name}> is not a valid message type";
                    return false;
                }

                if (typeInfo.IsOpenGeneric())
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

        static bool ScanForConsumerInterfaces()
        {
            Type[] interfaces = typeof(T).GetTypeInfo().GetInterfaces();

            return interfaces.Any(t => t.HasInterface(typeof(IConsumer<>))
                || t.HasInterface(typeof(IJobConsumer<>)));
        }

        static bool ScanForSagaInterfaces()
        {
            Type[] interfaces = typeof(T).GetTypeInfo().GetInterfaces();

            if (interfaces.Contains(typeof(ISaga)))
                return true;

            return interfaces.Any(t => t.HasInterface(typeof(InitiatedBy<>))
                || t.HasInterface(typeof(Orchestrates<>))
                || t.HasInterface(typeof(InitiatedByOrOrchestrates<>))
                || t.HasInterface(typeof(Observes<,>)));
        }


        static class Cached
        {
            internal static readonly Lazy<IMessageTypeCache> Metadata =
                new Lazy<IMessageTypeCache>(() => new MessageTypeCache<T>(), LazyThreadSafetyMode.PublicationOnly);
        }
    }
}
