namespace MassTransit.Metadata
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using Automatonymous;
    using Courier;
    using Definition;
    using Internals.Extensions;
    using Saga;


    public static class TypeMetadataCache
    {
        static readonly object _typeCacheLock = new object();

        static CachedType GetOrAdd(Type type)
        {
            return Cached.Instance.GetOrAdd(type, _ => (CachedType)Activator.CreateInstance(typeof(CachedType<>).MakeGenericType(type)));
        }

        public static string GetShortName(Type type)
        {
            return GreenPipes.Internals.Extensions.TypeCache.GetShortName(type);
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

        public static Type GetImplementationType(Type type)
        {
            lock (_typeCacheLock)
            {
                return GreenPipes.Internals.Extensions.TypeCache.GetImplementationType(type);
            }
        }

        /// <summary>
        /// Returns true if the type is a consumer, or a consumer definition
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsConsumerOrDefinition(Type type)
        {
            Type[] interfaces = type.GetTypeInfo().GetInterfaces();

            return interfaces.Any(t => t.HasInterface(typeof(IConsumer<>)) || t.HasInterface(typeof(IConsumerDefinition<>)));
        }

        /// <summary>
        /// Returns true if the type is a saga, or a saga definition
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsSagaOrDefinition(Type type)
        {
            Type[] interfaces = type.GetTypeInfo().GetInterfaces();

            if (interfaces.Contains(typeof(ISaga)))
                return true;

            return interfaces.Any(t => t.HasInterface(typeof(InitiatedBy<>))
                || t.HasInterface(typeof(Orchestrates<>))
                || t.HasInterface(typeof(Observes<,>))
                || t.HasInterface(typeof(ISagaDefinition<>)));
        }

        /// <summary>
        /// Returns true if the type is a state machine or saga definition
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsSagaStateMachineOrDefinition(Type type)
        {
            Type[] interfaces = type.GetTypeInfo().GetInterfaces();

            return interfaces.Any(t => t.HasInterface(typeof(SagaStateMachine<>))
                || t.HasInterface(typeof(ISagaDefinition<>)));
        }

        /// <summary>
        /// Returns true if the type is an activity
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsActivityOrDefinition(Type type)
        {
            Type[] interfaces = type.GetTypeInfo().GetInterfaces();

            return interfaces.Any(t => t.HasInterface(typeof(IExecuteActivity<>))
                || t.HasInterface(typeof(ICompensateActivity<>))
                || t.HasInterface(typeof(IActivityDefinition<,,>))
                || t.HasInterface(typeof(IExecuteActivityDefinition<,>)));
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
            public bool HasConsumerInterfaces => TypeMetadataCache<T>.HasConsumerInterfaces;
            public bool HasSagaInterfaces => TypeMetadataCache<T>.HasSagaInterfaces;
            public bool IsTemporaryMessageType => TypeMetadataCache<T>.IsTemporaryMessageType;
            public bool IsValidMessageType => TypeMetadataCache<T>.IsValidMessageType;
            public Type[] MessageTypes => TypeMetadataCache<T>.MessageTypes;
            public string[] MessageTypeNames => TypeMetadataCache<T>.MessageTypeNames;

            public IEnumerable<PropertyInfo> Properties => TypeMetadataCache<T>.Properties;
        }
    }


    public class TypeMetadataCache<T> :
        ITypeMetadataCache<T>
    {
        readonly Lazy<bool> _hasConsumerInterfaces;
        readonly Lazy<bool> _hasSagaInterfaces;
        readonly Lazy<Type> _implementationType;
        readonly Lazy<bool> _isTemporaryMessageType;
        readonly Lazy<bool> _isValidMessageType;
        readonly Lazy<string[]> _messageTypeNames;
        readonly Lazy<string> _diagnosticAddress;
        readonly Lazy<Type[]> _messageTypes;
        readonly Lazy<List<PropertyInfo>> _properties;
        readonly string _shortName;
        string _invalidMessageTypeReason;

        TypeMetadataCache()
        {
            _shortName = GreenPipes.Internals.Extensions.TypeCache<T>.ShortName;

            _hasSagaInterfaces = new Lazy<bool>(ScanForSagaInterfaces, LazyThreadSafetyMode.PublicationOnly);
            _hasConsumerInterfaces = new Lazy<bool>(() => !_hasSagaInterfaces.Value && ScanForConsumerInterfaces(), LazyThreadSafetyMode.PublicationOnly);

            static List<PropertyInfo> PropertyListFactory() =>
                typeof(T).GetAllProperties()
                    .GroupBy(x => x.Name)
                    .Select(x => x.Last())
                    .ToList();

            _properties = new Lazy<List<PropertyInfo>>(PropertyListFactory);

            _isValidMessageType = new Lazy<bool>(CheckIfValidMessageType);
            _isTemporaryMessageType = new Lazy<bool>(() => CheckIfTemporaryMessageType(typeof(T).GetTypeInfo()));
            _messageTypes = new Lazy<Type[]>(() => GetMessageTypes().ToArray());
            _messageTypeNames = new Lazy<string[]>(() => GetMessageTypeNames().ToArray());
            _diagnosticAddress = new Lazy<string>(GetDiagnosticAddress);
            _implementationType = new Lazy<Type>(() => TypeMetadataCache.GetImplementationType(typeof(T)));
        }

        public static string ShortName => Cached.Metadata.Value.ShortName;
        public static string DiagnosticAddress => Cached.Metadata.Value.DiagnosticAddress;
        public static bool HasSagaInterfaces => Cached.Metadata.Value.HasSagaInterfaces;
        public static bool HasConsumerInterfaces => Cached.Metadata.Value.HasConsumerInterfaces;
        public static IEnumerable<PropertyInfo> Properties => Cached.Metadata.Value.Properties;
        public static Type ImplementationType => Cached.Metadata.Value.ImplementationType;
        public static bool IsValidMessageType => Cached.Metadata.Value.IsValidMessageType;
        public static string InvalidMessageTypeReason => Cached.Metadata.Value.InvalidMessageTypeReason;
        public static bool IsTemporaryMessageType => Cached.Metadata.Value.IsTemporaryMessageType;
        public static Type[] MessageTypes => Cached.Metadata.Value.MessageTypes;
        public static string[] MessageTypeNames => Cached.Metadata.Value.MessageTypeNames;
        bool ITypeMetadataCache<T>.IsTemporaryMessageType => _isTemporaryMessageType.Value;
        string[] ITypeMetadataCache<T>.MessageTypeNames => _messageTypeNames.Value;
        string ITypeMetadataCache<T>.DiagnosticAddress => _diagnosticAddress.Value;
        IEnumerable<PropertyInfo> ITypeMetadataCache<T>.Properties => _properties.Value;
        bool ITypeMetadataCache<T>.IsValidMessageType => _isValidMessageType.Value;
        string ITypeMetadataCache<T>.InvalidMessageTypeReason => _invalidMessageTypeReason;
        Type[] ITypeMetadataCache<T>.MessageTypes => _messageTypes.Value;
        Type ITypeMetadataCache<T>.ImplementationType => _implementationType.Value;
        bool ITypeMetadataCache<T>.HasConsumerInterfaces => _hasConsumerInterfaces.Value;
        bool ITypeMetadataCache<T>.HasSagaInterfaces => _hasSagaInterfaces.Value;
        string ITypeMetadataCache<T>.ShortName => _shortName;

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
                _invalidMessageTypeReason = $"Message types must not be anonymous types: {ShortName}";
                return false;
            }

            if (typeInfo.Namespace == null)
            {
                _invalidMessageTypeReason = $"Messages types must have a valid namespace: {ShortName}";
                return false;
            }

            if (typeof(object).GetTypeInfo().Assembly.Equals(typeInfo.Assembly))
            {
                _invalidMessageTypeReason = $"Messages types must not be System types: {ShortName}";
                return false;
            }

            if (typeInfo.Namespace == "System")
            {
                _invalidMessageTypeReason = $"Messages types must not be in the System namespace: {ShortName}";
                return false;
            }

            var ns = typeInfo.Namespace;
            if (ns != null && ns.StartsWith("System."))
            {
                _invalidMessageTypeReason = $"Messages types must not be in the System namespace: {ShortName}";
                return false;
            }

            if (typeInfo.HasInterface<SendContext>()
                || typeInfo.HasInterface<ConsumeContext>()
                || typeInfo.HasInterface<ReceiveContext>())
            {
                _invalidMessageTypeReason = $"ConsumeContext, ReceiveContext, and SendContext are not valid message types: {ShortName}";
                return false;
            }

            if (typeInfo.IsGenericType)
            {
                var typeDefinition = typeInfo.GetGenericTypeDefinition();
                if (typeDefinition == typeof(CorrelatedBy<>))
                {
                    _invalidMessageTypeReason =
                        $"CorrelatedBy<{typeof(T).GetClosingArgument(typeof(CorrelatedBy<>)).Name} is not a valid message type";

                    return false;
                }

                if (typeDefinition == typeof(Orchestrates<>))
                {
                    _invalidMessageTypeReason =
                        $"Orchestrates<{typeof(T).GetClosingArgument(typeof(Orchestrates<>)).Name} is not a valid message type";

                    return false;
                }

                if (typeDefinition == typeof(InitiatedBy<>))
                {
                    _invalidMessageTypeReason =
                        $"InitiatedBy<{typeof(T).GetClosingArgument(typeof(InitiatedBy<>)).Name} is not a valid message type";

                    return false;
                }

                if (typeDefinition == typeof(Observes<,>))
                {
                    var closingArguments = typeof(T).GetClosingArguments(typeof(Observes<,>)).ToArray();
                    _invalidMessageTypeReason = $"Observes<{closingArguments[0].Name},{closingArguments[1].Name} is not a valid message type";
                    return false;
                }

                if (typeInfo.IsOpenGeneric())
                {
                    _invalidMessageTypeReason = $"Message types must not be open generic types: {ShortName}";
                    return false;
                }
            }

            return true;
        }

        static bool CheckIfTemporaryMessageType(TypeInfo messageTypeInfo)
        {
            return (!messageTypeInfo.IsVisible && messageTypeInfo.IsClass)
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
                foreach (var faultMessageType in TypeMetadataCache.GetMessageTypes(arguments[0]))
                {
                    var faultInterfaceType = typeof(Fault<>).MakeGenericType(faultMessageType);
                    if (faultInterfaceType != typeof(T))
                        yield return faultInterfaceType;
                }
            }

            var baseType = typeof(T).GetTypeInfo().BaseType;
            while (baseType != null && TypeMetadataCache.IsValidMessageType(baseType))
            {
                yield return baseType;

                baseType = baseType.GetTypeInfo().BaseType;
            }

            IEnumerable<Type> interfaces = typeof(T)
                .GetTypeInfo()
                .GetInterfaces()
                .Where(TypeMetadataCache.IsValidMessageType);

            foreach (var interfaceType in interfaces)
                yield return interfaceType;
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

            return interfaces.Any(t => t.HasInterface(typeof(IConsumer<>)));
        }

        static bool ScanForSagaInterfaces()
        {
            Type[] interfaces = typeof(T).GetTypeInfo().GetInterfaces();

            if (interfaces.Contains(typeof(ISaga)))
                return true;

            return interfaces.Any(t => t.HasInterface(typeof(InitiatedBy<>))
                || t.HasInterface(typeof(Orchestrates<>))
                || t.HasInterface(typeof(Observes<,>)));
        }


        static class Cached
        {
            internal static readonly Lazy<ITypeMetadataCache<T>> Metadata = new Lazy<ITypeMetadataCache<T>>(
                () => new TypeMetadataCache<T>(), LazyThreadSafetyMode.PublicationOnly);
        }
    }
}
